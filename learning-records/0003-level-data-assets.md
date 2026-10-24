# 第三课：关卡数据资产

用户完成了 `LevelDefinition` 与 `MazeBootstrapper` 的数据拆分，创建了 `Level_01` 和 `Level_02` 两个资产，场景当前绑定 `Level_02`，并把地图渲染与可通行判断改为从资产读取。Git 提交为 `refactor: move data into ScriptableObject`。

这说明后续可以直接在关卡资产上叠加格子规则与数值，不需要再改硬编码地图。需要继续巩固的是：测试关卡内容时先确认当前绑定的是哪个资产，以及把“临时调试 HUD”和最终 UI 分开。
