namespace GameLoop
{
    public sealed class OnDaytimeInCoffinPhaseHandler : IGamePhaseHandler
    {
        public GamePhase Phase => GamePhase.DaytimeInCoffin;

        public void Handle(GameLoopManager manager)
        {
            manager.ApplyRealSecondsPerGameMinute(0.001f);
        }
    }
}

