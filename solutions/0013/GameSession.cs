public static class GameSession
{
    public static LevelDefinition CurrentLevel { get; set; }

    [UnityEngine.RuntimeInitializeOnLoadMethod(
        UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        CurrentLevel = null;
    }
}
