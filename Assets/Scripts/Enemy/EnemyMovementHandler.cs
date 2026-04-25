using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyBase))]
    public class EnemyMovementHandler : MonoBehaviour
    {
        private EnemyBase _enemyBase;
        private Rigidbody _rigidbody;
        private const float FacingDeadZone = 0.01f;

        private void Awake()
        {
            _enemyBase = GetComponent<EnemyBase>();

            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
                _rigidbody.constraints =  RigidbodyConstraints.FreezeRotationX | 
                                          RigidbodyConstraints.FreezeRotationY | 
                                          RigidbodyConstraints.FreezeRotationZ;
            }
        }

        private void FixedUpdate()
        {
            var playerTransform = _enemyBase.PlayerTransform;
            if (!_enemyBase.CanSeePlayer || playerTransform == null)
            {
                return;
            }

            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            if (_enemyBase.Type == EnemyType.Ranged)
            {
                float distance = direction.magnitude;
                float minDistance = _enemyBase.PreferredDistanceToPlayer - _enemyBase.DistanceTolerance;
                float maxDistance = _enemyBase.PreferredDistanceToPlayer + _enemyBase.DistanceTolerance;

                if (distance >= minDistance && distance <= maxDistance)
                {
                    return;
                }

                if (distance < minDistance)
                {
                    direction = -direction;
                }
            }

            UpdateSpriteFacing(direction);

            Vector3 step = direction.normalized * (_enemyBase.MovementSpeed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(_rigidbody.position + step);
        }

        private void UpdateSpriteFacing(Vector3 direction)
        {
            var spriteRenderer = _enemyBase.SpriteRenderer;
            if (spriteRenderer == null)
            {
                return;
            }

            if (direction.x > FacingDeadZone)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < -FacingDeadZone)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}


