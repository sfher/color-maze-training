using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选关面板：主线按顺序解锁展示，购买的附加关卡排在尾部；
/// 点击可玩关卡回调主控制器加载。
/// </summary>
public class LevelSelectPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform rowsParent;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_FontAsset font;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    public void Show(
        ProgressService progress,
        ContentCatalog catalog,
        LevelDefinition firstMainLevel,
        System.Action<LevelDefinition> onPick)
    {
        panel.SetActive(true);

        for (int i = rowsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(rowsParent.GetChild(i).gameObject);
        }

        List<LevelDefinition> levels = new List<LevelDefinition>();
        LevelDefinition node = firstMainLevel;

        for (int guard = 0; node != null && guard < 64; guard++)
        {
            levels.Add(node);
            node = node.NextLevel;
        }

        if (catalog != null)
        {
            foreach (ContentEntry entry in catalog.Entries)
            {
                if (entry != null &&
                    entry.ContentType == ContentType.Level &&
                    entry.Level != null &&
                    progress.IsUnlocked(entry.Id) &&
                    !levels.Contains(entry.Level))
                {
                    levels.Add(entry.Level);
                }
            }
        }

        rowsParent.sizeDelta = new Vector2(
            rowsParent.sizeDelta.x, levels.Count * 84f + 8f);

        for (int i = 0; i < levels.Count; i++)
        {
            LevelDefinition level = levels[i];
            bool playable = progress.IsLevelPlayable(
                level, catalog, firstMainLevel);
            bool completed = progress.HasCompleted(level.name);
            bool extra = i >= CountMainChain(firstMainLevel);

            string label = extra
                ? $"附加 · {level.name}"
                : level.name;

            if (completed)
            {
                label += "  ·  已通关";
            }
            else if (!playable)
            {
                label += "  ·  未解锁";
            }

            Button button = UiFactory.CreateButton(
                rowsParent,
                $"Level_{level.name}",
                label,
                font,
                new Vector2(680f, 68f), 26);
            RectTransform buttonRect =
                button.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 1f);
            buttonRect.anchorMax = new Vector2(0.5f, 1f);
            buttonRect.pivot = new Vector2(0.5f, 1f);
            buttonRect.anchoredPosition =
                new Vector2(0f, -(i * 84f + 8f));
            button.interactable = playable;

            LevelDefinition picked = level;
            button.onClick.AddListener(() =>
            {
                Hide();
                onPick?.Invoke(picked);
            });
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private static int CountMainChain(LevelDefinition first)
    {
        int count = 0;
        LevelDefinition node = first;

        while (node != null && count < 64)
        {
            count += 1;
            node = node.NextLevel;
        }

        return count;
    }
}
