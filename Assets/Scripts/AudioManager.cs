using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public static event Action<AudioClip> SfxPlayed;

    [Header("Background SFX")]
    public AudioClip sfxBackground;
    [Range(0f,1f)] public float musicVolume = 0.75f;
    [SerializeField] bool musicPlayOnStart = true;
    private AudioSource _music;


    [Header("Active Voice Set")]
    [SerializeField] private CharacterVoiceSet activeVoiceSet;


    public enum ComboTier { None, Good, Sweet, Great, Super, Wow, Amazing, Extreme, Fantastic, Splendid, NoWay }

    [Header("Combo Tier SFX")]
    [SerializeField] private AudioClip sfxGood;
    [SerializeField] private AudioClip sfxSweet;
    [SerializeField] private AudioClip sfxGreat;
    [SerializeField] private AudioClip sfxSuper;
    [SerializeField] private AudioClip sfxWow;
    [SerializeField] private AudioClip sfxAmazing;
    [SerializeField] private AudioClip sfxExtreme;
    [SerializeField] private AudioClip sfxFantastic;
    [SerializeField] private AudioClip sfxSplendid;
    [SerializeField] private AudioClip sfxNoWay;


    [Header("Other SFX")]
    [SerializeField] private AudioClip sfxMilestone;
    [SerializeField] private AudioClip sfxCameraSpeedUp;
    [SerializeField] private AudioClip sfxGameOver;


    private AudioSource _a, _b;
    private ComboTier _lastTier = ComboTier.None;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad(gameObject);

        _a = gameObject.AddComponent<AudioSource>();
        _b = gameObject.AddComponent<AudioSource>();
        _a.playOnAwake = _b.playOnAwake = false;
        _a.spatialBlend = _b.spatialBlend = 0f;

        _music = gameObject.AddComponent<AudioSource>();
        _music.spatialBlend = 0f;
        _music.loop = true;
        _music.volume = musicVolume;
        _music.playOnAwake = false;
        _music.clip = sfxBackground;
    }

    void Start()
    {
        if (musicPlayOnStart && _music.clip)
            _music.Play();
    }
    
    public void SetCharacterVoice(CharacterVoiceSet voiceSet)
    {
        activeVoiceSet = voiceSet;
    }

    // Combo Sounds
    public void OnComboFloorsProgress(int floorsInCombo)
    {
        ComboTier tier = DetermineTier(floorsInCombo);
        if (tier > _lastTier) // play only when entering a higher tier
        {
            PlayTierSfx(tier);
            _lastTier = tier;
        }
    }

    public void PlayMilestone() => Play(sfxMilestone);
    public void PlayCameraSpeedUp() => Play(sfxCameraSpeedUp);
    public void PlayGameOver() => Play(sfxGameOver);


    // Combo SFX Functions
    public void ResetComboTierProgress() => _lastTier = ComboTier.None;

    private static ComboTier DetermineTier(int floors)
    {
        if (floors >= 200) return ComboTier.NoWay;
        if (floors >= 140) return ComboTier.Splendid;
        if (floors >= 100) return ComboTier.Fantastic;
        if (floors >= 70) return ComboTier.Extreme;
        if (floors >= 50) return ComboTier.Amazing;
        if (floors >= 35) return ComboTier.Wow;
        if (floors >= 25) return ComboTier.Super;
        if (floors >= 15) return ComboTier.Great;
        if (floors >= 7) return ComboTier.Sweet;
        if (floors >= 4) return ComboTier.Good;
        return ComboTier.None;
    }

    private void PlayTierSfx(ComboTier tier)
    {
        switch (tier)
        {
            case ComboTier.Good:       Play(sfxGood); break;
            case ComboTier.Sweet:      Play(sfxSweet); break;
            case ComboTier.Great:      Play(sfxGreat); break;
            case ComboTier.Super:      Play(sfxSuper); break;
            case ComboTier.Wow:        Play(sfxWow); break;
            case ComboTier.Amazing:    Play(sfxAmazing); break;
            case ComboTier.Extreme:    Play(sfxExtreme); break;
            case ComboTier.Fantastic:  Play(sfxFantastic); break;
            case ComboTier.Splendid:   Play(sfxSplendid); break;
            case ComboTier.NoWay:      Play(sfxNoWay); break;
        }
    }


    // Jump SFX
    public void PlayJumpByForce(float totalForce, float baseForce, float highRefForce)
    {
        float midThreshold = baseForce * 1.2f;
        float highThreshold = baseForce * 1.8f;

        if (totalForce >= highThreshold)
            Play(GetRandom(activeVoiceSet.jumpHigh));
        else if (totalForce >= midThreshold)
            Play(GetRandom(activeVoiceSet.jumpMid));
        else
            Play(GetRandom(activeVoiceSet.jumpLow));
    }

    private AudioClip GetRandom(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0) return null;
        if (clips.Count == 1) return clips[0];
        return clips[UnityEngine.Random.Range(0, clips.Count)];
    }


    // General

    private void Play(AudioClip clip)
    {
        if (!clip) return;
        var src = _a.isPlaying ? _b : _a; // allow overlap
        src.PlayOneShot(clip);
        SfxPlayed?.Invoke(clip);
    }
}