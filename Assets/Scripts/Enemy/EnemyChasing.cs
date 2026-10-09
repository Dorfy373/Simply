using UnityEngine;

public class EnemyChasing : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyMovement _movement;

    public void ChasePlayer(Vector3 playerPosition)
    {
        _movement.MoveTo(playerPosition);
    }
}
