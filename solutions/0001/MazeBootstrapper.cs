using UnityEngine;

public class MazeBootstrapper : MonoBehaviour
{
    private const int Width = 7;
    private const int Height = 7;

    private const string MazeMap =
        "#######" +
        "#.....#" +
        "#.RR..#" +
        "#.....#" +
        "#..BB.#" +
        "#.....#" +
        "#######";

    private void Start()
    {
        Sprite square = CreateSquareSprite();

        for (int index = 0; index < MazeMap.Length; index++)
        {
            int x = index % Width;
            int y = Height - 1 - (index / Width);

            CreateCell(square, x, y, MazeMap[index]);
        }
    }

    private void CreateCell(Sprite square, int x, int y, char tile)
    {
        GameObject cell = new GameObject($"Cell_{x}_{y}");
        cell.transform.SetParent(transform);
        cell.transform.position = new Vector3(
            x - (Width - 1) * 0.5f,
            y - (Height - 1) * 0.5f,
            0f);

        SpriteRenderer spriteRenderer = cell.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = square;
        spriteRenderer.color = ColorFor(tile);
    }

    private Color ColorFor(char tile)
    {
        switch (tile)
        {
            case '#': return new Color(0.10f, 0.10f, 0.12f);
            case '.': return new Color(0.93f, 0.92f, 0.88f);
            case 'R': return new Color(0.82f, 0.22f, 0.20f);
            case 'B': return new Color(0.16f, 0.45f, 0.86f);
            default:  return Color.magenta;
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
