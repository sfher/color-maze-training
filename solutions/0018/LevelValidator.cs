using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class LevelValidator
{
    [MenuItem("Color Maze/Levels/Validate All")]
    private static void ValidateAllLevels()
    {
        string[] guids =
            AssetDatabase.FindAssets("t:LevelDefinition");
        int checkedCount = 0;
        int failedCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelDefinition level =
                AssetDatabase.LoadAssetAtPath<LevelDefinition>(path);

            if (level == null)
            {
                continue;
            }

            checkedCount += 1;
            List<string> errors = Validate(level);

            if (errors.Count == 0)
            {
                continue;
            }

            failedCount += 1;
            Debug.LogWarning(
                $"关卡校验失败：{path}\n{string.Join("\n", errors)}",
                level);
        }

        EditorUtility.DisplayDialog(
            "关卡校验",
            $"检查 {checkedCount} 个关卡，" +
            $"{failedCount} 个需要修复。",
            "确定");
    }

    public static List<string> Validate(LevelDefinition level)
    {
        List<string> errors = new List<string>();

        if (level == null || level.Width <= 0 || level.Height <= 0)
        {
            errors.Add("地图为空或尺寸无效。");
            return errors;
        }

        if (!level.Contains(Vector2Int.one) ||
            !IsStartable(level, Vector2Int.one))
        {
            errors.Add("默认起点 (1,1) 不是可通行地板。");
        }

        Dictionary<int, int> portals =
            new Dictionary<int, int>();
        int materials = 0;
        int exits = 0;

        for (int y = 0; y < level.Height; y++)
        {
            for (int x = 0; x < level.Width; x++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                TileType tile = level.GetTileType(cell);

                if (tile == TileType.Material)
                {
                    materials += 1;
                }
                else if (tile == TileType.Exit)
                {
                    exits += 1;
                }
                else if (tile == TileType.Portal)
                {
                    int id = level.GetPortalId(cell);
                    portals.TryGetValue(id, out int count);
                    portals[id] = count + 1;
                }
            }
        }

        if (materials != 1)
        {
            errors.Add($"材料数量应为 1，当前为 {materials}。");
        }

        if (exits != 1)
        {
            errors.Add($"出口数量应为 1，当前为 {exits}。");
        }

        foreach (KeyValuePair<int, int> portal in portals)
        {
            if (portal.Value != 2)
            {
                errors.Add(
                    $"传送门 {portal.Key} 应有 2 个，" +
                    $"当前为 {portal.Value}。");
            }
        }

        return errors;
    }

    private static bool IsStartable(
        LevelDefinition level,
        Vector2Int cell)
    {
        TileType tile = level.GetTileType(cell);
        return tile != TileType.Wall &&
               tile != TileType.Stone;
    }
}
