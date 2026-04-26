namespace GameLoop
{
    public interface IGamePhaseHandler
    {
        GamePhase Phase { get; }
        void Handle(GameLoopManager manager);
    }
}

