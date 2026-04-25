using UnityEngine;

public enum BulletOwner
{
    Player,
    Enemy
}

public class BulletProjectile : MonoBehaviour
{
    private const float MinimumDirectionSqrMagnitude = 0.0001f;

    private Vector3 _direction;
    private float _speed;
    private float _remainingLifetime;
    private float _damage;
    private BulletOwner _owner;
    private Transform _ownerRoot;


    public static BulletProjectile Spawn(
        string objectName,
        Vector3 position,
        Vector3 direction,
        Sprite sprite,
        float speed,
        float lifetime,
        float damage,
        float radius,
        BulletOwner owner,
        Transform ownerRoot)
    {
        if (direction.sqrMagnitude <= MinimumDirectionSqrMagnitude)
        {
            direction = Vector3.forward;
        }

        direction.Normalize();

        GameObject projectileObject = new GameObject(objectName);
        projectileObject.transform.position = position;
        projectileObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        SpriteRenderer renderer = projectileObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        SphereCollider collider = projectileObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = Mathf.Max(0.01f, radius);

        Rigidbody rb = projectileObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        BulletProjectile projectile = projectileObject.AddComponent<BulletProjectile>();
        projectile.Initialize(direction, speed, lifetime, damage, owner, ownerRoot);
        return projectile;
    }

    public void Initialize(
        Vector3 direction,
        float speed,
        float lifetime,
        float damage,
        BulletOwner owner,
        Transform ownerRoot)
    {
        _direction = direction.normalized;
        _speed = speed;
        _remainingLifetime = lifetime;
        _damage = damage;
        _owner = owner;
        _ownerRoot = ownerRoot;
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
        if (_ownerRoot != null && other.transform.root == _ownerRoot)
        {
            return;
        }

        if (_owner == BulletOwner.Player)
        {
            global::Enemy.EnemyBase enemy = other.GetComponentInParent<global::Enemy.EnemyBase>();
            if (enemy != null)
            {
                enemy.DamageEnemy(_damage);
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            global::Player.PlayerHealth playerHealth = other.GetComponentInParent<global::Player.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage);
                Destroy(gameObject);
                return;
            }

            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
            if (player != null)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}

