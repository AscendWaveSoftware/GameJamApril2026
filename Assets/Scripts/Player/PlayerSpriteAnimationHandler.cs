using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PlayerEquipmentHandler))]
    public class PlayerSpriteAnimationHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PlayerEquipmentHandler equipmentHandler;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerAttackHandler attackHandler;

        [Header("Animation Speed")]
        [SerializeField] private float framesPerSecond = 8f;

        [Header("Unarmed")]
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] neutralMoveFrames;

        [Header("Knife")]
        [SerializeField] private Sprite[] idleKnifeFrames;
        [SerializeField] private Sprite[] meleeHoldFrames;
        [SerializeField] private Sprite[] meleeAimFrames;

        [Header("Rifle")]
        [SerializeField] private Sprite[] idleRifleFrames;
        [SerializeField] private Sprite[] rangeHoldFrames;
        [SerializeField] private Sprite[] rangeAimFrames;

        private Sprite[] currentFrames;
        private int currentFrameIndex;
        private float frameTimer;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (equipmentHandler == null)
            {
                equipmentHandler = GetComponent<PlayerEquipmentHandler>();
            }

            if (movement == null)
            {
                movement = GetComponent<PlayerMovement>();
            }

            if (attackHandler == null)
            {
                attackHandler = GetComponent<PlayerAttackHandler>();
            }
        }

        private void Update()
        {
            Sprite[] targetFrames = ResolveAnimationFrames();
            if (targetFrames == null || targetFrames.Length == 0 || spriteRenderer == null)
            {
                return;
            }

            if (!ReferenceEquals(currentFrames, targetFrames))
            {
                currentFrames = targetFrames;
                currentFrameIndex = 0;
                frameTimer = 0f;
                spriteRenderer.sprite = currentFrames[currentFrameIndex];
            }

            if (currentFrames.Length <= 1)
            {
                return;
            }

            float frameDuration = 1f / Mathf.Max(1f, framesPerSecond);
            frameTimer += Time.deltaTime;

            while (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                currentFrameIndex = (currentFrameIndex + 1) % currentFrames.Length;
                spriteRenderer.sprite = currentFrames[currentFrameIndex];
            }
        }

        private Sprite[] ResolveAnimationFrames()
        {
            bool isMoving = movement != null && movement.IsMoving;
            bool isAttacking = attackHandler != null && attackHandler.IsAttacking;
            Weapon equippedWeapon = equipmentHandler != null ? equipmentHandler.GetEquippedWeapon() : null;

            if (equippedWeapon is Knife)
            {
                if (isAttacking)
                {
                    return GetFallbackFrames(meleeAimFrames, meleeHoldFrames, idleKnifeFrames, idleFrames);
                }

                if (isMoving)
                {
                    return GetFallbackFrames(meleeHoldFrames, idleKnifeFrames, idleFrames);
                }

                return GetFallbackFrames(idleKnifeFrames, idleFrames);
            }

            if (equippedWeapon is Rifle)
            {
                if (isAttacking)
                {
                    return GetFallbackFrames(rangeAimFrames, rangeHoldFrames, idleRifleFrames, idleFrames);
                }

                if (isMoving)
                {
                    return GetFallbackFrames(rangeHoldFrames, idleRifleFrames, idleFrames);
                }

                return GetFallbackFrames(idleRifleFrames, idleFrames);
            }

            if (isMoving)
            {
                return GetFallbackFrames(neutralMoveFrames, idleFrames);
            }

            return GetFallbackFrames(idleFrames);
        }

        private static Sprite[] GetFallbackFrames(params Sprite[][] candidates)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                if (candidates[i] != null && candidates[i].Length > 0)
                {
                    return candidates[i];
                }
            }

            return null;
        }
    }
}

