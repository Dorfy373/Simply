using System.Collections;
using UnityEngine;

public class EnemyPatrolling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private EnemyMovement _movement;

    [Header("Settings")]
    [SerializeField] private float _patrolWaitTime = 2f;

    private int _currentPatrolIndex;
    private bool _isWaiting;

    public void Patrol()
    {
        if (_isWaiting) return;

        if (_movement.HasReachedDestination)
            StartCoroutine(WaitAtPatrolPoint());
    }

    public void GoToNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0) return;

        _movement.MoveTo(_patrolPoints[_currentPatrolIndex].position);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
    }

    public void GoToClosestPatrolPoint()
    {
        if(_patrolPoints.Length == 0) return;

        float closestDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < _patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, _patrolPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        _currentPatrolIndex = closestIndex;
        _movement.MoveTo(_patrolPoints[_currentPatrolIndex].position);
    }
    private IEnumerator WaitAtPatrolPoint()
    {
        _isWaiting = true;
        _movement.Stop();

        yield return new WaitForSeconds(_patrolWaitTime);

        GoToNextPatrolPoint();
        _isWaiting = false;
    }

    public void SetPatrolPoints(Transform[] points)
    {
        _patrolPoints = points;
    }
}