# CRAFT-02 · 传送门配对检查

> 匠人块协议不变：空白起步、只看本文件、卡住超 5 分钟报位置。预计 15–20 分钟。

## 游戏背景

地图里数字 `1`–`9` 是传送门：**同一个编号必须恰好出现 2 次**（一对）。出现 1 次或 3 次都是坏地图。其余字符（`. # S R B C M E`）与配对无关。这是 `LevelValidator` 里的真实规则，这次由你写它的核心。

## 规格

写一个静态类 `PortalChecker`，提供一个静态方法：

```csharp
public static List<string> Check(string[] rows)
```

- 遍历所有行的所有字符，只关心 `'1'`–`'9'`；
- 统计每个编号出现次数；
- 出现次数 ≠ 2 的编号，各生成一条错误，**格式精确到字符**：
  `传送门 3 应有 2 个，当前为 3 个。`（编号 = 3、实际 = 3 时）
- 全部成对 → 返回**空列表**（不是 null）；
- 空地图 / 无传送门 → 空列表。

新语法预告（本块唯一新概念）：`Dictionary<char, int>`——键值对容器。`dict[key]` 取值、`dict[key] = x` 写值、`dict.TryGetValue(key, out v)` 安全取值、`foreach (KeyValuePair<char,int> pair in dict)` 遍历。用哪几个随你。

## 开始

骨架：`practice/craft/craft02.cs`（测试 8 项，含三对门全对、单只门、三只门、多编号混合、无门、空数组）。

```bash
cd practice/craft
dotnet run craft02.cs
```

## 验收

- 10 项测试全绿。
- 口头答：你遍历 Dictionary 时，键的顺序是 1、2、3……吗？要保证错误信息按编号排序的话，你会怎么做？（答不出也没关系，写在交卷感想里）

## 交卷

代码或输出贴我。下一篇：CRAFT-03 地图行归一化（闭卷重写 TryNormalize——真正的试金石）。
