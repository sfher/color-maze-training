using System;
using System.Collections.Generic;

public class PlayerProgress
{
    private readonly Dictionary<UpgradeType, int> _levels =
        new Dictionary<UpgradeType, int>();

    public event Action Changed;

    public int Coins { get; private set; }

    public int GetLevel(UpgradeDefinition upgrade)
    {
        return upgrade != null
            ? GetLevel(upgrade.Type)
            : 0;
    }

    public int GetLevel(UpgradeType type)
    {
        return _levels.TryGetValue(type, out int level)
            ? level
            : 0;
    }

    public int GetCost(UpgradeDefinition upgrade)
    {
        return upgrade != null
            ? upgrade.CostFor(GetLevel(upgrade))
            : 0;
    }

    public PurchaseResult TryPurchase(
        UpgradeDefinition upgrade)
    {
        if (upgrade == null)
        {
            return PurchaseResult.InvalidUpgrade;
        }

        int level = GetLevel(upgrade);

        if (level >= upgrade.MaxLevel)
        {
            return PurchaseResult.MaxLevel;
        }

        int cost = upgrade.CostFor(level);

        if (Coins < cost)
        {
            return PurchaseResult.NotEnoughCoins;
        }

        Coins -= cost;
        _levels[upgrade.Type] = level + 1;
        Changed?.Invoke();
        return PurchaseResult.Purchased;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Coins += amount;
        Changed?.Invoke();
    }
}
