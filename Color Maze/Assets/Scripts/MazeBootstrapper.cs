using System.Collections.Generic;
using UnityEngine;

public class MazeBootstrapper : MonoBehaviour
{
    [SerializeField] private LevelDefinition level;
    [SerializeField] private TileVisuals visuals;

    private TileType[,] _tiles;
    private int[,] _portalIds;
    private SpriteRenderer[,] _renderers;
    private Sprite _squareSprite;
    private readonly HashSet<int> _knownPortalIds = new HashSet<int>();

    public LevelDefinition CurrentLevel => level;

    private int Width => level != null ? level.Width : 0;
    private int Height => level != null ? level.Height : 0;

    private void Awake()
    {
        if (GameSession.CurrentLevel != null)
        {
            level = GameSession.CurrentLevel;
        }
        else
        {
            GameSession.CurrentLevel = level;
        }
    }

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

        if (visuals == null)
        {
            Debug.LogError(
                "MazeBootstrapper: 请在 Inspector 里指定 Tile Visuals。",
                this);
            enabled = false;
            return;
        }

        _tiles = new TileType[Width, Height];
        _portalIds = new int[Width, Height];
        _renderers = new SpriteRenderer[Width, Height];

        // 地砖改用 TileVisuals 的像素贴图；
        // 白方块只留给炸弹标记 CreateBombMarker 使用。
        _squareSprite = CreateSquareSprite();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                TileType tile = level.GetTileType(cell);
                int portalId = level.GetPortalId(cell);

                _tiles[x, y] = tile;
                _portalIds[x, y] = portalId;
                _renderers[x, y] = CreateCell(x, y, tile);

                if (tile == TileType.Portal)
                {
                    _renderers[x, y].color = PortalColor(portalId);
                }
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

    public int GetPortalId(Vector2Int cell)
    {
        if (level == null || _portalIds == null || !level.Contains(cell))
        {
            return 0;
        }

        return _portalIds[cell.x, cell.y];
    }

    public bool TryGetPortalExit(
        Vector2Int entry,
        out Vector2Int exit)
    {
        exit = entry;
        int id = GetPortalId(entry);

        if (id == 0)
        {
            return false;
        }

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_portalIds[x, y] != id ||
                    (x == entry.x && y == entry.y))
                {
                    continue;
                }

                exit = new Vector2Int(x, y);
                return true;
            }
        }

        return false;
    }

    public void MarkPortalPairKnown(int id)
    {
        if (id == 0)
        {
            return;
        }

        _knownPortalIds.Add(id);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_portalIds[x, y] == id)
                {
                    _renderers[x, y].color = PortalColor(id);
                }
            }
        }
    }

    public bool TryBreakStone(Vector2Int cell)
    {
        return TryReplace(cell, TileType.Stone, TileType.Floor);
    }

    public bool TryOpenChest(Vector2Int cell)
    {
        return TryReplace(cell, TileType.Chest, TileType.Floor);
    }

    public bool TryCollectMaterial(Vector2Int cell)
    {
        return TryReplace(
            cell,
            TileType.Material,
            TileType.Floor);
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
        _renderers[cell.x, cell.y].sprite =
            visuals.GetSprite(replacement);
        // color 是乘法 tint：换成新地砖后必须归白，
        // 否则地板会带上石头灰、水蓝等旧颜色。
        _renderers[cell.x, cell.y].color = Color.white;
        return true;
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return transform.TransformPoint(CellToLocal(cell));
    }

    public GameObject CreateBombMarker(
        Vector2Int cell,
        Color color)
    {
        GameObject marker = new GameObject(
            $"Bomb_{cell.x}_{cell.y}");
        marker.transform.SetParent(transform, false);
        marker.transform.localPosition = CellToLocal(cell);
        marker.transform.localScale = Vector3.one * 0.5f;

        SpriteRenderer renderer =
            marker.AddComponent<SpriteRenderer>();
        renderer.sprite = _squareSprite;
        renderer.color = color;
        renderer.sortingOrder = 5;
        return marker;
    }

    private Vector3 CellToLocal(Vector2Int cell)
    {
        return new Vector3(
            cell.x - (Width - 1) * 0.5f,
            cell.y - (Height - 1) * 0.5f,
            0f);
    }

    private SpriteRenderer CreateCell(
        int x,
        int y,
        TileType tile)
    {
        GameObject cell = new GameObject($"Cell_{x}_{y}");
        cell.transform.SetParent(transform);
        cell.transform.localPosition = CellToLocal(new Vector2Int(x, y));

        SpriteRenderer spriteRenderer = cell.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = visuals.GetSprite(tile);
        spriteRenderer.color = Color.white;
        return spriteRenderer;
    }

    private Color PortalColor(int id)
    {
        return _knownPortalIds.Contains(id)
            ? new Color(0.78f, 0.55f, 0.95f)
            : new Color(0.52f, 0.22f, 0.72f);
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