using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private UIInputReader _uiInputReader;
    [SerializeField] private AudioSettingsController _audioSettings;

    [Header("Audio")]
    [SerializeField] private AudioSource _musicSource;
    private VisualElement _pauseOverlay;
    private VisualElement _pausePanel;
    private VisualElement _settingsPanel;

    private Button _resumeButton;
    private Button _settingsButton;
    private Button _mainMenuButton;
    private Button _settingsBackButton;

    private Slider _musicVolumeSlider;

    private bool _isPaused;
    private bool _isSettingsOpen;

    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;

        _pauseOverlay = root.Q<VisualElement>("PauseOverlay");
        _pausePanel = root.Q<VisualElement>("PausePanel");
        _settingsPanel = root.Q<VisualElement>("SettingsPanel");

        _resumeButton = root.Q<Button>("ResumeButton");
        _settingsButton = root.Q<Button>("SettingsButton");
        _mainMenuButton = root.Q<Button>("MainMenuButton");
        _settingsBackButton = root.Q<Button>("SettingsBackButton");

        _musicVolumeSlider = root.Q<Slider>("MusicVolumeSlider");


        if (_uiInputReader != null) _uiInputReader.PausePerformed += TogglePause;
        if (_resumeButton != null) _resumeButton.clicked += ResumeGame;
        if (_settingsButton != null) _settingsButton.clicked += OpenSettings;
        if (_mainMenuButton != null) _mainMenuButton.clicked += ReturnToMainMenu;
        if (_settingsBackButton != null) _settingsBackButton.clicked += CloseSettings;

        if (_settingsButton != null)
        {
            _settingsButton.pickingMode = PickingMode.Position;
            _settingsButton.focusable = true;
        }

        if (_musicVolumeSlider != null)
        {
            _musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
            _musicVolumeSlider.value = 1f;
        }

            SetPauseUI(false);
        HideCursor();
    }
    private void ShowCursor()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
    private void HideCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    private void TogglePause()
    {
        if (!_isPaused)
        {
            PauseGame();
            return;
        }
        if (_isSettingsOpen)
        {
            CloseSettings();
            return;
        }

        ResumeGame();
    }

    private void PauseGame()
    {
        _isPaused = true;

        Time.timeScale = 0f;

        if (_musicSource != null)
            _musicSource.Pause();

        ShowCursor();

        SetPauseUI(true);
    }

    private void ResumeGame()
    {
        _isPaused = false;

        Time.timeScale = 1f;

        if (_musicSource != null)
            _musicSource.UnPause();

        HideCursor();

        SetPauseUI(false);
    }

    private void SetPauseUI(bool show)
    {
        if (_pauseOverlay == null)
            return;

        _pauseOverlay.style.display =
            show ? DisplayStyle.Flex : DisplayStyle.None;

        if (show)
        {
            ShowPausePanel();
        }
    }

    private void ShowPausePanel()
    {
        _pausePanel.style.display = DisplayStyle.Flex;
        _settingsPanel.style.display = DisplayStyle.None;

        _isSettingsOpen = false;
    }

    private void OpenSettings()
    {

        if (_pausePanel == null || _settingsPanel == null) return;

        _pausePanel.style.display = DisplayStyle.None;
        _settingsPanel.style.display = DisplayStyle.Flex;

        _isSettingsOpen = true;
    }

    private void CloseSettings()
    {
        _settingsPanel.style.display = DisplayStyle.None;
        _pausePanel.style.display = DisplayStyle.Flex;

        _isSettingsOpen = false;
    }

    private void ReturnToMainMenu()
    {

        Time.timeScale = 1f;

        PlayerPrefs.SetString("SceneToLoad", "MainMenu");
        PlayerPrefs.Save();

        SceneManager.LoadScene("LoadingScreen");
    }
    private void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        if (_audioSettings != null)
        {
            _audioSettings.SetMusicVolume(evt.newValue);
        }

    }
    private void OnDisable()
    {
        if (_uiInputReader != null)
            _uiInputReader.PausePerformed -= TogglePause;

        if (_resumeButton != null) _resumeButton.clicked -= ResumeGame;
        if (_settingsButton != null) _settingsButton.clicked -= OpenSettings;
        if (_mainMenuButton != null) _mainMenuButton.clicked -= ReturnToMainMenu;
        if (_settingsBackButton != null) _settingsBackButton.clicked -= CloseSettings;
        if (_musicVolumeSlider != null) _musicVolumeSlider.UnregisterValueChangedCallback(OnMusicVolumeChanged);

        Time.timeScale = 1f;

    }
}
