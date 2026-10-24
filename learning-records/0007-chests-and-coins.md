# 第七课：宝箱、金币与炸弹来源

用户完成了宝箱、金币与炸弹奖励：`TileType.Chest`、`C` 映射、`TryOpenChest`、金币 HUD 与单次奖励都已存在；宝箱开完变成运行时 Floor，关卡资产保持 `C`。Git 提交为 `Add Chest to gain bombs and coins` 与 `docs: 07 chest`。

这说明运行时替换与资源所有权已经可以复用。后续传送门可以沿用 `_tiles` 与运行时状态，但需要额外处理配对编号、查找和公平性。
