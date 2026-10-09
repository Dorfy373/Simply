using Unity.Cinemachine;
using UnityEngine;
public class TPCameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 5f;
    [SerializeField] private float minZoomDistance = 2f;
    [SerializeField] private float maxZoomDistance = 10f;

    [Header("Input")]
    [SerializeField] private InputReader _inputReader;

    [Header("Components")]
    private CinemachineOrbitalFollow _orbitalFollow;

    [Header("State")]
    private Vector2 _scrollDelta;
    private float _targetZoom;
    private float _currentZoom;

    private void Awake()
    {
        _orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        _targetZoom = _currentZoom = _orbitalFollow.Radius;
    }

    private void OnEnable()
    {
        _inputReader.ZoomPerformed += HandleZoomPerformed;
        _inputReader.ZoomCanceled += HandleZoomCanceled;
    }

    private void HandleZoomPerformed(Vector2 delta) => _scrollDelta = delta;
    private void HandleZoomCanceled() => _scrollDelta = Vector2.zero;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleZoom();
    }

    /// <summary>
    /// Обрабатывает зум камеры (приближение/отдаление)
    /// </summary>
    private void HandleZoom()
    {
        // Проверяем, скроллил ли пользователь (y != 0 значит движение было)
        if (_scrollDelta.y != 0)
        {
            // Если компонент CinemachineOrbitalFollow найден:
            if (_orbitalFollow != null)
            {
                // Вычисляем желаемое расстояние камеры:
                // Radius - _scrollDelta.y * zoomSpeed
                //
                // Пример: Radius = 8м, scrollDelta.y = 1 (скролл вверх), zoomSpeed = 2
                // Результат: 8 - 1*2 = 6м  (приблизилась)
                //
                // Пример: Radius = 8м, scrollDelta.y = -1 (скролл вниз), zoomSpeed = 2
                // Результат: 8 - (-1)*2 = 10м  (отдалилась)

                float newZoom = _orbitalFollow.Radius - _scrollDelta.y * zoomSpeed;

                // Зажимаем значение, чтобы оно было в допустимом диапазоне:
                // minZoomDistance = 2м (минимальное расстояние)
                // maxZoomDistance = 10м (максимальное расстояние)
                // 
                // Если игрок скроллит слишком много и newZoom становится < 2м,
                // Clamp вернёт 2м. Если > 10м, вернёт 10м.
                _targetZoom = Mathf.Clamp(newZoom, minZoomDistance, maxZoomDistance);
            }
        }

        // Плавный переход текущего зума к целевому:
        // Lerp плавно двигает currentZoom к targetZoom
        // 
        // currentZoom: текущее расстояние (где камера сейчас)
        // targetZoom: желаемое расстояние (где камера должна быть)
        // zoomLerpSpeed * Time.deltaTime: скорость плавного движения
        //
        // zoomLerpSpeed = 5 (контролирует плавность)
        // Time.deltaTime ≈ 0.016 сек (время прошедшее за один фрейм)
        // 5 * 0.016 = 0.08 (движется на 8% пути каждый фрейм)
        _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, zoomLerpSpeed * Time.deltaTime);

        // Применяем новое расстояние к камере
        _orbitalFollow.Radius = _currentZoom;
    }

    private void OnDisable()
    {
        _inputReader.ZoomPerformed -= HandleZoomPerformed;
        _inputReader.ZoomCanceled -= HandleZoomCanceled;
    }
}

