using UnityEngine;

namespace GameLoop
{
    public class GameLoopManager : MonoBehaviour
    {
        
        [field: SerializeField] public float DayTimeRealSecondsPerGameMinute { get; private set; } = 1f;
        [field: SerializeField] public float NightTimeRealSecondsPerGameMinute { get; private set; } = 1f;
        
        public GamePhase CurrentPhase { get; private set; } = GamePhase.MainMenu;

        public void SetCurrentPhase(GamePhase newPhase)
        {
            CurrentPhase = newPhase;
            switch (newPhase)
            {
                case GamePhase.DayTime:
                    OnDayTime();
                    break;
                case GamePhase.InCoffin:
                    OnInCoffin();
                    break;
                case GamePhase.WaitingToStartNightTime:
                    OnWaitingToStartNightTime();
                    break;
                case GamePhase.NightTime:
                    OnNightTime();
                    break;
                case GamePhase.NightTimeDefeated:
                    OnNightTimeDefeated();
                    break;
            }
        }

        private void OnWaitingToStartNightTime()
        {
            _environmentDirector.Profile.RealSecondsPerGameMinute = 0f;
        }

        private void OnInCoffin()
        {
            _environmentDirector.Profile.RealSecondsPerGameMinute = 0.00001f;
        }

        private void OnDayTime()
        {
            _environmentDirector.Profile.RealSecondsPerGameMinute = DayTimeRealSecondsPerGameMinute;
        }

        private void OnNightTime()
        {
            _environmentDirector.Profile.RealSecondsPerGameMinute = NightTimeRealSecondsPerGameMinute;
        }

        private void OnNightTimeDefeated()
        {
            _environmentDirector.Profile.RealSecondsPerGameMinute = 0f;
        }

        private EnvironmentDirector _environmentDirector;

        private void Awake()
        {
            _environmentDirector = FindFirstObjectByType<EnvironmentDirector>();
            if (_environmentDirector == null || _environmentDirector.Profile == null)
            {
                return;
            }

            _environmentDirector.Profile.RealSecondsPerGameMinute = 0f;
        }
    }
}