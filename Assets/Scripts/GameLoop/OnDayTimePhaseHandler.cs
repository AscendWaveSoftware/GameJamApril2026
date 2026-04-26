using Item;
using UnityEngine;

namespace GameLoop
{
    public sealed class OnDayTimePhaseHandler : IGamePhaseHandler
    {
        public GamePhase Phase => GamePhase.DayTime;

        public void Handle(GameLoopManager manager)
        {
            manager.ApplyRealSecondsPerGameMinute(manager.DayTimeRealSecondsPerGameMinute);
            foreach (var itemSpawner in Resources.FindObjectsOfTypeAll<DayTimeItemSpawner>())
            {
                itemSpawner.SpawnItem();
            }
        }
    }
}
