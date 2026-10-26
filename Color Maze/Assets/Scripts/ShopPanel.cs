using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店面板：按目录动态生成条目行，展示金币余额，
/// 处理购买（购买即自动装备皮肤/主题）与装备切换。
/// 事件绑定与刷新全部运行时完成；引用由建场景脚本注入。
/// </summary>
public class ShopPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private RectTransform rowsParent;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_FontAsset font;

    private ProgressService _progress;
    private ContentCatalog _catalog;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void OnDestroy()
    {
        if (_progress != null)
        {
            _progress.Changed -= HandleChanged;
        }
    }

    public void Show(
        ProgressService progress,
        ContentCatalog catalog)
    {
        if (_progress != null)
        {
            _progress.Changed -= HandleChanged;
        }

        _progress = progress;
        _catalog = catalog;
        _progress.Changed += HandleChanged;

        panel.SetActive(true);
        Rebuild();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private void HandleChanged()
    {
        if (panel.activeSelf)
        {
            Rebuild();
        }
    }

    private void Rebuild()
    {
        for (int i = rowsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(rowsParent.GetChild(i).gameObject);
        }

        coinsText.text = $"金币  {_progress.Coins}";

        var entries = _catalog.Entries;
        rowsParent.sizeDelta = new Vector2(
            rowsParent.sizeDelta.x, entries.Count * 84f + 8f);

        for (int i = 0; i < entries.Count; i++)
        {
            BuildRow(entries[i], i);
        }
    }

    private void BuildRow(ContentEntry entry, int index)
    {
        GameObject row = new GameObject(
            $"Row_{entry.Id}", typeof(RectTransform),
            typeof(Image));
        row.transform.SetParent(rowsParent, false);
        row.GetComponent<Image>().color =
            new Color(1f, 1f, 1f, 0.06f);

        RectTransform rowRect = row.GetComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0.5f, 1f);
        rowRect.anchorMax = new Vector2(0.5f, 1f);
        rowRect.pivot = new Vector2(0.5f, 1f);
        rowRect.anchoredPosition = new Vector2(0f, -(index * 84f + 4f));
        rowRect.sizeDelta = new Vector2(880f, 76f);

        UiFactory.CreateImage(
            row.transform,
            "Preview",
            entry.PreviewSprite,
            new Vector2(64f, 64f))
            .rectTransform.anchoredPosition = new Vector2(-396f, 0f);

        TextMeshProUGUI name = UiFactory.CreateText(
            row.transform,
            "Name",
            $"{entry.DisplayName}（{TypeName(entry.ContentType)}）",
            font, 26, Color.white, TextAnchor.MiddleCenter);
        name.rectTransform.anchoredPosition = new Vector2(-180f, 12f);
        name.rectTransform.sizeDelta = new Vector2(420f, 36f);

        bool owned = _progress.IsUnlocked(entry.Id);
        bool equipped = IsEquipped(entry);
        TextMeshProUGUI status = UiFactory.CreateText(
            row.transform,
            "Status",
            owned
                ? (equipped ? "使用中" : "已拥有")
                : $"{entry.Price} 金币",
            font, 22,
            equipped ? new Color(0.55f, 1f, 0.6f) : new Color(1f, 0.85f, 0.4f),
            TextAnchor.MiddleCenter);
        status.rectTransform.anchoredPosition = new Vector2(-180f, -18f);
        status.rectTransform.sizeDelta = new Vector2(420f, 30f);

        Button action = UiFactory.CreateButton(
            row.transform,
            "Action",
            ActionLabel(entry, owned, equipped),
            font,
            new Vector2(150f, 54f), 24);
        action.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(358f, 0f);
        action.interactable = !equipped;

        action.onClick.AddListener(() => HandleAction(entry));
    }

    private void HandleAction(ContentEntry entry)
    {
        if (_progress.IsUnlocked(entry.Id))
        {
            _progress.Equip(entry);
            ShowHint($"已装备 {entry.DisplayName}");
            return;
        }

        PurchaseOutcome outcome = _progress.TryPurchase(entry);

        switch (outcome)
        {
            case PurchaseOutcome.Purchased:
                _progress.Equip(entry);
                ShowHint($"已购买并装备 {entry.DisplayName}");
                break;
            case PurchaseOutcome.NotEnoughCoins:
                ShowHint("金币不足。");
                break;
            case PurchaseOutcome.AlreadyOwned:
                ShowHint("已经拥有该内容。");
                break;
            default:
                ShowHint("该条目无效。");
                break;
        }
    }

    private void ShowHint(string message)
    {
        hintText.text = message;
    }

    private bool IsEquipped(ContentEntry entry)
    {
        return entry.ContentType == ContentType.Skin
            ? _progress.Data.equippedSkinId == entry.Id
            : NormalizeThemeId(
                _progress.Data.equippedThemeId) == entry.Id;
    }

    private static string NormalizeThemeId(string themeId)
    {
        return string.IsNullOrEmpty(themeId)
            ? "theme-default"
            : themeId;
    }

    private static string ActionLabel(
        ContentEntry entry,
        bool owned,
        bool equipped)
    {
        if (equipped)
        {
            return "使用中";
        }

        if (owned)
        {
            return entry.ContentType == ContentType.Level
                ? "已解锁"
                : "装备";
        }

        return entry.ContentType == ContentType.Level
            ? "购买解锁"
            : "购买";
    }

    private static string TypeName(ContentType type)
    {
        switch (type)
        {
            case ContentType.Skin: return "皮肤";
            case ContentType.Theme: return "主题";
            default: return "关卡";
        }
    }
}
