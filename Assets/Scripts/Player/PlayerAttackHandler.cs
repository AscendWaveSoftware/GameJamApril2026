using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerAttackHandler : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float fireRate = 4f;
    [SerializeField] private float fireRateMultiplikator = 1f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private float projectileSpawnOffset = 0.6f;
    [SerializeField] private float projectileSpawnHeight = 0f;
    [SerializeField] private float projectileRadius = 0.15f;
    [SerializeField] private Sprite projectileSprite;

    [Header("Facing")]
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    private float _nextShootTime;

    private void Awake()
    {
        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (projectileSprite == null)
        {
            Debug.LogWarning("PlayerAttackHandler: projectileSprite is not set. Assign a sprite to fire projectiles.", this);
        }
    }

    private void Update()
    {
        UpdateFacing();
        
        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
        {
            TryShoot();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryShoot();
        }
    }

    public void ChangeFireRateMultiplikator(float value)
    {
        fireRateMultiplikator = Mathf.Max(0.01f, value);
    }

    public void ChangePlayerDamage(float value)
    {
        damage = Mathf.Max(0f, value);
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

    private void TryShoot()
    {
        if (Time.time < _nextShootTime)
        {
            return;
        }

        Shoot();

        float effectiveFireRate = Mathf.Max(0.01f, fireRate * fireRateMultiplikator);
        _nextShootTime = Time.time + (1f / effectiveFireRate);
    }

    private void Shoot()
    {
        if (projectileSprite == null)
        {
            return;
        }

        Vector3 direction = GetShootDirection();

        Vector3 spawnPosition = transform.position + direction * projectileSpawnOffset + Vector3.up * projectileSpawnHeight;
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




