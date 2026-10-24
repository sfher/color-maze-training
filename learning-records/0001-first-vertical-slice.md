# 第一课：跑通颜色网格的垂直切片

用户在 `Color Maze` 中独立完成 Universal 2D 工程创建，把 `MazeBootstrapper` 挂到场景对象上，按 Play 生成 7×7 颜色网格，并完成了工程基线、第一屏功能与开发日志的 Git 提交。证据是 `Assets/Scripts/MazeBootstrapper.cs` 存在且可编译、`Main.unity` 中存在脚本组件、Git 历史包含 `feat` 与 `docs` 提交。

这意味着后续可以直接从“输入 → 网格规则 → 移动”开始，不必再重讲 GameObject/Component/Start 的基础。需要继续巩固的是 Inspector 引用绑定、`Update` 生命周期，以及用数据而不是颜色判断可通行。
