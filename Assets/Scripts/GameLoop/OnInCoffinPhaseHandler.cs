namespace GameLoop
{
    public sealed class OnInCoffinPhaseHandler : IGamePhaseHandler
    {
        public GamePhase Phase => GamePhase.InCoffin;

        public void Handle(GameLoopManager manager)
        {
            manager.ApplyRealSecondsPerGameMinute(0.001f);
        }
    }
}

