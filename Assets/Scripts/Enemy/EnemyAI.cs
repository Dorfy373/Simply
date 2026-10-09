using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Components")]
    [SerializeField] private EnemyPatrolling _patrolling;
    [SerializeField] private EnemyPerception _perception;
    [SerializeField] private EnemyChasing _chasing;
    [SerializeField] private EnemyMovement _movement;
    [SerializeField] private EnemyAnimator _animator;
    [SerializeField] private EnemyHealth _health;
    [SerializeField] private EnemyCombat _combat;
    [SerializeField] private PlayerHealth _playerHealth;

    [Header("Stopping Distances")]
    [SerializeField] private float _patrolStoppingDistance = 0.5f;
    [SerializeField] private float _chaseStoppingDistance = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 2f;

    [Header("Death Settings")]
    [SerializeField] private float _destroyDelay = 3f;

    [Header("State")]
    [SerializeField] private EnemyState _currentState = EnemyState.Patrolling;

    private float _timeSinceLostPlayer = 0f;

    private bool _isDead;

    private void OnEnable()
    {
        _health.OnDeath += HandleDeath;
    }
    private void Start()
    {
        _movement.SetStoppingDistance(_patrolStoppingDistance);
        _patrolling.GoToNextPatrolPoint();
    }

    private void Update()
    {
        if(_isDead) return;


        switch (_currentState)
        {
            case EnemyState.Patrolling:
                _patrolling.Patrol();

                if(_movement.IsMoving)
                    _animator.MovementAnimation(true, false);
                else
                    _animator.MovementAnimation(false, false);

                _movement.SetSpeed(false);
                if (_perception.CanDetectPlayer(_playerTransform.position))
                {
                    _currentState = EnemyState.Chasing;
                    _movement.SetStoppingDistance(_chaseStoppingDistance);
                }
                break;

            case EnemyState.Chasing:
                _chasing.ChasePlayer(_playerTransform.position);

                if (_perception.IsPlayerInChaseRange(_playerTransform.position) && !_playerHealth.IsDead)
                {

                    float distance = _perception.DistanceToPlayer(_playerTransform.position);
                    if(distance<= _attackRange)
                    {
                        _currentState = EnemyState.Attacking;
                        _movement.Stop();
                        break;
                    }

                    if (!_movement.IsMoving)
                    {
                        _animator.MovementAnimation(false, false);
                    }
                    else if(_perception.DistanceToPlayer(_playerTransform.position) <= 6)
                    {
                        _animator.MovementAnimation(true, false);
                        _movement.SetSpeed(false);
                    }
                    else
                    {
                        _animator.MovementAnimation(false, true);
                        _movement.SetSpeed(true);
                    }
                    _timeSinceLostPlayer = 0f;
                }
                else
                {
                    _timeSinceLostPlayer += Time.deltaTime;
                    if (_timeSinceLostPlayer >= _perception.LosePlayerTime)
                    {
                        _currentState = EnemyState.Patrolling;
                        _movement.SetStoppingDistance(_patrolStoppingDistance);
                        _patrolling.GoToClosestPatrolPoint();
                    }
                }
                break;

            case EnemyState.Attacking:

                if (_playerHealth.IsDead)
                {
                    _currentState = EnemyState.Chasing;
                    break;
                }

                _animator.MovementAnimation(false, false);

                float distanceToPlayer = _perception.DistanceToPlayer(_playerTransform.position);

                if (distanceToPlayer > _attackRange)
                {
                    _currentState = EnemyState.Chasing;
                    break;
                }

                _combat.Attack();
                break;
        }
    }
    public void Initialize(Transform playerTransform, PlayerHealth playerHealth, Transform[] patrolPoints)
    {
        _playerTransform = playerTransform;
        _playerHealth = playerHealth;
        _patrolling.SetPatrolPoints(patrolPoints);
    }
    private void HandleDeath()
    {
        _isDead = true;

        _movement.Stop();
        _animator.TriggerDeath();

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, _destroyDelay);
    }

    private void OnDisable()
    {
        _health.OnDeath -= HandleDeath;
    }
}
public enum EnemyState
{
    Patrolling,
    Chasing,
    Attacking
}
