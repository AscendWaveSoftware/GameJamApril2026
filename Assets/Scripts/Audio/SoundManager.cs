using System.Linq;
using UnityEngine;

/// <summary>
/// Globaler Sound-Manager (Singleton). Verhindert, dass derselbe Sound zweimal hintereinander abgespielt wird.
/// Muss als GameObject in der Szene vorhanden sein.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource _audioSource;
    private AudioClip _lastPlayedClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f;
    }

    /// <summary>
    /// Spielt einen bestimmten AudioClip ab.
    /// </summary>
    public void PlayClip(AudioClip clip)
    {
        if (clip == null || _audioSource == null)
        {
            return;
        }

        _audioSource.PlayOneShot(clip);
        _lastPlayedClip = clip;
    }

    /// <summary>
    /// Spielt einen AudioClip mit zufälligem Pitch ab (verhindert monotone Wiederholungen).
    /// Verwendet ein temporäres AudioSource-Objekt damit der Pitch zuverlässig angewendet wird.
    /// </summary>
    public void PlayClipWithRandomPitch(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f, float volume = 1f)
    {
        if (clip == null) return;

        GameObject tempGo = new GameObject("TempAudio_Pitched");
        DontDestroyOnLoad(tempGo);
        AudioSource source = tempGo.AddComponent<AudioSource>();
        source.clip = clip;
        source.pitch = Random.Range(minPitch, maxPitch);
        source.volume = Mathf.Clamp01(volume);
        source.spatialBlend = 0f;
        source.playOnAwake = false;
        source.Play();
        Destroy(tempGo, clip.length / Mathf.Max(0.01f, Mathf.Abs(source.pitch)) + 0.1f);

        _lastPlayedClip = clip;
    }

    /// <summary>
    /// Spielt einen zufälligen AudioClip aus dem Array ab.
    /// Verhindert dabei, dass derselbe Clip zweimal hintereinander gespielt wird (sofern mehrere Clips vorhanden sind).
    /// </summary>
    public void PlayRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0 || _audioSource == null)
        {
            return;
        }

        AudioClip clip;

        if (clips.Length == 1)
        {
            clip = clips[0];
        }
        else
        {
            // Alle Clips außer dem zuletzt gespielten als Kandidaten wählen
            AudioClip[] candidates = clips.Where(c => c != null && c != _lastPlayedClip).ToArray();

            // Fallback: Falls alle Clips gefiltert wurden, alle nehmen
            if (candidates.Length == 0)
            {
                candidates = clips.Where(c => c != null).ToArray();
            }

            if (candidates.Length == 0)
            {
                return;
            }

            clip = candidates[Random.Range(0, candidates.Length)];
        }

        if (clip == null)
        {
            return;
        }

        _audioSource.PlayOneShot(clip);
        _lastPlayedClip = clip;
    }
}

