using UnityEngine;

/// <summary>
/// 一套关卡视觉皮肤：每种 TileType 对应一张像素地砖。
/// 换主题 = 换一张 TileVisuals 资产，渲染代码不变。
/// </summary>
[CreateAssetMenu(
    menuName = "Color Maze/Tile Visuals",
    fileName = "TileVisuals_Classic")]
public class TileVisuals : ScriptableObject
{
    [SerializeField] private Sprite floor;
    [SerializeField] private Sprite wall;
    [SerializeField] private Sprite stone;
    [SerializeField] private Sprite water;
    [SerializeField] private Sprite lava;
    [SerializeField] private Sprite chest;
    [SerializeField] private Sprite material;
    [SerializeField] private Sprite exit;
    [SerializeField] private Sprite portalBase;

    public Sprite GetSprite(TileType tile)
    {
        switch (tile)
        {
            case TileType.Wall: return wall;
            case TileType.Stone: return stone;
            case TileType.Lava: return lava;
            case TileType.Water: return water;
            case TileType.Chest: return chest;
            case TileType.Portal: return portalBase;
            case TileType.Material: return material;
            case TileType.Exit: return exit;
            default: return floor;
        }
    }
}
