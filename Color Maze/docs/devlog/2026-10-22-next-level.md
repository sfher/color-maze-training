# 2026-10-22 · MAYA 与下一关

## 今天做了什么
- 完成关卡后显示 MAYA 线索与“下一关”按钮。
- 通过 `GameSession` 保留当前关卡，支持进入下一关并在当前关重开。
- 将 `Level_FlowTest` 的下一关指向 `Level_05 Lava & Stone`。
- 新增 `MazeCameraFitter`，根据当前关卡宽高和窗口比例调整正交相机，避免 15 行的 Level_05 被裁切。

## 卡在哪里 / 怎么解决
- 旧 `MazeBootstrapper` 错误日志使用 GBK 编码，已转换为 UTF-8，避免混合编码影响后续编辑。
- 切换关卡的静态流程原本已通过，但 Level_05 比旧关卡高很多；固定 `orthographicSize = 5` 只能显示 10 个单位高度。相机适配改用宽、高两个约束中的较大值。
- 当前 Windows UI 自动化通道不可用，实际 Play 验收仍需在 Unity 编辑器内完成。

## 验证
- 最新 `Assets/Scripts` 使用 Unity 工程引用重新编译：0 警告、0 错误。
- `Level_FlowTest.NextLevel`、Level_05 的空 NextLevel、Maze / Flow / HUD / Camera 场景绑定与 Build Profiles 列表已交叉检查。
- Level_05 尺寸为 17×15；在 16:9 下计算出的正交尺寸为 8.25，顶部出口进入镜头。

## 下次先做什么
- 验证下一关、当前关重开、失败隐藏按钮和停止 Play 后恢复初始关。
- 把 MAYA 对话扩展成独立场景或正式对话面板。
