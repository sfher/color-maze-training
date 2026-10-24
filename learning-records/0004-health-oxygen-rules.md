# 第四课：生命、氧气与格子规则

用户完成了 `TileType` 与 `PlayerStats`，场景中的 Player 已挂上 PlayerStats，玩家可以在 `Level_01` 中踩岩浆扣生命、踩水耗氧气，并用临时 HUD 看数值。Git 提交为 `feat: add health and oxygen tile effects`，其中也包含课程 04 的开发日志。

这说明后续可以直接把“生命归零”当作一个正式状态来处理，不需要再解释 tile 映射、状态字段或基础 HUD。需要继续巩固的是事件订阅生命周期、构建场景列表，以及临时调试 UI 与正式 UI 的边界。
