namespace GameLoop
{
    public sealed class OnNighttimeInCoffinPhaseHandler : IGamePhaseHandler
    {
        public GamePhase Phase => GamePhase.NightTimeInCoffin;

        public void Handle(GameLoopManager manager)
        {
            manager.ApplyRealSecondsPerGameMinute(0.001f);
        }
    }
}

