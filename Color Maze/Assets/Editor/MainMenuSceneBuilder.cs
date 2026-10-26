using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 一键搭建主菜单场景：菜单六按钮、覆盖确认框、商店/选关/设置面板、
/// 开场通讯，全部程序化创建并注入引用（无需手动拖 Inspector），
/// 场景保存到 Assets/Scenes/MainMenu.unity 并置于 Build 首位。
/// </summary>
public static class MainMenuSceneBuilder
{
    private const string ScenePath =
        "Assets/Scenes/MainMenu.unity";
    private static readonly string[] CatalogPaths =
    {
        "Assets/Data/Content/ContentCatalog.asset",
        "Assets/Data/ContentCatalog.asset",
    };
    private const string FirstLevelPath =
        "Assets/Data/Level_01.asset";

    private static TMP_FontAsset _font;

    [MenuItem("Color Maze/Setup/Create Main Menu Scene")]
    private static void Build()
    {
        _font = LoadFont();
        if (_font == null)
        {
            Debug.LogError("未找到 TMP FontAsset，无法建场景。");
            return;
        }

        Scene scene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene,
            NewSceneMode.Single);

        CreateCamera();
        CreateEventSystem();
        Canvas canvas = CreateCanvas();

        CreateTitle(canvas.transform);

        MainMenuController controller = canvas.gameObject
            .AddComponent<MainMenuController>();
        ShopPanel shop = canvas.gameObject.AddComponent<ShopPanel>();
        LevelSelectPanel levelSelect =
            canvas.gameObject.AddComponent<LevelSelectPanel>();
        ProgressService progress =
            canvas.gameObject.AddComponent<ProgressService>();

        // Keep the accepted release layout compact around the title.
        Button start = MenuButton(canvas.transform, "开始游戏", 120f);
        Button cont = MenuButton(canvas.transform, "读取存档", 45f);
        Button delete = MenuButton(canvas.transform, "清空存档", -30f);
        Button levels = MenuButton(canvas.transform, "选择关卡", -105f);
        Button shopButton = MenuButton(canvas.transform, "商店", -180f);
        Button settings = MenuButton(canvas.transform, "设置", -255f);
        Button quit = UiFactory.CreateButton(
            canvas.transform,
            "Button_退出游戏",
            "退出游戏",
            _font,
            new Vector2(160f, 48f),
            24);
        RectTransform quitRect =
            (RectTransform)quit.transform;
        quitRect.anchorMin = new Vector2(1f, 1f);
        quitRect.anchorMax = new Vector2(1f, 1f);
        quitRect.anchoredPosition = new Vector2(-110f, -55f);

        GameObject confirmPanel = CreateConfirmPanel(canvas.transform);
        GameObject settingsPanel =
            CreateSettingsPanel(canvas.transform);
        GameObject shopPanel =
            CreateShopPanel(canvas.transform, shop);
        GameObject levelPanel =
            CreateLevelPanel(canvas.transform, levelSelect);
        GameObject introPanel =
            CreateIntroPanel(canvas.transform, out TMP_Text introText);

        Inject(controller, "progress", progress);
        Inject(controller, "catalog",
            LoadCatalog());
        Inject(controller, "firstMainLevel",
            AssetDatabase.LoadAssetAtPath<LevelDefinition>(
                FirstLevelPath));
        Inject(controller, "startButton", start);
        Inject(controller, "continueButton", cont);
        Inject(controller, "deleteButton", delete);
        Inject(controller, "levelsButton", levels);
        Inject(controller, "shopButton", shopButton);
        Inject(controller, "settingsButton", settings);
        Inject(controller, "quitButton", quit);
        Inject(controller, "confirmPanel", confirmPanel);
        Inject(controller, "confirmText",
            confirmPanel.transform.Find("Message")
                .GetComponent<TMPro.TMP_Text>());
        Inject(controller, "confirmYesButton",
            confirmPanel.transform.Find("YesButton")
                .GetComponent<Button>());
        Inject(controller, "confirmNoButton",
            confirmPanel.transform.Find("NoButton")
                .GetComponent<Button>());
        Inject(controller, "shopPanel", shop);
        Inject(controller, "levelSelectPanel", levelSelect);
        Inject(controller, "settingsPanel", settingsPanel);
        Inject(controller, "volumeSlider",
            settingsPanel.transform.Find("VolumeSlider")
                .GetComponent<Slider>());
        Inject(controller, "fullscreenToggle",
            settingsPanel.transform.Find("FullscreenToggle")
                .GetComponent<Toggle>());
        Inject(controller, "introPanel", introPanel);
        Inject(controller, "introText", introText);
        InjectIntroLines(controller);

