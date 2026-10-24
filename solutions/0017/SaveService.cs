using System;
using System.IO;
using UnityEngine;

public static class SaveService
{
    public const int CurrentVersion = 1;

    private const string FileName = "save.json";
    private const string TempName = "save.tmp";
    private const string BackupName = "save.backup.json";

    public static string SavePath =>
        Path.Combine(Application.persistentDataPath, FileName);

    public static string BackupPath =>
        Path.Combine(Application.persistentDataPath, BackupName);

    public static bool LastLoadUsedBackup { get; private set; }

    public static bool TryLoad(out GameSaveData data)
    {
        LastLoadUsedBackup = false;

        if (TryRead(SavePath, out data))
        {
            return true;
        }

        if (TryRead(BackupPath, out data))
        {
            LastLoadUsedBackup = true;
            Debug.LogWarning("存档损坏，已从 backup 恢复。");
            return true;
        }

        data = null;
        return false;
    }

    public static void Save(GameSaveData data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        data.version = CurrentVersion;

        string directory =
            Path.GetDirectoryName(SavePath);
        Directory.CreateDirectory(directory);

        string tempPath =
            Path.Combine(directory, TempName);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(tempPath, json);

        if (File.Exists(SavePath))
        {
            File.Replace(
                tempPath,
                SavePath,
                BackupPath);
        }
        else
        {
            File.Move(tempPath, SavePath);
        }
    }

    private static bool TryRead(
        string path,
        out GameSaveData data)
    {
        data = null;

        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<GameSaveData>(json);

            return data != null &&
                   data.version > 0 &&
                   data.version <= CurrentVersion;
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"读取存档失败：{path}\n{exception.Message}");
            data = null;
            return false;
        }
    }
}
