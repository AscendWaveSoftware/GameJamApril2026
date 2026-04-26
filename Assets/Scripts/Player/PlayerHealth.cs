using System;
using UnityEngine;

namespace Player
{
    [UnityEngine.RequireComponent(typeof(CapsuleCollider))]
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private float currentHealth = 10f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;

        public event Action OnHealthChanged;
        public event Action OnDeath;

        private void Awake()
        {
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (currentHealth <= 0f)
            {
                return;
            }

            float damage = Mathf.Max(0f, amount);
            currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
            OnHealthChanged?.Invoke();

            if (currentHealth <= 0f)
            {
                OnDeath?.Invoke();
            }
        }

        public void AddHealth(float amount)
        {
            float healthToAdd = Mathf.Max(0f, amount);
            if (healthToAdd <= 0f)
            {
                return;
            }

            float previousHealth = currentHealth;
            currentHealth = Mathf.Clamp(currentHealth + healthToAdd, 0f, maxHealth);

            if (!Mathf.Approximately(previousHealth, currentHealth))
            {
                OnHealthChanged?.Invoke();
            }
        }

        public void ChangeMaxHealth(float amount)
        {
            float newMaxHealth = Mathf.Max(0.0001f, amount);

            float previousMaxHealth = maxHealth;
            float previousCurrentHealth = currentHealth;

            maxHealth = newMaxHealth;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (!Mathf.Approximately(previousMaxHealth, maxHealth) ||
                !Mathf.Approximately(previousCurrentHealth, currentHealth))
            {
                OnHealthChanged?.Invoke();
            }
        }
    }
}

