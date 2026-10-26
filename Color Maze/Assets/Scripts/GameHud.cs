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
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endTitleText;
    [SerializeField] private TMP_Text endHintText;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseRestartButton;
    [SerializeField] private Button returnHomeButton;
    [SerializeField] private Button nextLevelButton;

    private Coroutine _hintRoutine;
    private TMP_Text _actionButtonLabel;

    private void Start()
    {
        if (nextLevelButton != null)
        {
            _actionButtonLabel =
                nextLevelButton.GetComponentInChildren<TMP_Text>(
                    true);
        }

        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        playerStats.StatsChanged += RefreshStats;
        flow.StateChanged += HandleStateChanged;
        flow.HintShown += ShowHint;
        flow.StepsChanged += RefreshStats;
        dialogue.Finished += HandleDialogueFinished;

        restartButton.onClick.AddListener(flow.Restart);
        resumeButton.onClick.AddListener(flow.Resume);
        pauseRestartButton.onClick.AddListener(flow.Restart);
        returnHomeButton.onClick.AddListener(flow.ReturnToMenu);
        nextLevelButton.onClick.AddListener(HandleActionButton);

        endPanel.SetActive(false);
        pausePanel.SetActive(false);
        hintText.gameObject.SetActive(false);
        dialogue.HideImmediate();

        RefreshStats();
        HandleStateChanged();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        // Enter：与“下一关”按钮同一启用条件——对话结束后才生效。
        if (flow.State == LevelState.Completed &&
            nextLevelButton.gameObject.activeSelf &&
            keyboard != null &&
            keyboard.enterKey.wasPressedThisFrame)
        {
            HandleActionButton();
            return;
        }

        if (dialogue == null || !dialogue.IsShowing)
        {
            return;
        }

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
            flow.StepsChanged -= RefreshStats;
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

        if (returnHomeButton != null && flow != null)
        {
            returnHomeButton.onClick.RemoveListener(
                flow.ReturnToMenu);
        }

        if (nextLevelButton != null && flow != null)
        {
            nextLevelButton.onClick.RemoveListener(
                HandleActionButton);
        }
    }

    private bool ValidateReferences()
    {
        if (playerStats == null || flow == null ||
            dialogue == null || statsText == null ||
            hintText == null || endPanel == null ||
            endTitleText == null || endHintText == null ||
            restartButton == null || pausePanel == null ||
            resumeButton == null || pauseRestartButton == null ||
            returnHomeButton == null || nextLevelButton == null ||
            _actionButtonLabel == null)
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
        int requiredMaterials = flow.CurrentLevel != null
            ? flow.CurrentLevel.RequiredMaterials
            : 1;
        statsText.text =
            $"生命  {playerStats.Health} / {PlayerStats.MaxHealth}\n" +
            $"氧气  {playerStats.Oxygen} / {PlayerStats.MaxOxygen}\n" +
            $"炸弹  {playerStats.Bombs}\n" +
            $"金币  {playerStats.Coins}\n" +
            $"材料  {playerStats.Materials} / {requiredMaterials}\n" +
            $"步数  {flow.StepsRemaining} / {flow.StepLimit}";
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
            dialogue.HideImmediate();
            return;
        }

        if (flow.State == LevelState.Failed)
        {
            dialogue.HideImmediate();
            restartButton.gameObject.SetActive(true);
            SetActionButton("返回主页");
            endTitleText.text = "关卡失败";
            endHintText.text =
                $"{flow.FailureReason}\n按 R 重试，或返回主页逛逛商店。";
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

        string summary =
            playerStats.Health == PlayerStats.MaxHealth
                ? $"满血完成 · 使用 {flow.StepLimit - flow.StepsRemaining} 步"
                : $"完成但残血 · 生命 {playerStats.Health} / {PlayerStats.MaxHealth}";
        endTitleText.text = "关卡完成";
        restartButton.gameObject.SetActive(true);
        SetActionButton(
            flow.CanAdvance ? "下一关" : "返回主页");
        endHintText.text = flow.CanAdvance
            ? $"{summary}\n按 Enter 或按钮进入下一关。"
            : $"{summary}\n返回主页后可以逛逛商店。";
    }

    private void SetActionButton(string label)
    {
        nextLevelButton.gameObject.SetActive(true);
        if (_actionButtonLabel != null)
        {
            _actionButtonLabel.text = label;
        }
    }

    private void HandleActionButton()
    {
        if (flow.State == LevelState.Completed &&
            flow.CanAdvance)
        {
            flow.AdvanceToNextLevel();
            return;
        }

        flow.ReturnToMenu();
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
