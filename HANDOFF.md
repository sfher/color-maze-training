# 色彩迷宫课程 · 会话迁移交接

最后更新：2026-10-24（第二次：14b–14d 工具实现）

## 工作区

- 课程工作区：`D:\WorkSpace\学习\AI课程\Unity项目实训`
- 游戏工程：`D:\WorkSpace\学习\AI课程\Unity项目实训\Color Maze`
- Unity：`6000.6.4f1`，Universal 2D
- 教学技能：使用 `$teach` 的长期教学工作区结构

## 当前进度

已完成：

- 课程 01–08：工程、网格移动、ScriptableObject、生命/氧气、失败、炸弹/灰石、宝箱金币、传送门、材料与出口。
- Lab 01：闭卷网格移动，逻辑通过；运行时验收由用户主动跳过。
- 课程 09a：Canvas/TextMeshPro 正式 HUD、结算面板、事件刷新。
- 课程 09b：Fusion Pixel 中文字体与 TMP Font Asset；修复 Material 不匹配和 Atlas 满图集问题。
- 课程 09c：素材搜索、许可、来源记录；已加入 Kenney CC0 starter audio。
- 课程 09d：事件驱动音效，8 个 Clip 已绑定。
- 课程 09e：暂停菜单、继续/重开、`Time.timeScale`。

课程 10 已完成并通过用户 Play 验收：

- [课程 10 · 通关之后，故事继续](lessons/0010-maya-and-next-level.html)
- 目标：`LevelDefinition.NextLevel` → `GameSession.CurrentLevel` → 场景重载 → 下一关。
- 当前关卡链已重组为 `Level_01 → Level_02 → Level_03 → Level_04 → 结束`。
- `Main` 绑定 `Level_01`；`Level_Water World` 是 `Assets/Content/Levels` 中的侧关卡，不在主线链。
- 已新增 `GameSession`、关卡引用、Maze 的 `CurrentLevel`、流程切换和“下一关”按钮。
- 新增 `MazeCameraFitter`：Level_05 的 15 行地图会自动适配正交镜头，不再沿用固定尺寸 5。
- 最新源码 Roslyn 使用 Unity 生成引用编译通过（0 警告、0 错误）；场景 YAML 引用检查通过。
- 功能提交：`4c2a162 feat(level): add MAYA next-level flow`。
- 日志提交：`bb30615 docs: write lesson 10 devlog`。

课程 10b 已完成并通过用户验收：

- [课程 10b · MAYA 开口以后](lessons/0010b-maya-dialogue.html)
- [参考卡 15 · MAYA 对话与逐字显示](reference/0015-maya-dialogue-cheatsheet.html)
- 目标：关卡对白数组 → 逐字显示 → 跳过当前句 → 最后一句后开放结算按钮。
- `solutions/0014` 已用 Unity 工程引用编译通过（0 警告、0 错误）。
- 已接入实际游戏工程：
  - EndHintText 原为 Bottom 对齐且仅 54px 高，追加“继续”后正文上移撞标题；现改为 Top 对齐、540×120。
  - 对话 Finished 后标题从“通讯接入”切换为“关卡完成”。
  - 回归检查：`tools/validate-dialogue-ui.ps1`。
- 功能提交：`177babc feat(dialogue): add Maya level dialogue`。
- 日志提交：`879625b docs: write lesson 10b devlog`。

课程 11 已完成并通过用户验收：

- [课程 11 · 让每次操作都有回音](lessons/0011-feedback-layer.html)
- [参考卡 16 · 反馈层与界面动画](reference/0016-feedback-layer-cheatsheet.html)
- 目标：用 `GameFeedback` 统一组合 AudioManager 与全屏短闪光。
- `solutions/0015` 提供 `GameFeedback.cs` 与迁移后的 `PlayerGridMover.cs`。
- 功能提交：`1885f0c feat(feedback): add flash feedback on events`。
- 日志提交：`b068c3b docs: write lesson 11 devlog`。
- Unity 引用编译 0 警告、0 错误。
- 审查发现：`FeedbackFlash` 当前是 Canvas 的最后一个子物体，会绘制在 HUD 与结算面板之上；Raycast Target 已关闭，不影响点击，但层级应移到 StatsPanel 之前。
- 日志问题：开发日志只写了三行摘要，且“下次先做什么”保留了表格分隔符；下次提交前应整理成可复用的故障记录。

