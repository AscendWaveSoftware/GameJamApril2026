using UnityEngine;
using Player;

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
        [SerializeField] private float projectileSpawnHeight = 0.8f;
        [SerializeField] private float projectileRadius = 0.15f;

        private EnemyBase _enemyBase;
        private PlayerHealth _playerHealth;
        private PlayerAttackHandler _playerAttackHandler;
        private float _nextAttackTime;
        private const float PlayerMeleeAdvantage = 0.15f;

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

            if (_playerHealth == null)
            {
                _playerHealth = playerTransform.GetComponentInParent<PlayerHealth>();
            }

            if (_playerAttackHandler == null)
            {
                _playerAttackHandler = playerTransform.GetComponentInParent<PlayerAttackHandler>();
            }

            PerformAttack(playerTransform);
            _nextAttackTime = Time.time + Mathf.Max(0.05f, timeBetweenShots);
        }

        private bool IsPlayerInAttackRange(Transform playerTransform)
        {
            float effectiveRange = _enemyBase.AttackRange;
            if (_enemyBase.Type == EnemyType.Meele && _playerAttackHandler != null)
            {
                float cappedMeleeRange = Mathf.Max(0.1f, _playerAttackHandler.MeleeReach - PlayerMeleeAdvantage);
                effectiveRange = Mathf.Min(effectiveRange, cappedMeleeRange);
            }

            Vector3 toPlayer = playerTransform.position - transform.position;
            toPlayer.y = 0f;
            return toPlayer.sqrMagnitude <= effectiveRange * effectiveRange;
        }

        private void PerformAttack(Transform playerTransform)
        {
            if (!IsPlayerInAttackRange(playerTransform))
            {
                return;
            }

            if (_enemyBase.Type == EnemyType.Ranged)
            {
                FireProjectile(playerTransform);
            }
            else if (_enemyBase.Type == EnemyType.Meele && _playerHealth != null)
            {
                _playerHealth.TakeDamage(_enemyBase.AttackDamage);
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


