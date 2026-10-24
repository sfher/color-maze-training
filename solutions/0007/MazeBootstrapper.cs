using UnityEngine;

public class MazeBootstrapper : MonoBehaviour
{
    [SerializeField] private LevelDefinition level;

    private TileType[,] _tiles;
    private SpriteRenderer[,] _renderers;

    private int Width => level != null ? level.Width : 0;
    private int Height => level != null ? level.Height : 0;

    private void Start()
    {
        if (level == null)
        {
            Debug.LogError(
                "MazeBootstrapper: 请在 Inspector 里指定 Level Definition。",
                this);
            enabled = false;
            return;
        }

        _tiles = new TileType[Width, Height];
        _renderers = new SpriteRenderer[Width, Height];

        Sprite square = CreateSquareSprite();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                TileType tile = level.GetTileType(new Vector2Int(x, y));
                _tiles[x, y] = tile;
                _renderers[x, y] = CreateCell(square, x, y, tile);
            }
        }
    }

    public bool IsWalkable(Vector2Int cell)
    {
        TileType tile = GetTileType(cell);
        return tile != TileType.Wall && tile != TileType.Stone;
    }

    public TileType GetTileType(Vector2Int cell)
    {
        if (level == null || _tiles == null || !level.Contains(cell))
        {
            return TileType.Wall;
        }

        return _tiles[cell.x, cell.y];
    }

    public bool TryBreakStone(Vector2Int cell)
    {
        return TryReplace(cell, TileType.Stone, TileType.Floor);
    }

    public bool TryOpenChest(Vector2Int cell)
    {
        return TryReplace(cell, TileType.Chest, TileType.Floor);
    }

    private bool TryReplace(
        Vector2Int cell,
        TileType expected,
        TileType replacement)
    {
        if (GetTileType(cell) != expected)
        {
            return false;
        }

        _tiles[cell.x, cell.y] = replacement;
        _renderers[cell.x, cell.y].color = ColorFor(replacement);
        return true;
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return transform.TransformPoint(CellToLocal(cell));
    }

    private Vector3 CellToLocal(Vector2Int cell)
    {
        return new Vector3(
            cell.x - (Width - 1) * 0.5f,
            cell.y - (Height - 1) * 0.5f,
            0f);
    }

    private SpriteRenderer CreateCell(
        Sprite square,
        int x,
        int y,
        TileType tile)
    {
        GameObject cell = new GameObject($"Cell_{x}_{y}");
        cell.transform.SetParent(transform);
        cell.transform.localPosition = CellToLocal(new Vector2Int(x, y));

        SpriteRenderer spriteRenderer = cell.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = square;
        spriteRenderer.color = ColorFor(tile);
        return spriteRenderer;
    }

    private Color ColorFor(TileType tile)
    {
        switch (tile)
        {
            case TileType.Wall: return new Color(0.10f, 0.10f, 0.12f);
            case TileType.Floor: return new Color(0.93f, 0.92f, 0.88f);
            case TileType.Lava: return new Color(0.82f, 0.22f, 0.20f);
            case TileType.Water: return new Color(0.16f, 0.45f, 0.86f);
            case TileType.Stone: return new Color(0.45f, 0.45f, 0.48f);
            case TileType.Chest: return new Color(0.95f, 0.72f, 0.18f);
            default: return Color.magenta;
        }
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f);
    }
}
