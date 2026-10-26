# -*- coding: utf-8 -*-
"""Color Maze 关卡求解器 v2：求「不死亡通关」与「全程不受伤通关」的最少步路线。

规则保真表（每条都对应运行时代码，改代码时同步改这里）：
- 移动花 1 步；撞墙/灰石不计步不推引信；放炸弹(E) 0 步不推引信
  —— PlayerGridMover.TryMove / TryUseBomb
- 炸弹放脚下、场上唯一、第 3 次成功移动后立即爆炸 —— DelayedBombState
- 爆炸十字(中心+四邻)：只破灰石；玩家当时在爆炸格上则受 1 伤
  —— ExplodeBomb / TryBreakStone
- 移动后先结算爆炸、再结算格子效果（TryMove 顺序）：
  水=氧-1(氧 0 则血-1)；岩浆=血-1 且氧+1；其余(含传送门落点/宝箱/材料/
  出口) = 氧+1 —— PlayerStats.ApplyTile
- 传送门：踩上即跳「扫描序(y 外 x 内)首个同 ID 非入口门」，算 1 步；
  无配对门则原地不动、按地板回氧 —— MazeBootstrapper.TryGetPortalExit
- 宝箱 +1 炸弹 +3 金币，变地板；材料 +1，变地板；重复踩 = 普通地板
- 出口：进入时材料数 ≥ 所需即通关，否则提示并可站立 —— ReachExit
- 初始 血 3 / 氧 3 / 炸弹 0；出生格不触发效果；步数超限判负
  —— PlayerStats 常量 / LevelFlowController.TryUseStep

架构：
- astar()  用 A*（0 费放炸弹 + 1 费移动）求最优；启发 = 松弛图
  （灰石视为可通行、忽略氧气/血量）上「位置→最近材料→出口」的 BFS
  距离，是步数下界（可采纳且一致）；同核心状态 (位置,氧,血,料,弹,引信)
  做 (步数, 已破坏集) 支配剪枝，消掉 frozenset 组合爆炸。
- Sim     与搜索器「分开实现」的回放校验器：任何答案路线必须回放
  通过（逐步断言合法性、无死亡/无受伤约束、最终通关）才被接受。
- --selftest 手算期望值的合成关卡，锁住氧气/炸弹/传送门/孤立门语义。

用法：
  python tools/solve_level.py "Color Maze/Assets/Content/Levels/Level_Water World.asset"
  可选：--mode both|no-death|damage-free   --start 行,列   --require N
        --selftest   --no-verify
"""
import argparse
import heapq
import re
import sys
from collections import deque

MAX_HP = 3
MAX_O2 = 3
ALLOWED = set(".#SRBCME123456789")
# 屏幕方向：资产 rows[0] = 屏幕顶部（文件首行在上）。
# Unity 出生点 startCell(x,y) 的 y 自底向上，换算：资产行 = H-1-y。
# 故行号 -1 = 屏幕上方；本文件内部统一用资产坐标 (行, 列)。
DIRS = [(-1, 0, "上", "W"), (1, 0, "下", "S"), (0, -1, "左", "A"), (0, 1, "右", "D")]


def load_rows(path):
    rows = []
    in_rows = False
    with open(path, encoding="utf-8") as f:
        for line in f:
            if re.match(r"\s*rows:\s*$", line):
                in_rows = True
                continue
            if not in_rows:
                continue
            m = re.match(r"\s*-\s+(.+?)\s*$", line)
            if not m:
                break                       # rows 列表结束（nextLevel 等）
            val = m.group(1)
            if len(val) >= 2 and val[0] == "'" and val[-1] == "'":
                val = val[1:-1]             # YAML 只给 '#' 开头的行加引号
            rows.append(val)
    if not rows:
        raise SystemExit(f"未在 {path} 找到 rows 行")
    width = len(rows[0])
    bad = [i for i, r in enumerate(rows) if len(r) != width]
    if bad:
        raise SystemExit(f"行宽不一致: 第 {bad} 行")
    return rows


