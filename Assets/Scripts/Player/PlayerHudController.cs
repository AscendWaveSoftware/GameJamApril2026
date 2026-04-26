using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHudController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI goldAmount;
        public TMPro.TextMeshProUGUI time;
        public Slider healthSlider;
        public PlayerHealth playerHealth;
        public PlayerGold playerGold;

        private ClockService _clock;

        void Start()
        {
            healthSlider.maxValue = playerHealth.MaxHealth;
            healthSlider.value = playerHealth.CurrentHealth;
            playerHealth.OnHealthChanged += UpdateHealthSlider;
            goldAmount.text = playerGold.CurrentGold.ToString();
            playerGold.OnGoldChanged += UpdateGoldAmount;

            EnvironmentDirector envDir = FindFirstObjectByType<EnvironmentDirector>();
            if (envDir != null)
            {
                _clock = envDir.TimeSource;
                _clock.MinutesElapsed += UpdateTime;
                UpdateTime();
            }
        }

        private void UpdateGoldAmount()
        {
            goldAmount.text = playerGold.CurrentGold.ToString();
        }

        private void UpdateHealthSlider()
        {
            healthSlider.value = playerHealth.CurrentHealth;
        }

        private void UpdateTime()
        {
            if (time == null || _clock == null) return;
            time.text = $"{_clock.Hours:00}:{_clock.Minutes:00} (Day {_clock.Days})";
        }
    }
}