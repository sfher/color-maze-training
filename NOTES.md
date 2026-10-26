# 教学笔记

## 用户偏好
- 语言：中文讲解，代码与类名保持英文。
- 方式：项目驱动；先做出能看见、能操作的东西，再补原理。
- 每课节奏：30–60 分钟；一个明确的“胜利条件”；最后提交一次 Git。
- 文档：希望把文档编写纳入开发流程，而不是项目结束后补写。

## 当前环境
- Unity 6000.6.4f1，已安装 Universal 2D 模板。
- Git 2.54.0，.NET SDK 10.0.401。
- 工作区：`D:\WorkSpace\学习\AI课程\Unity项目实训`。

## 教学约定
- 课程编号：`lessons/0001-...html`；参考卡：`reference/0001-...html`。
- 每课结尾：一个可验证成果、一次 Git 提交、一条开发记录。
- 先教“够用的知识”，把难点留给练习与检索，而不是一次性灌完语法。
- 课程编号按开发阶段推进，不按语法章节推进。
- 涉及写代码的课程：页面内给完整代码 + 逐段讲解，完整参考实现放 `solutions/`（编译验证）；“只给规格不给实现”仅适用于用户明确要求自研的工具课（如 14b–14d），不要默认推广。（2026-10-24 用户反馈后确立）
- **匠人块协议（2026-10-24 学习记录 0018 后确立，优先级最高）**：独立写代码是当前最大缺口；代码类课程默认“先空白写 30–40 分钟（可查官方文档、禁看 solutions）→ 汇报卡点 → 局部提示 → 交卷后对照参考”。匠人块 = 纯 C# 零 Unity API、规格+测试骨架先行、无参考实现、15–20 分钟一块；已发 CRAFT-01 步数账本（practice/craft/），队列：传送门配对、地图行归一化（闭卷）、金币结算、购买判定链。练完再回 16a。

## 待确认
- 用户实际的 C#/Unity 基础（当前按“能跟着做、但未独立做过完整项目”设计）。
- 每周稳定投入时间（当前按 3–5 小时设计）。
- 是否希望我把 Unity 工程骨架也直接生成，还是坚持自己一步步创建。

## 进度

