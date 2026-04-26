using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLoop
{
    public class GameLoopManager : MonoBehaviour
    {
        [field: SerializeField] public float DayTimeRealSecondsPerGameMinute { get; private set; } = 1f;
        [field: SerializeField] public float NightTimeRealSecondsPerGameMinute { get; private set; } = 1f;

        [SerializeField] private AudioClip readyClip;

        public Canvas BuffSelection;
        
        public GamePhase CurrentPhase { get; private set; } = GamePhase.MainMenu;

        private EnvironmentDirector _environmentDirector;
        private Dictionary<GamePhase, IGamePhaseHandler> _phaseHandlers;
        private CoffinProximityHintHandler _coffinHintHandler;

        private void Update()
        {
            HandleAutomaticTransitions();
            HandleInputTransitions();
        }

        private void Awake()
        {
            _environmentDirector = FindFirstObjectByType<EnvironmentDirector>();
            BuildPhaseHandlers();
            ApplyRealSecondsPerGameMinute(0f);
            ScreenTitleManager.ShowTitleUntilKey("To start the game press E.", KeyCode.E);
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

        private void HandleInputTransitions()
        {
            if (!IsEPressedThisFrame())
            {
                return;
            }

            switch (CurrentPhase)
            {
                case GamePhase.MainMenu:
                    PlayReadySound();
                    SetCurrentPhase(GamePhase.DayTime);
                    break;
                case GamePhase.DayTime:
                    if (IsPlayerNearCoffin())
                    {
                        PlayReadySound();
                        SetCurrentPhase(GamePhase.DaytimeInCoffin);
                    }
                    break;
                case GamePhase.WaitingToStartNightTime:
                    PlayReadySound();
                    SetCurrentPhase(GamePhase.NightTime);
                    break;
                case GamePhase.NightTimeDefeated:
                    PlayReadySound();
                    SetCurrentPhase(GamePhase.DayTime);
                    break;
            }
        }

        private void PlayReadySound()
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayClip(readyClip);
            }
        }

        private void HandleAutomaticTransitions()
        {
            ClockService clock = _environmentDirector != null ? _environmentDirector.TimeSource : null;
            if (clock == null)
            {
                return;
            }

            if ((CurrentPhase == GamePhase.DayTime || CurrentPhase == GamePhase.DaytimeInCoffin) && IsAtOrAfter(clock, 18, 0))
            {
                SetCurrentPhase(GamePhase.WaitingToStartNightTime);
                return;
            }

            if (CurrentPhase == GamePhase.NightTime && IsNightFinished(clock))
            {
                SetCurrentPhase(GamePhase.NightTimeDefeated);
            }
        }

        private bool IsPlayerNearCoffin()
        {
            if (_coffinHintHandler == null)
            {
                _coffinHintHandler = FindFirstObjectByType<CoffinProximityHintHandler>();
            }

            return _coffinHintHandler != null && _coffinHintHandler.IsNearCoffin;
        }

        private static bool IsEPressedThisFrame()
        {
            return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
        }

        private static bool IsAtOrAfter(ClockService clock, int targetHour, int targetMinute)
        {
            int current = clock.Hours * 60 + clock.Minutes;
            int target = Mathf.Clamp(targetHour, 0, 23) * 60 + Mathf.Clamp(targetMinute, 0, 59);
            return current >= target;
        }

        private static bool IsNightFinished(ClockService clock)
        {
            int current = clock.Hours * 60 + clock.Minutes;
            int morningStart = 6 * 60;
            int daytimeStart = 18 * 60;
            return current >= morningStart && current < daytimeStart;
        }

        private void BuildPhaseHandlers()
        {
            _phaseHandlers = new Dictionary<GamePhase, IGamePhaseHandler>
            {
                { GamePhase.DayTime, new OnDayTimePhaseHandler() },
                { GamePhase.DaytimeInCoffin, new OnDaytimeInCoffinPhaseHandler() },
                { GamePhase.WaitingToStartNightTime, new OnWaitingToStartNightTimePhaseHandler() },
                { GamePhase.NightTime, new OnNightTimePhaseHandler() },
                { GamePhase.NightTimeDefeated, new OnNightTimeDefeatedPhaseHandler() }
            };
        }
    }
}