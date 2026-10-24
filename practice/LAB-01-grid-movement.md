# Lab 01 · 闭卷重写网格移动

**时间**：30–40 分钟  
**前置**：完成课程 05  
**允许**：Unity 官方文档、当前工程的类名与字段名  
**不允许**：复制课程代码、参考实现或让 AI 直接生成整段答案

## 目标

在 `Assets/Scripts/Labs/GridMovementLab.cs` 中新建临时脚本（在 Unity 里创建 `Labs` 文件夹和脚本），不看 `PlayerGridMover.cs`，独立写出四个核心部分：

1. `MazeBootstrapper.IsWalkable(Vector2Int cell)`：越界和墙返回 `false`，其他格子返回 `true`。
2. `MazeBootstrapper.CellToWorld(Vector2Int cell)`：把格子坐标换成世界坐标，并考虑 Maze 根对象的 Transform。
3. `ReadDirection()`：读 W/A/S/D 或方向键，每次按键只返回一次方向；没按键返回 `Vector2Int.zero`。
4. `TryMove(Vector2Int direction)`：先算目标格，验证可走，再更新当前格和世界位置；失败时不改状态。

可以先用伪代码写出数据流，再写成 C#。不要先打开参考实现。

写代码前先做一次“类型契约”：把四个方法的参数和返回类型写出来，说明为什么是这个类型。IDE 的补全可以帮你补 API 名称，但不能替你决定类型和数据流。

## 验收用例

- 从 `(1,1)` 向上按一次，目标格是 `(1,2)`。
- 目标格是墙 `#` 时，位置和 `currentCell` 都不变。
- 目标格越界时，不抛异常，也不移动。
- 按住 W 不放，只移动一格；再按一次才移动第二格。
- Maze 根对象移动到 `(5,5)` 后，玩家仍然落在格子中心。

## 自检问题

1. 为什么 `currentCell` 要在验证成功之后才更新？
2. 如果把 `wasPressedThisFrame` 改成 `isPressed`，会发生什么？
3. `index % Width` 和 `index / Width` 分别代表什么？
4. 如果 `CellToWorld` 不用 `TransformPoint`，Maze 移动后会出现什么问题？
5. 哪一行是“读取输入”，哪一行是“提交状态”？

## 完成后对照

再打开 [solutions/0002/PlayerGridMover.cs](../solutions/0002/PlayerGridMover.cs) 和 [solutions/0004/MazeBootstrapper.cs](../solutions/0004/MazeBootstrapper.cs)。只记录三类差异：

- 你漏掉的边界情况；
- 你写出了但说不清原因的代码；
- 参考实现里可以当模板、不需要背的 API。

把这三类差异写进 `learning-records/`，下一次 Lab 前先回顾。
