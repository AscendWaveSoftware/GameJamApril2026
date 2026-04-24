using Enemy;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Transform))]
public class DummyAttackHandler : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private float projectileSpawnOffset = 0.6f;
    [SerializeField] private float projectileRadius = 0.15f;
    [SerializeField] private Sprite projectileSprite;

    [Header("Facing")]
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    private void Awake()
    {
        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 direction = GetShootDirection();

        GameObject projectileObject = new GameObject("DummyProjectile");
        projectileObject.transform.position = transform.position + direction * projectileSpawnOffset;
        projectileObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        SpriteRenderer renderer = projectileObject.AddComponent<SpriteRenderer>();
        renderer.sprite = projectileSprite;

        SphereCollider collider = projectileObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = projectileRadius;

        Rigidbody rb = projectileObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        DummyProjectile projectile = projectileObject.AddComponent<DummyProjectile>();
        projectile.Initialize(direction, projectileSpeed, projectileLifetime, damage);
    }

    private Vector3 GetShootDirection()
    {
        if (playerSpriteRenderer != null)
        {
            return playerSpriteRenderer.flipX ? Vector3.left : Vector3.right;
        }

        Vector3 forward = transform.forward;
        forward.y = 0f;
        return forward.sqrMagnitude > 0.0001f ? forward.normalized : Vector3.forward;
    }
}

public class DummyProjectile : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private float _remainingLifetime;
    private float _damage;

    public void Initialize(Vector3 direction, float speed, float lifetime, float damage)
    {
        _direction = direction.normalized;
        _speed = speed;
        _remainingLifetime = lifetime;
        _damage = damage;
    }

    private void Update()
    {
        transform.position += _direction * (_speed * Time.deltaTime);

        _remainingLifetime -= Time.deltaTime;
        if (_remainingLifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.DamageEnemy(_damage);
            Destroy(gameObject);
        }
    }
}

