using UnityEngine;

[CreateAssetMenu(
    menuName = "Color Maze/Upgrade Definition",
    fileName = "Upgrade_New")]
public class UpgradeDefinition : ScriptableObject
{
    [SerializeField] private UpgradeType type;
    [SerializeField] private string displayName = "新强化";
    [SerializeField, Min(0)] private int maxLevel = 2;
    [SerializeField, Min(0)] private int baseCost = 3;
    [SerializeField, Min(0)] private int costStep = 2;

    public UpgradeType Type => type;
    public string DisplayName => displayName;
    public int MaxLevel => maxLevel;

    public int CostFor(int currentLevel)
    {
        int safeLevel = Mathf.Max(0, currentLevel);
        return baseCost + safeLevel * costStep;
    }
}