class Level:
    """地图 + 只读查询；语义与运行时逐条对齐。"""

    def __init__(self, rows):
        self.rows = rows
        self.h, self.w = len(rows), len(rows[0])
        for y, row in enumerate(rows):
            bad = set(row) - ALLOWED
            if bad:
                raise SystemExit(f"第 {y} 行含非法字符 {bad}")
        self.exits = [(y, x) for y in range(self.h) for x in range(self.w)
                      if rows[y][x] == "E"]
        self.materials = [(y, x) for y in range(self.h) for x in range(self.w)
                          if rows[y][x] == "M"]
        self.chests = [(y, x) for y in range(self.h) for x in range(self.w)
                       if rows[y][x] == "C"]
        self.portals = {}
        for y in range(self.h):          # 扫描序：y 外 x 内，与运行时一致
            for x in range(self.w):
                if rows[y][x].isdigit():
                    self.portals.setdefault(rows[y][x], []).append((y, x))
        if len(self.exits) != 1:
            raise SystemExit(f"出口数 {len(self.exits)} != 1")

    def ch(self, p):
        return self.rows[p[0]][p[1]]

    def inb(self, p):
        return 0 <= p[0] < self.h and 0 <= p[1] < self.w

    def portal_exit(self, p):
        """运行时语义：同 ID 扫描序首个非入口门；无配对返回 None。"""
        for q in self.portals[self.ch(p)]:
            if q != p:
                return q
        return None

    def relaxed_adj(self):
        """松弛图邻接（灰石可走、忽略地形效果）；有向（传送门单向跳）。"""
        adj = {}
        for y in range(self.h):
            for x in range(self.w):
                u = (y, x)
                if self.ch(u) == "#":
                    continue
                outs = []
                for dy, dx, _, _ in DIRS:
                    v = (y + dy, x + dx)
                    if not self.inb(v) or self.ch(v) == "#":
                        continue
                    dest = v
                    if self.ch(v).isdigit():
                        pair = self.portal_exit(v)
                        dest = pair if pair is not None else v  # 孤立门=原地
                    outs.append(dest)
                adj[u] = outs
        return adj


def rev_bfs(adj, target):
    """松弛图上所有点到 target 的最短步数（在反图上 BFS）。"""
    radj = {u: [] for u in adj}
    for u, outs in adj.items():
        for v in outs:
            radj[v].append(u)
    dist = {target: 0}
    dq = deque([target])
    while dq:
        u = dq.popleft()
        for p in radj[u]:
            if p not in dist:
                dist[p] = dist[u] + 1
                dq.append(p)
    return dist


class Sim:
    """独立回放校验器：与 astar 分开实现，逐步断言。"""

    def __init__(self, level, start, require, step_limit, damage_free):
        self.lv, self.pos, self.req = level, start, require
        self.limit, self.damage_free = step_limit, damage_free
        self.o2, self.hp, self.bombs, self.mat = MAX_O2, MAX_HP, 0, 0
        self.steps = 0
        self.done = set()                # 已破坏灰石/已开宝箱/已拾材料
        self.fuse = None                 # (cell, 剩余移动次数)
        self.trace = []

    def note(self, text):
        self.trace.append(
            f"  第{self.steps:>3}步  {text}  | 氧{self.o2} 血{self.hp} "
            f"弹{self.bombs} 料{self.mat} 引信{self.fuse}")

    def walkable(self, p):
        ch = self.lv.ch(p)
        return ch != "#" and (ch != "S" or p in self.done)

    def place_bomb(self):
        assert self.fuse is None and self.bombs > 0, "非法放弹"
        self.bombs -= 1
        self.fuse = (self.pos, 3)
        self.note(f"[E] 在{self.pos}放炸弹")

    def move(self, dy, dx, name):
        t = (self.pos[0] + dy, self.pos[1] + dx)
        assert self.lv.inb(t) and self.walkable(t), f"非法移动 →{t}"
        self.steps += 1
        assert self.steps <= self.limit, "步数超限"
        self.pos = t
        # 1) 引信（先于格子效果）
        if self.fuse is not None:
            cell, n = self.fuse
            if n <= 1:
                blast = {cell, (cell[0] - 1, cell[1]), (cell[0] + 1, cell[1]),
                         (cell[0], cell[1] - 1), (cell[0], cell[1] + 1)}
                for c in blast:
                    if self.lv.inb(c) and self.lv.ch(c) == "S" and c not in self.done:
                        self.done.add(c)
                if self.pos in blast:
                    self.hp -= 1
                self.fuse = None
            else:
                self.fuse = (cell, n - 1)
        # 2) 格子效果
        ch = self.lv.ch(self.pos)
        event = name + "→" + str(self.pos)
        if ch.isdigit():
            pair = self.lv.portal_exit(self.pos)
            if pair is not None:
                self.pos = pair
                event += f" 传送→{pair}"
            self.o2 = min(MAX_O2, self.o2 + 1)
        elif ch == "C" and self.pos not in self.done:
            self.done.add(self.pos)
            self.bombs += 1
            self.o2 = min(MAX_O2, self.o2 + 1)
            event += " 开箱+1弹"
        elif ch == "M" and self.pos not in self.done:
            self.done.add(self.pos)
            self.mat += 1
            self.o2 = min(MAX_O2, self.o2 + 1)
            event += " 拾材料"
        elif ch == "E":
            self.o2 = min(MAX_O2, self.o2 + 1)
            if self.mat >= self.req:
                self.note(event + " 出口[通关]")
                assert self.hp > 0 and not (self.damage_free and self.hp < 3)
                return True
        elif ch == "B":
            if self.o2 > 0:
                self.o2 -= 1
            else:
                self.hp -= 1
        elif ch == "R":
            self.hp -= 1
            self.o2 = min(MAX_O2, self.o2 + 1)
        else:
            self.o2 = min(MAX_O2, self.o2 + 1)
        self.note(event)
        assert self.hp > 0, "死亡"
        assert not (self.damage_free and self.hp < 3), "受伤"
        return False

    def replay(self, actions):
        for act in actions:
            if act[0] == "bomb":
                self.place_bomb()
            else:
                if self.move(act[1], act[2], act[3]):
                    return self.trace
        raise AssertionError("路线走完但未通关")


