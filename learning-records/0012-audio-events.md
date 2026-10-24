# 09d：事件驱动音效

用户完成了 AudioManager 与 8 个 Kenney CC0 音效的绑定，事件驱动音效已提交。检查发现过关无声的真实原因是 AudioManager 的 `Level Complete` 字段为 `None`，而不是播放顺序；补齐 Clip 后过关音正常。

这说明用户已经理解“事件成功一次，反馈一次”的结构。也再次验证：调试音效时先查 Clip 引用，再查播放顺序和 AudioSource。
