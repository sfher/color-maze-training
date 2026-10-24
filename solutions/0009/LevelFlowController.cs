using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelFlowController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    public LevelState State { get; private set; } = LevelState.Playing;
    public bool IsPlaying => State == LevelState.Playing;

    private string _hint;
    private float _hintUntil;
    private GUIStyle _titleStyle;
    private GUIStyle _hintStyle;

    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError(
                "LevelFlowController: 请指定 PlayerStats。",
                this);
            enabled = false;
            return;
        }

        playerStats.Died += HandlePlayerDied;
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.Died -= HandlePlayerDied;
        }
    }

    public void ReachExit(bool hasMaterial)
    {
        if (State != LevelState.Playing)
        {
            return;
        }

        if (hasMaterial)
        {
            State = LevelState.Completed;
            return;
        }

        ShowHint("还需要找到材料。");
    }

    private void ShowHint(string message)
    {
        _hint = message;
        _hintUntil = Time.time + 2f;
    }

    private void HandlePlayerDied()
    {
        State = LevelState.Failed;
    }

    private void Update()
    {
        if (State == LevelState.Playing)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnGUI()
    {
        EnsureStyles();

        if (State == LevelState.Playing)
        {
            DrawHint();
            return;
        }

        GUI.depth = -10;
        GUI.color = new Color(0f, 0f, 0f, 0.68f);
        GUI.DrawTexture(
            new Rect(0f, 0f, Screen.width, Screen.height),
            Texture2D.whiteTexture);
        GUI.color = Color.white;

        string title = State == LevelState.Completed
            ? "关卡完成"
            : "生命耗尽";
        string hint = "按 R 重开";

        GUI.Label(
            new Rect(0f, Screen.height * 0.5f - 70f, Screen.width, 60f),
            title,
            _titleStyle);
        GUI.Label(
            new Rect(0f, Screen.height * 0.5f + 10f, Screen.width, 40f),
            hint,
            _hintStyle);
    }

    private void DrawHint()
    {
        if (string.IsNullOrEmpty(_hint) || Time.time >= _hintUntil)
        {
            return;
        }

        GUI.Box(
            new Rect(Screen.width * 0.5f - 180f, Screen.height - 80f, 360f, 52f),
            "");
        GUI.Label(
            new Rect(Screen.width * 0.5f - 160f, Screen.height - 68f, 320f, 30f),
            _hint,
            _hintStyle);
    }

    private void EnsureStyles()
    {
        if (_titleStyle != null)
        {
            return;
        }

        _titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 42,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        _titleStyle.normal.textColor = Color.white;

        _hintStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            alignment = TextAnchor.MiddleCenter
        };
        _hintStyle.normal.textColor = new Color(1f, 0.85f, 0.45f);
    }
}
