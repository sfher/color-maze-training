# 09e：暂停菜单与关卡状态

用户完成了 Paused 状态、Esc 切换、Time.timeScale、暂停面板和继续/重开按钮；测试通过并提交 `feat: add pause menu adn resume flow`。暂停时移动由 flow.IsPlaying 停止，而不是只依赖时间缩放。

这说明关卡流程已经具备 Playing/Paused/Completed/Failed 四种正式状态。下一步可以安全地加入“下一关”选择和 MAYA 线索。
