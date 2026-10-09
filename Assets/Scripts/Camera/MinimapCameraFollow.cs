using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [Header("Настройки следования")]
    public Transform _playerTransform;
    public float cameraHeight = 25f;

    private void LateUpdate()
    {
            if (_playerTransform == null) return;

            Vector3 targetPosition = _playerTransform.position;
            targetPosition.y += cameraHeight;
            transform.position = targetPosition;

            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
