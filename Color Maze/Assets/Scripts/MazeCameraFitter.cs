using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MazeCameraFitter : MonoBehaviour
{
    [SerializeField] private MazeBootstrapper maze;
    [SerializeField] private float padding = 0.75f;
    [SerializeField] private float minimumSize = 5f;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        if (maze == null)
        {
            Debug.LogError(
                "MazeCameraFitter: 请在 Inspector 里指定 Maze。",
                this);
            enabled = false;
            return;
        }

        if (!_camera.orthographic)
        {
            Debug.LogError(
                "MazeCameraFitter: Camera 必须使用正交投影。",
                this);
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        LevelDefinition level = maze.CurrentLevel;

        if (level == null)
        {
            return;
        }

        Vector3 mazeScale = maze.transform.lossyScale;
        float halfWidth =
            level.Width * Mathf.Abs(mazeScale.x) * 0.5f;
        float halfHeight =
            level.Height * Mathf.Abs(mazeScale.y) * 0.5f;

        float sizeForHeight = halfHeight + padding;
        float sizeForWidth =
            (halfWidth + padding) / _camera.aspect;

        _camera.orthographicSize = Mathf.Max(
            minimumSize,
            sizeForHeight,
            sizeForWidth);

        Vector3 cameraPosition = transform.position;
        cameraPosition.x = maze.transform.position.x;
        cameraPosition.y = maze.transform.position.y;
        transform.position = cameraPosition;
    }
}
