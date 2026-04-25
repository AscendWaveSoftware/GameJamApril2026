using UnityEngine;

namespace Currency
{
    public class GoldCoin : MonoBehaviour
    {
        [SerializeField] private int goldAmount = 1;

        public int GoldAmount => goldAmount;
    }
}

