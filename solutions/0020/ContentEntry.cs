using UnityEngine;

/// <summary>
/// 一条可购买内容。id 全局唯一且一经发布不要改名；
/// 同一类型只会用到对应的载体字段（Skin→skinSprite，Theme→themeVisuals，Level→level）。
/// </summary>
[CreateAssetMenu(
    menuName = "Color Maze/Content Entry",
    fileName = "content_skin_new")]
public class ContentEntry : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private ContentType contentType;
    [SerializeField, Min(0)] private int price;
    [SerializeField] private Sprite previewSprite;
    [SerializeField] private Sprite skinSprite;
    [SerializeField] private TileVisuals themeVisuals;
    [SerializeField] private LevelDefinition level;

    public string Id => id;
    public string DisplayName => displayName;
    public ContentType ContentType => contentType;
    public int Price => price;
    public Sprite PreviewSprite => previewSprite;
    public Sprite SkinSprite => skinSprite;
    public TileVisuals ThemeVisuals => themeVisuals;
    public LevelDefinition Level => level;
}
