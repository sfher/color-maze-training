using System.Collections.Generic;
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

        if (level == null)
        {
            errors.Add("没有可校验的关卡。");
            return errors;
        }

        SerializedObject serialized =
            new SerializedObject(level);
        SerializedProperty rowsProperty =
            serialized.FindProperty("rows");

        if (rowsProperty == null ||
            !rowsProperty.isArray ||
            rowsProperty.arraySize == 0)
        {
            errors.Add("地图为空。");
            return errors;
        }

        string[] rows = new string[rowsProperty.arraySize];
        for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
        {
            rows[rowIndex] = rowsProperty
                .GetArrayElementAtIndex(rowIndex)
                .stringValue;
        }

        return ValidateRows(
            rows,
            level.StepLimit,
            level.RequiredMaterials);
    }

    /// <summary>
    /// 校验一段行数组（行序为上到下）。资产校验与编辑器窗口的
    /// 内存缓冲共用这一条规则，保证两处报错完全一致。
    /// 材料规则：地图中的 M 数量不少于 requiredMaterials，允许更多。
    /// </summary>
    public static List<string> ValidateRows(
        string[] rows,
        int stepLimit,
        int requiredMaterials)
    {
        List<string> errors = new List<string>();

        if (rows == null || rows.Length == 0)
        {
            errors.Add("地图为空。");
            return errors;
        }

        if (stepLimit <= 0)
        {
            errors.Add("步数限制必须大于 0。");
        }

        if (requiredMaterials < 1)
        {
            errors.Add("所需材料数必须至少为 1。");
        }

        int width = -1;
        bool structureValid = true;

        for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
        {
            string row = rows[rowIndex];

            if (string.IsNullOrEmpty(row))
            {
                errors.Add($"第 {rowIndex + 1} 行为空。");
                structureValid = false;
                continue;
            }

            if (width < 0)
            {
                width = row.Length;
            }
            else if (row.Length != width)
            {
                errors.Add(
                    $"第 {rowIndex + 1} 行宽为 {row.Length}，" +
                    $"应为 {width}。");
                structureValid = false;
            }

            for (int column = 0;
                 column < row.Length;
                 column++)
            {
                if (!LevelMapCodec.IsAllowedTile(row[column]))
                {
                    errors.Add(
                        $"第 {rowIndex + 1} 行第 {column + 1} 列" +
                        $"包含未知字符 '{row[column]}'。");
                    structureValid = false;
                }
            }
        }

        if (!structureValid)
        {
            return errors;
        }

        int height = rows.Length;

        if (width <= 1 || height <= 1 ||
            !IsStartable(rows[height - 2][1]))
        {
            errors.Add("默认起点 (1,1) 不是可通行地板。");
        }

        Dictionary<int, int> portals =
            new Dictionary<int, int>();
        int materials = 0;
        int exits = 0;

        foreach (string row in rows)
        {
            foreach (char tile in row)
            {
                if (tile == 'M')
                {
                    materials += 1;
                }
                else if (tile == 'E')
                {
                    exits += 1;
                }
                else if (tile >= '1' && tile <= '9')
                {
                    int id = tile - '0';
                    portals.TryGetValue(id, out int count);
                    portals[id] = count + 1;
                }
            }
        }

        if (materials < requiredMaterials)
        {
            errors.Add(
                $"材料数量应不少于 {requiredMaterials}，" +
                $"当前为 {materials}。");
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

    private static bool IsStartable(char tile)
    {
        return tile != '#' && tile != 'S';
    }
}
