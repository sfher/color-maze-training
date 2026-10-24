# 08：材料、出口与关卡流程

用户完成了材料与出口、`LevelFlowController` 状态机和 PlayerGridMover 的流程接入：无材料出口只提示，有材料出口进入 Completed，生命归零进入 Failed，R 可重载场景。Git 提交为 `add material, exit and levelflow` 与 `docs: write lesson 08 devlog`。

这说明完整关卡状态已经能运行。下一步适合把临时 OnGUI 替换成正式 Canvas/TMP UI，同时保持 PlayerStats 和 LevelFlowController 作为状态所有者。
