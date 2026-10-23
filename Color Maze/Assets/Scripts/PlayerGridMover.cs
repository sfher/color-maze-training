using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGridMover : MonoBehaviour
{
    [SerializeField] private MazeBootstrapper maze;
    [SerializeField] private LevelFlowController flow;
    [SerializeField] private GameFeedback feedback;
    [SerializeField] private Vector2Int startCell = new Vector2Int(1, 1);

    private Vector2Int currentCell;
    private PlayerStats stats;
    private DelayedBombState _activeBomb;
    private Vector2Int _activeBombCell;
    private GameObject _activeBombMarker;
    private const int ChestCoins = 3;
    private const int ChestBombs = 1;

    private void Start()
    {
        if (maze == null || flow == null || feedback == null)
        {
            Debug.LogError(
                "PlayerGridMover: 请指定 Maze、Flow 和 Feedback。",
                this);
            enabled = false;
            return;
        }

        stats = GetComponent<PlayerStats>();

        if (stats == null)
        {
            Debug.LogError(
                "PlayerGridMover: Player 上缺少 PlayerStats。",
                this);
            enabled = false;
            return;
        }

        stats.Damaged += HandleDamaged;
        currentCell = startCell;
        transform.position = maze.CellToWorld(currentCell);
    }

    private void OnDestroy()
    {
        if (stats != null)
        {
            stats.Damaged -= HandleDamaged;
        }
    }

    private void Update()
    {
        if (!stats.IsAlive || !flow.IsPlaying)
        {
            return;
        }

        if (ReadBombPressed())
        {
            TryUseBomb();
            return;
        }

        Vector2Int direction = ReadDirection();

        if (direction != Vector2Int.zero)
        {
            TryMove(direction);
        }
    }

    private Vector2Int ReadDirection()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return Vector2Int.zero;
        }

        if (keyboard.wKey.wasPressedThisFrame ||
            keyboard.upArrowKey.wasPressedThisFrame)
        {
            return Vector2Int.up;
        }

        if (keyboard.sKey.wasPressedThisFrame ||
            keyboard.downArrowKey.wasPressedThisFrame)
        {
            return Vector2Int.down;
        }

        if (keyboard.aKey.wasPressedThisFrame ||
            keyboard.leftArrowKey.wasPressedThisFrame)
        {
            return Vector2Int.left;
        }

        if (keyboard.dKey.wasPressedThisFrame ||
            keyboard.rightArrowKey.wasPressedThisFrame)
        {
            return Vector2Int.right;
        }

        return Vector2Int.zero;
    }

    private bool ReadBombPressed()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return false;
        }

        return keyboard.eKey.wasPressedThisFrame;
    }

    private void TryUseBomb()
    {
        if (_activeBomb != null)
        {
            Debug.Log("场上已经有一个炸弹。");
            feedback.Error();
            return;
        }

        if (!stats.TryConsumeBomb())
        {
            Debug.Log("没有炸弹了。");
            feedback.Error();
            return;
        }

        _activeBomb = new DelayedBombState();
        _activeBombCell = currentCell;
        _activeBombMarker = maze.CreateBombMarker(
            _activeBombCell,
            BombColor(_activeBomb.MovesRemaining));
        feedback.BombPlaced();
        Debug.Log("炸弹已放置，移动三步后爆炸。");
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int targetCell = currentCell + direction;

        if (!maze.IsWalkable(targetCell))
        {
            return;
        }

        if (!flow.TryUseStep())
        {
            return;
        }

        currentCell = targetCell;
        transform.position = maze.CellToWorld(currentCell);

        AdvanceBombAfterMove();

        if (!stats.IsAlive || flow.State != LevelState.Playing)
        {
            return;
        }

        TileType tile = maze.GetTileType(currentCell);

        if (tile == TileType.Portal)
        {
            TryTeleport();
        }
        else if (tile == TileType.Chest)
        {
            if (maze.TryOpenChest(currentCell))
            {
                stats.AddCoins(ChestCoins);
                stats.AddBombs(ChestBombs);
                feedback.ChestOpened();
                Debug.Log(
                    $"打开宝箱，获得 {ChestCoins} 个金币和 {ChestBombs} 个炸弹。");
            }

            stats.ApplyTile(TileType.Floor);
        }
        else if (tile == TileType.Material)
        {
            if (maze.TryCollectMaterial(currentCell))
            {
                stats.AddMaterial();
                feedback.Pickup();
                Debug.Log("找到材料。");
            }

            stats.ApplyTile(TileType.Floor);
        }
        else if (tile == TileType.Exit)
        {
            stats.ApplyTile(TileType.Floor);
            flow.ReachExit(stats.HasMaterial);
        }
        else
        {
            stats.ApplyTile(tile);

        }

        flow.NotifyMoveResolved();
    }

    private void TryTeleport()
    {
        Vector2Int entry = currentCell;

        if (!maze.TryGetPortalExit(entry, out Vector2Int exit))
        {
            Debug.LogWarning("传送门没有配对的出口。");
            feedback.Error();
            stats.ApplyTile(TileType.Floor);
            return;
        }

        currentCell = exit;
        transform.position = maze.CellToWorld(currentCell);
        maze.MarkPortalPairKnown(maze.GetPortalId(currentCell));
        stats.ApplyTile(TileType.Floor);
        feedback.PortalUsed();
        Debug.Log($"传送门: ({entry.x},{entry.y}) ↔ ({exit.x},{exit.y})");
    }

    private void AdvanceBombAfterMove()
    {
        if (_activeBomb == null)
        {
            return;
        }

        if (_activeBomb.AdvanceMove())
        {
            ExplodeBomb();
            return;
        }

        SpriteRenderer renderer =
            _activeBombMarker.GetComponent<SpriteRenderer>();
        renderer.color =
            BombColor(_activeBomb.MovesRemaining);
    }

    private void ExplodeBomb()
    {
        Vector2Int[] blastCells =
        {
            _activeBombCell + Vector2Int.up,
            _activeBombCell + Vector2Int.down,
            _activeBombCell + Vector2Int.left,
            _activeBombCell + Vector2Int.right
        };

        foreach (Vector2Int cell in blastCells)
        {
            maze.TryBreakStone(cell);
        }

        bool hitPlayer =
            currentCell == _activeBombCell ||
            currentCell == _activeBombCell + Vector2Int.up ||
            currentCell == _activeBombCell + Vector2Int.down ||
            currentCell == _activeBombCell + Vector2Int.left ||
            currentCell == _activeBombCell + Vector2Int.right;

        Object.Destroy(_activeBombMarker);
        _activeBombMarker = null;
        _activeBomb = null;
        feedback.BombExploded();

        if (hitPlayer)
        {
            stats.TakeDamage(1, DamageCause.Bomb);
        }

        Debug.Log(
            $"炸弹在 ({_activeBombCell.x},{_activeBombCell.y}) 爆炸。",
            this);
    }

    private static Color BombColor(int movesRemaining)
    {
        switch (movesRemaining)
        {
            case 1: return new Color(0.85f, 0.15f, 0.1f);
            case 2: return new Color(0.95f, 0.52f, 0.1f);
            default: return new Color(0.95f, 0.82f, 0.18f);
        }
    }

    private void HandleDamaged(
        DamageCause cause,
        int amount)
    {
        feedback.Damage(cause);
    }
}
