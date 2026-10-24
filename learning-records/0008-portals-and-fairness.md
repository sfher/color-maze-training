# 07b：成对传送门与公平性

用户完成了传送门配对、首次使用后的记忆标记和缺失配对的安全失败路径：`TileType.Portal`、数字编号、`_portalIds`、`TryGetPortalExit`、`MarkPortalPairKnown` 都已存在；关卡数据中有一对固定传送门，初始同色，使用后整对变亮。Git 提交包含 `feat: add paired portals and memory hint` 与 `docs: add lesson 07b devlog`。

这说明用户已经能处理运行时配对数据与表现层提示。下一步可以把“目标、出口、胜负”做成独立的关卡流程状态，而不是继续往移动脚本里堆条件。
