# Unity / C# / 独立游戏开发 Resources

## Knowledge

- [Unity 6 Manual](https://docs.unity3d.com/Manual/index.html)
  官方手册。用于查场景、GameObject、组件、预制体、构建与版本控制等编辑器行为；遇到“Unity 到底怎么工作”的问题先查这里。
- [Unity Scripting API](https://docs.unity3d.com/ScriptReference/index.html)
  官方 API 参考。用于确认类名、方法签名与生命周期回调；课程中的代码以此为准。
- [Unity Manual: Coroutines](https://docs.unity3d.com/6000.0/Documentation/Manual/Coroutines.html)
  官方协程说明。用于理解 `IEnumerator` 如何跨帧暂停、在什么对象上启动，以及何时销毁；逐字对话课程的主要资料。
- [Unity UI API: Graphic](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/api/UnityEngine.UI.Graphic.html)
  uGUI 图形基类参考。用于确认 `color`、`raycastTarget` 与 Canvas 渲染相关属性；反馈遮罩课程的主要 UI 资料。
- [Unity UI Manual: Button](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/script-Button.html) · [Unity UI API: Button](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/api/UnityEngine.UI.Button.html)
  官方按钮说明与 API。用于确认 `onClick`、`interactable` 和颜色过渡；金币升级商店课程的主要 UI 资料。
- [Unity Learn: Create with Code](https://learn.unity.com/course/create-with-code)
  官方项目式入门课。用于补充编辑器操作与基础脚本的演示；作为主线课程的辅助，不替代动手项目。
- [Unity Learn: Junior Programmer](https://learn.unity.com/pathway/junior-programmer)
  官方程序员路径。用于在中后期补面向对象、数据与调试的系统知识。
- [Microsoft Learn: C# 文档](https://learn.microsoft.com/en-us/dotnet/csharp/)
  C# 语言与 .NET 的权威文档。用于查类型、数组、类、集合、异常与语言特性。
- [Microsoft Learn: Properties](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties) · [Enumeration types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum)
  C# 属性与枚举的官方说明。用于设计只读状态转发、动态氧气上限和可解释的购买结果。
- [A tour of C#](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
  一小时左右的 C# 全貌。适合在进入第 2–4 课之间通读一遍，建立语言地图。
- [.NET C# coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
  官方命名与代码风格约定。用于统一字段、属性、方法、常量的写法。
- [Unity Manual: Texture Import Settings](https://docs.unity3d.com/6000.0/Documentation/Manual/class-TextureImporter.html)
  官方纹理导入设置参考。像素素材课（15a）的主要资料；Texture Type、Pixels Per Unit、Filter Mode、Compression 的取值以这里为准，子页含 Sprite (2D and UI) 的完整选项。
- [Unity Manual: Version Control](https://docs.unity3d.com/Manual/VersionControl.html)
  Unity 与版本控制的官方说明。用于理解哪些目录该进仓库、`.meta` 文件为什么重要。
- [GitHub: Unity.gitignore](https://github.com/github/gitignore/blob/main/Unity.gitignore)
  官方维护的 Unity 忽略规则。每个新 Unity 工程都应从它开始，而不是手写一堆规则。
- [Conventional Commits 1.0.0](https://www.conventionalcommits.org/en/v1.0.0/) · [Chris Beams: How to Write a Git Commit Message](https://cbea.ms/git-commit/)
  提交信息约定与写作原则。本项目采用轻量 Conventional Commits；提交类型只是团队约定，不是 Git 的硬性规定。
- [Game Programming Patterns](https://gameprogrammingpatterns.com/)
  免费在线书。用于理解状态机、对象池、组件、观察者等模式在游戏中的实际用途；后期设计系统时查阅。
- [Unity 2D Game Kit](https://learn.unity.com/project/2d-game-kit)
  官方 2D 示例项目。用于观察一个完整 2D 项目的场景、预制体与脚本组织方式。

## Wisdom (Communities)

- [Unity Discussions](https://discussions.unity.com/)
  官方社区。用于查具体报错、编辑器行为与版本差异；提问时附版本、复现步骤和最小代码。
- [r/Unity2D](https://www.reddit.com/r/Unity2D/)
  2D 开发者聚集地。用于看真实项目的做法、美术工作流与常见坑。
- [r/gamedev](https://www.reddit.com/r/gamedev/)
  更广泛的独立开发社区。用于试玩反馈、范围控制与发布经验。
- [itch.io Game Jams](https://itch.io/jams)
  用短周期 Game Jam 检验“能否在限制内交付一个可玩版本”；这是比教程更真实的反馈场。

## Asset Sources & Licensing

- [Kenney Assets](https://kenney.nl/assets)
  以 CC0 为主的 UI、2D、3D 与音效素材。用于原型和占位资产；仍然逐包核对许可与署名要求。
- [OpenGameArt](https://opengameart.org/)
  社区素材库，授权混合。用于寻找 2D、音频和字体；必须逐个素材确认许可证，不能按站点整体判断。
- [itch.io Free Game Assets](https://itch.io/game-assets/free)
  独立素材市场，授权差异大。用于寻找风格化素材；以作者页面和随包许可文件为准。
- [Google Fonts](https://fonts.google.com/)
  字体来源，多为 SIL Open Font License。用于 UI 字体；先确认字形覆盖，尤其是中文和符号。
- [Freesound](https://freesound.org/)
  音效素材库，可筛选 CC0/CC BY。用于短音效；逐条记录作者、许可与原始链接。
- [Game-icons.net](https://game-icons.net/)
  图标库，常见 CC BY 3.0。用于 UI 图标；按许可完成署名。
- [Creative Commons: CC0](https://creativecommons.org/publicdomain/zero/1.0/) · [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)
  许可原文。用于判断商用、修改和署名要求；以原文为准。
- [SIL Open Font License](https://openfontlicense.org/)
  字体许可说明。用于确认字体嵌入、修改和分发边界。
- [Fusion Pixel Font](https://github.com/TakWolf/fusion-pixel-font)
  支持简体中文的像素字体，字体使用 OFL 1.1、仓库代码使用 MIT。用于中文 HUD；下载 release 后保留 LICENSE-OFL，并生成 TMP Font Asset。

## Gaps

- 面向零基础、同时讲 Unity 6 工程规范与文档流程的中文系统资料仍偏少；课程将以官方英文文档 + 中文讲解组合补齐。
- 像素素材的导入与许可管线已由课程 15a 落地（Unity 官方 Import Settings + CC 许可原文 + 既有 Kenney 先例）；剩余缺口：自绘像素美术的生产工具链（Aseprite）与九宫格 UI 切片，留到需要时再专项检索。
