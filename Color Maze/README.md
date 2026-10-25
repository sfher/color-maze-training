# 色彩迷宫（Color Maze）

一款用固定资源做路线取舍的网格解谜游戏：在步数耗尽前收集材料、抵达出口。
Unity 6 学习实训项目——从空工程到可发布的完整开发流程记录。

## 玩法

- 玩家固定从 `(1,1)` 出发；每次成功移动消耗 1 步（撞墙不计）。
- 收集材料 → 到达出口通关；生命耗尽或步数用尽失败。
- 岩浆扣生命、水耗氧气、灰石要炸弹炸、宝箱给金币和炸弹、传送门成对瞬移。
- 通关结算金币（宝箱 3/箱 + 首通奖励 10），金币在**内容商店**消费：
  皮肤（8 款）、主题（深海 / 熔岩之心 / 赛博朋克）、附加关卡。
- 关卡按顺序解锁；主题装备后下一局生效。

## 操作

| 按键 | 作用 |
|---|---|
| 方向键 / WASD | 移动 |
| E | 放置炸弹（三步引信） |
| R | 任意状态重开本关 |
| ESC | 暂停 / 继续 |
| 暂停中 M | 返回主菜单 |
| Enter / 空格 / 点击 | 推进对话；通关后进入下一关 |

## 从源码构建

1. Unity **6000.6.4f1** 打开 `Color Maze` 工程。
2. 首次打开后运行菜单 `Color Maze → Setup → Create Main Menu Scene`（生成主菜单场景并排入构建设置；该场景已入库则可跳过）。
3. 菜单 `Color Maze → Build → Windows 64-bit`，产物在 `Builds/Windows64/ColorMaze.exe`。

## 关卡制作

外部关卡策划请读 `docs/LEVEL_DESIGN_GUIDE.md`；工程内可用
`Color Maze → Levels → Level Painter` 可视化绘制关卡（支持批量粘贴、
实时校验、新建/调整尺寸），或用 `Validate All` 一键校验全部关卡。

## 素材与许可

全部第三方素材的来源与许可证记录在 `THIRD_PARTY_NOTICES.md`
（Kenney CC0 地砖与音效、Fusion Pixel 字体 OFL 等）；其余美术为本项目原创。
扫描件、分包许可原文随仓库保留。

## 文档索引

- `docs/CONTENT_SHOP_SPEC.md` —— 内容商店与主菜单需求基准
- `docs/LEVEL_DESIGN_GUIDE.md` / `docs/LEVEL_PATTERNS.md` —— 关卡设计规范
- `docs/COMMIT_CONVENTION.md` —— 提交信息约定
- 仓库根 `GLOSSARY.md` 与 `docs/adr/` —— 领域术语与关键决策
