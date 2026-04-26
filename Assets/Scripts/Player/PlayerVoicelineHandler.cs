using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerHealth))]
    public sealed class PlayerVoicelineHandler : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float triggerChance = 0.5f;

        private PlayerHealth _playerHealth;
        private AudioSource _audioSource;
        private float _lastKnownHealth;
        private AudioClip[] _damageClips;
        private AudioClip[] _killClips;
        private Transform _playerRoot;

        private void Awake()
        {
            _playerRoot = transform.root;
            _playerHealth = GetComponent<PlayerHealth>();
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;

            _lastKnownHealth = _playerHealth != null ? _playerHealth.CurrentHealth : 0f;
            LoadClipsFromVoicelinesFolder();
        }

        private void OnEnable()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged += HandlePlayerHealthChanged;
            }

            EnemyBase.OnAnyEnemyDied += HandleAnyEnemyDied;
        }

        private void OnDisable()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged -= HandlePlayerHealthChanged;
            }

            EnemyBase.OnAnyEnemyDied -= HandleAnyEnemyDied;
        }

        private void HandlePlayerHealthChanged()
        {
            if (_playerHealth == null)
            {
                return;
            }

            float currentHealth = _playerHealth.CurrentHealth;
            bool tookDamage = currentHealth < _lastKnownHealth - 0.0001f;
            _lastKnownHealth = currentHealth;

            if (!tookDamage)
            {
                return;
            }

            TryPlayRandomClip(_damageClips);
        }

        private void HandleAnyEnemyDied(EnemyBase enemy, Transform damageDealerRoot)
        {
            if (damageDealerRoot == null || _playerRoot == null)
            {
                return;
            }

            if (damageDealerRoot != _playerRoot)
            {
                return;
            }

            TryPlayRandomClip(_killClips);
        }

        private void TryPlayRandomClip(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
            {
                return;
            }

            if (Random.value > Mathf.Clamp01(triggerChance))
            {
                return;
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayRandomClip(clips);
            }
            else if (_audioSource != null)
            {
                // Fallback falls kein SoundManager in der Szene vorhanden ist
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                if (clip != null)
                {
                    _audioSource.PlayOneShot(clip);
                }
            }
        }

        private void LoadClipsFromVoicelinesFolder()
        {
            AudioClip[] allClips = Resources.LoadAll<AudioClip>("Voicelines");
            if (allClips == null || allClips.Length == 0)
            {
                _damageClips = new AudioClip[0];
                _killClips = new AudioClip[0];
                return;
            }

            List<AudioClip> damage = new List<AudioClip>();
            List<AudioClip> kill = new List<AudioClip>();

            for (int i = 0; i < allClips.Length; i++)
            {
                AudioClip clip = allClips[i];
                if (clip == null)
                {
                    continue;
                }

                string clipName = clip.name.ToLowerInvariant();
                if (clipName.StartsWith("damage"))
                {
                    damage.Add(clip);
                }
                else if (clipName.StartsWith("kill"))
                {
                    kill.Add(clip);
                }
            }

            _damageClips = damage.ToArray();
            _killClips = kill.ToArray();
        }
    }
}

