using System.Collections.Generic;
using UnityEngine;

namespace GameLoop
{
    public class GameLoopManager : MonoBehaviour
    {
        [field: SerializeField] public float DayTimeRealSecondsPerGameMinute { get; private set; } = 1f;
        [field: SerializeField] public float NightTimeRealSecondsPerGameMinute { get; private set; } = 1f;

        public GamePhase CurrentPhase { get; private set; } = GamePhase.MainMenu;

        private EnvironmentDirector _environmentDirector;
        private Dictionary<GamePhase, IGamePhaseHandler> _phaseHandlers;

        private void Awake()
        {
            _environmentDirector = FindFirstObjectByType<EnvironmentDirector>();
            BuildPhaseHandlers();
            ApplyRealSecondsPerGameMinute(0f);
        }

        public void SetCurrentPhase(GamePhase newPhase)
        {
            CurrentPhase = newPhase;

            if (_phaseHandlers != null && _phaseHandlers.TryGetValue(newPhase, out IGamePhaseHandler handler))
            {
                handler.Handle(this);
            }
        }

        public void ApplyRealSecondsPerGameMinute(float value)
        {
            if (_environmentDirector == null || _environmentDirector.Profile == null)
            {
                return;
            }

            _environmentDirector.Profile.RealSecondsPerGameMinute = value;
        }

        private void BuildPhaseHandlers()
        {
            _phaseHandlers = new Dictionary<GamePhase, IGamePhaseHandler>
            {
                { GamePhase.DayTime, new OnDayTimePhaseHandler() },
                { GamePhase.DaytimeInCoffin, new OnDaytimeInCoffinPhaseHandler() },
                { GamePhase.WaitingToStartNightTime, new OnWaitingToStartNightTimePhaseHandler() },
                { GamePhase.NightTime, new OnNightTimePhaseHandler() },
                { GamePhase.NightTimeDefeated, new OnNightTimeDefeatedPhaseHandler() },
                { GamePhase.NightTimeInCoffin, new OnNighttimeInCoffinPhaseHandler() }
            };
        }
    }
}