        Inject(shop, "panel", shopPanel);
        Inject(shop, "coinsText",
            shopPanel.transform.Find("CoinsText")
                .GetComponent<TMPro.TMP_Text>());
        Inject(shop, "hintText",
            shopPanel.transform.Find("HintText")
                .GetComponent<TMPro.TMP_Text>());
        Inject(shop, "rowsParent",
            shopPanel.transform
                .Find("ScrollView/Viewport/Content")
                .GetComponent<RectTransform>());
        Inject(shop, "closeButton",
            shopPanel.transform.Find("CloseButton")
                .GetComponent<Button>());
        Inject(shop, "font", _font);

        Inject(levelSelect, "panel", levelPanel);
        Inject(levelSelect, "rowsParent",
            levelPanel.transform
                .Find("ScrollView/Viewport/Content")
                .GetComponent<RectTransform>());
        Inject(levelSelect, "closeButton",
            levelPanel.transform.Find("CloseButton")
                .GetComponent<Button>());
        Inject(levelSelect, "font", _font);

        confirmPanel.SetActive(false);
        settingsPanel.SetActive(false);
        shopPanel.SetActive(false);
        levelPanel.SetActive(false);
        introPanel.SetActive(false);

        EditorSceneManager.SaveScene(scene, ScenePath);
        PutFirstInBuildSettings();
        Selection.activeObject =
            AssetDatabase.LoadAssetAtPath<GameObject>(ScenePath);
        EditorGUIUtility.PingObject(
            AssetDatabase.LoadAssetAtPath<Object>(ScenePath));
        Debug.Log($"主菜单场景已创建：{ScenePath}");
    }

    public static void BuildFromCommandLine()
    {
        try
        {
            Build();
            EditorApplication.Exit(0);
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    // ---------- 元素工厂 ----------

    private static ContentCatalog LoadCatalog()
    {
        foreach (string path in CatalogPaths)
        {
            ContentCatalog catalog =
                AssetDatabase.LoadAssetAtPath<ContentCatalog>(
                    path);
            if (catalog != null)
            {
                return catalog;
            }
        }

        Debug.LogError("未找到 ContentCatalog.asset。");
        return null;
    }

    private const string FontPath =
        "Assets/ThirdParty/Fonts/FusionPixel/FusionPixel12SDF.asset";

    private static TMP_FontAsset LoadFont()
    {
        TMP_FontAsset font =
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);

        if (font != null)
        {
            return font;
        }

        // 兜底：只在 Assets 内找（FindAssets 会把包缓存的
        // LiberationSans SDF 排在前面，导致中文全是方框）。
        foreach (string guid in AssetDatabase.FindAssets(
                     "t:TMP_FontAsset"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (path.StartsWith("Assets/"))
            {
                return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                    path);
            }
        }

        return null;
    }

    private static void CreateCamera()
    {
        GameObject cameraGo = new GameObject("Main Camera");
        Camera camera = cameraGo.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor =
            new Color(0.07f, 0.08f, 0.12f);
        camera.orthographic = true;
        cameraGo.AddComponent<AudioListener>();
        cameraGo.tag = "MainCamera";
    }

    private static void CreateEventSystem()
    {
        GameObject system = new GameObject("EventSystem");
        system.AddComponent<UnityEngine.EventSystems.EventSystem>();
        system.AddComponent<InputSystemUIInputModule>();
    }

    private static Canvas CreateCanvas()
    {
        GameObject go = new GameObject(
            "MenuCanvas", typeof(Canvas), typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.screenMatchMode =
            CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private static void CreateTitle(Transform parent)
    {
        TextMeshProTitle(parent);
    }

    private static void TextMeshProTitle(Transform parent)
    {
        TMPro.TextMeshProUGUI title = UiFactory.CreateText(
            parent,
            "Title",
            "色彩迷宫",
            _font,
            64,
            new Color(0.95f, 0.85f, 0.4f),
            TextAnchor.MiddleCenter);
        title.rectTransform.anchoredPosition = new Vector2(0f, 240f);
        title.rectTransform.sizeDelta = new Vector2(600f, 90f);

        TMPro.TextMeshProUGUI subtitle = UiFactory.CreateText(
            parent,
            "Subtitle",
            "MAYA 在迷宫深处等你",
            _font,
            24,
            new Color(0.6f, 0.65f, 0.75f),
            TextAnchor.MiddleCenter);
        subtitle.rectTransform.anchoredPosition =
            new Vector2(0f, 185f);
        subtitle.rectTransform.sizeDelta = new Vector2(600f, 40f);
    }

    private static Button MenuButton(
        Transform parent,
        string label,
        float y)
    {
        Button button = UiFactory.CreateButton(
            parent,
            $"Button_{label}",
            label,
            _font,
            new Vector2(340f, 66f),
            30);
        ((RectTransform)button.transform).anchoredPosition = new Vector2(0f, y);
        return button;
    }

    private static GameObject CreateConfirmPanel(Transform parent)
    {
        GameObject panel = UiFactory.CreatePanel(
            parent,
            "ConfirmPanel",
            new Color(0.08f, 0.09f, 0.14f, 1f),
            new Vector2(720f, 260f));

        TMPro.TextMeshProUGUI message = UiFactory.CreateText(
            panel.transform,
            "Message",
            "确认？",
            _font, 28, Color.white, TextAnchor.MiddleCenter);
        message.rectTransform.anchoredPosition =
            new Vector2(0f, 50f);
        message.rectTransform.sizeDelta = new Vector2(640f, 120f);

        Button yes = UiFactory.CreateButton(
            panel.transform, "YesButton", "确定",
            _font, new Vector2(160f, 56f), 26);
        ((RectTransform)yes.transform).anchoredPosition =
            new Vector2(-110f, -70f);

        Button no = UiFactory.CreateButton(
            panel.transform, "NoButton", "取消",
            _font, new Vector2(160f, 56f), 26);
        ((RectTransform)no.transform).anchoredPosition =
            new Vector2(110f, -70f);

        return panel;
    }

    private static GameObject CreateSettingsPanel(Transform parent)
    {
        GameObject panel = UiFactory.CreatePanel(
            parent,
            "SettingsPanel",
            new Color(0.08f, 0.09f, 0.14f, 1f),
            new Vector2(560f, 300f));

        TMPro.TextMeshProUGUI title = UiFactory.CreateText(
            panel.transform, "Title", "设置",
            _font, 34, Color.white, TextAnchor.MiddleCenter);
        title.rectTransform.anchoredPosition = new Vector2(0f, 105f);

        TMPro.TextMeshProUGUI volumeLabel = UiFactory.CreateText(
            panel.transform, "VolumeLabel", "音量",
            _font, 24, Color.white, TextAnchor.MiddleCenter);
        volumeLabel.rectTransform.anchoredPosition =
            new Vector2(-180f, 35f);
        volumeLabel.rectTransform.sizeDelta = new Vector2(120f, 40f);

        Slider slider = UiFactory.CreateSlider(
            panel.transform, "VolumeSlider", 260f);
        ((RectTransform)slider.transform).anchoredPosition =
            new Vector2(60f, 35f);

        TMPro.TextMeshProUGUI fullLabel = UiFactory.CreateText(
            panel.transform, "FullscreenLabel", "全屏",
            _font, 24, Color.white, TextAnchor.MiddleCenter);
        fullLabel.rectTransform.anchoredPosition =
            new Vector2(-180f, -25f);
        fullLabel.rectTransform.sizeDelta = new Vector2(120f, 40f);

        Toggle toggle = UiFactory.CreateToggle(
            panel.transform, "FullscreenToggle", 32f);
        ((RectTransform)toggle.transform).anchoredPosition =
            new Vector2(-60f, -25f);

        TMPro.TextMeshProUGUI note = UiFactory.CreateText(
            panel.transform, "Note",
            "分辨率等更多设置请进入游戏内设置面板。",
            _font, 18, new Color(0.6f, 0.65f, 0.75f),
            TextAnchor.MiddleCenter);
        note.rectTransform.anchoredPosition =
            new Vector2(0f, -110f);
        note.rectTransform.sizeDelta = new Vector2(480f, 36f);

        return panel;
    }

    private static GameObject CreateShopPanel(
        Transform parent,
        ShopPanel shop)
    {
        GameObject panel = UiFactory.CreatePanel(
            parent,
            "ShopPanel",
            new Color(0.08f, 0.09f, 0.14f, 1f),
            new Vector2(960f, 620f));

        TMPro.TextMeshProUGUI title = UiFactory.CreateText(
            panel.transform, "Title", "商店",
            _font, 36, Color.white, TextAnchor.MiddleCenter);
        title.rectTransform.anchoredPosition =
            new Vector2(-380f, 265f);

        TMPro.TextMeshProUGUI coins = UiFactory.CreateText(
            panel.transform, "CoinsText", "金币  0",
            _font, 28, new Color(1f, 0.85f, 0.4f),
            TextAnchor.MiddleCenter);
        coins.rectTransform.anchoredPosition =
            new Vector2(300f, 265f);
        coins.rectTransform.sizeDelta = new Vector2(260f, 50f);

        CreateScrollList(panel.transform, 880f, 440f, 70f);

        TMPro.TextMeshProUGUI hint = UiFactory.CreateText(
            panel.transform, "HintText", " ",
            _font, 22, new Color(0.7f, 0.75f, 0.85f),
            TextAnchor.MiddleCenter);
        hint.rectTransform.anchoredPosition =
            new Vector2(0f, -235f);
        hint.rectTransform.sizeDelta = new Vector2(820f, 44f);

        TMPro.TextMeshProUGUI note = UiFactory.CreateText(
            panel.transform, "Note",
            "主题装备后下一局生效。",
            _font, 18, new Color(0.6f, 0.65f, 0.75f),
            TextAnchor.MiddleCenter);
        note.rectTransform.anchoredPosition =
            new Vector2(0f, 215f);

        Button close = UiFactory.CreateButton(
            panel.transform, "CloseButton", "关闭",
            _font, new Vector2(150f, 52f), 24);
        ((RectTransform)close.transform).anchoredPosition =
            new Vector2(390f, -275f);

        return panel;
    }

    private static GameObject CreateLevelPanel(
        Transform parent,
        LevelSelectPanel levelSelect)
    {
        GameObject panel = UiFactory.CreatePanel(
            parent,
            "LevelSelectPanel",
            new Color(0.08f, 0.09f, 0.14f, 1f),
            new Vector2(760f, 560f));

        TMPro.TextMeshProUGUI title = UiFactory.CreateText(
            panel.transform, "Title", "选择关卡",
            _font, 34, Color.white, TextAnchor.MiddleCenter);
        title.rectTransform.anchoredPosition =
            new Vector2(0f, 240f);

        CreateScrollList(panel.transform, 700f, 400f, 0f);

        Button close = UiFactory.CreateButton(
            panel.transform, "CloseButton", "关闭",
            _font, new Vector2(150f, 52f), 24);
        ((RectTransform)close.transform).anchoredPosition =
            new Vector2(0f, -245f);

        return panel;
    }

    private static GameObject CreateIntroPanel(
        Transform parent,
        out TMP_Text introText)
    {
        GameObject panel = UiFactory.CreatePanel(
            parent,
            "IntroPanel",
            new Color(0.02f, 0.03f, 0.05f, 1f),
            new Vector2(1280f, 720f));

        introText = UiFactory.CreateText(
            panel.transform,
            "IntroText",
            " ",
            _font,
            30,
            new Color(0.75f, 0.85f, 1f),
            TextAnchor.MiddleCenter);
        introText.rectTransform.sizeDelta = new Vector2(900f, 240f);

        TMPro.TextMeshProUGUI hint = UiFactory.CreateText(
            panel.transform, "Hint",
            "按 Enter / 空格 / 点击 继续",
            _font, 20, new Color(0.5f, 0.55f, 0.65f),
            TextAnchor.MiddleCenter);
        hint.rectTransform.anchoredPosition =
            new Vector2(0f, -220f);
        hint.rectTransform.sizeDelta = new Vector2(600f, 40f);

        return panel;
    }

    private static void CreateScrollList(
        Transform parent,
        float width,
        float height,
        float yOffset)
    {
        GameObject scrollGo = new GameObject(
            "ScrollView",
            typeof(RectTransform),
            typeof(Image),
            typeof(ScrollRect));
        scrollGo.transform.SetParent(parent, false);
        RectTransform scrollRect =
            scrollGo.GetComponent<RectTransform>();
        scrollRect.sizeDelta = new Vector2(width, height);
        scrollRect.anchoredPosition = new Vector2(0f, yOffset);
        scrollGo.GetComponent<Image>().color =
            new Color(1f, 1f, 1f, 0.03f);

        // RectMask2D：矩形裁剪不依赖模板缓冲。老式 Mask + 透明图会把
        // 整条内容子树的模板测试全部吞掉（面板空白事故的根因）。
        GameObject viewport = new GameObject(
            "Viewport", typeof(RectTransform), typeof(RectMask2D));
        viewport.transform.SetParent(scrollGo.transform, false);
        Stretch(viewport.GetComponent<RectTransform>());

        GameObject content = new GameObject(
            "Content", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect =
            content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 1f);
        contentRect.anchorMax = new Vector2(0.5f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.sizeDelta = new Vector2(width, 0f);

        ScrollRect scroll = scrollGo.GetComponent<ScrollRect>();
        scroll.viewport = viewport.GetComponent<RectTransform>();
        scroll.content = contentRect;
        scroll.horizontal = false;
        scroll.scrollSensitivity = 30f;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    // ---------- 注入与构建设置 ----------

    private static void Inject(
        Component target,
        string field,
        Object value)
    {
        SerializedObject serialized = new SerializedObject(target);
        SerializedProperty property =
            serialized.FindProperty(field);

        if (property == null)
        {
            Debug.LogError(
                $"字段不存在：{target.GetType().Name}.{field}");
            return;
        }

        property.objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void InjectIntroLines(
        MainMenuController controller)
    {
        string[] lines =
        {
            "MAYA：信号接通了……你能听到吗？",
            "这片迷宫深处有一份我需要的数据材料，但信号只能覆盖你脚下的格子。",
            "每一次移动都会消耗氧气——规划路线，收集材料，找到出口。",
            "我会一直在通讯里。出发吧。",
        };

        SerializedObject serialized =
            new SerializedObject(controller);
        SerializedProperty property =
            serialized.FindProperty("introLines");
        property.arraySize = lines.Length;

        for (int i = 0; i < lines.Length; i++)
        {
            property.GetArrayElementAtIndex(i).stringValue = lines[i];
        }

        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void PutFirstInBuildSettings()
    {
        var scenes = new System.Collections.Generic.List<
            EditorBuildSettingsScene>();

        foreach (EditorBuildSettingsScene sceneInfo in
                 EditorBuildSettings.scenes)
        {
            if (sceneInfo.path != ScenePath)
            {
                scenes.Add(sceneInfo);
            }
        }

        scenes.Insert(0, new EditorBuildSettingsScene(
            ScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
