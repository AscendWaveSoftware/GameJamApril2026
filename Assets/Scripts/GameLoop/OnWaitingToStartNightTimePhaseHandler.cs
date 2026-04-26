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
            ScreenTitleManager.ShowTitleUntilKey("Night is ready. Press E to start.", KeyCode.E);
        }
    }
}
