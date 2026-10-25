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

    // v2：内容商店与关卡进度
    public string equippedSkinId = "skin-default";
    public string equippedThemeId = "";
    public string lastLevelId = "";
    public string[] completedLevelIds = Array.Empty<string>();
}