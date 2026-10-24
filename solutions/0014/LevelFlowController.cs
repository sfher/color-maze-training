using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelFlowController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private AudioManager audio;
    [SerializeField] private MazeBootstrapper maze;

    public event System.Action StateChanged;
    public event System.Action<string> HintShown;

    public LevelState State { get; private set; }
        = LevelState.Playing;

    public bool IsPlaying =>
        State == LevelState.Playing;

    public LevelDefinition CurrentLevel =>
        maze != null ? maze.CurrentLevel : null;

    public bool CanAdvance =>
        State == LevelState.Completed &&
        CurrentLevel != null &&
        CurrentLevel.NextLevel != null;

    private void Start()
    {
        Time.timeScale = 1f;

        if (playerStats == null || audio == null || maze == null)
        {
            Debug.LogError(
                "LevelFlowController: 请指定 PlayerStats、Audio 和 Maze。",
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

        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
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
            audio.PlayLevelComplete();
            SetState(LevelState.Completed);
            return;
        }

        audio.PlayUiError();
        HintShown?.Invoke("还需要找到材料。");
    }

    public void TogglePause()
    {
        if (State == LevelState.Playing)
        {
            audio.PlayUiConfirm();
            SetState(LevelState.Paused);
            Time.timeScale = 0f;
            return;
        }

        if (State == LevelState.Paused)
        {
            Resume();
        }
    }

    public void Resume()
    {
        if (State != LevelState.Paused)
        {
            return;
        }

        audio.PlayUiConfirm();
        SetState(LevelState.Playing);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }

    public void AdvanceToNextLevel()
    {
        if (!CanAdvance)
        {
            return;
        }

        LevelDefinition next = CurrentLevel.NextLevel;
        audio.PlayUiConfirm();
        GameSession.CurrentLevel = next;
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }

    private void HandlePlayerDied()
    {
        SetState(LevelState.Failed);
        audio.PlayDamage();
    }

    private void SetState(LevelState nextState)
    {
        if (State == nextState)
        {
            return;
        }

        State = nextState;
        StateChanged?.Invoke();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null &&
            keyboard.escapeKey.wasPressedThisFrame &&
            (State == LevelState.Playing ||
             State == LevelState.Paused))
        {
            TogglePause();
            return;
        }

        if (State != LevelState.Completed &&
            State != LevelState.Failed)
        {
            return;
        }

        if (keyboard != null &&
            keyboard.rKey.wasPressedThisFrame)
        {
            Restart();
        }
    }
}
