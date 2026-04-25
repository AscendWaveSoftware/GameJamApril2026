using Player;
using UnityEngine;

namespace Currency
{
    [RequireComponent(typeof(GoldCoin))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class GoldCoinPickupHandler : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField] private float spawnChance = 0.3f;

        private GoldCoin _goldCoin;
        private SphereCollider _sphereCollider;

        private void Awake()
        {
            if (Random.value > spawnChance)
            {
                Destroy(gameObject);
                return;
            }

            _goldCoin = GetComponent<GoldCoin>();
            _sphereCollider = GetComponent<SphereCollider>();
            _sphereCollider.isTrigger = true;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }

            rb.useGravity = false;
            rb.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerGold playerGold = other.GetComponentInParent<PlayerGold>();
            if (playerGold == null)
            {
                return;
            }

            playerGold.AddGold(_goldCoin.GoldAmount);
            Destroy(gameObject);
        }
    }
}
