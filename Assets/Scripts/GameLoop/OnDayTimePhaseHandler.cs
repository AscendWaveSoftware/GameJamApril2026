using Item;
using Player;
using UnityEngine;

namespace GameLoop
{
    public sealed class OnDayTimePhaseHandler : IGamePhaseHandler
    {
        public GamePhase Phase => GamePhase.DayTime;

        public void Handle(GameLoopManager manager)
        {
            ScreenTitleManager.ShowTitle(@"During the day you need to collect as much gold as possible.
Being exposed to the sun will dramatically speed up the time that passes.", 5);
            manager.ApplyRealSecondsPerGameMinute(manager.DayTimeRealSecondsPerGameMinute);
            foreach (var itemSpawner in Resources.FindObjectsOfTypeAll<DayTimeItemSpawner>())
            {
                itemSpawner.SpawnItem();
            }
        }
        
    }
}