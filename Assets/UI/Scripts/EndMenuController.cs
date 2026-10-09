using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIDocument _uiDocument;

    [Header("Scenes")]
    [SerializeField] private string _loadingSceneName = "LoadingScreen";

    private Button _restartButton;
    private Button _backToMainMenuButton;
    private Button _quitButton;

    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;

        _restartButton = root.Q<Button>("RestartButton");
        _backToMainMenuButton = root.Q<Button>("BackToMainMenuButton");
        _quitButton = root.Q<Button>("QuitButton");

        if (_restartButton != null)
            _restartButton.clicked += RestartGame;

        if (_backToMainMenuButton != null)
            _backToMainMenuButton.clicked += BackToMainMenu;

        if (_quitButton != null)
            _quitButton.clicked += QuitGame;

        Time.timeScale = 1f;

        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    private void RestartGame()
    {
        PlayerPrefs.SetString("SceneToLoad", "Game");
        PlayerPrefs.Save();

        SceneManager.LoadScene("LoadingScreen");
    }

    private void BackToMainMenu()
    {
        PlayerPrefs.SetString("SceneToLoad", "MainMenu");
        PlayerPrefs.Save();

        SceneManager.LoadScene("LoadingScreen");
    }

    private void LoadThroughLoadingScreen(string sceneName)
    {
        Time.timeScale = 1f;

        PlayerPrefs.SetString("SceneToLoad", sceneName);
        PlayerPrefs.Save();

        SceneManager.LoadScene(_loadingSceneName);
    }

    private void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        if (_restartButton != null)
            _restartButton.clicked -= RestartGame;

        if (_backToMainMenuButton != null)
            _backToMainMenuButton.clicked -= BackToMainMenu;

        if (_quitButton != null)
            _quitButton.clicked -= QuitGame;
    }
}
