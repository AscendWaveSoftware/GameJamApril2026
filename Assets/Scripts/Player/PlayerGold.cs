using System;
using UnityEngine;

namespace Player
{
    public class PlayerGold : MonoBehaviour
    {
        [SerializeField] private int currentGold;

        public int CurrentGold => currentGold;

        public event Action OnGoldChanged;

        public void AddGold(int amount)
        {
            int goldToAdd = Mathf.Max(0, amount);
            if (goldToAdd == 0)
            {
                return;
            }

            currentGold += goldToAdd;
            OnGoldChanged?.Invoke();
        }

        public bool TrySpendGold(int amount)
        {
            int goldToSpend = Mathf.Max(0, amount);
            if (goldToSpend == 0)
            {
                return true;
            }

            if (currentGold < goldToSpend)
            {
                return false;
            }

            currentGold -= goldToSpend;
            OnGoldChanged?.Invoke();
            return true;
        }
    }
}

