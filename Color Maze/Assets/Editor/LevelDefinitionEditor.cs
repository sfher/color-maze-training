using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// LevelDefinition 的自定义 Inspector：
/// 用“整图粘贴”替代逐行编辑。写入前先经过 LevelMapCodec 归一化，
/// 失败的文本不落盘；写入通过 SerializedObject 完成，天然支持 Undo。
/// 写入后用 LevelValidator 复核，并把错误显示在面板里而不是只进 Console。
/// </summary>
[CustomEditor(typeof(LevelDefinition))]
public class LevelDefinitionEditor : Editor
{
    private string pasteText = "";
    private string statusMessage = "";
    private MessageType statusType = MessageType.None;
    private List<string> validationErrors = new List<string>();

    private LevelDefinition Level => (LevelDefinition)target;

    private void OnEnable()
    {
        pasteText = "";
        statusMessage = "";
        statusType = MessageType.None;
        RefreshValidation();
    }

    public override void OnInspectorGUI()
    {
        if (targets.Length > 1)
        {
            DrawDefaultInspector();
            return;
        }

        serializedObject.Update();

        // rows 由粘贴工具接管，其余字段保持默认绘制。
        DrawPropertiesExcluding(serializedObject, "rows");

        EditorGUILayout.Space();
        DrawCurrentMap();

        EditorGUILayout.Space();
        DrawPasteTool();

        EditorGUILayout.Space();
        DrawValidation();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCurrentMap()
    {
        SerializedProperty rowsProperty =
            serializedObject.FindProperty("rows");

        EditorGUILayout.LabelField(
            "当前地图",
            $"{Level.Width} × {Level.Height}");

        if (rowsProperty == null || rowsProperty.arraySize == 0)
        {
            EditorGUILayout.HelpBox(
                "地图为空。粘贴一张地图或用 Level Painter 新建。",
                MessageType.Warning);
            return;
        }

        EditorGUILayout.SelectableLabel(
            LevelMapCodec.Encode(ReadRows(rowsProperty)),
            EditorStyles.textArea,
            GUILayout.MaxHeight(180));
    }

    private void DrawPasteTool()
    {
        EditorGUILayout.LabelField(
            "批量粘贴地图（支持 CRLF、缩进和首尾空行）");
        pasteText = EditorGUILayout.TextArea(
            pasteText,
            GUILayout.MinHeight(96));

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("粘贴并应用"))
            {
                ApplyPastedMap();
            }

            if (GUILayout.Button("复制当前地图"))
            {
                SerializedProperty rowsProperty =
                    serializedObject.FindProperty("rows");
                EditorGUIUtility.systemCopyBuffer =
                    LevelMapCodec.Encode(ReadRows(rowsProperty));
            }

            if (GUILayout.Button("清空文本"))
            {
                pasteText = "";
            }
        }
    }

    private void DrawValidation()
    {
        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.HelpBox(statusMessage, statusType);
        }

        if (validationErrors.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "校验通过：结构、起点、目标与传送门均合法。",
                MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "关卡校验未通过：\n" + string.Join("\n", validationErrors),
                MessageType.Error);
        }

        // stepLimit 等其它字段改动后，可手动刷新校验结果。
        if (GUILayout.Button("重新校验"))
        {
            RefreshValidation();
        }
    }

    private void ApplyPastedMap()
    {
        if (!LevelMapCodec.TryNormalize(
                pasteText,
                out string[] rows,
                out string error))
        {
            statusMessage = error;
            statusType = MessageType.Error;
            return;
        }

        SerializedProperty rowsProperty =
            serializedObject.FindProperty("rows");
        rowsProperty.arraySize = rows.Length;
        for (int i = 0; i < rows.Length; i++)
        {
            rowsProperty.GetArrayElementAtIndex(i).stringValue = rows[i];
        }

        // ApplyModifiedProperties 自动注册 Undo，Ctrl+Z 可恢复上一张地图。
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(Level);

        pasteText = "";
        statusMessage = $"已写入 {rows.Length} 行（Ctrl+Z 可撤销）。";
        statusType = MessageType.Info;
        RefreshValidation();
    }

    private void RefreshValidation()
    {
        validationErrors = LevelValidator.Validate(Level);
    }

    private static string[] ReadRows(SerializedProperty rowsProperty)
    {
        string[] rows = new string[rowsProperty.arraySize];
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i] = rowsProperty
                .GetArrayElementAtIndex(i)
                .stringValue;
        }

        return rows;
    }
}
