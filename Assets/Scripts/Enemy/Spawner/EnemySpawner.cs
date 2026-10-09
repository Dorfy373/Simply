using UnityEngine;
[System.Serializable]
public class EnemySpawnPoint
{
    public Transform spawnTransform;
    public Transform[] patrolPoints;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject _enemyPrefab;

    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerExperience _playerExperience;

    [Header("Spawn Points")]
    [SerializeField] private EnemySpawnPoint[] _spawnPoints;

    private void Start()
    {
        foreach (EnemySpawnPoint spawnPoint in _spawnPoints)
        {
            SpawnEnemy(spawnPoint);
        }
    }

    private void SpawnEnemy(EnemySpawnPoint spawnPoint)
    {
        GameObject enemyInstance = Instantiate(_enemyPrefab, spawnPoint.spawnTransform.position, Quaternion.identity);
        enemyInstance.GetComponent<EnemyHealth>().Initialize(_playerExperience);
        EnemyAI enemyAI = enemyInstance.GetComponent<EnemyAI>();
        enemyAI.Initialize(_playerTransform, _playerHealth, spawnPoint.patrolPoints);
    }
}
