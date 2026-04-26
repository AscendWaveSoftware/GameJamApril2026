using System;
using UnityEngine;

namespace Enemy
{
    public enum EnemyType
    {
        Meele,
        Ranged
    }

    [RequireComponent(typeof(EnemyVisionHandler))]
    [RequireComponent(typeof(EnemyMovementHandler))]
    [RequireComponent(typeof(EnemyAttackHandler))]
    [RequireComponent(typeof(EnemyHealthBarHandler))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class EnemyBase : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private CapsuleCollider _capsuleCollider;
        private SpriteRenderer _spriteRenderer;

        public Transform PlayerTransform { get; private set; }
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        // Attributes
        [field: SerializeField] public EnemyType Type { get; private set; } = EnemyType.Meele;
        [field: SerializeField] public float Mass { get; private set; } = 1f;
        [field: SerializeField] public float MovementSpeed { get; private set; } = 1f;
        [field: SerializeField] public float VisionRange { get; private set; } = 10f;
        [field: SerializeField] public float PreferredDistanceToPlayer { get; private set; } = 5f;
        [field: SerializeField] public float DistanceTolerance { get; private set; } = 0.5f;
        [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
        [field: SerializeField] public float CurrentHealth { get; private set; } = 10f;
        [field: SerializeField] public float AttackDamage { get; private set; } = 1f;
        [field: SerializeField] public float AttackRange { get; private set; } = 6f;
        [field: SerializeField] public float TimeBetweenAttacks { get; private set; } = 1f;
        [SerializeField] private Sprite meeleSprite;
        [SerializeField] private Sprite rangedSprite;

        // States
        [field: SerializeField] public bool CanSeePlayer { get; set; }

        // Events
        public event Action OnDamage;
        public event Action OnDeath;
        public event Action OnAttackPlayer;
        public event Action OnPlayerSeen;
        public event Action OnPlayerLost;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            ApplyTypeSprite();
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            var player = FindFirstObjectByType<Player.PlayerMovementHandler>();
            if (player != null)
            {
                PlayerTransform = player.transform;
            }
        }

        private void OnValidate()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            ApplyTypeSprite();
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.mass = Mass;

            _capsuleCollider = GetComponent<CapsuleCollider>();
        }

        // APIs
        public void DamageEnemy(float damage)
        {
            if (CurrentHealth <= 0f)
            {
                return;
            }

            damage = Mathf.Max(0f, damage);
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
            OnDamage?.Invoke();
            if (CurrentHealth <= 0f)
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }
        }

        public void AttackPlayer()
        {
            OnAttackPlayer?.Invoke();
        }
        
        public void SetCanSeePlayer(bool canSeePlayer)
        {
            if (CanSeePlayer == canSeePlayer)
            {
                return;
            }

            CanSeePlayer = canSeePlayer;

            if (CanSeePlayer)
            {
                OnPlayerSeen?.Invoke();
            }
            else
            {
                OnPlayerLost?.Invoke();
            }
        }

        private void ApplyTypeSprite()
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            if (Type == EnemyType.Ranged)
            {
                if (rangedSprite != null)
                {
                    _spriteRenderer.sprite = rangedSprite;
                }

                return;
            }

            if (meeleSprite != null)
            {
                _spriteRenderer.sprite = meeleSprite;
            }
        }

        

    }
}
