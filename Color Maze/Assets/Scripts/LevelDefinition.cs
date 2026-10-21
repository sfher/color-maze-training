using UnityEngine;

[CreateAssetMenu(
    menuName = "Color Maze/Level Definition",
    fileName = "Level_01")]
public class LevelDefinition : ScriptableObject
{
    [SerializeField]
    private string[] rows =
    {
        "#######",
        "#.....#",
        "#.RR..#",
        "#.....#",
        "#..BB.#",
        "#.S...#",
        "#######"
    };

    public int Width => rows != null && rows.Length > 0
        ? rows[0].Length
        : 0;

    public int Height => rows != null ? rows.Length : 0;

    public bool Contains(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < Width &&
               cell.y >= 0 && cell.y < Height;
    }

    public char GetTile(Vector2Int cell)
    {
        int row = Height - 1 - cell.y;
        return rows[row][cell.x];
    }

    public TileType GetTileType(Vector2Int cell)
    {
        switch (GetTile(cell))
        {
            case '.': return TileType.Floor;
            case '#': return TileType.Wall;
            case 'S': return TileType.Stone;
            case 'R': return TileType.Lava;
            case 'B': return TileType.Water;
            default: return TileType.Wall;
        }
    }

    private void OnValidate()
    {
        if (rows == null || rows.Length == 0)
        {
            Debug.LogWarning("LevelDefinition: 地图至少需要一行。", this);
            return;
        }

        int width = rows[0].Length;

        for (int i = 0; i < rows.Length; i++)
        {
            if (string.IsNullOrEmpty(rows[i]) ||
                rows[i].Length != width)
            {
                Debug.LogWarning(
                    $"LevelDefinition: 第 {i + 1} 行长度不一致，应为 {width}。",
                    this);
            }
        }
    }
}