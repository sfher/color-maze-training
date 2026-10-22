using System;

[Serializable]
public class GameSaveData
{
    public int version = SaveService.CurrentVersion;
    public int coins;
    public string[] unlockedContentIds = Array.Empty<string>();
    public float masterVolume = 0.8f;
    public bool fullscreen = true;
    public int screenWidth = 1280;
    public int screenHeight = 720;
}