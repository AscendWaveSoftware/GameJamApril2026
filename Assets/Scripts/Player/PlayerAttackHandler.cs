using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerEquipmentHandler))]
    public class PlayerAttackHandler : MonoBehaviour
    {
        [Header("Attack")] [SerializeField] private float damage = 1f;
        [SerializeField] private float fireRate = 4f;
        [SerializeField] private float fireRateMultiplikator = 1f;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float projectileLifetime = 2f;
        [SerializeField] private float projectileSpawnOffset = 0.6f;
        [SerializeField] private float projectileSpawnHeight = 0.8f;
        [SerializeField] private float projectileRadius = 0.15f;
        [SerializeField] private Sprite projectileSprite;

        [Header("Melee")] [SerializeField] private float meleeRange = 1f;
        [SerializeField] private float meleeRadius = 0.6f;
        [SerializeField] private float meleeHeightOffset = 0.6f;

        [Header("Animation")] [SerializeField] private float attackAnimationDuration = 0.12f;

        [Header("Facing")] [SerializeField] private SpriteRenderer playerSpriteRenderer;

        [SerializeField] private PlayerEquipmentHandler equipmentHandler;

        private float _nextAttackTime;
        private float _lastAttackTime = -999f;

        public bool IsAttacking => Time.time <= _lastAttackTime + attackAnimationDuration;

        public float Damage
        {
            get => damage;
            set => damage = Mathf.Max(0f, value);
        }

        private void Awake()
        {
            if (playerSpriteRenderer == null)
            {
                playerSpriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (equipmentHandler == null)
            {
                equipmentHandler = GetComponent<PlayerEquipmentHandler>();
            }

            if (projectileSprite == null)
            {
                Debug.LogWarning(
                    "PlayerAttackHandler: projectileSprite is not set. Assign a sprite to fire projectiles.", this);
            }
        }

        private void Update()
        {
            UpdateFacing();

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                TryAttack(true);
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                TryAttack(false);
            }
        }

        public void ChangeFireRateMultiplikator(float value)
        {
            fireRateMultiplikator = Mathf.Max(0.01f, value);
        }

        public void ChangePlayerDamage(float value)
        {
            Damage = value;
        }

        private void UpdateFacing()
        {
            if (playerSpriteRenderer == null || Mouse.current == null)
            {
                return;
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            float screenMidX = Screen.width / 2f;

            bool shouldFlipX = mousePos.x < screenMidX;
            playerSpriteRenderer.flipX = shouldFlipX;
        }

        private void TryAttack(bool fromSpacebar)
        {
            Weapon equippedWeapon = equipmentHandler != null ? equipmentHandler.GetEquippedWeapon() : null;
            if (equippedWeapon == null)
            {
                return;
            }

            if (Time.time < _nextAttackTime)
            {
                return;
            }

            if (equippedWeapon is Knife)
            {
                if (!fromSpacebar)
                {
                    return;
                }

                DoMeleeAttack();
                ApplyAttackCooldown();
                return;
            }

            if (equippedWeapon is Rifle)
            {
                Shoot();
                ApplyAttackCooldown();
            }
        }

        private void ApplyAttackCooldown()
        {
            float effectiveFireRate = Mathf.Max(0.01f, fireRate * fireRateMultiplikator);
            _nextAttackTime = Time.time + (1f / effectiveFireRate);
            _lastAttackTime = Time.time;
        }

        private void DoMeleeAttack()
        {
            Vector3 direction = GetShootDirection();
            Vector3 center = transform.position + direction * Mathf.Max(0f, meleeRange) +
                             Vector3.up * meleeHeightOffset;

            Collider[] hits = Physics.OverlapSphere(center, Mathf.Max(0.01f, meleeRadius));
            for (int i = 0; i < hits.Length; i++)
            {
                Enemy.EnemyBase enemy = hits[i].GetComponentInParent<Enemy.EnemyBase>();
                if (enemy != null)
                {
                    enemy.DamageEnemy(damage);
                }
            }
        }

        private void Shoot()
        {
            if (projectileSprite == null)
            {
                return;
            }

            Vector3 direction = GetShootDirection();

            Vector3 spawnPosition = transform.position + direction * projectileSpawnOffset +
                                    Vector3.up * projectileSpawnHeight;
            BulletProjectile.Spawn(
                "PlayerBullet",
                spawnPosition,
                direction,
                projectileSprite,
                projectileSpeed,
                projectileLifetime,
                damage,
                projectileRadius,
                BulletOwner.Player,
                transform.root);
        }

        private Vector3 GetShootDirection()
        {
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;
            if (mainCamera == null)
            {
                return Vector3.forward;
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            float playerY = transform.position.y;
            float t = (playerY - ray.origin.y) / ray.direction.y;

            if (t > 0f)
            {
                Vector3 targetPoint = ray.origin + ray.direction * t;
                Vector3 direction = targetPoint - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.0001f)
                {
                    return direction.normalized;
                }
            }

            return Vector3.forward;
        }
    }
}