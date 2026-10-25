// CRAFT-01 · 步数账本
// 运行：dotnet run craft01.cs
// 规格：practice/CRAFT-01-step-ledger.md

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
    var ledger = new StepLedger(3);
    Check("初始剩余 3", ledger.Remaining == 3);
    Check("初始未耗尽", !ledger.IsExhausted);
    Check("第 1 步成功", ledger.TrySpendStep());
    Check("第 2 步成功", ledger.TrySpendStep());
    Check("剩余 1", ledger.Remaining == 1);
    Check("第 3 步成功", ledger.TrySpendStep());
    Check("已耗尽", ledger.IsExhausted);
    Check("第 4 步被拒绝", !ledger.TrySpendStep());
    Check("剩余不为负", ledger.Remaining == 0);

    var one = new StepLedger(1);
    Check("单步关卡：消耗成功", one.TrySpendStep());
    Check("单步关卡：立即耗尽", one.IsExhausted);

    System.Console.WriteLine(
        failed == 0
            ? $"全部通过（{passed} 项）"
            : $"{failed} 项失败，{passed} 项通过");
}

class StepLedger
{
    // 假设 limit >= 1
    public StepLedger(int limit)
    {
        Remaining = limit;
    }

    public int Remaining { get; private set; }

    public bool IsExhausted => Remaining == 0;

    public bool TrySpendStep()
    {
        if (Remaining > 0)
        {
            Remaining--;
            return true;
        }
        return false;
    }
}
