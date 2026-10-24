# Lab 01：闭卷重写网格移动

用户独立重写了 `GridMovementLab` 的四个核心部分：`IsWalkable`、`CellToWorld`、`ReadDirection`、`TryMove`。代码可编译，边界检查、行索引翻转、`Update` 到移动的接线、`TransformPoint` 与位置更新都正确。用户没有在 Inspector 绑定 `mazeRoot`，也没有执行运行时验收，属于设置步骤跳过，不是逻辑错误。

这说明基础网格移动已经能脱稿重建；后续可以提高要求到“写完必须跑一次验收用例”。Lab 记录为“逻辑通过、运行时未验证”。
