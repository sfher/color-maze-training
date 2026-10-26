using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Regression probe for the release save path. It backs up the active save,
/// settles a synthetic completion, reloads from disk, then restores the save.
/// </summary>
public static class SavePersistenceProbe
{
    private const string LevelPath =
        "Assets/Data/Level_01.asset";
    private const string TempFileName = "save.tmp";

    [MenuItem("Color Maze/Tests/Probe Save Persistence")]
    public static void Run()
    {
        string tempPath = Path.Combine(
            Path.GetDirectoryName(SaveService.SavePath),
            TempFileName);
        byte[] originalSave = ReadBytes(SaveService.SavePath);
        byte[] originalBackup = ReadBytes(SaveService.BackupPath);
        GameObject gameObject = null;
        GameObject reloadedObject = null;

        try
        {
            DeleteIfExists(SaveService.SavePath);
            DeleteIfExists(SaveService.BackupPath);
            DeleteIfExists(tempPath);

            LevelDefinition level =
                AssetDatabase.LoadAssetAtPath<LevelDefinition>(
                    LevelPath);
            if (level == null)
            {
                throw new InvalidOperationException(
                    $"Missing probe level: {LevelPath}");
            }

            gameObject = new GameObject("SavePersistenceProbe");
            ProgressService progress =
                gameObject.AddComponent<ProgressService>();
            PlayerStats stats =
                gameObject.AddComponent<PlayerStats>();
            InvokeAwake(progress);
            stats.AddCoins(7);
            progress.SettleLevelComplete(level, stats);
            int expectedCoins = progress.Coins;
            UnityEngine.Object.DestroyImmediate(gameObject);
            gameObject = null;

            reloadedObject =
                new GameObject("SavePersistenceProbeReload");
            ProgressService reloaded =
                reloadedObject.AddComponent<ProgressService>();
            InvokeAwake(reloaded);

            if (expectedCoins != 17)
            {
                throw new InvalidOperationException(
                    $"Expected 17 settled coins, got {expectedCoins}.");
            }

            if (!reloaded.HasCompleted(level.name))
            {
                throw new InvalidOperationException(
                    "Completed level was not restored.");
            }

            if (reloaded.Coins != expectedCoins)
            {
                throw new InvalidOperationException(
                    $"Reloaded coins {reloaded.Coins} != {expectedCoins}.");
            }

            Debug.Log(
                "Save persistence probe passed: 17 coins and completion restored.");
        }
        finally
        {
            if (gameObject != null)
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
            }

            if (reloadedObject != null)
            {
                UnityEngine.Object.DestroyImmediate(reloadedObject);
            }

            Restore(SaveService.SavePath, originalSave);
            Restore(SaveService.BackupPath, originalBackup);
            DeleteIfExists(tempPath);
        }
    }

    public static void RunFromCommandLine()
    {
        try
        {
            Run();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void InvokeAwake(ProgressService progress)
    {
        MethodInfo awake = typeof(ProgressService).GetMethod(
            "Awake",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (awake == null)
        {
            throw new MissingMethodException(
                "ProgressService.Awake");
        }

        awake.Invoke(progress, null);
    }

    private static byte[] ReadBytes(string path)
    {
        return File.Exists(path)
            ? File.ReadAllBytes(path)
            : null;
    }

    private static void Restore(string path, byte[] contents)
    {
        if (contents == null)
        {
            DeleteIfExists(path);
            return;
        }

        Directory.CreateDirectory(
            Path.GetDirectoryName(path));
        File.WriteAllBytes(path, contents);
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
