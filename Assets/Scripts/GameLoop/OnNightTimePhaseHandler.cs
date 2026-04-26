namespace GameLoop
{
    public sealed class OnNightTimePhaseHandler : IGamePhaseHandler
    {
        public GamePhase Phase => GamePhase.NightTime;

        public void Handle(GameLoopManager manager)
        {
            manager.ApplyRealSecondsPerGameMinute(manager.NightTimeRealSecondsPerGameMinute);
        }
    }
}

