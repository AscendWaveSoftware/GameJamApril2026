using Currency;
using Player;
using UnityEngine;

namespace GameLoop
{
    public sealed class OnWaitingToStartNightTimePhaseHandler : IGamePhaseHandler
    {
     
        
        public GamePhase Phase => GamePhase.WaitingToStartNightTime;

        public void Handle(GameLoopManager manager)
        {
            manager.ApplyRealSecondsPerGameMinute(0f);
            RemoveAllSpawnedCoins();
            manager.BuffSelection.gameObject.SetActive(true);   
        }

        private static void RemoveAllSpawnedCoins()
        {
            GoldCoin[] coins = Object.FindObjectsByType<GoldCoin>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < coins.Length; i++)
            {
                if (coins[i] != null)
                {
                    Object.Destroy(coins[i].gameObject);
                }
            }
        }
    }
}
