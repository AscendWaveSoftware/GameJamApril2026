using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyBase))]
    public class EnemyAttackHandler : MonoBehaviour
    {
        [Header("Attack Timing")]
        [SerializeField] private float timeBetweenShots = 1f;

        [Header("Ranged Projectile")]
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private float projectileSpeed = 8f;
        [SerializeField] private float projectileLifetime = 3f;
        [SerializeField] private float projectileSpawnDistance = 0.6f;
        [SerializeField] private float projectileSpawnHeight = 0f;
        [SerializeField] private float projectileRadius = 0.15f;

        private EnemyBase _enemyBase;
        private float _nextAttackTime;

        private void Awake()
        {
            _enemyBase = GetComponent<EnemyBase>();
        }

        private void Update()
        {
            Transform playerTransform = _enemyBase.PlayerTransform;
            if (!_enemyBase.CanSeePlayer || playerTransform == null)
            {
                return;
            }

            if (Time.time < _nextAttackTime)
            {
                return;
            }

            if (!IsPlayerInAttackRange(playerTransform))
            {
                return;
            }

            PerformAttack(playerTransform);
            _nextAttackTime = Time.time + Mathf.Max(0.05f, timeBetweenShots);
        }

        private bool IsPlayerInAttackRange(Transform playerTransform)
        {
            Vector3 toPlayer = playerTransform.position - transform.position;
            toPlayer.y = 0f;
            return toPlayer.sqrMagnitude <= _enemyBase.AttackRange * _enemyBase.AttackRange;
        }

        private void PerformAttack(Transform playerTransform)
        {
            if (_enemyBase.Type == EnemyType.Ranged)
            {
                FireProjectile(playerTransform);
            }

            // Shared attack entry-point for both enemy types.
            _enemyBase.AttackPlayer();
        }

        private void FireProjectile(Transform playerTransform)
        {
            if (projectileSprite == null)
            {
                return;
            }

            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = transform.forward;
            }

            direction.Normalize();

            Vector3 spawnPosition = transform.position + direction * projectileSpawnDistance + Vector3.up * projectileSpawnHeight;
            global::BulletProjectile.Spawn(
                "EnemyBullet",
                spawnPosition,
                direction,
                projectileSprite,
                projectileSpeed,
                projectileLifetime,
                _enemyBase.AttackDamage,
                projectileRadius,
                global::BulletOwner.Enemy,
                transform.root);
        }
    }
}


