using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// 一键 Windows 构建：按 EditorBuildSettings 的场景列表
///（MainMenu 首位 + Main）打出 64 位可执行文件。
/// </summary>
public static class GameBuilder
{
    private const string OutputDir = "Builds/Windows64";
    private const string ExecutableName = "ColorMaze.exe";

    [MenuItem("Color Maze/Build/Windows 64-bit")]
    private static void BuildWindows64()
    {
        string outputPath = Path.Combine(OutputDir, ExecutableName);
        Directory.CreateDirectory(OutputDir);

        BuildReport report = BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes,
            outputPath,
            BuildTarget.StandaloneWindows64,
            BuildOptions.None);

        BuildSummary summary = report.summary;

        if (summary.result != BuildResult.Succeeded)
        {
            Debug.LogError(
                $"构建失败：{summary.result}，错误 {summary.totalErrors} 个。");
            return;
        }

        Debug.Log(
            $"构建完成：{outputPath}\n" +
            $"大小 {summary.totalSize / (1024 * 1024)} MB，" +
            $"警告 {summary.totalWarnings} 个。");
        EditorUtility.RevealInFinder(outputPath);
    }
}
