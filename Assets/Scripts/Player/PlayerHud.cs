using UnityEngine;

namespace Player
{
    public class PlayerHud : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerGold playerGold;
        [SerializeField] private Vector2 position = new Vector2(16f, 16f);
        [SerializeField] private Vector2 size = new Vector2(220f, 28f);

        private void Awake()
        {
            if (playerHealth == null)
            {
                playerHealth = FindFirstObjectByType<PlayerHealth>();
            }

            if (playerGold == null)
            {
                playerGold = FindFirstObjectByType<PlayerGold>();
            }
        }

        private void OnGUI()
        {
            if (playerHealth == null)
            {
                return;
            }

            float max = Mathf.Max(0.0001f, playerHealth.MaxHealth);
            float ratio = Mathf.Clamp01(playerHealth.CurrentHealth / max);

            Rect bgRect = new Rect(position.x, position.y, size.x, size.y);
            GUI.Box(bgRect, string.Empty);

            Rect fillRect = new Rect(position.x + 2f, position.y + 2f, (size.x - 4f) * ratio, size.y - 4f);
            Color prevColor = GUI.color;
            GUI.color = Color.Lerp(Color.red, Color.green, ratio);
            GUI.Box(fillRect, string.Empty);
            GUI.color = prevColor;

            GUI.Label(new Rect(position.x + 8f, position.y + 4f, size.x, size.y),
                $"HP: {Mathf.CeilToInt(playerHealth.CurrentHealth)}/{Mathf.CeilToInt(playerHealth.MaxHealth)}");

            if (playerGold != null)
            {
                GUI.Label(new Rect(position.x, position.y + size.y + 6f, size.x, size.y),
                    $"Gold: {playerGold.CurrentGold}");
            }
        }
    }
}

