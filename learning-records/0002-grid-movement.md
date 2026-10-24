# 第二课：网格移动与新输入系统

用户独立实现了 `PlayerGridMover`：读取 W/A/S/D 与方向键、用 `Vector2Int` 计算目标格、通过 `MazeBootstrapper.IsWalkable` 拦住墙和越界，并把玩家位置更新到格子中心。场景中 Player 的 `maze` 引用已经绑定，Git 有 `feat: add grid-based player movement` 与 `docs: add lesson 02 devlog` 两次提交。

这说明后续可以直接进入“关卡数据 → 格子规则 → 资源消耗”，不需要再重讲 `Update`、`Vector2Int` 或基础输入。需要继续巩固的是 Inspector 引用、场景层级清理，以及运行时与编辑器程序集的边界。
