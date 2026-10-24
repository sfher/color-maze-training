using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public const int MaxHealth = 3;
    public const int MaxOxygen = 3;

    public event System.Action Died;

    public int Health { get; private set; } = MaxHealth;
    public int Oxygen { get; private set; } = MaxOxygen;
    public int Bombs { get; private set; } = 1;

    public bool IsAlive => Health > 0;

    private bool _deathAnnounced;
    private GUIStyle _labelStyle;

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
    }

    public bool TryConsumeBomb()
    {
        if (Bombs <= 0)
        {
            return false;
        }

        Bombs -= 1;
        Debug.Log($"炸弹 -1，剩余 {Bombs}");
        return true;
    }

    private void LoseHealth(int amount)
    {
        Health = Mathf.Max(0, Health - amount);
        Debug.Log($"生命 -{amount}，剩余 {Health}");

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
            Debug.Log($"氧气 -1，剩余 {Oxygen}");
            return;
        }

        LoseHealth(1);
    }

    private void RecoverOxygen()
    {
        Oxygen = Mathf.Min(MaxOxygen, Oxygen + 1);
    }

    private void OnGUI()
    {
        if (_labelStyle == null)
        {
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.fontSize = 22;
            _labelStyle.normal.textColor = Color.black;
        }

        GUI.Box(new Rect(12f, 12f, 260f, 132f), "");
        GUI.Label(
            new Rect(24f, 22f, 236f, 30f),
            $"生命: {Health} / {MaxHealth}",
            _labelStyle);
        GUI.Label(
            new Rect(24f, 58f, 236f, 30f),
            $"氧气: {Oxygen} / {MaxOxygen}",
            _labelStyle);
        GUI.Label(
            new Rect(24f, 94f, 236f, 30f),
            $"炸弹: {Bombs}",
            _labelStyle);
    }
}