课程 12 已根据经济方向调整：

- [课程 12a · 先把货架装满](lessons/0012a-store-content.html)：数据部分已完成。
- [商店内容模板](practice/STORE-CONTENT-CATALOG.md)：主题、图片、额外关卡的候选目录。
- [课程 12 · MAYA 补给站](lessons/0012-shop-and-upgrades.html)：保留但标记“暂时搁置”。
- [参考卡 17](reference/0017-shop-upgrades-cheatsheet.html)：保留，等可购买内容完成后再重做商店时复用。
- 新方向：金币主要购买内容与装饰，不优先购买数值强化。
- 用户提交 `99084de`，新增 `Level_Water World`；内容目录状态为 `data-ready`。

课程 13 已提交并补齐缺失 UI：

- [课程 13 · 留一份可以回来找的记录](lessons/0013-save-and-settings.html)
- [参考卡 18 · 存档与设置](reference/0018-save-and-settings-cheatsheet.html)
- 目标：JSON 进度与设置、版本字段、临时文件替换、备份恢复。
- `solutions/0017` 已编译通过（0 警告、0 错误）。
- 用户负责 UI 绑定与边界调试。
- 用户提交 `832415d` 后，主线程补齐 SettingsPanel 全屏遮罩、PausePanel 的 SettingsButton、Open/Close 事件和控件绑定。
- Assembly-CSharp 编译 0 警告 0 错误；场景契约检查通过。

课程 14 已完成本地实现，代码与机制仍待用户整理提交：

- [课程 14 · 一键检查关卡](lessons/0014-level-validation-tool.html)
- [参考卡 19 · 关卡校验](reference/0019-level-validation-cheatsheet.html)
- 目标：用 Editor 菜单扫描所有 LevelDefinition，检查起点、材料、出口和传送门配对。
- `solutions/0018/LevelValidator.cs` 已通过 Editor 程序集编译（0 警告、0 错误）。

课程 14b–14d 已于 2026-10-24 由助手一次实现（关卡策划需要工具产关），待用户在 Unity 内人工验收与提交：

- [14b · 整张地图一次贴进去](lessons/0014b-map-paste-editor.html)
- [14c · 基础关卡画笔](lessons/0014c-level-painter.html)
- [14d · 把编辑器变成工作流](lessons/0014d-level-editor-polish.html)
- [参考卡 20 · 关卡编辑器工具](reference/0020-level-editor-tools-cheatsheet.html)
- 产出：`Assets/Editor/LevelMapCodec.cs`（纯 C# 归一化）、`LevelDefinitionEditor.cs`（粘贴 Inspector + Validator HelpBox）、`LevelPainterWindow.cs`（调色板画笔、Apply/Revert/Undo、New Level 9×9、Resize 保留左上、未保存提示、缓冲实时校验、复制地图文本）。
- `LevelValidator` 抽出 `ValidateRows(rows, stepLimit)` 供资产与窗口缓冲共用，`AllowedTiles` 单一来源在 `LevelMapCodec`。
- 已验证：Unity 6000.6.4f1 程序集引用编译 0 警告 0 错误；Codec 6 用例自测全部通过（CRLF/缩进/首尾空行、行宽、中间空行、未知字符行列定位、全空白、无外墙双 M）。
- 未做：Unity 内人工验收（粘贴坏文本不落盘、Ctrl+Z、画笔左右键、切关提示、新建关卡）。用户之后按 14b–14d 课程材料自行复习补课。

课程 15a 已完成（2026-10-24 用户验收并提交）；课程 15b 已编写，等待用户实现：

