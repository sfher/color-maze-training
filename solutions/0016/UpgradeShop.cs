using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{
    [Serializable]
    private class UpgradeRow
    {
        [SerializeField] private UpgradeDefinition definition;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;

        public UpgradeDefinition Definition => definition;
        public Button Button => button;
        public TMP_Text Label => label;
    }

    [SerializeField] private TMP_Text walletText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameFeedback feedback;
    [SerializeField] private UpgradeRow[] rows;

    private readonly List<UnityAction> _actions =
        new List<UnityAction>();

    private bool _isReady;

    private void Awake()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        for (int i = 0; i < rows.Length; i++)
        {
            int index = i;
            UnityAction action = () => TryPurchase(index);
            _actions.Add(action);
            rows[i].Button.onClick.AddListener(action);
        }

        _isReady = true;
    }

    private void OnDestroy()
    {
        if (rows == null)
        {
            return;
        }

        for (int i = 0;
             i < rows.Length && i < _actions.Count;
             i++)
        {
            if (rows[i].Button != null)
            {
                rows[i].Button.onClick.RemoveListener(
                    _actions[i]);
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (_isReady)
        {
            resultText.text = "强化会在下一关生效。";
            Refresh();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Refresh()
    {
        if (!_isReady)
        {
            return;
        }

        PlayerProgress progress = GameSession.Progress;
        walletText.text = $"金币  {progress.Coins}";

        foreach (UpgradeRow row in rows)
        {
            int level = progress.GetLevel(row.Definition);

            if (level >= row.Definition.MaxLevel)
            {
                row.Label.text =
                    $"{row.Definition.DisplayName}\n" +
                    $"Lv.{level}/{row.Definition.MaxLevel}" +
                    " · 已满级";
                row.Button.interactable = false;
                continue;
            }

            int cost = row.Definition.CostFor(level);
            row.Label.text =
                $"{row.Definition.DisplayName}\n" +
                $"Lv.{level}/{row.Definition.MaxLevel}" +
                $" · {cost} 金币\n" +
                EffectText(row.Definition.Type);
            row.Button.interactable =
                progress.Coins >= cost;
        }
    }

    private void TryPurchase(int index)
    {
        UpgradeRow row = rows[index];
        PurchaseResult result =
            GameSession.Progress.TryPurchase(
                row.Definition);

        switch (result)
        {
            case PurchaseResult.Purchased:
                feedback.PurchaseCompleted();
                resultText.text =
                    $"{row.Definition.DisplayName}升级成功，" +
                    "下一关生效。";
                break;
            case PurchaseResult.NotEnoughCoins:
                feedback.Error();
                resultText.text = "金币不够。返回关卡寻找宝箱。";
                break;
            case PurchaseResult.MaxLevel:
                resultText.text = "这个强化已经满级。";
                break;
            default:
                resultText.text = "强化配置无效。";
                Debug.LogError(
                    "UpgradeShop: 强化资产没有绑定。",
                    this);
                break;
        }

        Refresh();
    }

    private bool ValidateReferences()
    {
        bool valid =
            walletText != null &&
            resultText != null &&
            feedback != null &&
            rows != null &&
            rows.Length > 0;

        if (valid)
        {
            foreach (UpgradeRow row in rows)
            {
                if (row.Definition == null ||
                    row.Button == null ||
                    row.Label == null)
                {
                    valid = false;
                    break;
                }
            }
        }

        if (!valid)
        {
            Debug.LogError(
                "UpgradeShop: 强化行有引用没有绑定。",
                this);
        }

        return valid;
    }

    private string EffectText(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.MaxHealth:
                return "下一关生命上限 +1";
            case UpgradeType.OxygenCapacity:
                return "下一关氧气上限 +1";
            case UpgradeType.StartingBombs:
                return "下一关初始炸弹 +1";
            default:
                return "效果未定义";
        }
    }
}
