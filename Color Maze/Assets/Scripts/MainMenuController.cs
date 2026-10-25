using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 主菜单：六个按钮 + 覆盖确认 + 开场通讯 + 选关/商店/设置面板调度。
/// 事件绑定全部在 Awake 运行时完成，引用由建场景脚本注入。
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private ProgressService progress;
    [SerializeField] private ContentCatalog catalog;
    [SerializeField] private LevelDefinition firstMainLevel;
    [SerializeField] private string mainSceneName = "Main";

    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button levelsButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button settingsButton;

    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TMP_Text confirmText;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    [SerializeField] private ShopPanel shopPanel;
    [SerializeField] private LevelSelectPanel levelSelectPanel;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [SerializeField] private GameObject introPanel;
    [SerializeField] private TMP_Text introText;
    [SerializeField] private string[] introLines;

    private System.Action _onConfirmYes;
    private int _introIndex;
    private bool _introActive;

    private void Awake()
    {
        startButton.onClick.AddListener(HandleStart);
        continueButton.onClick.AddListener(HandleContinue);
        deleteButton.onClick.AddListener(HandleDelete);
        levelsButton.onClick.AddListener(HandleLevels);
        shopButton.onClick.AddListener(HandleShop);
        settingsButton.onClick.AddListener(HandleSettings);

        confirmYesButton.onClick.AddListener(() =>
        {
            confirmPanel.SetActive(false);
            _onConfirmYes?.Invoke();
            _onConfirmYes = null;
        });
        confirmNoButton.onClick.AddListener(() =>
        {
            confirmPanel.SetActive(false);
            _onConfirmYes = null;
        });

        volumeSlider.onValueChanged.AddListener(HandleVolumeChanged);
        fullscreenToggle.onValueChanged.AddListener(
            HandleFullscreenChanged);
    }

    private void Start()
    {
        progress.Changed += HandleProgressChanged;
        HandleProgressChanged();
    }

    private void OnDestroy()
    {
        if (progress != null)
        {
            progress.Changed -= HandleProgressChanged;
        }
    }

    private void Update()
    {
        if (!_introActive)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        bool advance =
            (keyboard != null &&
             (keyboard.enterKey.wasPressedThisFrame ||
              keyboard.spaceKey.wasPressedThisFrame)) ||
            (mouse != null && mouse.leftButton.wasPressedThisFrame);

        if (advance)
        {
            AdvanceIntro();
        }
    }

    private void HandleProgressChanged()
    {
        bool hasSave = progress.LoadedFromDisk;
        continueButton.interactable = hasSave;
        deleteButton.interactable = hasSave;

        volumeSlider.SetValueWithoutNotify(
            progress.Data.masterVolume);
        fullscreenToggle.SetIsOnWithoutNotify(
            progress.Data.fullscreen);
        AudioListener.volume = progress.Data.masterVolume;
    }

    private void HandleStart()
    {
        if (progress.LoadedFromDisk)
        {
            ShowConfirm(
                "已有存档。开始新游戏会覆盖现有进度，确定？",
                () =>
                {
                    progress.DeleteSave();
                    BeginIntro();
                });
            return;
        }

        BeginIntro();
    }

    private void HandleContinue()
    {
        LoadMain(ResolveLevel(progress.Data.lastLevelId));
    }

    private void HandleDelete()
    {
        ShowConfirm(
            "确定清空存档？金币、已购内容与通关进度都会丢失。",
            () => progress.DeleteSave());
    }

    private void HandleLevels()
    {
        levelSelectPanel.Show(
            progress,
            catalog,
            firstMainLevel,
            LoadMain);
    }

    private void HandleShop()
    {
        shopPanel.Show(progress, catalog);
    }

    private void HandleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void HandleVolumeChanged(float value)
    {
        progress.Data.masterVolume = value;
        AudioListener.volume = value;
        progress.Save();
    }

    private void HandleFullscreenChanged(bool value)
    {
        progress.Data.fullscreen = value;
        Screen.fullScreen = value;
        progress.Save();
    }

    private void ShowConfirm(string message, System.Action onYes)
    {
        confirmText.text = message;
        _onConfirmYes = onYes;
        confirmPanel.SetActive(true);
    }

    private void BeginIntro()
    {
        _introIndex = 0;
        _introActive = true;
        introPanel.SetActive(true);
        introText.text = introLines.Length > 0
            ? introLines[0]
            : string.Empty;
    }

    private void AdvanceIntro()
    {
        _introIndex += 1;

        if (_introIndex < introLines.Length)
        {
            introText.text = introLines[_introIndex];
            return;
        }

        _introActive = false;
        introPanel.SetActive(false);
        LoadMain(firstMainLevel);
    }

    private void LoadMain(LevelDefinition level)
    {
        GameSession.CurrentLevel = level != null
            ? level
            : firstMainLevel;
        SceneManager.LoadScene(mainSceneName);
    }

    private LevelDefinition ResolveLevel(string levelId)
    {
        LevelDefinition node = firstMainLevel;

        for (int guard = 0; node != null && guard < 64; guard++)
        {
            if (node.name == levelId)
            {
                return node;
            }

            node = node.NextLevel;
        }

        if (catalog != null)
        {
            foreach (ContentEntry entry in catalog.Entries)
            {
                if (entry != null &&
                    entry.ContentType == ContentType.Level &&
                    entry.Level != null &&
                    entry.Level.name == levelId)
                {
                    return entry.Level;
                }
            }
        }

        return firstMainLevel;
    }
}
