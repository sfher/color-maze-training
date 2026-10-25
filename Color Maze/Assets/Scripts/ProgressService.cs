using System;
using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// 元玩法唯一的存档持有者：金币、解锁内容、装备、关卡进度。
/// 由 SaveCoordinator 升级而来——设置项的读写逻辑原样保留，
/// 新增购买/装备/结算 API。任何对存档的修改都必须经过这里，
/// 保证"内存一份、磁盘一份"不会互相覆盖。
/// </summary>
public class ProgressService : MonoBehaviour
{
    private const int FirstClearBonus = 10;

    [SerializeField] private SettingsController settings;
    [SerializeField, Min(0.05f)] private float saveDelay = 0.25f;

    /// <summary>金币、解锁或装备发生变化；商店与 HUD 用它刷新显示。</summary>
    public event Action Changed;

    public GameSaveData Data { get; private set; }

    public int Coins => Data != null ? Data.coins : 0;

    public bool HasSave => Data != null;

    /// <summary>磁盘上是否存在存档文件（区别于内存中的全新数据）。</summary>
    public bool LoadedFromDisk { get; private set; }

    private Coroutine _saveRoutine;

    // Awake 而非 Start：主菜单按钮状态依赖存档数据，
    // 必须在同场景所有 Start 之前完成加载。
    private void Awake()
    {
        LoadedFromDisk = SaveService.TryLoad(out GameSaveData data);

        if (!LoadedFromDisk)
        {
            data = new GameSaveData();
        }

        Data = data;

        if (settings != null)
        {
            settings.Apply(Data);
            settings.Changed += QueueSave;
        }

        ApplyEquipToSession();
    }

    private void OnDestroy()
    {
        if (settings != null)
        {
            settings.Changed -= QueueSave;
        }

        if (_saveRoutine != null)
        {
            StopCoroutine(_saveRoutine);
        }
    }

    public bool IsUnlocked(string contentId)
    {
        return Array.IndexOf(
            Data.unlockedContentIds,
            contentId) >= 0;
    }

    public PurchaseOutcome TryPurchase(ContentEntry entry)
    {
        if (entry == null)
        {
            return PurchaseOutcome.InvalidEntry;
        }

        if (IsUnlocked(entry.Id))
        {
            return PurchaseOutcome.AlreadyOwned;
        }

        if (Data.coins < entry.Price)
        {
            return PurchaseOutcome.NotEnoughCoins;
        }

        Data.coins -= entry.Price;
        Data.unlockedContentIds =
            Append(Data.unlockedContentIds, entry.Id);
        Save();
        Changed?.Invoke();
        return PurchaseOutcome.Purchased;
    }

    /// <summary>装备已解锁的皮肤或主题；写档并同步本局 GameSession。</summary>
    public void Equip(ContentEntry entry)
    {
        if (entry == null || !IsUnlocked(entry.Id))
        {
            return;
        }

        switch (entry.ContentType)
        {
            case ContentType.Skin:
                Data.equippedSkinId = entry.Id;
                break;
            case ContentType.Theme:
                Data.equippedThemeId = entry.Id;
                break;
            default:
                return;
        }

        Save();
        ApplyEquipToSession();
        Changed?.Invoke();
    }

    public bool HasCompleted(string levelId)
    {
        return Array.IndexOf(
            Data.completedLevelIds,
            levelId) >= 0;
    }

    /// <summary>
    /// 主线按顺序解锁（通关 N 解锁 N+1）；
    /// 目录里存在的关卡条目按购买状态判定。
    /// </summary>
    public bool IsLevelPlayable(
        LevelDefinition level,
        ContentCatalog catalog,
        LevelDefinition firstMainLevel)
    {
        if (level == null)
        {
            return false;
        }

        if (catalog != null)
        {
            foreach (ContentEntry entry in catalog.Entries)
            {
                if (entry != null &&
                    entry.ContentType == ContentType.Level &&
                    entry.Level == level)
                {
                    return IsUnlocked(entry.Id);
                }
            }
        }

        if (firstMainLevel == null ||
            level.name == firstMainLevel.name)
        {
            return true;
        }

        return IsUnlockedByChain(level, firstMainLevel);
    }

    /// <summary>
    /// 通关结算：本局金币入档、首通奖励、通关标记、推进 lastLevelId。
    /// 由 LevelFlowController 在进入 Completed 状态时调用一次；
    /// 失败/重开不结算，防止刷币。
    /// </summary>
    public void SettleLevelComplete(
        LevelDefinition level,
        PlayerStats stats)
    {
        if (level == null)
        {
            return;
        }

        Data.coins += stats != null ? stats.Coins : 0;

        if (!HasCompleted(level.name))
        {
            Data.coins += FirstClearBonus;
            Data.completedLevelIds =
                Append(Data.completedLevelIds, level.name);
        }

        Data.lastLevelId = level.NextLevel != null
            ? level.NextLevel.name
            : level.name;

        Save();
        Changed?.Invoke();
    }

    /// <summary>清空存档：删除磁盘档并重置内存数据（16b 主菜单使用）。</summary>
    public void DeleteSave()
    {
        if (File.Exists(SaveService.SavePath))
        {
            File.Delete(SaveService.SavePath);
        }

        if (File.Exists(SaveService.BackupPath))
        {
            File.Delete(SaveService.BackupPath);
        }

        Data = new GameSaveData();
        LoadedFromDisk = false;

        if (settings != null)
        {
            settings.Apply(Data);
        }

        ApplyEquipToSession();
        Changed?.Invoke();
    }

    public void Save()
    {
        if (settings != null)
        {
            settings.CaptureTo(Data);
        }

        SaveService.Save(Data);
    }

    private void ApplyEquipToSession()
    {
        GameSession.EquippedSkinId =
            string.IsNullOrEmpty(Data.equippedSkinId)
                ? "skin-default"
                : Data.equippedSkinId;
        GameSession.EquippedThemeId = Data.equippedThemeId;
    }

    private bool IsUnlockedByChain(
        LevelDefinition level,
        LevelDefinition firstMainLevel)
    {
        LevelDefinition node = firstMainLevel;

        for (int guard = 0;
             node != null && node.NextLevel != null && guard < 64;
             guard++)
        {
            if (node.NextLevel.name == level.name &&
                HasCompleted(node.name))
            {
                return true;
            }

            node = node.NextLevel;
        }

        return false;
    }

    private void QueueSave()
    {
        if (_saveRoutine != null)
        {
            StopCoroutine(_saveRoutine);
        }

        _saveRoutine = StartCoroutine(SaveAfterDelay());
    }

    private IEnumerator SaveAfterDelay()
    {
        yield return new WaitForSecondsRealtime(saveDelay);
        _saveRoutine = null;
        Save();
    }

    private static string[] Append(string[] source, string value)
    {
        string[] result = new string[source.Length + 1];
        Array.Copy(source, result, source.Length);
        result[source.Length] = value;
        return result;
    }
}
