using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelFlowController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private AudioManager audio;

    public event System.Action StateChanged;
    public event System.Action<string> HintShown;

    public LevelState State { get; private set; }
        = LevelState.Playing;

    public bool IsPlaying =>
        State == LevelState.Playing;

    private void Start()
    {
        if (playerStats == null || audio == null)
        {
            Debug.LogError(
                "LevelFlowController: 请指定 PlayerStats 和 Audio。",
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
            // 先播音效再改状态，避免 Completed 触发的逻辑打断 AudioSource
            audio.PlayLevelComplete();
            SetState(LevelState.Completed);
            return;
        }


        audio.PlayUiError();
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