- [15a · 找到并接住像素素材](lessons/0015a-pixel-art-sourcing.html) + [参考卡 21](reference/0021-pixel-art-import-cheatsheet.html)：已提交 `474c303`（Kenney 6 块 + 自绘 lava/portal/stone + 8 款皮肤 + notices）。遗留 `Assets/Art.meta` 未入库，随 15b 补交。
- [15b · 把方块换成像素地砖](lessons/0015b-pixel-tile-swap.html) + [参考卡 22](reference/0022-pixel-tile-swap-cheatsheet.html)：新建 `TileVisuals` ScriptableObject（未来商店主题挂点），`MazeBootstrapper.CreateCell` 改取像素地砖、`TryReplace` 换 sprite 并重置 color（本课核心坑），传送门白螺旋沿用 `PortalColor` 染色与记忆机制，Player 场景换 `player_default` + 颜色归白（order 10 不动）。用户实现，助手只做局部提示。
- 提交建议：`feat(visuals): swap runtime squares for pixel tiles`，含 `Main.unity`（本次场景变化是功能本体）。

## 当前游戏状态

已工作的部分：

- 玩家网格移动、生命、氧气、炸弹、金币、材料。
- 灰石爆炸、宝箱、成对传送门、出口、完成/失败。
- Canvas HUD、中文 Fusion Pixel 字体、结算/暂停面板。
- AudioManager 的 8 个 Kenney CC0 音效。

近期验证过：

- 中文 HUD 与结算文本正常。
- 过关音效正常。
- 暂停/继续/重开流程正常。

2026-10-22 核心规则更新：

- `LevelDefinition` 新增 `StepLimit`；撞墙不计步，最后一步后未通关则失败。
- 炸弹改为 E 原地放置，三步引信；爆炸破坏上下左右灰石，玩家在十字范围内受到 1 点伤害。
- 同时只能存在一个未爆炸炸弹。
- HUD 显示剩余步数；结算显示满血完成或残血完成。
- `tools/validate-dialogue-ui.ps1` 之外，关卡校验已增加 `StepLimit > 0`。
- 规则测试：步数不越界、炸弹放置当回合不爆炸、第三次移动后爆炸，共 10 项通过。

2026-10-24 关卡设计规则更新（文档已同步；`requiredMaterials` 与校验已落地，运行时未迁移）：

- 地图矩形范围本身构成边界，不要求用 `#` 围住最外圈。
- 每关包含 `requiredMaterials`（默认 1，可填更大）；`LevelValidator` 要求地图 `M` 数不少于该值，允许更多。
- `LevelDefinition` 新增 `RequiredMaterials` 属性；`ValidateRows(rows, stepLimit, requiredMaterials)` 为资产与画笔共用的校验入口。
- 现运行时仍按“至少持有 1 个材料”判断出口（`PlayerStats.HasMaterial`），HUD 硬编码 `/1`；`requiredMaterials > 1` 的关卡迁移前不能标记为已可玩。

## 当前 Git 状态

远程仓库：https://github.com/sfher/color-maze-training（私有），分支 `master`，`gh` CLI 已配置（agent 技能配置见根目录 `AGENTS.md` 与 `docs/agents/`）。

2026-10-24 应用户要求，助手把全部待办分组提交：`fix(art)`（tile PPU 归 16、无缝地板/墙面、不满格素材烘焙底图）、`feat(visuals)`（15b 像素换装：TileVisuals + MazeBootstrapper + 场景接线）、`assets(level)`（Level_05/06 地图调整）、`chore(assets)`（字库图谱更新）、`chore(repo)`（agent 技能配置 + 根 .gitignore）、`docs(curriculum)`（教学工作区材料首次入库）。

工作树遗留（有意不提交）：根目录 `Packages/`、`ProjectSettings/`、`THIRD_PARTY_NOTICES.md` 为 2026-10-19 建仓当天的重复工程骨架残留，建议人工删除。

Git 根目录是教学工作区，提交永远走显式路径，不要 `git add .`。

课程 12a 的数据目录已完成；商店实现等待主题、图片与关卡预览达到 `ready`。

## 下一会话从哪里开始