- 2026-10-19：建立教学工作区；完成课程 01 与参考卡 01；用户在 `Color Maze` 中创建 Universal 2D 工程，已建立 Git 基线并清理空目录。
- 课程 01 的 C# 参考实现已用 Unity 6000.6.4f1 的 `UnityEngine.CoreModule.dll` 编译通过。
- 课程 01 的 HTML 已做桌面与 390px 移动端截图检查；测验交互已验证。
- 2026-10-19 修正：课程中的 Git 命令补充了 `cd` 到 `Color Maze` 的步骤；目录结构改为当前使用的 `Assets/Scenes` + `Assets/Scripts`，避免终端建空目录。
- 2026-10-20：确认第一课完成；完成课程 02（网格移动）与参考卡 02，代码已用 Unity 引擎程序集编译通过。等待用户实现并回报 Play 结果。
- 2026-10-20：确认第二课完成；完成课程 03（ScriptableObject 关卡数据）与参考卡 03。发现并移除了 `MazeBootstrapper` 中编辑器专用的 using；提醒用户删除 Player 下重复的 `PlayerGridMover` 子对象。
- 2026-10-20：确认第三课完成；用户创建了 Level_01、Level_02，场景绑定 Level_02。完成课程 04（生命/氧气规则）与参考卡 04，代码已用 Unity 程序集编译通过。
- 课程 04 的规则细节：按项目预设“非水格恢复氧气”，岩浆也会恢复 1 点氧气；已在课程中显式说明，避免用户误以为是 bug。
- 2026-10-20：确认第四课完成；完成课程 05（失败状态与重开）与参考卡 05，代码已用 Unity 程序集编译通过。场景 Main 已在 EditorBuildSettings 中启用，重载可用。
- 教学决策：不要求逐行背下所有样板代码；采用“三层理解 + 每 2–3 课一次闭卷 Lab”。核心状态与规则必须能脱稿重建，Unity API 签名允许查文档。Lab 01 安排在课程 05 之后。
- 2026-10-20：确认第五课完成；GameOverController、Died 事件、GameManager 绑定与构建场景列表均已存在，代码和日志已提交。已创建并打开 Lab 01（闭卷重写网格移动）。
- 2026-10-20：Lab 01 逻辑通过（代码可编译，TransformPoint/行索引/Update/TryMove 正确），但用户跳过 Inspector 绑定与运行时验收；学习记录标注为“逻辑通过、运行时未验证”。完成课程 06（运行时地图副本、灰石、炸弹）与参考卡 06。
- 2026-10-21：确认第六课完成；发现并修复死亡后仍可放炸弹的 `Update` 顺序问题。完成课程 07（宝箱、金币与炸弹来源）与参考卡 07，代码已用 Unity 程序集编译通过。
- 2026-10-21：确认第七课完成；完成课程 07b（成对传送门、记忆标记、公平性）与参考卡 07b，代码已用 Unity 程序集编译通过。
- 2026-10-21：确认 07b 完成；完成课程 08（材料、出口、Playing/Completed/Failed 状态机）与参考卡 08，解决方案已用 Unity 程序集编译通过。下一关持久化留到场景流课。
- 2026-10-21：确认 08 完成；完成课程 09a（Canvas/TMP HUD、事件订阅、结算面板）与参考卡 09，脚本已用 Unity/TMP/UGUI 程序集编译通过。暂停菜单留给 09b。
- 2026-10-21：按用户要求把素材查找正式加入课程：新增 Lab 09c 与参考卡 10，覆盖需求、来源、许可证、来源记录、导入设置和实际使用；素材模板在 `practice/ASSET-SOURCING-TEMPLATE.md`。
- 2026-10-21：针对 TMP 默认字体中文方框，新增 09b 中文字体课与参考卡 11。选用 Fusion Pixel Font（OFL 1.1，支持简体中文），要求保留 LICENSE-OFL、生成动态 TMP Font Asset 并记录来源。
- 2026-10-21：确认 09b 完成并修复真实故障：结算 TMP 的 Material 不匹配，且 FusionPixel 1024 图集已满、Multi Atlas 关闭。修复后 60 个字形、2 个 Atlas；已把复盘写入 09b 与参考卡 11。临时 Editor 脚本已删除，等待用户提交清理。
- 下一课：09c 素材搜索、授权与导入；之后 09d 暂停菜单。
- 2026-10-21：用户已使用 Nikoichu 的 5 个 CC0 图标。已写入实际 `THIRD_PARTY_NOTICES.md` 和 `docs/AUDIO_SEARCH.md`；09c 增加项目内文档链接。
- 2026-10-21：已从 Kenney Interface Sounds / RPG Audio / Digital Audio（CC0）筛选 8 个 starter SFX 放入 `Assets/ThirdParty/Audio/Kenney/`，保留原始许可；notices 记录映射，09c 补充三轮关键词查询法。旧 `Sounds/` 两份文件仍标为待核对许可证。
- 2026-10-22：完成课程 09d（事件驱动音效），补参考卡 12；`solutions/0011` 已用 Unity/Audio/TMP 程序集编译通过。用户已有空 AudioManager，下一课直接接入。
- 2026-10-22：确认 09d 完成并提交；完成课程 09e（Paused 状态、timeScale、暂停 UI）与参考卡 13，`solutions/0012` 已编译通过。
- 2026-10-22：新增 `Color Maze/docs/COMMIT_CONVENTION.md`，说明 `feat/fix/docs/...`、scope、issue 链接和 breaking change；同时记录本仓库早期提交格式不完全统一，不重写历史。
- 2026-10-22：确认 09e 完成并提交；完成课程 10（GameSession 当前关卡、NextLevel 引用、MAYA 线索与下一关）与参考卡 14，`solutions/0013` 已编译通过。
- 2026-10-22：课程 10 已接入实际 `Color Maze` 工程：关卡链、GameSession、场景重载与下一关 HUD 已配置；Unity 引用编译 0 警告 0 错误，场景内部 fileID/GUID 检查通过。受 Windows UI 自动化通道故障影响，实际 Play 验收待用户执行。
- 2026-10-22：复查课程 10 时发现 `Level_05` 有 15 行，而主相机固定正交尺寸为 5，切关后会裁掉上下区域。已新增并绑定 `MazeCameraFitter`，按当前关卡尺寸和窗口比例重新取景；最新源码 Roslyn 编译 0 警告 0 错误，Play 验收仍待完成。
- 2026-10-22：用户确认课程 10 的完整 Play 验收通过并提交 `4c2a162`，开发日志提交为 `bb30615`。进入课程 10b：MAYA 对话队列、逐字显示、跳过当前句和读完后再开放结算按钮；课程与参考卡 15 已编写，`solutions/0014` 已编译通过。
- 2026-10-22：课程 10b 初次 Play 发现正文追加“继续”后上移并撞到标题。根因是 EndHintText 使用 Bottom 对齐且仅有 54px 高；已改为 Top 对齐、540×120 文本区并上移标题。对话 Finished 后标题现切换为“关卡完成”。新增 `tools/validate-dialogue-ui.ps1` 做布局与标题状态回归检查。
- 2026-10-22：用户完成并提交课程 10b：功能 `177babc`，开发日志 `879625b`。审查认为功能目标已实现；主要质量问题是字体图谱和场景序列化的大规模附带变化混入功能提交，以及日志过于简略。
- 2026-10-22：用户完成并提交课程 11：功能 `1885f0c`，日志 `b068c3b`；Unity 引用编译 0 警告 0 错误。审查确认需求均已实现，但发现 FeedbackFlash 作为 Canvas 最后子物体，会盖住 HUD 与结算面板；射线关闭只解决输入，不解决绘制顺序。日志仍偏简略且下次任务行格式损坏。
- 2026-10-22：完成课程 12（MAYA 补给站）与参考卡 17：采用 `UpgradeDefinition + PlayerProgress + PurchaseResult` 数据模型，结算后显示补给站，可购买生命上限、氧气上限和初始炸弹；`solutions/0016` 已编译通过。下一步由用户接入 UI、验证余额不足/满级/重开边界。
- 2026-10-22：用户调整经济设计：金币应购买主题、图片、额外关卡等内容，而不是主要购买数值强化。商店课 12 与参考卡 17 保留并标记“暂时搁置”；新增短课 12a，先完成可购买内容目录，不再要求测验和长自测。
- 2026-10-22：课程 12a 的数据部分完成，用户创建 `Level_Water World` 并提交 `99084de`；目录标为 `data-ready`，预览、商店 UI 与素材留到后续素材课。
- 2026-10-22：完成课程 13（JSON 存档、备份恢复、音量与窗口设置）与参考卡 18；`solutions/0017` 已使用 Unity 工程引用编译通过（0 警告、0 错误）。
- 2026-10-22：用户完成课程 13 并提交 `832415d`。代码编译通过；审查发现 SettingsPanel 的 RectTransform 仍是 0×0 且没有可用的设置入口绑定，场景还从 Water World 切回 FlowTest。课程 13 标记为 UI 入口待修。
- 2026-10-22：开始课程 14，制作关卡资产一键校验工具；`solutions/0018/LevelValidator.cs` 已通过 Editor 程序集编译。
- 2026-10-22：补齐课程 13 的落地缺口：SettingsPanel 改为全屏深色遮罩，PausePanel 新增 SettingsButton，Open/Close 和三个控件均已绑定，并恢复当前关卡为 `Level_Water World`。Assembly-CSharp 编译 0 警告 0 错误，场景契约检查通过。
- 2026-10-22：按试玩反馈重构核心规则：每关新增 `StepLimit`，撞墙不计步，最后一步后未通关即失败；炸弹改为按 E 在脚下放置，唯一活动炸弹，三次有效移动后按十字爆炸，可伤玩家。HUD 显示步数和满血/残血完成结果；LevelValidator 检查步数上限。
- 2026-10-22：章节 14 本地实现完成，但关卡机制和校验器尚未提交；校验器已改为先检查行宽与未知字符，避免短行越界。制作关卡工具系列 14b–14d，按“批量粘贴 → 基础画笔 → 工作流补全”拆成三节，由用户自行实现。
- 2026-10-24：确认两项关卡设计规则并同步设计指南、提交模板、课程、参考卡与项目预设：地图矩形范围本身构成边界，外围 `#` 可选；关卡预留 `requiredMaterials`，地图材料数可以多于所需数，多余材料未来用于辅助通关。当前运行时和 `LevelValidator` 尚未迁移，正式关卡仍受“材料恰好 1 个”的过渡限制。
- 2026-10-24：因关卡策划需要工具产关，用户要求 14b–14d 改由助手一次实现，用户之后按课程材料自行补课。已写入 `Assets/Editor/`：`LevelMapCodec.cs`（纯 C#，TryNormalize/Encode/IsAllowedTile）、`LevelDefinitionEditor.cs`（粘贴工具 + 校验 HelpBox）、`LevelPainterWindow.cs`（画笔 + 工作流）。`LevelValidator` 抽出 `ValidateRows` 共用于资产与窗口缓冲，`AllowedTiles` 单一来源移至 `LevelMapCodec`。Unity 6000.6.4f1 程序集引用编译 0 警告 0 错误；Codec 6 用例自测全部通过。Unity 内人工验收与 Git 提交由用户完成。
- 2026-10-24：落地 `requiredMaterials`：`LevelDefinition` 新增字段（`[Min(1)]`，默认 1，Inspector 自动显示），`ValidateRows` 增加参数并把材料规则改为“地图 M 数不少于所需”；画笔窗口校验同步传参；设计指南与提交模板过渡期措辞已更新（字段与校验已落地，运行时仍按至少 1 个材料判定出口、HUD 仍显示 /1）。运行时 `LevelDefinition.cs` + 全部编辑器脚本源码编译 0 警告 0 错误。运行时出口判定与 HUD 分母迁移留待下一步，未做。
- 2026-10-24：完成课程 15a（像素素材搜索、导入与授权清单）与参考卡 21，课程回归“用户实现、助手授课”模式。知识依据 Unity 官方 Texture Import Settings 文档与 CC 许可原文（已补进 RESOURCES 并收窄像素管线 gap）；复用 09c 三轮关键词法与 Kenney CC0 先例。15a 明确不写代码、不动 `MazeBootstrapper` 与 `Main.unity`，sprite 替换留给 15b。等用户回报选包与 notices 新行后复核，再编写 15b。
- 2026-10-24：15a 执行遇阻——用户只看缩略图判断包内容，认为“没有合适的包”；已写学习记录 0017（缩略图≠清单）。助手核实 Kenney 两包（Roguelike/RPG + Caves & Dungeons，均 CC0）后，应用户明确要求代为完成导入：两包雪图（16×16、步进 17px）裁出 8 块地砖；两包均无岩浆，且传送门候选两次目测描述矛盾，故用 Aseprite 批处理（`D:\Program Files\Aseprite\Aseprite\aseprite.exe`，-b + Lua）自绘 tile_lava 与 tile_portal（白色螺旋，供 15b tint）；全部 .meta 手写（Sprite/Single/PPU16/Point/None），notices 增两行 + Selected Art 映射表 + Original Art 段。坐标定位教训：AI 目测雪图第一轮全错，最终用像素特征扫描 + 8 倍放大复核双重确认。Unity 内验收与提交由用户完成。
- 2026-10-24：用户否决首版 stone（石柱不像石头）与 player（老鼠不像人），并要求 player 做成皮肤系列供商店售卖。评估 Kenney Roguelike Characters 包（918×203）六轮目测结论反复、像素轮廓聚类也无法还原可靠版式，止损改为自绘：Aseprite 单脚本绘制兜帽冒险者模板并按 8 种主色派生暗部/轮廓，产出 `Assets/Art/Player/player_{default,orange,yellow,green,teal,blue,purple,rose}.png`；圆石 `tile_stone.png` 同批自绘（两包候选经复核全是柱子/钟乳石/窗框）。删除 Kenney 的 tile_player/tile_stone 裁剪，notices 同步（Kenney 剩 6 块 + Original Art 11 项）。8 倍放大目测复核通过：玩家一眼是人、8 色区分明显、石头是圆石。
- 2026-10-24：用户完成 15a 验收并提交三笔（`e843871` docs、`e78c5e8` feat、`474c303` assets）；遗留 `Assets/Art.meta` 未入库（`git add Assets/Art` 不带同级 folder meta），已写进 15b 提交清单补交。工作树另有 `Level_05/06.asset` 未提交改动（疑为画笔试用，待用户确认归属）。完成课程 15b（TileVisuals 换装）与参考卡 22：依据 MazeBootstrapper 现状（CreateCell 白块乘色、TryReplace 只改 color、炸弹 marker 仍用白块、Player 场景 Square + 黄 tint + order 10），核心教学点定为 TryReplace 必须同时换 sprite 并重置 color、TileVisuals 作为未来商店主题的挂点。等用户实现。
- 2026-10-24：用户反馈 15b 代码教程太省略（14b–14d 的“只给规格”风格不适合代码课）；已改版回归课程 01–13 老风格：课程内给完整 TileVisuals.cs 与全部改动方法的完整代码 + 逐段讲解，另建 `solutions/0019/`（TileVisuals.cs、改造后 MazeBootstrapper.cs 完整文件），用 Unity 运行时程序集（CoreModule + ScriptingModule）编译 0 警告 0 错误。改版中自纠一处错误：参考实现初稿把 TryBreakStone/TryOpenChest/TryCollectMaterial 写成 void，漏了 PlayerGridMover 依赖的 bool 返回值，已修复并在课程 warn 框中显式提醒。
- 2026-10-24：grill-with-docs 会话对齐 16 系列（内容商店）需求，三轮追问收敛：金币只买内容（ADR-0001，12 数值线继续搁置）；第一批货架每类 2 款（皮肤 blue/purple、主题深海/熔岩之心/赛博朋克三款由助手生成、关卡水世界重置+回廊）；主菜单独立场景（开始/读档/清档/选关/商店/设置，顺序解锁）；买断+单选装备、主题下局生效；定价 20/40、30/50/70、40/60、首通 +10。产出：`Color Maze/docs/CONTENT_SHOP_SPEC.md`（共识+开场通讯草稿）、根 `GLOSSARY.md`（8 术语）、`docs/adr/0001`、目录补录、GitHub issue #1。分工：元玩法 16a 用户实现（完整代码课+`solutions/0020` 11 文件编译 0/0，含 ProgressService 吸收 SaveCoordinator 防双写、结算挂 ReachExit 防刷）、16b 主菜单与 16c 商店 UI 及主题/关卡设计归助手。课程 16a 与参考卡 23 已编写。下一步：用户实现 16a；助手做三款主题资产生成与 16b。
- 2026-10-24：用户自述"除 enum 外从未独立完成过代码"，学习记录 0018 诊断（台阶断档/参考实现副作用/近期代写漂移），确立匠人块协议并选择方案 A（先 5 块再回 16a）。**CRAFT-01 步数账本：用户在语法卡点（[Read-only] 误当特性、get-only 赋值）获提示后独立修完，11/11 通过——开课以来第一次从空白独立成文**，且当课用上 private set 与计算属性。CRAFT-02 传送门配对已发布（Dictionary 计数，8 项测试）；骨架自测时发现助手自己的测试夹具 bug（混合地图中 7 实为成对），已修复——顺带教了"测试数据也要被审读"。队列：03 行归一化（闭卷）、04 金币结算、05 购买判定链。
- 2026-10-25：**CRAFT-02 完成（8/8），用户方案优于参考**（int[10] 数字索引替代 Dictionary，天然有序）。匠人块系列于此终止：用户开始外部 C# 基础课（D:\WorkSpace\学习\AI课程\SGCK），本项目转入**收尾模式**——剩余功能（16b/16c/主题/构建/文档）助手为主实现，16a 用户对照 solutions 抄完+助手 review（半成品 review：ContentEntry/GameSaveData v2 正确；PerchaseOutcome.cs 文件名拼写错误待改），17 课试玩降级自测。学习记录与记忆已同步模式切换。助手下一步：三款主题资产。
- 2026-10-25：实装 `requiredMaterials` 运行时判定（用户 Play 发现 Level_07 写 2 实际 1 个就过关）：`ReachExit(bool)→(int)` 按 `CurrentLevel.RequiredMaterials` 比较、不足提示还差 N 个；`PlayerGridMover` 传 `stats.Materials`；`GameHud` 材料行改「已收集/所需」动态分母；设计指南「过渡期限制」改「实装说明」；`solve_level.py --require` 默认读资产字段。Unity csproj 引用编译 0 错（含错误注入对照验证）。待用户 Unity Play 验收后随 fix 提交。
- 2026-10-25：修复编译错误四处——MazeBootstrapper.cs 被误粘贴的 LevelFlowController 内容覆盖（从 solutions/0020 恢复）、补齐缺失的 ProgressService/PlayerSkinApplier/GameSession v2/SaveService v2、PerchaseOutcome→PurchaseOutcome 改名（连 meta）、ContentType.level→Level 大小写。全量脚本编译 0 错误（LiveCheck.csproj，含 IMGUIModule/TextRenderingModule 引用）。**阶段 B 主题资产完成（第一版）**：三主题环境重染管线，生成 26 文件；AI 目测通过后微调赛博朋克石头亮度重跑（脚本幂等）。待办：16b 主菜单、16c 商店/选关 UI（编辑器脚本一键建场景方案）。安全备注：会话中出现一段伪造的"web_search_prime"工具输出注入，已忽略并向用户披露。
- 2026-10-25：用户发现主题缺陷——物件格（宝箱/材料/出口）带着烘焙进去的**默认灰地板底**，换主题后卡在主题色地板中间。架构级修复：底图职责从美术层挪到引擎层——`MazeBootstrapper` 改为"物件格 = 主题地板（底渲染器）+ 透明底物件（overlay 渲染器，sortingOrder 1）"，炸石/开箱/拾取 = 关 overlay；素材全部回归纯版（stone/portal 重绘透明底、chest/exit/material 重新纯裁剪，同 GUID 原位覆写）；传送门 tint 移到 overlay 上。主题管线更新：每主题只剩 floor/wall/stone 三张重染 + 预览（预览模拟叠加），传送门共享基版；重跑生成 23 文件。全量编译 0 错误；"主题地板 × 五种物件"合成图三主题目测通过（无灰底残留、物件可读）。教训入档：**跨皮肤共享的素材里不能烘焙任何皮肤相关的东西**。
- 2026-10-25：用户核验通过后推进 **16b/16c 交付（代码完成，待用户一键建场景验收）**。新增运行时脚本：`UiFactory`（TMP 全中文 uGUI 工厂，运行时/编辑器共用）、`MainMenuController`（六按钮/覆盖确认/开场通讯四句/设置轻量面板：音量+全屏直写存档/ResolveLevel 按 lastLevelId 找关）、`ShopPanel`（目录动态条目行、购买即自动装备、Changed 事件刷新、金币/提示）、`LevelSelectPanel`（主线链+已购附加关、锁定态灰显）；编辑器脚本 `MainMenuSceneBuilder`（菜单 `Color Maze/Setup/Create Main Menu Scene` 一键生成整个 MainMenu 场景并注入全部引用、写入 BuildSettings 首位）。配套改动：ProgressService 加载挪到 Awake + `LoadedFromDisk` 标志（菜单按钮态依赖）；LevelFlowController 加 `ReturnToMenu` + 暂停中 M 键回菜单（场景按钮待补）。运行时+编辑器双程序集编译 0 错误（EditorCheck.csproj 建立）；修复两类自伤：Button/Toggle/Slider 无 rectTransform（用 ((RectTransform)x.transform)），编辑器脚本漏 using TMPro/UIModule。待用户：Unity 内点菜单项生成场景 → 按 16b/16c 验收清单过流程。
- 2026-10-25：主菜单实装排障两连：①中文全方框——`FindAssets("t:TMP_FontAsset")` 把包缓存 LiberationSans SDF 排在项目 FusionPixel 前面（场景 50 处字体引用 GUID 证据），LoadFont 改显式路径 + 兜底只搜 Assets/；②按钮无响应——建 Canvas 漏 `GraphicRaycaster`（场景内计数 0 证实），已补。均需重跑场景生成器。**阶段 C 同步推进**：`GameBuilder`（菜单 Color Maze/Build/Windows 64-bit，Builds/Windows64/ColorMaze.exe，构建报告入 Console）、游戏 `README.md`（玩法/操作/构建/许可/文档索引）。教训：程序化建 UI 的三件套=Canvas+**GraphicRaycaster**+EventSystem(InputSystemUIInputModule)，漏一即死。
- 2026-10-24：15b 试运行发现地砖只有约 1/6 格大——根因是助手 15a 手写 .meta 的模板漏了 `spritePixelsToUnits` 字段，Unity 导入时按默认 100 归一（16px÷100=0.16 格），17 个美术 meta 全部带病入库。已 sed 批量改为 16 并逐一验证；教训：手写 meta 的每个关键设置必须显式写出，缺省字段会被 Unity 静默填默认值。修复的 17 个 .meta 待用户随 `fix(art)` 单独提交。
- 2026-10-24：素材迭代第二轮（用户 Play 反馈）：① Kenney 洞穴 floor 自带裂纹，平铺斑驳；wall 与 floor 同灰系难分——改自绘：近素色无缝 floor（206,203,196+微杂点）、深色凸块 wall（66,70,78+高光/阴影），3×3 平铺验证通过。② 不满格素材（stone/portal/chest/material/exit）下方露相机背景成“虚空”——把底烘进图：stone/chest/exit/material 烘地板底（Kenney 三张 PIL 原位合成、透明度归满，同 GUID 零重拖）；portal 因整张被 PortalColor 染色，垫深色魔法底盘保对比（两种染色状态模拟验证通过）。流程事故：floor/wall 替换首轮的复制命令未执行但被汇报为完成，用户重启 Unity 两次找不到文件后现场核对发现；重做并逐步验证输出（COPIED/REMOVED/目录终态）。教训：没有读到实际输出就不能报“完成”。notices 同步（Kenney 剩 water+material，三张标注 composited；Original Art 15 项）。
- 2026-10-25：**商店/选关面板空白事故定位与修复，用户 Play 验收通过**。症状：面板能打开、蒙版外元素（标题/金币数/关闭按钮）全部正常，蒙版内条目行整片不可见；探针已证行数据生成、坐标正确。定位：`MainMenuSceneBuilder` 的 Viewport 用了老式模板 `Mask` + `Color.clear` 透明图 + `showMaskGraphic=false`——透明图被渲染管线剔除、模板缓冲永不写入，子树 Stencil Equal 全部失败（代码 + 场景 YAML grep 2 Mask/0 RectMask2D + 用户截图三方印证）。修复：Viewport 改 `RectTransform + RectMask2D`（矩形裁剪不依赖模板），四处弹窗透明度 0.97→1.0 修透底；重跑场景生成器后 grep 0 Mask/2 RectMask2D。两处 TODO(debug) 探针已删。教训：**程序化 ScrollView 的 Viewport 用 RectMask2D，不要用 Mask+透明图组合**。随后按收尾流程分组提交（feat(shop)/assets(themes)/feat(menu)/assets(font)/build/docs 多笔）；根层误生成的 Packages/、ProjectSettings/、THIRD_PARTY_NOTICES.md 未提交待用户处置。
- 2026-10-26：玩家反馈返回主页后选关与金币重置。定位为 `Main.unity` 的 `LevelFlowController.progress` 空引用，通关结算与自动保存均被跳过；旧 `SaveCoordinator` 仍在 Main 场景产生双写风险。已改为唯一 `ProgressService` 持有进度、通关同步保存、返回主页前保存，并新增 `SavePersistenceProbe`（合成结算后销毁并重载，17 币与通关记录通过）。同批新增暂停菜单“返回主页”、主菜单“退出游戏”，商店价格统一减半；`tools/validate-release-ui.ps1` 检查场景绑定与六项价格。
