using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public const int MaxHealth = 3;
    public const int MaxOxygen = 3;

    public event System.Action Died;
    public event System.Action StatsChanged;
    public event System.Action<DamageCause, int> Damaged;

    public int Health { get; private set; } = MaxHealth;
    public int Oxygen { get; private set; } = MaxOxygen;
    public int Bombs { get; private set; }
    public int Coins { get; private set; }
    public int Materials { get; private set; }

    public bool IsAlive => Health > 0;
    public bool HasMaterial => Materials > 0;

    private bool _deathAnnounced;

    public void ApplyTile(TileType tile)
    {
        switch (tile)
        {
            case TileType.Lava:
                LoseHealth(1, DamageCause.Lava);
                RecoverOxygen();
                break;
            case TileType.Water:
                ConsumeOxygen(DamageCause.Drowning);
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
        Coins += Mathf.Max(0, amount);
        StatsChanged?.Invoke();
    }

    public void AddMaterial()
    {
        Materials += 1;
        StatsChanged?.Invoke();
    }

    public void TakeDamage(
        int amount,
        DamageCause cause)
    {
        if (amount <= 0)
        {
            return;
        }

        LoseHealth(amount, cause);
        StatsChanged?.Invoke();
    }

    private void LoseHealth(
        int amount,
        DamageCause cause)
    {
        int healthBefore = Health;
        Health = Mathf.Max(0, Health - amount);
        int damageTaken = healthBefore - Health;

        if (damageTaken > 0)
        {
            Damaged?.Invoke(cause, damageTaken);
        }

        if (Health == 0 && !_deathAnnounced)
        {
            _deathAnnounced = true;
            Died?.Invoke();
        }
    }

    private void ConsumeOxygen(DamageCause cause)
    {
        if (Oxygen > 0)
        {
            Oxygen -= 1;
            return;
        }

        LoseHealth(1, cause);
    }

    private void RecoverOxygen()
    {
        Oxygen = Mathf.Min(MaxOxygen, Oxygen + 1);
    }
}
