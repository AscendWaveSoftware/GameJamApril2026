using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PlayerEquipmentHandler))]
    public class PlayerSpriteAnimationHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PlayerEquipmentHandler equipmentHandler;
        [FormerlySerializedAs("movement")] [SerializeField] private PlayerMovementHandler movementHandler;
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

            if (movementHandler == null)
            {
                movementHandler = GetComponent<PlayerMovementHandler>();
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
            bool isMoving = movementHandler && movementHandler.IsMoving;
            bool isAttacking = attackHandler && attackHandler.IsAttacking;
            Weapon equippedWeapon = equipmentHandler ? equipmentHandler.GetEquippedWeapon() : null;

            if (equippedWeapon is Knife)
            {
                if (isAttacking)
                {
                    return GetFallbackFrames(meleeAimFrames, meleeHoldFrames, idleKnifeFrames, idleFrames);
                }

                return isMoving ? GetFallbackFrames(meleeHoldFrames, idleKnifeFrames, idleFrames) : GetFallbackFrames(idleKnifeFrames, idleFrames);
            }

            if (equippedWeapon is not Rifle)
                return isMoving ? GetFallbackFrames(neutralMoveFrames, idleFrames) : GetFallbackFrames(idleFrames);
            if (isAttacking)
            {
                return GetFallbackFrames(rangeAimFrames, rangeHoldFrames, idleRifleFrames, idleFrames);
            }

            return isMoving ? GetFallbackFrames(rangeHoldFrames, idleRifleFrames, idleFrames) : GetFallbackFrames(idleRifleFrames, idleFrames);

        }

        private static Sprite[] GetFallbackFrames(params Sprite[][] candidates)
        {
            return candidates.FirstOrDefault(t => t is { Length: > 0 });
        }
    }
}

