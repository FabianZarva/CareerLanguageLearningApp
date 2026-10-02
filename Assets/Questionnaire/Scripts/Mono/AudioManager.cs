using System;
using UnityEngine;

[System.Serializable()]
public struct SoundParameters
{
    [Range(0, 1)]
    public float Volume;
    [Range(-3, 3)]
    public float Pitch;
    public bool Loop;
}

[System.Serializable()]
public class Sound
{
    [SerializeField] private string name = string.Empty;
    public string Name { get { return name; } }

    [SerializeField] private AudioClip clip = null;
    public AudioClip Clip { get { return clip; } }

    [SerializeField] private SoundParameters parameters = new SoundParameters();
    public SoundParameters Parameters { get { return parameters; } }

    [HideInInspector]
    public AudioSource Source = null;

    public void Play()
    {
        Source.clip = Clip;
        Source.volume = Parameters.Volume;
        Source.pitch = Parameters.Pitch;
        Source.loop = Parameters.Loop;
        Source.Play();
    }

    public void Stop()
    {
        Source.Stop();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    [SerializeField] private Sound[] sounds = null;
    [SerializeField] private AudioSource sourcePrefab = null;

    // One dedicated source for short one-shot SFX (clicks, stings)
    // so they never cut off the music track.
    private AudioSource _sfxSource;

    [SerializeField] private string startupTrack = string.Empty;

    private string currentMusicTrack = string.Empty;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitSounds();

        // Create a dedicated one-shot SFX AudioSource
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.loop = false;
        _sfxSource.volume = 0.8f;
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(startupTrack))
        {
            PlayMusic(startupTrack);
        }
    }

    void InitSounds()
    {
        foreach (var sound in sounds)
        {
            AudioSource source = Instantiate(sourcePrefab, gameObject.transform);
            source.name = sound.Name;
            sound.Source = source;
        }
    }

    // ── Music (looping, one at a time) ───────────────────────────────────────

    public void PlayMusic(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        if (currentMusicTrack == name)
            return;

        if (!string.IsNullOrEmpty(currentMusicTrack))
        {
            StopSound(currentMusicTrack);
        }

        PlaySound(name);
        currentMusicTrack = name;
    }

    public void StopCurrentMusic()
    {
        if (!string.IsNullOrEmpty(currentMusicTrack))
        {
            StopSound(currentMusicTrack);
            currentMusicTrack = string.Empty;
        }
    }

    // ── Named sounds (uses dedicated AudioSource per sound) ──────────────────

    public void PlaySound(string name)
    {
        var sound = GetSound(name);
        if (sound != null)
        {
            sound.Play();
        }
        else
        {
            Debug.LogWarning("Sound by the name " + name + " is not found! Issues occurred at AudioManager.PlaySound()");
        }
    }

    public void StopSound(string name)
    {
        var sound = GetSound(name);
        if (sound != null)
        {
            sound.Stop();
        }
        else
        {
            Debug.LogWarning("Sound by the name " + name + " is not found! Issues occurred at AudioManager.StopSound()");
        }
    }

    // ── One-shot SFX (plays on the shared SFX source, never cuts music) ──────
    // Use this for button clicks, correct/wrong stings, dialogue swoosh, etc.

    public void PlaySFX(string name)
    {
        var sound = GetSound(name);
        if (sound == null)
        {
            Debug.LogWarning("SFX by the name " + name + " is not found! Issues occurred at AudioManager.PlaySFX()");
            return;
        }

        _sfxSource.pitch  = sound.Parameters.Pitch;
        _sfxSource.volume = sound.Parameters.Volume;
        _sfxSource.PlayOneShot(sound.Clip);
    }

    // ─────────────────────────────────────────────────────────────────────────

    Sound GetSound(string name)
    {
        foreach (var sound in sounds)
        {
            if (sound.Name == name)
            {
                return sound;
            }
        }

        return null;
    }
}