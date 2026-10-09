using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIDocument _uiDocument;


    private Button _startButton;
    private Button _settingsButton;
    private Button _quitButton;
    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;

        _startButton = root.Q<Button>("StartButton");
        _settingsButton = root.Q<Button>("SettingsButton");
        _quitButton = root.Q<Button>("QuitButton");

        if (_startButton != null) _startButton.clicked += OnStartClicked;

        if (_settingsButton != null) _settingsButton.clicked += OnSettingsClicked;

        if (_quitButton != null) _quitButton.clicked += OnQuitClicked;

        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
    private void OnStartClicked()
    {
        PlayerPrefs.SetString("SceneToLoad", "Game");
        PlayerPrefs.Save();

        SceneManager.LoadScene("LoadingScreen");

    }
    private void OnSettingsClicked()
    {
        // Позже сделаем панель настроек
    }

    private void OnQuitClicked()
    {

       #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
       #else
            Application.Quit();
       #endif
    }

    private void OnDisable()
    {
        if (_startButton != null) _startButton.clicked -= OnStartClicked;

        if (_settingsButton != null) _settingsButton.clicked -= OnSettingsClicked;

        if (_quitButton != null) _quitButton.clicked -= OnQuitClicked;
    }
}
