using UnityEngine;
using UnityEngine.InputSystem;

public class GridMovementLab : MonoBehaviour
{
    [SerializeField] private Transform mazeRoot;
    private string[] rows =
    {
        "#######",
        "#.....#",
        "#.RR..#",
        "#.....#",
        "#..BB.#",
        "#.....#",
        "#######"
    }; //为了防止VS报错
    Vector2Int currentCell = new Vector2Int(1, 1); //防止报错

    private void Start()
    {
        if (mazeRoot == null)
        {
            Debug.LogError("GridMovementLab: 请指定 Maze Root。", this);
            enabled = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2Int direction = ReadDirection();

        if (direction != Vector2Int.zero)
        {
            TryMove(direction);
        }
    }

    public bool IsWalkable(Vector2Int cell)
    {
        if(cell.x < 0 || cell.x >= rows[0].Length || cell.y < 0 || cell.y >= rows.Length)
        {
            return false;
        }
        int row = rows.Length - 1 - cell.y;

        if (rows[row][cell.x] == '#')
        {
            return false;
        }
        return true;
    }
    public Vector3 CellToWorld(Vector2Int cell)
    {
        float halfWidth = (rows[0].Length - 1) * 0.5f;
        float halfHeight = (rows.Length - 1) * 0.5f;

        Vector3 local = new Vector3(
            cell.x - halfWidth,
            cell.y - halfHeight,
            0f);

        return mazeRoot.TransformPoint(local);
    }
    private Vector2Int ReadDirection()
    {
        Keyboard keyboard = Keyboard.current;
        if(keyboard == null)
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
        if (!IsWalkable(targetCell))
        {
            return;
        }
        currentCell = targetCell;
        transform.position = CellToWorld(currentCell);
    }
}