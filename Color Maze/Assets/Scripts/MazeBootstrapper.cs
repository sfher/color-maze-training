using System.Collections.Generic;
using UnityEngine;

public class MazeBootstrapper : MonoBehaviour
{
    [SerializeField] private LevelDefinition level;
    [SerializeField] private TileVisuals defaultVisuals;
    [SerializeField] private ContentCatalog cosmeticsCatalog;

    private TileType[,] _tiles;
    private int[,] _portalIds;
    private SpriteRenderer[,] _renderers;
    private SpriteRenderer[,] _overlays;
    private Sprite _squareSprite;
    private TileVisuals _activeVisuals;
    private readonly HashSet<int> _knownPortalIds = new HashSet<int>();

    public LevelDefinition CurrentLevel => level;
    public ContentCatalog CosmeticsCatalog => cosmeticsCatalog;

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

        if (defaultVisuals == null)
        {
            Debug.LogError(
                "MazeBootstrapper: 请在 Inspector 里指定 Tile Visuals。",
                this);
            enabled = false;
            return;
        }

        // 装备主题在进关时生效：解析不到主题（未购买/未装备）用默认视觉。
        _activeVisuals = defaultVisuals;

        if (cosmeticsCatalog != null)
        {
            TileVisuals themed = cosmeticsCatalog.ResolveVisuals(
                GameSession.EquippedThemeId);

            if (themed != null)
            {
                _activeVisuals = themed;
            }
        }

        _tiles = new TileType[Width, Height];
        _portalIds = new int[Width, Height];
        _renderers = new SpriteRenderer[Width, Height];
        _overlays = new SpriteRenderer[Width, Height];

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

                // 物件格（石/箱/材料/出口/传送门）：底图是主题地板，
                // 物件本体作为透明底 overlay 叠在其上——主题换色不需要
                // 为物件重烤任何素材。
                if (IsOverlayTile(tile))
                {
                    _overlays[x, y] = CreateOverlay(x, y, tile);

                    if (tile == TileType.Portal)
                    {
                        _overlays[x, y].color = PortalColor(portalId);
                    }
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
                if (_portalIds[x, y] == id && _overlays[x, y] != null)
                {
                    _overlays[x, y].color = PortalColor(id);
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

        // 底图本来就是主题地板：物件被移除 = 关掉 overlay 即可。
        if (_overlays[cell.x, cell.y] != null)
        {
            _overlays[cell.x, cell.y].enabled = false;
        }

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

    private static bool IsOverlayTile(TileType tile)
    {
        return tile == TileType.Stone ||
               tile == TileType.Chest ||
               tile == TileType.Material ||
               tile == TileType.Exit ||
               tile == TileType.Portal;
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

        // 物件格的底图固定为主题地板。
        TileType baseTile = IsOverlayTile(tile)
            ? TileType.Floor
            : tile;

        SpriteRenderer spriteRenderer = cell.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _activeVisuals.GetSprite(baseTile);
        spriteRenderer.color = Color.white;
        return spriteRenderer;
    }

    private SpriteRenderer CreateOverlay(
        int x,
        int y,
        TileType tile)
    {
        GameObject overlay = new GameObject($"Cell_{x}_{y}_Overlay");
        overlay.transform.SetParent(transform, false);
        overlay.transform.localPosition = CellToLocal(new Vector2Int(x, y));

        SpriteRenderer spriteRenderer =
            overlay.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _activeVisuals.GetSprite(tile);
        spriteRenderer.color = Color.white;
        spriteRenderer.sortingOrder = 1;
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
