using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGridMover : MonoBehaviour
{
    [SerializeField] private MazeBootstrapper maze;
    [SerializeField] private LevelFlowController flow;
    [SerializeField] private AudioManager audio;
    [SerializeField] private Vector2Int startCell = new Vector2Int(1, 1);

    private Vector2Int currentCell;
    private Vector2Int facing = Vector2Int.up;
    private PlayerStats stats;
    private const int ChestCoins = 3;
    private const int ChestBombs = 1;

    private void Start()
    {
        if (maze == null || flow == null || audio == null)
        {
            Debug.LogError(
                "PlayerGridMover: 请指定 Maze、Flow 和 Audio。",
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

        currentCell = startCell;
        transform.position = maze.CellToWorld(currentCell);
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

        return keyboard.eKey.wasPressedThisFrame ||
               keyboard.spaceKey.wasPressedThisFrame;
    }

    private void TryUseBomb()
    {
        Vector2Int targetCell = currentCell + facing;

        if (maze.GetTileType(targetCell) != TileType.Stone)
        {
            Debug.Log("这里没有灰色石头。");
            audio.PlayUiError();
            return;
        }

        if (!stats.TryConsumeBomb())
        {
            Debug.Log("没有炸弹了。");
            audio.PlayUiError();
            return;
        }

        if (maze.TryBreakStone(targetCell))
        {
            audio.PlayStoneBreak();
            Debug.Log("灰色石头被炸开了。");
        }
    }

    private void TryMove(Vector2Int direction)
    {
        facing = direction;
        Vector2Int targetCell = currentCell + direction;

        if (!maze.IsWalkable(targetCell))
        {
            return;
        }

        currentCell = targetCell;
        transform.position = maze.CellToWorld(currentCell);

        TileType tile = maze.GetTileType(currentCell);

        if (tile == TileType.Portal)
        {
            TryTeleport();
            return;
        }

        if (tile == TileType.Chest)
        {
            if (maze.TryOpenChest(currentCell))
            {
                stats.AddCoins(ChestCoins);
                stats.AddBombs(ChestBombs);
                audio.PlayChestOpen();
                Debug.Log($"打开宝箱，获得 {ChestCoins} 个金币和 {ChestBombs} 个炸弹。");
            }

            stats.ApplyTile(TileType.Floor);
            return;
        }

        if (tile == TileType.Material)
        {
            if (maze.TryCollectMaterial(currentCell))
            {
                stats.AddMaterial();
                audio.PlayPickup();
                Debug.Log("找到材料。");
            }

            stats.ApplyTile(TileType.Floor);
            return;
        }

        if (tile == TileType.Exit)
        {
            stats.ApplyTile(TileType.Floor);
            flow.ReachExit(stats.HasMaterial);
            return;
        }

        stats.ApplyTile(tile);

        if (tile == TileType.Lava && stats.IsAlive)
        {
            audio.PlayDamage();
        }
    }

    private void TryTeleport()
    {
        Vector2Int entry = currentCell;

        if (!maze.TryGetPortalExit(entry, out Vector2Int exit))
        {
            Debug.LogWarning("传送门没有配对的出口。");
            audio.PlayUiError();
            stats.ApplyTile(TileType.Floor);
            return;
        }

        currentCell = exit;
        transform.position = maze.CellToWorld(currentCell);
        maze.MarkPortalPairKnown(maze.GetPortalId(currentCell));
        stats.ApplyTile(TileType.Floor);
        audio.PlayPortal();
        Debug.Log($"传送门: ({entry.x},{entry.y}) ↔ ({exit.x},{exit.y})");
    }
}
