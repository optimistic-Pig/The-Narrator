using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton audio manager for The Narrator.
/// Handles music crossfading and one-shot SFX.
///
/// SETUP:
///   1. Create a new empty GameObject in your persistent scene, name it "AudioManager".
///   2. Attach this script.
///   3. Drag your AudioClip assets into the Inspector slots.
///   4. The manager persists across scenes (DontDestroyOnLoad).
///
/// SUGGESTED CLIP ASSIGNMENTS (reassign after listening in-engine):
///   officeMusic           → Ambiance_Of_The_Air_Loop
///   izulMusic             → Inanimate_Immortality_Loop   (eerie / alien)
///   kortnaraMusic         → Through_The_Clouds           (mysterious)
///   gorpMusic             → Fountain_of_Stars            (quirky / wonder)
///   andrewMusic           → Galactic_Overdrive           (tense / urgent)
///   endingMusic           → The_Tragedy_Of_Cruising
///   secretEndingMusic     → (reassign one of the above or leave blank to reuse endingMusic)
///   menuMusic             → (any remaining track)
///
///   sfxTeleport        → One-off_1
///   sfxPublish         → One-off_2
///   sfxDictionary      → One-off_3
///   sfxPageFlip        → One-off_2__Synth_
///
/// USAGE (from other managers):
///   // Per-character interview music (pass CharacterID from GameStateManager):
///   AudioManager.Instance.SwitchInterviewMusic(GameStateManager.Instance.GetID(current));
///
///   // Generic track switch:
///   AudioManager.Instance.SwitchMusic(AudioManager.MusicTrack.Office);
///   AudioManager.Instance.PlaySFX(AudioManager.SFX.Teleport);
///   AudioManager.Instance.StopMusic();
/// </summary>
public class AudioManager : MonoBehaviour
{
    // =====================================================================
    // SINGLETON
    // =====================================================================

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create two AudioSource components at runtime — no manual setup needed
        _musicSourceA = gameObject.AddComponent<AudioSource>();
        _musicSourceB = gameObject.AddComponent<AudioSource>();
        _sfxSource    = gameObject.AddComponent<AudioSource>();

        ConfigureMusicSource(_musicSourceA);
        ConfigureMusicSource(_musicSourceB);
        _sfxSource.playOnAwake = false;

