using UnityEngine;

namespace Item
{
    public class DayTimeItemSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject itemPrefab;
        [Range(0f, 1f)]
        [SerializeField] private float spawnChance = 1f;

        public void SpawnItem()
        {
            if (itemPrefab == null)
            {
                Debug.LogWarning("DayTimeItemSpawner: itemPrefab is not assigned.", this);
                return;
            }

            float chance = Mathf.Clamp01(spawnChance);
            if (Random.value > chance)
            {
                return;
            }

            Instantiate(itemPrefab, transform.position, transform.rotation);
        }
    }
}