1. 14b–14d 工具与 `requiredMaterials` 已提交（`e843871` docs、`e78c5e8` feat）；如 Unity 内人工验收仍有遗漏项，按需补。
2. 用户实现课程 15b（TileVisuals 换装），提交 `feat(visuals)`；`Level_05/06.asset` 未提交改动需先确认归属。
3. 15b 之后：16（性能与 Windows 构建）或按用户意愿先做皮肤/主题商店接口——8 款皮肤与 TileVisuals 已为商店留好挂点。
4. 用户随时对照 14b–14d / 15a / 15b 课程材料提问，助手做局部讲解，不重复授课。

参考实现：

- `solutions/0013/LevelFlowController.cs`
- `solutions/0013/GameSession.cs`
- `solutions/0013/MazeBootstrapper.cs`
- `solutions/0013/GameHud.cs`
- `solutions/0013/MazeCameraFitter.cs`
- `solutions/0014/MayaDialogueView.cs`
- `solutions/0014/GameHud.cs`
- `solutions/0015/GameFeedback.cs`
- `solutions/0015/PlayerGridMover.cs`
- `solutions/0016/PurchaseResult.cs`
- `solutions/0016/UpgradeType.cs`
- `solutions/0016/UpgradeDefinition.cs`
- `solutions/0016/PlayerProgress.cs`
- `solutions/0016/GameSession.cs`
- `solutions/0016/PlayerStats.cs`
- `solutions/0016/UpgradeShop.cs`
- `solutions/0016/GameHud.cs`
- `solutions/0016/GameFeedback.cs`
- `solutions/0017/GameSaveData.cs`
- `solutions/0017/SaveService.cs`
- `solutions/0017/SettingsController.cs`
- `solutions/0017/SaveCoordinator.cs`

课程 14d 完成后，下一课回到素材与商店内容管线。

## 关键文档

- `MISSION.md`：学习使命
- `CURRICULUM.md`：当前课程路线
- `NOTES.md`：教学偏好与进度
- `RESOURCES.md`：高信任资料和素材源
- `Color Maze/THIRD_PARTY_NOTICES.md`：素材来源与许可证
- `Color Maze/docs/AUDIO_SEARCH.md`：音效搜索 brief
- `Color Maze/docs/COMMIT_CONVENTION.md`：提交信息约定
- `Color Maze/docs/LEVEL_DESIGN_GUIDE.md`：外部关卡策划协作指南
- `Color Maze/docs/LEVEL_PATTERNS.md`：短版进阶关卡结构速查
- `Color Maze/docs/templates/LEVEL_SUBMISSION.md`：单关设计说明模板

## 新会话启动提示

把下面这段发给新会话：

```text
$teach

请先读取当前工作区的 HANDOFF.md、MISSION.md、CURRICULUM.md、NOTES.md，
并在 Color Maze 工程根目录执行 git status 和 git log -5。

14b–14d 关卡工具已由上一会话实现（LevelMapCodec / LevelDefinitionEditor /
LevelPainterWindow，LevelValidator 已抽出 ValidateRows）。
当前任务是陪我在 Unity 里跑完人工验收清单，再分 docs 与 feat 两次提交；
之后我自己复习 14b–14d 课程材料，有疑问再局部提问。
不要删除既有课程、参考卡或 Level_Water World，也不要重新初始化工程。
```

## `$teach` 技能可用性

- 个人技能副本：`C:\Users\ASUS\.codex\skills\teach\SKILL.md`
- 原始插件缓存：`C:\Users\ASUS\.codex\plugins\cache\mattpocock\mattpocock-skills\1.3.1\skills\productivity\teach\SKILL.md`
- 该技能设置了 `disable-model-invocation: true`，必须由用户显式输入 `$teach` 调用。
- 安装后需要**新开一轮对话或重启会话**，技能列表才会重新扫描。
- 如果新会话仍未列出 `$teach`，直接让新助手先读取个人技能副本的 `SKILL.md` 和三个格式文件，再继续；课程工作区不依赖技能自动化本身。

## 不要做

- 不要删除 `Color Maze` 工程或重建 Unity 项目。
- 不要删除用户已有的素材、许可证或未确认来源的文件。
- 不要提交 `Library/`、`Temp/`、`Logs/` 等 Unity 缓存。
- 不要把课程 HTML 和游戏工程混成同一个 Unity `Assets` 目录。
