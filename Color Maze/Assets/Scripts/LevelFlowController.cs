using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelFlowController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    public event System.Action StateChanged;
    public event System.Action<string> HintShown;

    public LevelState State { get; private set; }
        = LevelState.Playing;

    public bool IsPlaying =>
        State == LevelState.Playing;

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
            SetState(LevelState.Completed);
            return;
        }

        HintShown?.Invoke("还需要找到材料。");
    }

    public void Restart()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }

    private void HandlePlayerDied()
    {
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
        if (State == LevelState.Playing)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null &&
            keyboard.rKey.wasPressedThisFrame)
        {
            Restart();
        }
    }
}