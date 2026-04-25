using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnDistance = 10f;
        [SerializeField] private float spawnDelaySeconds = 5f;
        [SerializeField] private float spawnSuccessChance = 0.8f;

        [Header("Debug")]
        [SerializeField] private bool debug;

        private Transform _playerTransform;
        private float _nextSpawnAttemptTime;

        private void Awake()
        {
            var playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement != null)
            {
                _playerTransform = playerMovement.transform;
            }

            _nextSpawnAttemptTime = Random.Range(0f, spawnDelaySeconds);
        }

        private void Update()
        {
            if (_playerTransform == null)
            {
                return;
            }

            if (Time.time >= _nextSpawnAttemptTime)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

                if (distanceToPlayer >= spawnDistance)
                {
                    float roll = Random.value;
                    bool success = roll <= spawnSuccessChance;

                    if (success)
                    {
                        SpawnEnemy();
                    }
                }

                _nextSpawnAttemptTime = Time.time + spawnDelaySeconds;
            }
        }

        private void SpawnEnemy()
        {
            if (enemyPrefab == null)
            {
                Debug.LogWarning("EnemySpawner: enemyPrefab is not assigned!", this);
                return;
            }

            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            if (!debug)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Mathf.Max(0f, spawnDistance));
        }
    }
}