def astar(level, start, require, step_limit, damage_free):
    """返回 (最少步数, 动作序列)。动作 = ('bomb',) 或 ('move',dy,dx,名)。"""
    adj = level.relaxed_adj()
    d_exit = rev_bfs(adj, level.exits[0])
    d_mats = [rev_bfs(adj, m) for m in level.materials]
    # consumed 用整数位掩码表示（灰石/宝箱/材料各占一bit），
    # 支配判断的超集测试从 frozenset 的 O(n) 降为一次 &。
    bit = {}
    for y in range(level.h):
        for x in range(level.w):
            if level.ch((y, x)) in "SCM":
                bit[(y, x)] = 1 << len(bit)

    # 多材料链式下界：h[k][pos] = 松弛图上 pos 途经任意 k 个不同材料再到出口
    # 的最小距离（对材料子集做小 DP；require>1 时远紧于"单材料"启发）。
    INF = float("inf")
    ms = level.materials
    n = len(ms)
    full = 1 << n
    g = [[INF] * n for _ in range(full)]
    for j in range(n):
        g[1 << j][j] = 0
    for s in range(full):
        for j in range(n):
            if not (s >> j) & 1 or g[s][j] == INF:
                continue
            for i in range(n):
                if (s >> i) & 1:
                    continue
                t = s | (1 << i)
                v = g[s][j] + d_mats[i].get(ms[j], INF)
                if v < g[t][i]:
                    g[t][i] = v
    h_table = [{p: d_exit.get(p, INF) for p in adj}]
    for k in range(1, require + 1):
        tab = {}
        for p in adj:
            best = INF
            for s in range(full):
                if bin(s).count("1") != k:
                    continue
                for j in range(n):
                    if not (s >> j) & 1 or g[s][j] == INF:
                        continue
                    a = d_mats[j].get(p, INF)
                    if a == INF:
                        continue
                    v = a + g[s][j] + d_exit.get(ms[j], INF)
                    if v < best:
                        best = v
            tab[p] = best
        h_table.append(tab)

    def h(pos, mat):
        k = require - mat
        return h_table[k if k > 0 else 0].get(pos, INF)

    # 状态: (pos, o2, hp, mat, bombs, consumed(位掩码int), fuse(None|(cell,1..3)))
    init = (start, MAX_O2, MAX_HP, 0, 0, 0, None)
    g_best = {init: 0}
    parent = {init: None}
    # 资源支配：核心=(位置,材料,引信)，前沿项=(步数,氧,血,弹,已破坏集)。
    # 资源沿途只被消耗，故「步数≤ 且 氧≥血≥弹≥破坏⊇」的既有记录支配新状态，
    # 把氧/血/炸弹三个维度压进前沿比较，避免宝箱多的图状态爆炸。
    dom = {}
    tick = 0

    def dominates(a, b):
        # a 支配 b：步数≤ 且 氧≥血≥弹≥，且已破坏/已收集为超集（位掩码）
        return (a[0] <= b[0] and a[1] >= b[1] and a[2] >= b[2]
                and a[3] >= b[3] and a[4] & b[4] == b[4])

    def try_record(ns, ng, prev, act):
        nonlocal tick
        core = (ns[0], ns[3], ns[6])
        entry = (ng, ns[1], ns[2], ns[4], ns[5])
        front = dom.setdefault(core, [])
        for e in front:
            if dominates(e, entry):
                return False
        if g_best.get(ns, 1 << 30) <= ng:
            return False
        g_best[ns] = ng
        parent[ns] = (prev, act)
        front[:] = [e for e in front if not dominates(entry, e)]
        front.append(entry)
        hv = h(ns[0], ns[3])
        if hv == float("inf"):
            return False
        tick += 1
        heapq.heappush(open_list, (ng + hv, ng, tick, ns))
        return True

    open_list = []
    dom[(start, 0, None)] = [(0, MAX_O2, MAX_HP, 0, 0)]
    heapq.heappush(open_list, (h(start, 0), 0, 0, init))
    expanded = 0
    while open_list:
        _, g, _, s = heapq.heappop(open_list)
        if g > g_best.get(s, 1 << 30):    # 陈旧堆项
            continue
        pos, o2, hp, mat, bombs, consumed, fuse = s
        if (g, o2, hp, bombs, consumed) not in dom[(pos, mat, fuse)]:
            continue                       # 堆里残留、后来被资源支配的项
        expanded += 1
        if pos == level.exits[0] and mat >= require:
            actions = []
            cur = s
            while parent[cur] is not None:
                prev, act = parent[cur]
                actions.append(act)
                cur = prev
            actions.reverse()
            return g, actions, expanded
        # 动作 0：放炸弹（0 步）。十字内一块可破灰石都没有时纯亏，不放。
        if fuse is None and bombs > 0:
            blast = {pos, (pos[0] - 1, pos[1]), (pos[0] + 1, pos[1]),
                     (pos[0], pos[1] - 1), (pos[0], pos[1] + 1)}
            if any(level.inb(c) and level.ch(c) == "S"
                   and not consumed & bit.get(c, 0)
                   for c in blast):
                ns = (pos, o2, hp, mat, bombs - 1, consumed, (pos, 3))
                try_record(ns, g, s, ("bomb",))
        # 动作 1：移动（1 步）
        for dy, dx, name, _ in DIRS:
            t = (pos[0] + dy, pos[1] + dx)
            if not level.inb(t):
                continue
            ch = level.ch(t)
            if ch == "#" or (ch == "S" and not consumed & bit.get(t, 0)):
                continue
            if g + 1 > step_limit:
                continue
            nfuse, ncons, nbombs = fuse, consumed, bombs
            nhp, no2, nmat = hp, o2, mat
            if fuse is not None:
                cell, n = fuse
                if n <= 1:
                    blast = {cell, (cell[0] - 1, cell[1]), (cell[0] + 1, cell[1]),
                             (cell[0], cell[1] - 1), (cell[0], cell[1] + 1)}
                    nm = 0
                    for c in blast:
                        if level.inb(c) and level.ch(c) == "S":
                            nm |= bit.get(c, 0)
                    ncons = consumed | nm
                    if t in blast:
                        nhp -= 1
                    nfuse = None
                else:
                    nfuse = (cell, n - 1)
            if nhp <= 0:
                continue
            if ch.isdigit():
                pair = level.portal_exit(t)
                t = t if pair is None else pair
                no2 = min(MAX_O2, no2 + 1)
            elif ch == "C":
                tb = bit.get(t, 0)
                if tb and not consumed & tb:
                    nbombs = bombs + 1
                    ncons |= tb
                no2 = min(MAX_O2, no2 + 1)
            elif ch == "M":
                tb = bit.get(t, 0)
                if tb and not consumed & tb:
                    nmat = mat + 1
                    ncons |= tb
                no2 = min(MAX_O2, no2 + 1)
            elif ch == "E":
                no2 = min(MAX_O2, no2 + 1)
            elif ch == "B":
                if no2 > 0:
                    no2 -= 1
                else:
                    nhp -= 1
            elif ch == "R":
                nhp -= 1
                no2 = min(MAX_O2, no2 + 1)
            else:
                no2 = min(MAX_O2, no2 + 1)
            if nhp <= 0:
                continue
            if damage_free and nhp < MAX_HP:
                continue
            nmat = min(nmat, require)      # 超出所需不再区分
            ns = (t, no2, nhp, nmat, nbombs, ncons, nfuse)
            try_record(ns, g + 1, s, ("move", dy, dx, name))
    return None