        _activeMusicSource = _musicSourceA;
    }

    // =====================================================================
    // MUSIC CLIP SLOTS  (assign in Inspector)
    // =====================================================================

    [Header("Music Clips")]
    [Tooltip("Ambiance_Of_The_Air_Loop — calm office background")]
    public AudioClip officeMusic;

    [Header("Per-Character Interview Music")]
    [Tooltip("Inanimate_Immortality_Loop — eerie/alien, suits Izul")]
    public AudioClip izulMusic;

    [Tooltip("Through_The_Clouds — mysterious, suits Kortnara")]
    public AudioClip kortnaraMusic;

    [Tooltip("Fountain_of_Stars — wonder/quirky, suits Gorp")]
    public AudioClip gorpMusic;

    [Tooltip("Galactic_Overdrive — tense/urgent, suits Andrew")]
    public AudioClip andrewMusic;

    [Header("Other Music")]
    [Tooltip("The_Tragedy_Of_Cruising — standard ending screen")]
    public AudioClip endingMusic;

    [Tooltip("Galactic_Overdrive — secret ending only")]
    public AudioClip secretEndingMusic;

    [Tooltip("Fountain_of_Stars or Through_The_Clouds — main menu")]
    public AudioClip menuMusic;

    // =====================================================================
    // SFX CLIP SLOTS  (assign in Inspector)
    // =====================================================================

    [Header("SFX Clips")]
    [Tooltip("One-off_1 — console teleport to secret base")]
    public AudioClip sfxTeleport;

    [Tooltip("One-off_2 — article published")]
    public AudioClip sfxPublish;

    [Tooltip("One-off_3 — dictionary panel opens")]
    public AudioClip sfxDictionary;

    [Tooltip("One-off_2__Synth_ — page flip / option selected")]
    public AudioClip sfxPageFlip;

    // =====================================================================
    // VOLUME SETTINGS
    // =====================================================================

    [Header("Volume")]
    [Range(0f, 1f)] public float musicVolume = 0.45f;
    [Range(0f, 1f)] public float sfxVolume   = 0.85f;

    [Tooltip("Seconds for one track to fade out while the other fades in.")]
    [Range(0.1f, 4f)] public float crossfadeDuration = 1.5f;

    // =====================================================================
    // ENUMS — used as call-site API
    // =====================================================================

    public enum MusicTrack { None, Office, Ending, SecretEnding, Menu }
    public enum SFX        { Teleport, Publish, Dictionary, PageFlip }

    // =====================================================================
    // PRIVATE STATE
    // =====================================================================

    private AudioSource _musicSourceA;
    private AudioSource _musicSourceB;
    private AudioSource _sfxSource;
    private AudioSource _activeMusicSource;   // whichever source is currently "live"

    private MusicTrack  _currentTrack = MusicTrack.None;
    private Coroutine   _crossfadeRoutine;
    private GameStateManager.CharacterID _currentInterviewCharacter = GameStateManager.CharacterID.None;

    // =====================================================================
    // PUBLIC API — MUSIC
    // =====================================================================

    /// <summary>
    /// Crossfades to the requested track.
    /// If the track is already playing, does nothing.
    /// Pass MusicTrack.None to fade out entirely.
    /// </summary>
    public void SwitchMusic(MusicTrack track)
    {
        if (track == _currentTrack) return;

        AudioClip clip = ClipForTrack(track);

        // MusicTrack.None with no clip = fade out only
        if (track != MusicTrack.None && clip == null)
        {
            Debug.LogWarning($"[AudioManager] No clip assigned for track: {track}");
            return;
        }

        _currentTrack = track;
        _currentInterviewCharacter = GameStateManager.CharacterID.None; // reset so next interview fires fresh

        if (_crossfadeRoutine != null) StopCoroutine(_crossfadeRoutine);
        _crossfadeRoutine = StartCoroutine(CrossfadeRoutine(clip));
    }

    /// <summary>Fade out current music without starting anything new.</summary>
    public void StopMusic()
    {
        SwitchMusic(MusicTrack.None);
    }

    /// <summary>
    /// Called from DialogueManager.BeginInterview().
    /// Crossfades to the music assigned to this specific character.
    /// Usage: AudioManager.Instance.SwitchInterviewMusic(GameStateManager.Instance.GetID(current));
    /// </summary>
    public void SwitchInterviewMusic(GameStateManager.CharacterID id)
    {
        AudioClip clip = ClipForCharacter(id);
        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] No interview clip assigned for character: {id}");
            return;
        }

        // Use a sentinel value in _currentTrack so the same character's music
        // doesn't restart if BeginInterview is called twice (e.g. resume after pause).
        // We store the CharacterID separately and only crossfade on a real change.
        if (id == _currentInterviewCharacter) return;
        _currentInterviewCharacter = id;
        _currentTrack = MusicTrack.None;   // clear generic track so crossfade runs

        if (_crossfadeRoutine != null) StopCoroutine(_crossfadeRoutine);
        _crossfadeRoutine = StartCoroutine(CrossfadeRoutine(clip));
    }

    /// <summary>Immediately mute / unmute music (no crossfade).</summary>
    public void SetMusicMuted(bool muted)
    {
        if (_musicSourceA != null) _musicSourceA.mute = muted;
        if (_musicSourceB != null) _musicSourceB.mute = muted;
    }

    // =====================================================================
    // PUBLIC API — SFX
    // =====================================================================

    /// <summary>Fire-and-forget one-shot SFX.</summary>
    public void PlaySFX(SFX sfx)
    {
        AudioClip clip = ClipForSFX(sfx);
        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] No clip assigned for SFX: {sfx}");
            return;
        }
        _sfxSource.PlayOneShot(clip, sfxVolume);
    }

    /// <summary>Play any arbitrary clip as a one-shot SFX.</summary>
    public void PlaySFXClip(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // =====================================================================
    // CROSSFADE COROUTINE
    // =====================================================================

    private IEnumerator CrossfadeRoutine(AudioClip incomingClip)
    {
        // Identify the "outgoing" source (currently playing) and the "incoming" one
        AudioSource outgoing = _activeMusicSource;
        AudioSource incoming = (outgoing == _musicSourceA) ? _musicSourceB : _musicSourceA;

        // Set up the incoming source
        incoming.clip   = incomingClip;
        incoming.volume = 0f;
        incoming.loop   = true;

        if (incomingClip != null)
            incoming.Play();
        else
            incoming.Stop();

        // Crossfade
        float elapsed = 0f;
        while (elapsed < crossfadeDuration)
        {
            float t = elapsed / crossfadeDuration;
            outgoing.volume = Mathf.Lerp(musicVolume, 0f, t);
            if (incomingClip != null)
                incoming.volume = Mathf.Lerp(0f, musicVolume, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        outgoing.volume = 0f;
        outgoing.Stop();
        if (incomingClip != null) incoming.volume = musicVolume;

        _activeMusicSource = incoming;
        _crossfadeRoutine  = null;
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    private void ConfigureMusicSource(AudioSource src)
    {
        src.loop        = true;
        src.playOnAwake = false;
        src.volume      = 0f;   // starts silent; CrossfadeRoutine brings it up
        src.spatialBlend = 0f;  // 2D audio — no positional falloff
    }

    private AudioClip ClipForTrack(MusicTrack track)
    {
        switch (track)
        {
            case MusicTrack.Office:       return officeMusic;
            case MusicTrack.Ending:       return endingMusic;
            case MusicTrack.SecretEnding: return secretEndingMusic;
            case MusicTrack.Menu:         return menuMusic;
            default:                      return null;
        }
    }

    private AudioClip ClipForCharacter(GameStateManager.CharacterID id)
    {
        switch (id)
        {
            case GameStateManager.CharacterID.Izul:     return izulMusic;
            case GameStateManager.CharacterID.Kortnara: return kortnaraMusic;
            case GameStateManager.CharacterID.Gorp:     return gorpMusic;
            case GameStateManager.CharacterID.Andrew:   return andrewMusic;
            default:                                     return null;
        }
    }

    private AudioClip ClipForSFX(SFX sfx)
    {
        switch (sfx)
        {
            case SFX.Teleport:   return sfxTeleport;
            case SFX.Publish:    return sfxPublish;
            case SFX.Dictionary: return sfxDictionary;
            case SFX.PageFlip:   return sfxPageFlip;
            default:             return null;
        }
    }
}
