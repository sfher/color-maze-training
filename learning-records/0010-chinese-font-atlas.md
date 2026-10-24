# 中文 HUD 方框：Material 与 Atlas 双层问题

用户的状态栏中文正常，但结算界面仍有方框。排查确认：结算文本的 Font Asset 指向 FusionPixel，但 Material 仍使用 LiberationSans；同时 FusionPixel 的 1024 图集已排到约 1013×1013，且 Multi Atlas Textures 关闭，Dynamic 无法继续添加“找到了关卡”等字符。修复方式是统一所有 TMP Text 的 Font Asset + Material、开启 Multi Atlas、预先加入 HUD 字符；字体资产从 38 个字形扩展到 60 个，并生成第二个 Atlas。

这是高价值排障经验：Dynamic 不等于无限容量；只换 Font Asset 不换 Material 也会查错图集。以后遇到“部分文本正常、部分方框”，先查 Material，再查 Atlas 容量。
