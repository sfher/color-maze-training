# 第六课：炸弹、灰石与运行时地图

用户完成了灰石、炸弹和运行时地图副本：`TileType.Stone`、`S` 映射、`_tiles` / `_renderers`、`TryBreakStone` 与炸弹 HUD 都已存在；关卡资产保留 `S`，破坏只发生在运行时。检查时发现并修复了一个边界顺序问题：`PlayerGridMover.Update` 原先在 `IsAlive` 检查前处理炸弹，死亡后仍可能消耗炸弹。

这说明用户已经能处理“模板数据 + 运行时状态”的双层结构。下一课可以继续复用 `TryReplace` 做宝箱，而不需要重新解释运行时副本。
