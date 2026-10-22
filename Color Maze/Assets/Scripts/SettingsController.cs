using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private GameObject settingsPanel;

    private readonly List<Vector2Int> _resolutions =
        new List<Vector2Int>();
    private bool _initializing;

    public event Action Changed;

    private void Awake()
    {
        if (volumeSlider == null ||
            fullscreenToggle == null ||
            resolutionDropdown == null ||
            settingsPanel == null)
        {
            Debug.LogError(
                "SettingsController: 有引用没有绑定，请检查 Inspector。",
                this);
            enabled = false;
            return;
        }

        BuildResolutionOptions();
        volumeSlider.onValueChanged.AddListener(
            HandleChanged);
        fullscreenToggle.onValueChanged.AddListener(
            HandleChanged);
        resolutionDropdown.onValueChanged.AddListener(
            HandleChanged);
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(
                HandleChanged);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(
                HandleChanged);
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveListener(
                HandleChanged);
        }
    }

    public void Apply(GameSaveData data)
    {
        if (data == null)
        {
            return;
        }

        _initializing = true;
        volumeSlider.SetValueWithoutNotify(
            Mathf.Clamp01(data.masterVolume));
        fullscreenToggle.SetIsOnWithoutNotify(data.fullscreen);
        resolutionDropdown.SetValueWithoutNotify(
            FindResolutionIndex(
                data.screenWidth,
                data.screenHeight));
        _initializing = false;

        ApplyCurrentValues();
    }

    public void Open()
    {
        settingsPanel.SetActive(true);
        settingsPanel.transform.SetAsLastSibling();
    }

    public void Close()
    {
        settingsPanel.SetActive(false);
    }

    public void CaptureTo(GameSaveData data)
    {
        if (data == null)
        {
            return;
        }

        data.masterVolume = volumeSlider.value;
        data.fullscreen = fullscreenToggle.isOn;

        Vector2Int resolution =
            _resolutions[resolutionDropdown.value];
        data.screenWidth = resolution.x;
        data.screenHeight = resolution.y;
    }

    private void HandleChanged(float _) => HandleChanged();
    private void HandleChanged(bool _) => HandleChanged();
    private void HandleChanged(int _) => HandleChanged();

    private void HandleChanged()
    {
        if (_initializing)
        {
            return;
        }

        ApplyCurrentValues();
        Changed?.Invoke();
    }

    private void ApplyCurrentValues()
    {
        AudioListener.volume =
            Mathf.Clamp01(volumeSlider.value);

        Vector2Int resolution =
            _resolutions[resolutionDropdown.value];
        Screen.SetResolution(
            resolution.x,
            resolution.y,
            fullscreenToggle.isOn);
    }

    private void BuildResolutionOptions()
    {
        _resolutions.Clear();

        foreach (Resolution resolution in Screen.resolutions)
        {
            Vector2Int size =
                new Vector2Int(
                    resolution.width,
                    resolution.height);

            if (!_resolutions.Contains(size))
            {
                _resolutions.Add(size);
            }
        }

        Vector2Int current =
            new Vector2Int(Screen.width, Screen.height);

        if (!_resolutions.Contains(current))
        {
            _resolutions.Add(current);
        }

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (Vector2Int size in _resolutions)
        {
            options.Add($"{size.x} × {size.y}");
        }

        resolutionDropdown.AddOptions(options);
    }

    private int FindResolutionIndex(int width, int height)
    {
        int index = _resolutions.IndexOf(
            new Vector2Int(width, height));
        return index >= 0 ? index : 0;
    }
}