def run_case(rows, label, expect_no_death, expect_free, start=(1, 1), require=1):
    ok = True
    for mode, expect in (("no-death", expect_no_death),
                         ("damage-free", expect_free)):
        r = astar(Level(rows), start, require, 999, mode == "damage-free")
        got = None if r is None else r[0]
        if got != expect:
            print(f"  [FAIL] {label} {mode}: 期望 {expect} 实得 {got}")
            ok = False
    print(f"  [{'PASS' if ok else 'FAIL'}] {label}")
    return ok


def selftest():
    print("自测（期望值为手算）：")
    all_ok = True
    all_ok &= run_case(
        ["#####", "#.ME#", "#####"], "T1 直路拾料进出口", 2, 2)
    all_ok &= run_case(
        ["##########", "#.MBBBBBE#", "##########"],
        "T2 拾料后五连水：耗 2 血 7 步存活，无伤无解", 7, None)
    all_ok &= run_case(
        ["##########", "#..C..SME#", "##########"],
        "T3 炸石：贴脸吃 1 血省 2 步(11) vs 完整躲避(13)", 11, 13)
    all_ok &= run_case(
        ["########", "#.1..ME#", "#......#", "#..1...#", "########"],
        "T4 传送门捷径", 6, 6)
    all_ok &= run_case(
        ["#######", "#.1.ME#", "#######"], "T5 孤立门=原地回氧", 4, 4)
    print("自测全部通过" if all_ok else "自测有失败！")
    return all_ok


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("asset", nargs="?", default=
                    "Color Maze/Assets/Content/Levels/Level_Water World.asset")
    ap.add_argument("--start", default="1,1")
    ap.add_argument("--mode", choices=["both", "no-death", "damage-free"],
                    default="both")
    ap.add_argument("--require", type=int, default=None,
                    help="出口所需材料数；默认读资产 requiredMaterials（缺省 1）")
    ap.add_argument("--selftest", action="store_true")
    ap.add_argument("--no-verify", action="store_true")
    args = ap.parse_args()

    if args.selftest:
        sys.exit(0 if selftest() else 1)

    level = Level(load_rows(args.asset))
    sx, sy = (int(v) for v in args.start.split(","))
    start = (level.h - 1 - sy, sx)     # Unity(x,y) → 资产(行,列)
    assert level.inb(start), "出生点越界"
    if level.ch(start) in "#S":
        # 运行时合法：出生不检测格子、只检测移动目标（可走出不可走回）。
        print(f"注意: 出生点 {start} 是'{level.ch(start)}'（嵌墙出生，游戏内可走出）")
    limit = 999                          # StepLimit 从资产读不到时用默认
    asset_text = open(args.asset, encoding="utf-8").read()
    m = re.search(r"stepLimit: (\d+)", asset_text)
    if m:
        limit = int(m.group(1))
    require = args.require               # requiredMaterials 已实装进运行时
    if require is None:
        m = re.search(r"requiredMaterials: (\d+)", asset_text)
        require = int(m.group(1)) if m else 1
    print(f"地图 {level.h}x{level.w}，出生 {start}='{level.ch(start)}'，"
          f"出口 {level.exits[0]}，材料 {len(level.materials)} 个，"
          f"宝箱 {len(level.chests)} 个，传送门 {level.portals}，"
          f"StepLimit={limit}，所需材料={require}")

    modes = ["no-death", "damage-free"] if args.mode == "both" else [args.mode]
    for mode in modes:
        label = "不死亡(允许受伤)" if mode == "no-death" else "全程不受伤"
        r = astar(level, start, require, limit, mode == "damage-free")
        if r is None:
            print(f"\n[{label}] 无解（StepLimit={limit} 内）")
            continue
        steps, actions, expanded = r
        compact = "".join(a[3] if a[0] == "move" else "[E]" for a in actions)
        print(f"\n[{label}] 最少 {steps} 步通关"
              f"（展开 {expanded} 态，StepLimit 余量 {limit - steps}）")
        print(f"  按键序列: {compact}")
        if not args.no_verify:
            sim = Sim(level, start, require, limit,
                      mode == "damage-free")
            trace = sim.replay(actions)
            print(f"  回放校验: 通过（终态 氧{sim.o2} 血{sim.hp} "
                  f"弹{sim.bombs} 料{sim.mat} 步{sim.steps}）")
            for line in trace:
                print(line)


if __name__ == "__main__":
    main()
