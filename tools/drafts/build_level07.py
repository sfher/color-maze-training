# -*- coding: utf-8 -*-
"""Level_07 设计草稿 v7 生成器。

连通图设计（区域与唯一通路）：
- A 底部(出生) ↔ B 南走廊(row13-14)：四角开放
- B ↔ C 西双走廊(col1-2)：开放；C ↔ D 左上M4口袋：2连安全水门
- D ↔ E 顶部走廊：仅传送门②；(1,4)(2,4) 墙封死
- E ↔ F 东北出口区：row1 水段跳板（安全）开放
- B ↔ G 东水道(col15-17)：开放，北上 F 需趟 2 连水×2
- 山脉(rows4-12×cols3-12) 隔开 C/E；M2 西袋 1 弹、C3 东袋 1 弹
- M3 岩浆袋四侧全 R：进出各踩 1 次岩浆 = 2 血（高风险材料）
- 血水直道 row17：四连水×2 = 各 1 血（M1 的受伤捷径）
材料代价：M4=步数 / M2=炸弹 / M3=2血 / M1=步数或1血
出口 (1,16)：(1,15)S+(2,16)S 封门（(2,15) 一弹双开）
传送门：①南 Band 东西跳 ②口袋↔西南 ③南走廊 E-W ④之字跳岛 ⑤东南↔南西

用法: python tools/drafts/build_level07.py [输出路径]
"""
import sys

H = W = 19


def build():
    g = [["#"] * W for _ in range(H)]
    for r in range(1, H - 1):
        for c in range(1, W - 1):
            g[r][c] = "."

    def put(r, c, ch):
        g[r][c] = ch

    def rect(r0, r1, c0, c1, ch):
        for r in range(r0, r1 + 1):
            for c in range(c0, c1 + 1):
                g[r][c] = ch

    # ---- 山脉 rows4-12 × cols3-12 ----
    rect(4, 12, 3, 12, "S")
    put(9, 4, "M")      # M2 西袋（门=(9,3)，自内走廊 (9,2) 一弹）
    put(7, 11, "C")     # C3 东袋（门=(7,12)，自 (7,13) 一弹）

    # ---- 岩浆脊 row3 cols3-11（顶/山之间）----
    rect(3, 3, 3, 11, "R")

    # ---- M4 口袋 rows1-2 cols1-3，水门 (3,1)(3,2)，东侧封墙 ----
    put(3, 1, "B"); put(3, 2, "B")
    put(1, 4, "#"); put(2, 4, "#")
    put(2, 2, "M")      # M4

    # ---- 顶部走廊 rows1-2 cols5-12 + M3 全岩浆袋 ----
    put(1, 8, "R"); put(2, 7, "R"); put(2, 9, "R")
    put(2, 8, "M")      # M3（四侧 R：(3,8) 属岩浆脊）
    put(1, 5, "B"); put(1, 6, "B")      # E 段内 2 连水（安全跳）
    put(1, 10, "B"); put(1, 11, "B")

    # ---- 东北出口区 F：rows1-4 × cols13-17 ----
    put(1, 15, "S"); put(2, 16, "S")    # 出口双石（(2,15) 一弹双开）
    put(1, 16, "E")
    put(1, 17, "#")
    put(3, 16, "C")     # C5：出口区弹药源
    put(4, 15, "B"); put(4, 16, "B"); put(4, 17, "B")  # F 南水帘

    # ---- 东水道 col15：rows5-12（岛 (7,15)/(10,15)）----
    for r in (5, 6, 8, 9, 11, 12):
        put(r, 15, "B")
    put(7, 15, "."); put(10, 15, ".")

    # ---- 南走廊 row13 / 南 Band rows14-15 ----
    put(12, 2, "C")     # C6：内走廊宝箱
    put(15, 13, "C")    # C4：南 Band 宝箱

    # ---- 底部：之字安全水路 row16 / 血水直道 row17 ----
    zig = {1: ".", 2: "B", 3: "B", 4: "4", 5: "B", 6: "B", 7: ".",
           8: "B", 9: ".", 10: "B", 11: "B", 12: "4", 13: "B", 14: "B",
           15: ".", 16: ".", 17: "."}
    for c, ch in zig.items():
        put(16, c, ch)
    blood = {4: "B", 5: "B", 6: "B", 7: "B", 8: ".", 9: "M", 10: ".",
             11: "B", 12: "B", 13: "B", 14: "B"}
    for c, ch in blood.items():
        put(17, c, ch)
    put(17, 2, "C")     # C1：出生房宝箱
    put(17, 16, "C")    # C2：右下房宝箱

    # ---- 传送门（5 对跳板，两两远离）----
    put(15, 1, "1"); put(14, 10, "1")   # ① 南 Band W ↔ E
    put(1, 3, "2"); put(13, 1, "2")     # ② M4 口袋 ↔ 西南角
    put(13, 4, "3"); put(13, 14, "3")   # ③ 南走廊 E-W 跳
    put(16, 4, "4"); put(16, 12, "4")   # ④ 之字跳岛
    put(13, 16, "5"); put(14, 5, "5")   # ⑤ 东南 ↔ 南西

    return ["".join(row) for row in g]


def main():
    out = sys.argv[1] if len(sys.argv) > 1 else "tools/drafts/level07.asset"
    rows = build()
    assert all(len(r) == W for r in rows), "行宽错误"
    from collections import Counter
    cnt = Counter(ch for row in rows for ch in row if ch.isdigit())
    assert all(v == 2 for v in cnt.values()), f"传送门未成对: {dict(cnt)}"
    text = "\n".join(f"  - '{r}'" for r in rows)
    asset = f"""%YAML 1.1
%TAG !u! tag:unity3d,2011
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: d56df6da890dae54fa1059256e1a3658, type: 3}}
  m_Name: Level_07
  m_EditorClassIdentifier: Assembly-CSharp::LevelDefinition
  rows:
{text}
  nextLevel: {{fileID: 0}}
  completionDialogue: []
  stepLimit: 150
  requiredMaterials: 2
"""
    with open(out, "w", encoding="utf-8", newline="\n") as f:
        f.write(asset)
    print(f"written {out}")
    for r in rows:
        print(r)


if __name__ == "__main__":
    main()
