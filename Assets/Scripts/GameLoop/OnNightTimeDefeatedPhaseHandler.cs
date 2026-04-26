using Enemy;
using UnityEngine;

namespace GameLoop
{
    public sealed class OnNightTimeDefeatedPhaseHandler : IGamePhaseHandler
    {
        public GamePhase Phase => GamePhase.NightTimeDefeated;

        public void Handle(GameLoopManager manager)
        {
            manager.ApplyRealSecondsPerGameMinute(0f);
            foreach (var enemyBase in Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
            {
                Object.Destroy(enemyBase.gameObject);
            }
        }
    }
}