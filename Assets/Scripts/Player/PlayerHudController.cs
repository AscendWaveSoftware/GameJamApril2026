using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHudController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI goldAmount;
        public Slider healthSlider;
        public PlayerHealth playerHealth;
        public PlayerGold playerGold;

        void Start()
        {
            healthSlider.maxValue = playerHealth.MaxHealth;
            healthSlider.value = playerHealth.CurrentHealth;
            playerHealth.OnHealthChanged += UpdateHealthSlider;
            goldAmount.text = playerGold.CurrentGold.ToString();
            playerGold.OnGoldChanged += UpdateGoldAmount;
        }
        
        private void UpdateGoldAmount()
        {
            goldAmount.text = playerGold.CurrentGold.ToString();
        }
        
        private void UpdateHealthSlider()
        {
            healthSlider.value = playerHealth.CurrentHealth;
        }
    }
}