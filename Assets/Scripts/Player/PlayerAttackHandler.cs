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
        [SerializeField] private float knifeCooldownSeconds = 0.45f;

        [Header("Animation")] [SerializeField] private float attackAnimationDuration = 0.12f;

        [Header("Facing")] [SerializeField] private SpriteRenderer playerSpriteRenderer;

        [SerializeField] private PlayerEquipmentHandler equipmentHandler;

        [Header("Attack VFX")]
        [SerializeField] private ParticleSystem knifeSwooshEffectPrefab;
        [SerializeField] private ParticleSystem rifleSmokeEffectPrefab;
        [SerializeField] private float generatedVfxLifetime = 1f;

        [Header("Attack Audio")]
        [SerializeField] private AudioClip shootClip;
        [SerializeField] private float shootVolume = 0.3f;
        [SerializeField] private float shootPitchMin = 0.75f;
        [SerializeField] private float shootPitchMax = 1.25f;

        private float _nextAttackTime;
        private float _lastAttackTime = -999f;

        public bool IsAttacking => Time.time <= _lastAttackTime + attackAnimationDuration;
        public float MeleeReach => Mathf.Max(0f, meleeRange) + Mathf.Max(0.01f, meleeRadius);

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
                ApplyKnifeCooldown();
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

        private void ApplyKnifeCooldown()
        {
            _nextAttackTime = Time.time + Mathf.Max(0.05f, knifeCooldownSeconds);
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
                    enemy.DamageEnemy(damage, transform.root);
                }
            }

            PlayKnifeSwooshVfx(direction);
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

            PlayKnifeSwooshVfx(direction);

            if (shootClip != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayClipWithRandomPitch(shootClip, shootPitchMin, shootPitchMax, shootVolume);
            }
        }

        private void PlayKnifeSwooshVfx(Vector3 direction)
        {
            Vector3 safeDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
            Vector3 vfxPosition = transform.position + safeDirection * Mathf.Max(0.2f, meleeRange * 0.7f) + Vector3.up * meleeHeightOffset;
            Quaternion vfxRotation = Quaternion.LookRotation(safeDirection, Vector3.up);

            if (knifeSwooshEffectPrefab != null)
            {
                ParticleSystem instance = Instantiate(knifeSwooshEffectPrefab, vfxPosition, vfxRotation);
                if (instance != null)
                {
                    instance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ApplyKnifeSwooshStyle(instance);
                    instance.Play(true);
                    Destroy(instance.gameObject, Mathf.Max(0.25f, GetParticleLifetime(instance)));
                }
                return;
            }

            SpawnGeneratedKnifeSwoosh(vfxPosition, vfxRotation);
        }

        private void PlayRifleSmokeVfx(Vector3 spawnPosition, Vector3 direction)
        {
            Vector3 safeDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
            Quaternion vfxRotation = Quaternion.LookRotation(safeDirection, Vector3.up);

            if (rifleSmokeEffectPrefab != null)
            {
                ParticleSystem instance = Instantiate(rifleSmokeEffectPrefab, spawnPosition, vfxRotation);
                if (instance != null)
                {
                    instance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ApplyRifleSmokeStyle(instance);
                    instance.Play(true);
                    Destroy(instance.gameObject, Mathf.Max(0.25f, GetParticleLifetime(instance)));
                }
                return;
            }

            SpawnGeneratedRifleSmoke(spawnPosition, vfxRotation);
        }

        private void ApplyKnifeSwooshStyle(ParticleSystem ps)
        {
            if (ps == null)
            {
                return;
            }

            var main = ps.main;
            main.playOnAwake = false;
            main.loop = false;
            main.duration = 0.14f;
            main.startLifetime = 0.18f;
            main.startSpeed = 2.8f;
            main.startSize = 0.22f;
            main.startColor = new Color(1f, 1f, 1f, 0.9f);
            main.simulationSpace = ParticleSystemSimulationSpace.Local;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 28) });

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.36f;
            shape.arc = 150f;
            shape.radiusThickness = 0.05f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(1f, 1f, 1f), 0f),
                    new GradientColorKey(new Color(1f, 1f, 1f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0.9f, 0f),
                    new GradientAlphaKey(0f, 1f)
                });
            colorOverLifetime.color = gradient;
        }

        private void ApplyRifleSmokeStyle(ParticleSystem ps)
        {
            if (ps == null)
            {
                return;
            }

            var main = ps.main;
            main.playOnAwake = false;
            main.loop = false;
            main.duration = 0.16f;
            main.startLifetime = 0.22f;
            main.startSpeed = 4.8f;
            main.startSize = 0.24f;
            main.startColor = new Color(1f, 1f, 1f, 0.9f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 24) });

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 36f;
            shape.radius = 0.12f;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = false;
        }

        private void SpawnGeneratedKnifeSwoosh(Vector3 position, Quaternion rotation)
        {
            GameObject go = new GameObject("KnifeSwooshVFX");
            go.transform.position = position;
            go.transform.rotation = rotation;

            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = false;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            main.loop = false;
            main.duration = 0.14f;
            main.startLifetime = 0.18f;
            main.startSpeed = 2.8f;
            main.startSize = 0.22f;
            main.startColor = new Color(1f, 1f, 1f, 0.9f);
            main.simulationSpace = ParticleSystemSimulationSpace.Local;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 28) });

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.36f;
            shape.arc = 150f;
            shape.radiusThickness = 0.05f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(1f, 1f, 1f), 0f),
                    new GradientColorKey(new Color(1f, 1f, 1f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0.9f, 0f),
                    new GradientAlphaKey(0f, 1f)
                });
            colorOverLifetime.color = gradient;

            ps.Play(true);
            Destroy(go, Mathf.Max(0.25f, generatedVfxLifetime));
        }

        private void SpawnGeneratedRifleSmoke(Vector3 position, Quaternion rotation)
        {
            GameObject go = new GameObject("RifleSmokeVFX");
            go.transform.position = position;
            go.transform.rotation = rotation;

            ParticleSystem ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = false;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ApplyRifleSmokeStyle(ps);

            ps.Play(true);
            Destroy(go, Mathf.Max(0.25f, generatedVfxLifetime));
        }


        private float GetParticleLifetime(ParticleSystem ps)
        {
            var main = ps.main;
            float lifetime = main.duration;

            if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
            {
                lifetime += main.startLifetime.constant;
            }
            else if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
            {
                lifetime += main.startLifetime.constantMax;
            }
            else
            {
                lifetime += 1f;
            }

            return lifetime;
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