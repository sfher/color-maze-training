using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHud : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private LevelFlowController flow;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endTitleText;
    [SerializeField] private TMP_Text endHintText;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseRestartButton;

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

        restartButton.onClick.AddListener(flow.Restart);
        resumeButton.onClick.AddListener(flow.Resume);
        pauseRestartButton.onClick.AddListener(flow.Restart);

        endPanel.SetActive(false);
        pausePanel.SetActive(false);
        hintText.gameObject.SetActive(false);

        RefreshStats();
        HandleStateChanged();
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
    }

    private bool ValidateReferences()
    {
        if (playerStats == null || flow == null ||
            statsText == null || hintText == null ||
            endPanel == null || endTitleText == null ||
            endHintText == null || restartButton == null ||
            pausePanel == null || resumeButton == null ||
            pauseRestartButton == null)
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
            $"生命  {playerStats.Health} / {PlayerStats.MaxHealth}\n" +
            $"氧气  {playerStats.Oxygen} / {PlayerStats.MaxOxygen}\n" +
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
            return;
        }

        endTitleText.text =
            flow.State == LevelState.Completed
                ? "关卡完成"
                : "生命耗尽";

        endHintText.text =
            flow.State == LevelState.Completed
                ? "你找到了材料。"
                : "按 R 或按钮重开。";
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