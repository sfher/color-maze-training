using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 运行时与编辑器建场景共用的极简 uGUI 工厂。
/// 全部 TMP 文本（中文），字体由调用方传入。
/// </summary>
public static class UiFactory
{
    public static GameObject CreatePanel(
        Transform parent,
        string name,
        Color color,
        Vector2 size)
    {
        GameObject panel = new GameObject(
            name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        panel.GetComponent<Image>().color = color;
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        return panel;
    }

    public static TextMeshProUGUI CreateText(
        Transform parent,
        string name,
        string text,
        TMP_FontAsset font,
        int fontSize,
        Color color,
        TextAnchor align)
    {
        GameObject obj = new GameObject(
            name, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.font = font;
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
        SetAnchor(align, tmp.rectTransform);
        return tmp;
    }

    public static Button CreateButton(
        Transform parent,
        string name,
        string label,
        TMP_FontAsset font,
        Vector2 size,
        int fontSize)
    {
        GameObject obj = new GameObject(
            name, typeof(RectTransform), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        obj.GetComponent<Image>().color =
            new Color(0.22f, 0.24f, 0.30f, 0.95f);

        Button button = obj.GetComponent<Button>();
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = size;

        CreateText(
            obj.transform,
            "Label",
            label,
            font,
            fontSize,
            Color.white,
            TextAnchor.MiddleCenter);
        return button;
    }

    public static Image CreateImage(
        Transform parent,
        string name,
        Sprite sprite,
        Vector2 size)
    {
        GameObject obj = new GameObject(
            name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        Image image = obj.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        obj.GetComponent<RectTransform>().sizeDelta = size;
        return image;
    }

    /// <summary>手工拼一个横向 Slider（背景/填充/滑块）。</summary>
    public static Slider CreateSlider(
        Transform parent,
        string name,
        float width)
    {
        GameObject root = new GameObject(
            name, typeof(RectTransform));
        root.transform.SetParent(parent, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(width, 20f);

        GameObject bg = new GameObject(
            "Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(root.transform, false);
        bg.GetComponent<Image>().color =
            new Color(0.15f, 0.16f, 0.20f);
        Stretch(bg.GetComponent<RectTransform>());

        GameObject fillArea = new GameObject(
            "Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(root.transform, false);
        RectTransform fillAreaRect =
            fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(4f, 5f);
        fillAreaRect.offsetMax = new Vector2(-4f, -5f);

        GameObject fill = new GameObject(
            "Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        fill.GetComponent<Image>().color =
            new Color(0.35f, 0.55f, 0.95f);
        Stretch(fill.GetComponent<RectTransform>());

        GameObject handleArea = new GameObject(
            "Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(root.transform, false);
        RectTransform handleAreaRect =
            handleArea.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(8f, 2f);
        handleAreaRect.offsetMax = new Vector2(-8f, -2f);

        GameObject handle = new GameObject(
            "Handle", typeof(RectTransform), typeof(Image));
        handle.transform.SetParent(handleArea.transform, false);
        handle.GetComponent<Image>().color = Color.white;
        handle.GetComponent<RectTransform>().sizeDelta =
            new Vector2(14f, 24f);

        Slider slider = root.AddComponent<Slider>();
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        return slider;
    }

    /// <summary>手工拼一个 Toggle（背景 + 勾选标记）。</summary>
    public static Toggle CreateToggle(
        Transform parent,
        string name,
        float size)
    {
        GameObject root = new GameObject(
            name, typeof(RectTransform), typeof(Image), typeof(Toggle));
        root.transform.SetParent(parent, false);
        root.GetComponent<RectTransform>().sizeDelta =
            new Vector2(size, size);
        Image bg = root.GetComponent<Image>();
        bg.color = new Color(0.15f, 0.16f, 0.20f);

        GameObject check = new GameObject(
            "Checkmark", typeof(RectTransform), typeof(Image));
        check.transform.SetParent(root.transform, false);
        check.GetComponent<Image>().color =
            new Color(0.35f, 0.55f, 0.95f);
        Stretch(check.GetComponent<RectTransform>());

        Toggle toggle = root.GetComponent<Toggle>();
        toggle.graphic = check.GetComponent<Image>();
        toggle.targetGraphic = bg;
        return toggle;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetAnchor(
        TextAnchor align,
        RectTransform rect)
    {
        switch (align)
        {
            case TextAnchor.UpperLeft:
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                break;
            case TextAnchor.MiddleLeft:
                rect.anchorMin = new Vector2(0f, 0.5f);
                rect.anchorMax = new Vector2(0f, 0.5f);
                break;
            case TextAnchor.UpperCenter:
            case TextAnchor.MiddleCenter:
            default:
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                break;
        }
    }
}
