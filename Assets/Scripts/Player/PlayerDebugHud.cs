using UnityEngine;
using GameLoop;
using System;

namespace Player
{
    public class PlayerDebugHud : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerGold playerGold;
        private PlayerEquipmentHandler _equipmentHandler;
        private EnvironmentDirector _environmentDirector;
        private GameLoopManager _gameLoopManager;
        [SerializeField] private Vector2 position = new Vector2(16f, 16f);
        [SerializeField] private Vector2 size = new Vector2(220f, 28f);

        private void Awake()
        {
            ResolveReferences();
        }

        private void OnGUI()
        {
            ResolveReferences();

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

            if (_equipmentHandler != null)
            {
                string weaponName = _equipmentHandler.GetEquippedWeaponName();
                GUI.Label(new Rect(position.x, position.y + size.y * 2 + 12f, size.x, size.y),
                    $"Weapon: {weaponName}");
            }

            if (_environmentDirector != null && _environmentDirector.TimeSource != null)
            {
                ClockService clock = _environmentDirector.TimeSource;
                GUI.Label(new Rect(position.x, position.y + size.y * 3 + 18f, size.x, size.y),
                    $"Time: {clock.Hours:00}:{clock.Minutes:00} (Day {clock.Days})");
            }

            if (_gameLoopManager != null)
            {
                GUI.Label(new Rect(position.x, position.y + size.y * 4 + 24f, size.x, size.y),
                    $"Phase: {_gameLoopManager.CurrentPhase}");

                GamePhase[] phases = (GamePhase[])Enum.GetValues(typeof(GamePhase));
                for (int i = 0; i < phases.Length; i++)
                {
                    GamePhase phase = phases[i];
                    Rect phaseButtonRect = new Rect(position.x, position.y + size.y * (6 + i) + 36f + i * 6f, size.x, size.y);
                    if (GUI.Button(phaseButtonRect, $"Set Phase: {phase}"))
                    {
                        _gameLoopManager.SetCurrentPhase(phase);
                    }
                }
            }
        }

        private void ResolveReferences()
        {
            if (playerHealth == null)
            {
                playerHealth = FindFirstObjectByType<PlayerHealth>();
            }

            if (playerGold == null)
            {
                playerGold = FindFirstObjectByType<PlayerGold>();
            }

            if (_equipmentHandler == null)
            {
                _equipmentHandler = FindFirstObjectByType<PlayerEquipmentHandler>();
            }

            if (_environmentDirector == null)
            {
                _environmentDirector = FindFirstObjectByType<EnvironmentDirector>();
            }

            if (_gameLoopManager == null)
            {
                _gameLoopManager = FindFirstObjectByType<GameLoopManager>();
            }
        }
    }
}
