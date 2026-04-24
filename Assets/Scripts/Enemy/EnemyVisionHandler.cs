using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyBase))]
    public class EnemyVisionHandler : MonoBehaviour
    {
        private EnemyBase _enemyBase;

        private void Awake()
        {
            _enemyBase = GetComponent<EnemyBase>();
        }

        private void Update()
        {
            var playerTransform = _enemyBase.PlayerTransform;
            var canSeePlayer = playerTransform != null &&
                                Vector3.Distance(transform.position, playerTransform.position) <= _enemyBase.VisionRange;

            _enemyBase.SetCanSeePlayer(canSeePlayer);
        }
    }
}

