using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 主菜单问题诊断：从 Unity 内部核对目录资产、条目字段与场景引用，
/// 输出一份可复制的报告（用于排查商店/选关面板为空的问题）。
/// </summary>
public static class MenuSetupValidator
{
    [MenuItem("Color Maze/Setup/Validate Menu Setup")]
    private static void Validate()
    {
        var report = new StringBuilder();

        string[] catalogGuids =
            AssetDatabase.FindAssets("t:ContentCatalog");
        string catalogPath = catalogGuids.Length > 0
            ? AssetDatabase.GUIDToAssetPath(catalogGuids[0])
            : string.Empty;
        ContentCatalog catalog = string.IsNullOrEmpty(catalogPath)
            ? null
            : AssetDatabase.LoadAssetAtPath<ContentCatalog>(
                catalogPath);
        report.AppendLine($"目录路径: {catalogPath}");
        report.AppendLine($"目录资产加载: {catalog != null}");

        if (catalog != null)
        {
            report.AppendLine($"条目数量: {catalog.Entries.Count}");

            foreach (ContentEntry entry in catalog.Entries)
            {
                if (entry == null)
                {
                    report.AppendLine("  [空引用条目]");
                    continue;
                }

                report.AppendLine(
                    $"  {entry.Id} | {entry.ContentType} | " +
                    $"价格 {entry.Price} | 名称 '{entry.DisplayName}' | " +
                    $"预览 {(entry.PreviewSprite != null ? "OK" : "缺失")} | " +
                    $"皮肤图 {(entry.SkinSprite != null ? "OK" : "-")} | " +
                    $"主题视觉 {(entry.ThemeVisuals != null ? "OK" : "-")} | " +
                    $"关卡 {(entry.Level != null ? "OK" : "-")}");
            }
        }

        var scene = UnityEngine.SceneManagement.SceneManager
            .GetActiveScene();
        report.AppendLine($"当前场景: {scene.name}");

        MainMenuController controller =
            Object.FindAnyObjectByType<MainMenuController>();
        report.AppendLine($"控制器存在: {controller != null}");

        if (controller != null)
        {
            var so = new SerializedObject(controller);
            foreach (string field in new[]
                     {
                         "progress", "catalog", "firstMainLevel",
                         "shopPanel", "levelSelectPanel"
                     })
            {
                report.AppendLine(
                    $"  {field}: " +
                    (so.FindProperty(field).objectReferenceValue != null
                        ? "OK"
                        : "空!"));
            }
        }

        Debug.Log($"主菜单诊断报告\n{report}");
        EditorUtility.DisplayDialog(
            "主菜单诊断",
            "报告已写入 Console（绿色日志），请复制发给助手。",
            "确定");
    }
}
