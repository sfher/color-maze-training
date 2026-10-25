using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelFlowController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private AudioManager audio;
    [SerializeField] private MazeBootstrapper maze;
    [SerializeField] private ProgressService progress;

    public event System.Action StateChanged;
    public event System.Action<string> HintShown;
    public event System.Action StepsChanged;

    public LevelState State { get; private set; }
        = LevelState.Playing;

    public bool IsPlaying =>
        State == LevelState.Playing;

    public LevelDefinition CurrentLevel =>
        maze != null ? maze.CurrentLevel : null;

    public string FailureReason { get; private set; }
        = string.Empty;

    public int StepLimit => RunState.StepLimit;
    public int StepsRemaining => RunState.StepsRemaining;

    private LevelRunState _runState;

    private LevelRunState RunState
    {
        get
        {
            if (_runState == null)
            {
                int limit = CurrentLevel != null
                    ? CurrentLevel.StepLimit
                    : 1;
                _runState = new LevelRunState(limit);
            }

            return _runState;
        }
    }

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

        _ = StepLimit;
        FailureReason = string.Empty;
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
            // 金币结算与首通奖励在进入 Completed 时发生一次；
            // 失败/重开路径不会走到这里，天然防刷。
            if (progress != null)
            {
                progress.SettleLevelComplete(
                    CurrentLevel,
                    playerStats);
            }

            SetState(LevelState.Completed);
            return;
        }

        audio.PlayUiError();
        HintShown?.Invoke("还需要找到材料。");
    }

    public bool TryUseStep()
    {
        if (State != LevelState.Playing)
        {
            return false;
        }

        if (RunState.TrySpendStep())
        {
            StepsChanged?.Invoke();
            return true;
        }

        Fail("步数耗尽。");
        return false;
    }

    public void NotifyMoveResolved()
    {
        if (State == LevelState.Playing &&
            RunState.IsExhausted)
        {
            Fail("步数耗尽。");
        }
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

    /// <summary>回主菜单（build index 0）。临时入口：暂停中按 M。</summary>
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
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
        FailureReason = "生命耗尽。";
        SetState(LevelState.Failed);
    }

    public void Fail(string reason)
    {
        if (State != LevelState.Playing)
        {
            return;
        }

        FailureReason = reason;
        audio.PlayUiError();
        SetState(LevelState.Failed);
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

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.escapeKey.wasPressedThisFrame &&
            (State == LevelState.Playing ||
             State == LevelState.Paused))
        {
            TogglePause();
            return;
        }

        if (State == LevelState.Paused &&
            keyboard.mKey.wasPressedThisFrame)
        {
            ReturnToMenu();
            return;
        }

        // R：任意状态直接重开——关内、暂停中、结算后行为一致。
        if (keyboard.rKey.wasPressedThisFrame)
        {
            Restart();
        }
    }
}
