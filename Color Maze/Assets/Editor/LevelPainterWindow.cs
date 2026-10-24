using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 基础关卡画笔：选择或新建 LevelDefinition，用调色板和左右键编辑
/// 内存缓冲（char[,]），点击 Apply 才通过 SerializedObject 写回资产
/// （自动注册 Undo）。切换关卡、新建、关闭窗口时对未应用修改给出
/// 应用/放弃/取消提示；校验始终针对缓冲而不是磁盘旧资产。
/// </summary>
public class LevelPainterWindow : EditorWindow
{
    private const float CellSize = 30f;

    private static readonly char[] PaletteTerrainTiles =
    {
        '.', '#', 'S', 'R', 'B', 'C', 'M', 'E'
    };

    private static readonly char[] PalettePortalTiles =
    {
        '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    // 新建关卡的默认 9×9 脚手架；外围 # 只是视觉参考，不是格式要求。
    private static readonly string[] DefaultNewLevelRows =
    {
        "#########",
        "#.......#",
        "#.......#",
        "#.......#",
        "#...M...#",
        "#.......#",
        "#.......#",
        "#......E#",
        "#########"
    };

    private LevelDefinition level;
    private char[,] cells;          // [row: 上→下, column: 左→右]
    private char brush = '#';
    private bool isDirty;
    private int resizeWidth = 9;
    private int resizeHeight = 9;
    private Vector2 gridScroll;
    private List<string> validationErrors = new List<string>();
    private string statusMessage = "";
    private MessageType statusType = MessageType.None;

    [MenuItem("Color Maze/Levels/Level Painter")]
    private static void Open()
    {
        LevelPainterWindow window =
            GetWindow<LevelPainterWindow>("Level Painter");

        // 从 Project 选中关卡时直接载入；已有未应用修改时不打扰当前编辑。
        if (Selection.activeObject is LevelDefinition selected &&
            (window.level == null || !window.isDirty))
        {
            window.LoadLevel(selected);
        }
    }

    private void OnEnable()
    {
        Undo.undoRedoPerformed += HandleUndoRedoPerformed;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= HandleUndoRedoPerformed;
    }

    /// <summary>关闭窗口时的未保存提示由 hasUnsavedChanges 驱动。</summary>
    public override void SaveChanges()
    {
        if (ApplyToAsset())
        {
            base.SaveChanges();
        }
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawToolbar();

        if (level == null || cells == null)
        {
            EditorGUILayout.HelpBox(
                "选择一个 LevelDefinition 资产，或点击 New Level 新建。",
                MessageType.Info);
            return;
        }

        EditorGUILayout.Space();
        DrawPalette();

        EditorGUILayout.Space();
        DrawGrid();

        EditorGUILayout.Space();
        DrawResizeSection();

        EditorGUILayout.Space();
        DrawValidationSection();
    }

    private void DrawHeader()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUI.BeginChangeCheck();
            LevelDefinition picked = (LevelDefinition)
                EditorGUILayout.ObjectField(
                    "关卡资产",
                    level,
                    typeof(LevelDefinition),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                TrySwitchLevel(picked);
            }

            if (GUILayout.Button("New Level", GUILayout.Width(90)))
            {
                CreateNewLevel();
            }
        }

        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.HelpBox(statusMessage, statusType);
        }
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledScope(level == null || cells == null))
            {
                if (GUILayout.Button("Apply（写回资产）"))
                {
                    ApplyToAsset();
                }

                if (GUILayout.Button("Revert（恢复资产版本）"))
                {
                    LoadLevel(level);
                }

                if (GUILayout.Button("复制地图文本"))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        LevelMapCodec.Encode(BuildRows());
                    SetStatus("地图文本已复制到剪贴板。", MessageType.Info);
                }
            }
        }
    }

    private void DrawPalette()
    {
        EditorGUILayout.LabelField(
            "调色板",
            $"当前画笔：{brush}（{GetTileName(brush)}）");

        using (new EditorGUILayout.HorizontalScope())
        {
            foreach (char tile in PaletteTerrainTiles)
            {
                DrawPaletteButton(tile);
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            foreach (char tile in PalettePortalTiles)
            {
                DrawPaletteButton(tile);
            }
        }
    }

    private void DrawPaletteButton(char tile)
    {
        Color previous = GUI.backgroundColor;
        GUI.backgroundColor = tile == brush
            ? new Color(0.55f, 1f, 0.55f)
            : GetTileColor(tile);

        if (GUILayout.Button(
                new GUIContent(tile.ToString(), GetTileName(tile)),
                GUILayout.Width(CellSize),
                GUILayout.Height(24)))
        {
            brush = tile;
            Repaint();
        }

        GUI.backgroundColor = previous;
    }

    private void DrawGrid()
    {
        int height = cells.GetLength(0);
        int width = cells.GetLength(1);

        EditorGUILayout.LabelField(
            "地图",
            $"{width} × {height}  左键绘制，右键取样" +
            (isDirty ? "  · 有未应用修改" : ""));

        gridScroll = EditorGUILayout.BeginScrollView(gridScroll);

        for (int row = 0; row < height; row++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                for (int column = 0; column < width; column++)
                {
                    DrawCellButton(row, column, height);
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawCellButton(int row, int column, int height)
    {
        char tile = cells[row, column];
        int y = height - 1 - row;    // 游戏坐标 y 向上；rows[0] 是地图顶部
        bool isStart = column == 1 && y == 1;

        GUIContent content = new GUIContent(
            tile.ToString(),
            $"({column}, {y})  行 {row + 1} 列 {column + 1}" +
            (isStart ? "  · 起点 (1,1)" : ""));

        Color previous = GUI.backgroundColor;
        GUI.backgroundColor = isStart
            ? new Color(1f, 0.35f, 0.35f)
            : GetTileColor(tile);

        bool painted = GUILayout.Button(
            content,
            GUILayout.Width(CellSize),
            GUILayout.Height(CellSize));

        Rect cellRect = GUILayoutUtility.GetLastRect();
        GUI.backgroundColor = previous;

        if (painted)
        {
            cells[row, column] = brush;
            MarkBufferDirty();
        }
        else if (Event.current.type == EventType.ContextClick &&
                 cellRect.Contains(Event.current.mousePosition))
        {
            brush = cells[row, column];
            Event.current.Use();
            Repaint();
        }
    }

    private void DrawResizeSection()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            resizeWidth = Mathf.Clamp(
                EditorGUILayout.IntField("宽", resizeWidth), 2, 32);
            resizeHeight = Mathf.Clamp(
                EditorGUILayout.IntField("高", resizeHeight), 2, 32);

            if (GUILayout.Button("Resize", GUILayout.Width(70)))
            {
                ResizeBuffer();
            }
        }

        EditorGUILayout.HelpBox(
            "Resize 保留左上区域，新增格使用当前画笔。" +
            "修改先进缓冲，Apply 后才写回资产。",
            MessageType.None);
    }

    private void DrawValidationSection()
    {
        EditorGUILayout.LabelField("校验结果（针对窗口缓冲）");

        if (validationErrors.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "校验通过：结构、起点、目标与传送门均合法。",
                MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "未通过：\n" + string.Join("\n", validationErrors),
                MessageType.Error);
        }
    }

    private void ResizeBuffer()
    {
        int oldHeight = cells.GetLength(0);
        int oldWidth = cells.GetLength(1);

        if (resizeWidth == oldWidth && resizeHeight == oldHeight)
        {
            return;
        }

        if ((resizeWidth < oldWidth || resizeHeight < oldHeight) &&
            !EditorUtility.DisplayDialog(
                "缩小地图",
                "缩小会丢失右侧/下方的格子（仅缓冲，Apply 前可 Revert）。确定继续？",
                "缩小",
                "取消"))
        {
            resizeWidth = oldWidth;
            resizeHeight = oldHeight;
            return;
        }

        char[,] resized = new char[resizeHeight, resizeWidth];
        for (int row = 0; row < resizeHeight; row++)
        {
            for (int column = 0; column < resizeWidth; column++)
            {
                resized[row, column] =
                    row < oldHeight && column < oldWidth
                        ? cells[row, column]
                        : brush;
            }
        }

        cells = resized;
        MarkBufferDirty();
    }

    private void CreateNewLevel()
    {
        if (!ConfirmApplyOrDiscard("新建关卡"))
        {
            return;
        }

        string defaultFolder = AssetDatabase.IsValidFolder(
            "Assets/Content/Levels")
            ? "Assets/Content/Levels"
            : "Assets";
        string path = EditorUtility.SaveFilePanelInProject(
            "新建关卡",
            "Level_New_01",
            "asset",
            "选择新关卡的保存位置",
            defaultFolder);
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        LevelDefinition asset = ScriptableObject.CreateInstance<LevelDefinition>();
        SerializedObject serialized = new SerializedObject(asset);

        SerializedProperty rowsProperty = serialized.FindProperty("rows");
        rowsProperty.arraySize = DefaultNewLevelRows.Length;
        for (int i = 0; i < DefaultNewLevelRows.Length; i++)
        {
            rowsProperty.GetArrayElementAtIndex(i).stringValue =
                DefaultNewLevelRows[i];
        }

        serialized.FindProperty("stepLimit").intValue = 30;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);

        LoadLevel(asset);
        SetStatus($"已创建 {path}，可直接绘制。", MessageType.Info);
    }

    private bool TrySwitchLevel(LevelDefinition next)
    {
        if (!ConfirmApplyOrDiscard("切换关卡"))
        {
            return false;   // 取消：ObjectField 下一次重绘恢复为当前关卡
        }

        LoadLevel(next);
        return true;
    }

    /// <summary>
    /// 离开当前缓冲前的统一确认：应用 / 取消 / 放弃。
    /// 返回 false 表示用户取消，调用方应中止后续操作。
    /// </summary>
    private bool ConfirmApplyOrDiscard(string actionName)
    {
        if (!isDirty)
        {
            return true;
        }

        int choice = EditorUtility.DisplayDialogComplex(
            "未应用的画笔修改",
            $"是否先把当前修改应用到 {level.name}？\n（{actionName}前需要处理未保存内容）",
            "应用",
            "取消",
            "放弃");

        if (choice == 1)
        {
            return false;
        }

        return choice != 0 || ApplyToAsset();
    }

    private bool ApplyToAsset()
    {
        if (level == null || cells == null)
        {
            SetStatus("没有可写回的关卡。", MessageType.Warning);
            return false;
        }

        string[] rows = BuildRows();
        if (!LevelMapCodec.TryNormalize(
                LevelMapCodec.Encode(rows),
                out string[] normalized,
                out string error))
        {
            SetStatus($"无法写回：{error}", MessageType.Error);
            return false;
        }

        SerializedObject serialized = new SerializedObject(level);
        SerializedProperty rowsProperty = serialized.FindProperty("rows");
        rowsProperty.arraySize = normalized.Length;
        for (int i = 0; i < normalized.Length; i++)
        {
            rowsProperty.GetArrayElementAtIndex(i).stringValue = normalized[i];
        }

        // ApplyModifiedProperties 自动注册 Undo，Ctrl+Z 可恢复上一张地图。
        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();

        isDirty = false;
        hasUnsavedChanges = false;
        RefreshValidation();

        bool passed = validationErrors.Count == 0;
        SetStatus(
            passed
                ? $"已写回 {normalized.Length} 行并保存，校验通过。"
                : $"已写回 {normalized.Length} 行并保存，" +
                  "但校验未通过（可作草稿保留，不能标记为可玩）。",
            passed ? MessageType.Info : MessageType.Warning);
        return true;
    }

    private void LoadLevel(LevelDefinition target)
    {
        level = target;
        cells = null;
        isDirty = false;
        hasUnsavedChanges = false;
        statusMessage = "";
        statusType = MessageType.None;
        validationErrors.Clear();

        if (level == null)
        {
            return;
        }

        string[] rows = ReadAssetRows(level);
        if (rows == null || rows.Length == 0)
        {
            SetStatus(
                "资产地图为空，可先在 Inspector 粘贴一张地图。",
                MessageType.Warning);
            return;
        }

        if (!LevelMapCodec.TryNormalize(
                LevelMapCodec.Encode(rows),
                out string[] normalized,
                out string error))
        {
            SetStatus($"资产地图无法载入：{error}", MessageType.Error);
            return;
        }

        cells = new char[normalized.Length, normalized[0].Length];
        for (int row = 0; row < normalized.Length; row++)
        {
            for (int column = 0;
                 column < normalized[0].Length;
                 column++)
            {
                cells[row, column] = normalized[row][column];
            }
        }

        resizeWidth = cells.GetLength(1);
        resizeHeight = cells.GetLength(0);
        RefreshValidation();
    }

    private void HandleUndoRedoPerformed()
    {
        // 缓冲有未应用修改时不覆盖用户正在编辑的内容；
        // 否则跟随全局 Undo/Redo 重新读取资产。
        if (level != null && !isDirty)
        {
            LoadLevel(level);
            Repaint();
        }
    }

    private void MarkBufferDirty()
    {
        isDirty = true;
        hasUnsavedChanges = true;
        RefreshValidation();
        Repaint();
    }

    private void RefreshValidation()
    {
        validationErrors =
            level == null || cells == null
                ? new List<string>()
                : LevelValidator.ValidateRows(
                    BuildRows(),
                    level.StepLimit,
                    level.RequiredMaterials);
    }

    private string[] BuildRows()
    {
        if (cells == null)
        {
            return new string[0];
        }

        string[] rows = new string[cells.GetLength(0)];
        for (int row = 0; row < rows.Length; row++)
        {
            char[] line = new char[cells.GetLength(1)];
            for (int column = 0; column < line.Length; column++)
            {
                line[column] = cells[row, column];
            }

            rows[row] = new string(line);
        }

        return rows;
    }

    private static string[] ReadAssetRows(LevelDefinition asset)
    {
        SerializedObject serialized = new SerializedObject(asset);
        SerializedProperty rowsProperty = serialized.FindProperty("rows");
        if (rowsProperty == null ||
            !rowsProperty.isArray ||
            rowsProperty.arraySize == 0)
        {
            return null;
        }

        string[] rows = new string[rowsProperty.arraySize];
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i] = rowsProperty
                .GetArrayElementAtIndex(i)
                .stringValue;
        }

        return rows;
    }

    private void SetStatus(string message, MessageType type)
    {
        statusMessage = message;
        statusType = type;
    }

    private static string GetTileName(char tile)
    {
        switch (tile)
        {
            case '.': return "地板";
            case '#': return "墙";
            case 'S': return "灰石";
            case 'R': return "岩浆";
            case 'B': return "水";
            case 'C': return "宝箱";
            case 'M': return "材料";
            case 'E': return "出口";
            default: return $"传送门 {tile}";
        }
    }

    private static Color GetTileColor(char tile)
    {
        switch (tile)
        {
            case '.': return Color.white;
            case '#': return new Color(0.55f, 0.55f, 0.55f);
            case 'S': return new Color(0.75f, 0.75f, 0.75f);
            case 'R': return new Color(1f, 0.45f, 0.35f);
            case 'B': return new Color(0.45f, 0.7f, 1f);
            case 'C': return new Color(1f, 0.85f, 0.35f);
            case 'M': return new Color(0.8f, 0.5f, 1f);
            case 'E': return new Color(0.5f, 1f, 0.55f);
            default: return new Color(1f, 0.75f, 0.45f);  // 传送门 1–9
        }
    }
}
