public static class GameSession
{
    public static LevelDefinition CurrentLevel { get; set; }

    /// <summary>本局装备的皮肤 id；由 ProgressService 在场景启动时从存档写入。</summary>
    public static string EquippedSkinId { get; set; } = "skin-default";

    /// <summary>本局装备的主题 id；空串表示使用关卡默认视觉。</summary>
    public static string EquippedThemeId { get; set; } = "";

    [UnityEngine.RuntimeInitializeOnLoadMethod(
        UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        CurrentLevel = null;
        EquippedSkinId = "skin-default";
        EquippedThemeId = "";
    }
}
