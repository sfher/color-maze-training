// CRAFT-02 · 传送门配对检查
// 运行：dotnet run craft02.cs
// 规格：practice/CRAFT-02-portal-pairs.md

// ===== 测试（不要改）=====
int passed = 0;
int failed = 0;

RunTests();

void Check(string name, bool ok)
{
    if (ok)
    {
        passed++;
        System.Console.WriteLine($"PASS  {name}");
    }
    else
    {
        failed++;
        System.Console.WriteLine($"FAIL  {name}");
    }
}

void RunTests()
{
    // 三对门，全部成对
    var good = PortalChecker.Check(new[]
    {
        "#####",
        "#12.#",
        "#..1#",
        "#2..#",
        "#####",
    });
    Check("三对门：无错误", good.Count == 0);

    // 单只门 3
    var single = PortalChecker.Check(new[]
    {
        "#####",
        "#..3#",
        "#...#",
        "#####",
    });
    Check("单只门：一条错误", single.Count == 1);
    Check(
        "单只门：信息精确",
        single[0] == "传送门 3 应有 2 个，当前为 1 个。");

    // 三只门 5
    var triple = PortalChecker.Check(new[]
    {
        "#5..#",
        "#.5.#",
        "#..5#",
    });
    Check("三只门：信息精确",
        triple.Count == 1 &&
        triple[0] == "传送门 5 应有 2 个，当前为 3 个。");

    // 多编号混合：1 对、7 单、9 三
    var mixed = PortalChecker.Check(new[]
    {
        "#1.7#",
        "#1.9#",
        "#..9#",
        "#..9#",
    });
    bool mixedOk = mixed.Count == 2 &&
        mixed.Contains("传送门 7 应有 2 个，当前为 1 个。") &&
        mixed.Contains("传送门 9 应有 2 个，当前为 3 个。");
    Check("混合：只报坏门且各一条", mixedOk);

    // 无传送门
    Check(
        "无门：空列表",
        PortalChecker.Check(new[] { "#..#", "#ME#" }).Count == 0);

    // 空地图
    Check(
        "空数组：空列表",
        PortalChecker.Check(new string[0]).Count == 0);

    // 非门字符不算数
    var withTiles = PortalChecker.Check(new[]
    {
        "#SRB#",
        "#CME#",
    });
    Check("字母地砖被忽略", withTiles.Count == 0);

    System.Console.WriteLine(
        failed == 0
            ? $"全部通过（{passed} 项）"
            : $"{failed} 项失败，{passed} 项通过");
}

static class PortalChecker
{
    public static List<string> Check(string[] rows)
    {
        List<string> answer = new();
        int[] counts = new int[10];

        foreach (string row in rows)
        {
            foreach (char ch in row)
            {
                if (ch >= '1' && ch <= '9')
                {
                    counts[ch - '0']++;
                }
            }
        }

        for (int i = 1; i <= 9; i++) {
            if (counts[i] != 0 && counts[i] != 2) {
                answer.Add($"传送门 {i} 应有 2 个，当前为 {counts[i]} 个。");
            }
        }
        return answer;
    }
}
