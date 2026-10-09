using UnityEngine;

public class EnemyPerception : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _chaseRange = 14f;
    [SerializeField] private float _viewAngle = 90f;
    [SerializeField] private float _losePlayerTime = 3f;


    public float DetectionRange => _detectionRange;
    public float LosePlayerTime => _losePlayerTime;

    public float DistanceToPlayer(Vector3 playerPosition)
        => Vector3.Distance(playerPosition, transform.position);
    public bool CanDetectPlayer(Vector3 playerPosition)
        => DistanceToPlayer(playerPosition) <= _detectionRange && CanSeePlayer(playerPosition);

    public bool IsPlayerInChaseRange(Vector3 playerPosition)
        => DistanceToPlayer(playerPosition) <= _chaseRange && CanSeePlayer(playerPosition);


    public bool CanSeePlayer(Vector3 playerPosition)
    {
        return IsFacingPlayer(playerPosition) && HasClearPathToPlayer(playerPosition);
    }

    private bool IsFacingPlayer(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        return angleToPlayer < _viewAngle / 2f;
    }

    private bool HasClearPathToPlayer(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, directionToPlayer.magnitude))
        {
            return hit.collider.CompareTag("Player");
        }
        return true;
    }
}
