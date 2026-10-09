using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _stopAtDistance = 0.5f;
    private NavMeshAgent _navMeshAgent;

    [Header("Speed Settings")]
    [SerializeField] private float _runningSpeed = 7f;
    [SerializeField] private float _walkingSpeed = 3.5f;

    public bool HasReachedDestination =>
        !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _stopAtDistance;

    public bool IsMoving => _navMeshAgent.velocity.magnitude > 0.1f;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

   
    public void MoveTo(Vector3 destination)
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(destination);
    }

    public void SetSpeed(bool isRunning)
    {
        _navMeshAgent.speed = isRunning ? _runningSpeed : _walkingSpeed;
    }

    public void Stop()
    {
        _navMeshAgent.isStopped = true;
    }

    public void SetStoppingDistance(float distance)
    {
        _navMeshAgent.stoppingDistance = distance;
    }
}