public static class GameSession
{
    public static LevelDefinition CurrentLevel { get; set; }
    public static PlayerProgress Progress { get; private set; } =
        new PlayerProgress();

    [UnityEngine.RuntimeInitializeOnLoadMethod(
        UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        CurrentLevel = null;
        Progress = new PlayerProgress();
    }
}
