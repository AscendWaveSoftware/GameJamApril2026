using System;
using GameLoop;
using UnityEngine;

namespace Player
{
    [DisallowMultipleComponent]
    public sealed class CoffinProximityHintHandler : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 2.25f;
        [SerializeField] private float checkIntervalSeconds = 0.1f;
        [SerializeField] private LayerMask detectionMask = ~0;
        [SerializeField] private string hintText = "To sleep in this coffin press E";
        [SerializeField] private string[] coffinNameKeywords = { "coffin", "sarg" };

        private readonly Collider[] _overlapBuffer = new Collider[32];
        private float _nextCheckTime;
        private bool _isNearCoffin;
        private GameLoopManager _gameLoopManager;

        private void Update()
        {
            if (Time.unscaledTime < _nextCheckTime)
            {
                return;
            }

            _nextCheckTime = Time.unscaledTime + Mathf.Max(0.02f, checkIntervalSeconds);

            if (!IsHintPhaseAllowed())
            {
                if (_isNearCoffin)
                {
                    ScreenTitleManager.ShowTitle(string.Empty, 0f);
                    _isNearCoffin = false;
                }

                return;
            }

            bool nearCoffinNow = IsNearCoffin();

            if (nearCoffinNow)
            {
                // Refresh the prompt while in range so it remains visible.
                ScreenTitleManager.ShowTitle(hintText, 0.25f);
            }
            else if (_isNearCoffin)
            {
                // Trigger quick fade-out when leaving the coffin range.
                ScreenTitleManager.ShowTitle(string.Empty, 0f);
            }

            _isNearCoffin = nearCoffinNow;
        }

        private bool IsHintPhaseAllowed()
        {
            if (_gameLoopManager == null)
            {
                _gameLoopManager = FindFirstObjectByType<GameLoopManager>();
            }

            if (_gameLoopManager == null)
            {
                return false;
            }

            GamePhase phase = _gameLoopManager.CurrentPhase;
            return phase == GamePhase.DayTime || phase == GamePhase.NightTimeDefeated;
        }

        private bool IsNearCoffin()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                Mathf.Max(0.1f, detectionRadius),
                _overlapBuffer,
                detectionMask,
                QueryTriggerInteraction.Collide);

            for (int i = 0; i < hitCount; i++)
            {
                Collider col = _overlapBuffer[i];
                if (col == null)
                {
                    continue;
                }

                if (col.transform.root == transform.root)
                {
                    continue;
                }

                if (HasCoffinKeywordInHierarchy(col.transform))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasCoffinKeywordInHierarchy(Transform current)
        {
            while (current != null)
            {
                if (ContainsCoffinKeyword(current.name))
                {
                    return true;
                }

                current = current.parent;
            }

            return false;
        }

        private bool ContainsCoffinKeyword(string objectName)
        {
            if (string.IsNullOrEmpty(objectName) || coffinNameKeywords == null)
            {
                return false;
            }

            for (int i = 0; i < coffinNameKeywords.Length; i++)
            {
                string keyword = coffinNameKeywords[i];
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    continue;
                }

                if (objectName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
