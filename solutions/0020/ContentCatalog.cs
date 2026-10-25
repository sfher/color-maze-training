using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商店的全部货架：内容条目清单与查询。
/// 未上架的内容不放进目录——上架 = 加一条 entry。
/// </summary>
[CreateAssetMenu(
    menuName = "Color Maze/Content Catalog",
    fileName = "ContentCatalog")]
public class ContentCatalog : ScriptableObject
{
    [SerializeField] private List<ContentEntry> entries = new List<ContentEntry>();

    public IReadOnlyList<ContentEntry> Entries => entries;

    public ContentEntry Find(string id)
    {
        foreach (ContentEntry entry in entries)
        {
            if (entry != null && entry.Id == id)
            {
                return entry;
            }
        }

        return null;
    }

    public List<ContentEntry> GetByType(ContentType type)
    {
        List<ContentEntry> result = new List<ContentEntry>();
        foreach (ContentEntry entry in entries)
        {
            if (entry != null && entry.ContentType == type)
            {
                result.Add(entry);
            }
        }

        return result;
    }

    /// <summary>按皮肤 id 取换装图；找不到（含默认皮肤未建条目）返回 null，由调用方保留当前外观。</summary>
    public Sprite ResolveSkin(string id)
    {
        ContentEntry entry = Find(id);
        return entry != null && entry.ContentType == ContentType.Skin
            ? entry.SkinSprite
            : null;
    }

    /// <summary>按主题 id 取整套视觉；空 id 或未购买主题返回 null，使用关卡默认视觉。</summary>
    public TileVisuals ResolveVisuals(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        ContentEntry entry = Find(id);
        return entry != null && entry.ContentType == ContentType.Theme
            ? entry.ThemeVisuals
            : null;
    }
}
