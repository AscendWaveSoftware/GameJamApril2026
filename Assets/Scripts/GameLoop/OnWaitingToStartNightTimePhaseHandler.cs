namespace GameLoop
{
    public sealed class OnWaitingToStartNightTimePhaseHandler : IGamePhaseHandler
    {
        public GamePhase Phase => GamePhase.WaitingToStartNightTime;

        public void Handle(GameLoopManager manager)
        {
            manager.ApplyRealSecondsPerGameMinute(0f);
        }
    }
}

