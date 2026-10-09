using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIDocument _uiDocument;

    [Header("Loading")]
    [SerializeField] private float _minimumLoadingTime = 2f;

    private VisualElement _spinner;
    private Label _progressLabel;

    private float _spinnerAngle;

    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;

        _spinner = root.Q<VisualElement>("Spinner");
        _progressLabel = root.Q<Label>("ProgressLabel");

        string sceneToLoad =
            PlayerPrefs.GetString("SceneToLoad", "Game");

        StartCoroutine(LoadingRoutine(sceneToLoad));
    }

    private void Update()
    {
        if (_spinner == null)
            return;

        _spinnerAngle += 360f * Time.unscaledDeltaTime;

        if (_spinnerAngle >= 360f)
            _spinnerAngle -= 360f;

        _spinner.style.rotate = new Rotate(_spinnerAngle);
    }

    private IEnumerator LoadingRoutine(string sceneName)
    {
        // ------------------------------------------------
        // 1. Даём LoadingScreen реально появиться на экране
        // ------------------------------------------------

        if (_progressLabel != null)
            _progressLabel.text = "0%";

        yield return null;
        yield return null;

        // ------------------------------------------------
        // 2. Минимальное время показа LoadingScreen
        // ------------------------------------------------

        float timer = 0f;

        while (timer < _minimumLoadingTime)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(timer / _minimumLoadingTime);

            int percent =
                Mathf.RoundToInt(progress * 100f);

            if (_progressLabel != null)
                _progressLabel.text = percent + "%";

            yield return null;
        }

        // ------------------------------------------------
        // 3. Загружаем нужную сцену
        // ------------------------------------------------


        AsyncOperation operation =
            SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progress =
                Mathf.Clamp01(operation.progress / 0.9f);

            int percent =
                Mathf.RoundToInt(progress * 100f);

            if (_progressLabel != null)
                _progressLabel.text = percent + "%";

            yield return null;
        }

        // Сцена загружена, ждём её активации
        if (_progressLabel != null)
            _progressLabel.text = "100%";

        yield return new WaitForSecondsRealtime(0.2f);


        operation.allowSceneActivation = true;
    }
}
