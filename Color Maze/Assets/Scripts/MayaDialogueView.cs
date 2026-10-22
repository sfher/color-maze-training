using System.Collections;
using TMPro;
using UnityEngine;

public class MayaDialogueView : MonoBehaviour
{
    [SerializeField] private TMP_Text bodyText;
    [SerializeField, Min(1f)] private float charactersPerSecond = 32f;

    public event System.Action Finished;

    public bool IsShowing { get; private set; }

    private string[] _lines;
    private int _lineIndex;
    private bool _lineIsComplete;
    private Coroutine _typingRoutine;

    private void Awake()
    {
        if (bodyText == null)
        {
            Debug.LogError(
                "MayaDialogueView: 请指定 Body Text。",
                this);
            enabled = false;
            return;
        }

        bodyText.verticalAlignment =
            VerticalAlignmentOptions.Top;
    }

    public void Show(string[] lines)
    {
        HideImmediate();
        _lines = lines;
        _lineIndex = 0;
        IsShowing = true;

        if (_lines == null || _lines.Length == 0)
        {
            Complete();
            return;
        }

        StartCurrentLine();
    }

    public void Advance()
    {
        if (!IsShowing)
        {
            return;
        }

        if (!_lineIsComplete)
        {
            ShowCurrentLineImmediately();
            return;
        }

        if (_lineIndex + 1 < _lines.Length)
        {
            _lineIndex += 1;
            StartCurrentLine();
            return;
        }

        Complete();
    }

    public void HideImmediate()
    {
        if (_typingRoutine != null)
        {
            StopCoroutine(_typingRoutine);
        }

        _typingRoutine = null;
        _lines = null;
        _lineIndex = 0;
        _lineIsComplete = false;
        IsShowing = false;

        if (bodyText != null)
        {
            bodyText.text = string.Empty;
        }
    }

    private void StartCurrentLine()
    {
        if (_typingRoutine != null)
        {
            StopCoroutine(_typingRoutine);
        }

        _lineIsComplete = false;
        _typingRoutine = StartCoroutine(
            TypeLine(_lines[_lineIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        float visibleCharacters = 0f;
        bodyText.text = string.Empty;

        while (visibleCharacters < line.Length)
        {
            visibleCharacters +=
                Mathf.Max(1f, charactersPerSecond) *
                Time.unscaledDeltaTime;

            int count = Mathf.Min(
                line.Length,
                Mathf.FloorToInt(visibleCharacters));

            bodyText.text = line.Substring(0, count);
            yield return null;
        }

        bodyText.text = WithContinueCue(line);
        _lineIsComplete = true;
        _typingRoutine = null;
    }

    private void ShowCurrentLineImmediately()
    {
        if (_typingRoutine != null)
        {
            StopCoroutine(_typingRoutine);
        }

        _typingRoutine = null;
        bodyText.text = WithContinueCue(_lines[_lineIndex]);
        _lineIsComplete = true;
    }

    private void Complete()
    {
        HideImmediate();
        Finished?.Invoke();
    }

    private string WithContinueCue(string line)
    {
        return line +
            "\n\n<color=#D8C6A2>点击或按空格继续</color>";
    }
}
