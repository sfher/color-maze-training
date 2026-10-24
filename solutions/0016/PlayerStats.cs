using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public const int StartingMaxHealth = 3;
    public const int StartingMaxOxygen = 3;
    public const int StartingBombs = 1;

    public event System.Action Died;
    public event System.Action StatsChanged;

    public int MaxHealth =>
        StartingMaxHealth +
        GameSession.Progress.GetLevel(
            UpgradeType.MaxHealth);
    public int Health { get; private set; }
    public int Oxygen { get; private set; }
    public int Bombs { get; private set; }
    public int Coins => GameSession.Progress.Coins;
    public int Materials { get; private set; }

    public int MaxOxygen =>
        StartingMaxOxygen +
        GameSession.Progress.GetLevel(
            UpgradeType.OxygenCapacity);

    public bool IsAlive => Health > 0;
    public bool HasMaterial => Materials > 0;

    private bool _deathAnnounced;

    private void Awake()
    {
        Health = MaxHealth;
        Oxygen = MaxOxygen;
        Bombs =
            StartingBombs +
            GameSession.Progress.GetLevel(
                UpgradeType.StartingBombs);

        GameSession.Progress.Changed +=
            HandleProgressChanged;
    }

    private void OnDestroy()
    {
        GameSession.Progress.Changed -=
            HandleProgressChanged;
    }

    public void ApplyTile(TileType tile)
    {
        switch (tile)
        {
            case TileType.Lava:
                LoseHealth(1);
                RecoverOxygen();
                break;
            case TileType.Water:
                ConsumeOxygen();
                break;
            default:
                RecoverOxygen();
                break;
        }

        StatsChanged?.Invoke();
    }

    public bool TryConsumeBomb()
    {
        if (Bombs <= 0)
        {
            return false;
        }

        Bombs -= 1;
        StatsChanged?.Invoke();
        return true;
    }

    public void AddBombs(int amount)
    {
        Bombs += Mathf.Max(0, amount);
        StatsChanged?.Invoke();
    }

    public void AddCoins(int amount)
    {
        GameSession.Progress.AddCoins(amount);
    }

    public void AddMaterial()
    {
        Materials += 1;
        StatsChanged?.Invoke();
    }

    private void HandleProgressChanged()
    {
        StatsChanged?.Invoke();
    }

    private void LoseHealth(int amount)
    {
        Health = Mathf.Max(0, Health - amount);

        if (Health == 0 && !_deathAnnounced)
        {
            _deathAnnounced = true;
            Died?.Invoke();
        }
    }

    private void ConsumeOxygen()
    {
        if (Oxygen > 0)
        {
            Oxygen -= 1;
            return;
        }

        LoseHealth(1);
    }

    private void RecoverOxygen()
    {
        Oxygen = Mathf.Min(MaxOxygen, Oxygen + 1);
    }
}
