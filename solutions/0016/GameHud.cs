using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameHud : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private LevelFlowController flow;
    [SerializeField] private MayaDialogueView dialogue;
    [SerializeField] private UpgradeShop upgradeShop;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endTitleText;
    [SerializeField] private TMP_Text endHintText;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseRestartButton;
    [SerializeField] private Button nextLevelButton;

    private Coroutine _hintRoutine;

    private void Start()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        playerStats.StatsChanged += RefreshStats;
        flow.StateChanged += HandleStateChanged;
        flow.HintShown += ShowHint;
        dialogue.Finished += HandleDialogueFinished;

        restartButton.onClick.AddListener(flow.Restart);
        resumeButton.onClick.AddListener(flow.Resume);
        pauseRestartButton.onClick.AddListener(flow.Restart);
        nextLevelButton.onClick.AddListener(flow.AdvanceToNextLevel);

        endPanel.SetActive(false);
        pausePanel.SetActive(false);
        hintText.gameObject.SetActive(false);
        dialogue.HideImmediate();
        upgradeShop.Hide();

        RefreshStats();
        HandleStateChanged();
    }

    private void Update()
    {
        if (dialogue == null || !dialogue.IsShowing)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        bool advance =
            (keyboard != null &&
             (keyboard.spaceKey.wasPressedThisFrame ||
              keyboard.enterKey.wasPressedThisFrame)) ||
            (mouse != null &&
             mouse.leftButton.wasPressedThisFrame);

        if (advance)
        {
            dialogue.Advance();
        }
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.StatsChanged -= RefreshStats;
        }

        if (flow != null)
        {
            flow.StateChanged -= HandleStateChanged;
            flow.HintShown -= ShowHint;
        }

        if (dialogue != null)
        {
            dialogue.Finished -= HandleDialogueFinished;
        }

        if (restartButton != null && flow != null)
        {
            restartButton.onClick.RemoveListener(flow.Restart);
        }

        if (resumeButton != null && flow != null)
        {
            resumeButton.onClick.RemoveListener(flow.Resume);
        }

        if (pauseRestartButton != null && flow != null)
        {
            pauseRestartButton.onClick.RemoveListener(flow.Restart);
        }

        if (nextLevelButton != null && flow != null)
        {
            nextLevelButton.onClick.RemoveListener(
                flow.AdvanceToNextLevel);
        }
    }

    private bool ValidateReferences()
    {
        if (playerStats == null || flow == null ||
            dialogue == null || upgradeShop == null ||
            statsText == null || hintText == null ||
            endPanel == null || endTitleText == null ||
            endHintText == null || restartButton == null ||
            pausePanel == null || resumeButton == null ||
            pauseRestartButton == null || nextLevelButton == null)
        {
            Debug.LogError(
                "GameHud: 有引用没有绑定，请检查 Inspector。",
                this);
            return false;
        }

        return true;
    }

    private void RefreshStats()
    {
        statsText.text =
            $"生命  {playerStats.Health} / {playerStats.MaxHealth}\n" +
            $"氧气  {playerStats.Oxygen} / {playerStats.MaxOxygen}\n" +
            $"炸弹  {playerStats.Bombs}\n" +
            $"金币  {playerStats.Coins}\n" +
            $"材料  {playerStats.Materials} / 1";
    }

    private void HandleStateChanged()
    {
        bool ended =
            flow.State == LevelState.Completed ||
            flow.State == LevelState.Failed;
        bool paused = flow.State == LevelState.Paused;

        endPanel.SetActive(ended);
        pausePanel.SetActive(paused);

        if (!ended)
        {
            upgradeShop.Hide();
            dialogue.HideImmediate();
            return;
        }

        upgradeShop.Hide();

        if (flow.State == LevelState.Failed)
        {
            dialogue.HideImmediate();
            restartButton.gameObject.SetActive(true);
            nextLevelButton.gameObject.SetActive(false);
            endTitleText.text = "生命耗尽";
            endHintText.text = "按 R 或按钮重开。";
            return;
        }

        restartButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        endTitleText.text = "通讯接入";

        string[] lines =
            flow.CurrentLevel != null
                ? flow.CurrentLevel.CompletionDialogue
                : null;

        if (lines == null || lines.Length == 0)
        {
            lines = new[] { "材料已经拿到。旅程继续。" };
        }

        dialogue.Show(lines);
    }

    private void HandleDialogueFinished()
    {
        if (flow.State != LevelState.Completed)
        {
            return;
        }

        endTitleText.text = "关卡完成";
        restartButton.gameObject.SetActive(true);
        nextLevelButton.gameObject.SetActive(flow.CanAdvance);
        upgradeShop.Show();
    }

    private void ShowHint(string message)
    {
        if (_hintRoutine != null)
        {
            StopCoroutine(_hintRoutine);
        }

        _hintRoutine = StartCoroutine(HintRoutine(message));
    }

    private IEnumerator HintRoutine(string message)
    {
        hintText.text = message;
        hintText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        hintText.gameObject.SetActive(false);
        _hintRoutine = null;
    }
}
