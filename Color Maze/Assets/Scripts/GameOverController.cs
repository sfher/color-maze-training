using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    private bool _isGameOver;
    private GUIStyle _titleStyle;
    private GUIStyle _hintStyle;

    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError(
                "GameOverController: 请指定 PlayerStats。",
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

    private void Update()
    {
        if (!_isGameOver)
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

    private void HandlePlayerDied()
    {
        _isGameOver = true;
    }

    private void OnGUI()
    {
        if (!_isGameOver)
        {
            return;
        }

        GUI.depth = -10;
        GUI.color = new Color(0f, 0f, 0f, 0.68f);
        GUI.DrawTexture(
            new Rect(0f, 0f, Screen.width, Screen.height),
            Texture2D.whiteTexture);
        GUI.color = Color.white;

        if (_titleStyle == null)
        {
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

        GUI.Label(
            new Rect(0f, Screen.height * 0.5f - 70f, Screen.width, 60f),
            "生命耗尽",
            _titleStyle);
        GUI.Label(
            new Rect(0f, Screen.height * 0.5f + 10f, Screen.width, 40f),
            "按 R 重开",
            _hintStyle);
    }
}