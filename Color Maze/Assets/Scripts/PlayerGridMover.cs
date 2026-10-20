using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGridMover : MonoBehaviour
{
    [SerializeField] private MazeBootstrapper maze;
    [SerializeField] private Vector2Int startCell = new Vector2Int(1, 1);

    private Vector2Int currentCell;
    private PlayerStats stats;

    private void Start()
    {
        if (maze == null)
        {
            Debug.LogError(
                "PlayerGridMover: 请在 Inspector 里把 Maze 引用拖进来。",
                this);
            enabled = false;
            return;
        }

        stats = GetComponent<PlayerStats>();

        if(stats == null)
        {
            Debug.LogError(
                "PlayerGridMover: 请在 Inspector 里把 PlayerStats 脚本挂上。",
                this);
            enabled = false;
            return;
        }
        currentCell = startCell;
        transform.position = maze.CellToWorld(currentCell);
    }

    private void Update()
    {
        if(!stats.IsAlive)
        {
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

    private void TryMove(Vector2Int direction)
    {
        Vector2Int targetCell = currentCell + direction;

        if (!maze.IsWalkable(targetCell))
        {
            return;
        }

        currentCell = targetCell;
        transform.position = maze.CellToWorld(currentCell);
        stats.ApplyTile(maze.GetTileType(currentCell));
    }
}