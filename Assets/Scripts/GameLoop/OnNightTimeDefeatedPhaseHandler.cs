using Enemy;
using Player;
using UnityEngine;

namespace GameLoop
{
    public sealed class OnNightTimeDefeatedPhaseHandler : IGamePhaseHandler
    {
        public GamePhase Phase => GamePhase.NightTimeDefeated;

        public void Handle(GameLoopManager manager)
        {
            manager.ApplyRealSecondsPerGameMinute(0f);
            ScreenTitleManager.ShowTitleUntilKey("Night is over. Press E to return to daytime.", KeyCode.E);
            foreach (var enemyBase in Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
            {
                Object.Destroy(enemyBase.gameObject);
            }
        }
    }
}