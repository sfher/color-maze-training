using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// One-time release UI migration: bind progress persistence, add the
/// in-game return-home button, and add the main-menu quit button.
/// </summary>
public static class ReleaseUiSetup
{
    private const string MainScenePath =
        "Assets/Scenes/Main.unity";
    private const string MenuScenePath =
        "Assets/Scenes/MainMenu.unity";
    private const string FontPath =
        "Assets/ThirdParty/Fonts/FusionPixel/FusionPixel12SDF.asset";

    [MenuItem("Color Maze/Setup/Apply Release UI")]
    public static void Apply()
    {
        ConfigureMainScene();
        ConfigureMenuScene();
        AssetDatabase.SaveAssets();
        Debug.Log("Release UI migration complete.");
    }

    public static void ApplyFromCommandLine()
    {
        try
        {
            Apply();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void ConfigureMainScene()
    {
        Scene scene = EditorSceneManager.OpenScene(
            MainScenePath,
            OpenSceneMode.Single);

        LevelFlowController flow =
            UnityEngine.Object.FindAnyObjectByType<
                LevelFlowController>();
        GameHud hud =
            UnityEngine.Object.FindAnyObjectByType<GameHud>();

        if (flow == null || hud == null)
        {
            throw new InvalidOperationException(
                "Main scene is missing LevelFlowController or GameHud.");
        }

        SaveCoordinator legacy =
            flow.GetComponent<SaveCoordinator>();
        if (legacy != null)
        {
            UnityEngine.Object.DestroyImmediate(legacy);
        }

        SettingsController settings =
            flow.GetComponent<SettingsController>();
        ProgressService progress =
            flow.GetComponent<ProgressService>();

        if (progress == null)
        {
            progress = flow.gameObject.AddComponent<ProgressService>();
        }

        SetReference(
            progress,
            "settings",
            settings);
        SetReference(
            flow,
            "progress",
            progress);

        GameObject pausePanel =
            GetReference<GameObject>(hud, "pausePanel");
        if (pausePanel == null)
        {
            throw new InvalidOperationException(
                "GameHud.pausePanel is not assigned.");
        }

        TMP_FontAsset font = LoadFont();
        Transform pause = pausePanel.transform;
        Transform existing = pause.Find("ReturnHomeButton");
        Button returnHome = existing != null
            ? existing.GetComponent<Button>()
            : null;

        if (returnHome == null)
        {
            returnHome = UiFactory.CreateButton(
                pause,
                "ReturnHomeButton",
                "返回主页",
                font,
                new Vector2(300f, 50f),
                26);
        }

        RectTransform returnRect =
            (RectTransform)returnHome.transform;
        returnRect.anchorMin = new Vector2(0.5f, 0.5f);
        returnRect.anchorMax = new Vector2(0.5f, 0.5f);
        returnRect.anchoredPosition = new Vector2(0f, -20f);

        SetPosition(pause, "PauseTitle", 0f, 120f);
        SetPosition(pause, "ResumeButton", 160f, 215f);
        SetPosition(pause, "PauseRestartButton", -160f, 215f);
        SetReference(
            hud,
            "returnHomeButton",
            returnHome);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void ConfigureMenuScene()
    {
        Scene scene = EditorSceneManager.OpenScene(
            MenuScenePath,
            OpenSceneMode.Single);

        MainMenuController controller =
            UnityEngine.Object.FindAnyObjectByType<
                MainMenuController>();
        if (controller == null)
        {
            throw new InvalidOperationException(
                "MainMenu scene is missing MainMenuController.");
        }

        Transform existing =
            controller.transform.Find("Button_退出游戏");
        Button quitButton = existing != null
            ? existing.GetComponent<Button>()
            : null;

        if (quitButton == null)
        {
            quitButton = UiFactory.CreateButton(
                controller.transform,
                "Button_退出游戏",
                "退出游戏",
                LoadFont(),
                new Vector2(160f, 48f),
                24);
        }

        RectTransform quitRect =
            (RectTransform)quitButton.transform;
        quitRect.anchorMin = new Vector2(1f, 1f);
        quitRect.anchorMax = new Vector2(1f, 1f);
        quitRect.anchoredPosition = new Vector2(-110f, -55f);

        SetReference(
            controller,
            "quitButton",
            quitButton);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void SetPosition(
        Transform parent,
        string objectName,
        float x,
        float y)
    {
        Transform child = parent.Find(objectName);
        if (child == null)
        {
            throw new InvalidOperationException(
                $"Missing pause-panel object: {objectName}");
        }

        ((RectTransform)child).anchoredPosition =
            new Vector2(x, y);
    }

    private static T GetReference<T>(
        UnityEngine.Object target,
        string field)
        where T : UnityEngine.Object
    {
        SerializedObject serialized =
            new SerializedObject(target);
        SerializedProperty property =
            serialized.FindProperty(field);

        if (property == null)
        {
            throw new InvalidOperationException(
                $"Missing serialized field: {field}");
        }

        return property.objectReferenceValue as T;
    }

    private static void SetReference(
        UnityEngine.Object target,
        string field,
        UnityEngine.Object value)
    {
        SerializedObject serialized =
            new SerializedObject(target);
        SerializedProperty property =
            serialized.FindProperty(field);

        if (property == null)
        {
            throw new InvalidOperationException(
                $"Missing serialized field: {field}");
        }

        property.objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static TMP_FontAsset LoadFont()
    {
        TMP_FontAsset font =
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                FontPath);
        if (font == null)
        {
            throw new InvalidOperationException(
                $"Missing TMP font: {FontPath}");
        }

        return font;
    }
}
