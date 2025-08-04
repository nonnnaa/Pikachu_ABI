using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct SoundBG
{
    public AudioClip clip;
    public SoundBGType bgType;
}
[Serializable]
struct SoundVFX
{
    public AudioClip clip;
    public SoundVFXType vfxType;
}
public enum SoundVFXType
    {
        SoundOnClickMouse = 0,
        SoundWinGame = 1,
        SoundLoseGame = 2,
        SoundGetPoint = 3,
        SoundAlert = 4
    }
    public enum SoundBGType
    {
        SoundBG0 = 0,
        SoundBG1 = 1,
        SoundBG2 = 2,
        SoundBG3 = 3,
        SoundBG4 = 4,
        SoundBG5 = 5,
    }
public class SoundManager : SingletonMono<SoundManager>
{
    [SerializeField] private AudioSource bgAudioSource, vfxAudioSource, alertAudioSource;
    [SerializeField] List<SoundBG> bgSounds;
    [SerializeField] List<SoundVFX> vfsSounds;
    [SerializeField] private AudioClip soundInGame;
    [SerializeField] private AudioClip soundAlertGame;
    private Dictionary<SoundVFXType, AudioClip> soundVFXDict;
    private Dictionary<SoundBGType, AudioClip> soundBGDict;
    [Range(0f, 1f), SerializeField] private float musicBGVolumn, musicVFXVolumn;
    private UserSettings userSettings;

    private void Start()
    {
        userSettings = SaveLoadManager.Instance.LoadSettings();
        ApplySettings();
    }
    private void ApplySettings()
    {
        SetMusicBGVolumn(userSettings.musicBGVolume);
        SetMusicVFXVolumn(userSettings.musicVFXVolume);
    }

    private void Awake()
    {
        if (bgAudioSource == null && bgSounds == null)
        {
            AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
            bgAudioSource = audioSources[0];
            vfxAudioSource = audioSources[1];
            bgAudioSource.loop = true;
            vfxAudioSource.loop = false;
        }
        soundBGDict =  new Dictionary<SoundBGType, AudioClip>();
        foreach (var sound in bgSounds)
        {
            soundBGDict[sound.bgType] = sound.clip;
        }
        soundVFXDict = new Dictionary<SoundVFXType, AudioClip>();
        foreach (var sound in vfsSounds)
        {
            soundVFXDict[sound.vfxType] = sound.clip;
        }
        GameManager.Instance.OnStartGame += () =>
        {
            //Debug.Log("Game started");
            SetMusicBG(SoundBGType.SoundBG0);
        };
        GameManager.Instance.OnMainMenu += () =>
        {
            //Debug.Log("Level SoundManager");
            SetMusicBG(SoundBGType.SoundBG1);
        };
        GameManager.Instance.OnLevelStart += SetMusicInGame;
        GameManager.Instance.OnLevelEnd += () =>
        {
            TurnOffMusicBG();
            StopAlertMusic();
        };
        GameManager.Instance.OnGameWin += () =>
        {
            SetMusicVFX(SoundVFXType.SoundWinGame);
        };
        GameManager.Instance.OnGameLose += () =>
        {
            SetMusicVFX(SoundVFXType.SoundLoseGame);
        };
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            vfxAudioSource.PlayOneShot(soundVFXDict[SoundVFXType.SoundOnClickMouse]);
        }
    }
    
    public void SetMusicBG(SoundBGType type)
    {
        bgAudioSource.Stop();  
        bgAudioSource.clip = soundBGDict[type];
        bgAudioSource.time = 0f; 
        bgAudioSource.Play();
    }

    public void SetMusicVFX(SoundVFXType type)
    {
       vfxAudioSource.PlayOneShot(soundVFXDict[type]);
    }
    public void SetMusicBGVolumn(float volume)
    {
        musicBGVolumn = volume;
        bgAudioSource.volume = musicBGVolumn;
        userSettings.musicBGVolume = volume;
        SaveLoadManager.Instance.SaveSettings(userSettings);
    }
    public float GetMusicBGVolumn() => bgAudioSource.volume;
    public float GetMusicVFXVolumn() => vfxAudioSource.volume;

    public void SetMusicInGame()
    {
        bgAudioSource.Stop();  
        bgAudioSource.clip = soundInGame;
        bgAudioSource.time = 0f; 
        bgAudioSource.Play();
    }
    
    public void SetMusicVFXVolumn(float volume)
    {
        musicVFXVolumn = volume;
        vfxAudioSource.volume = musicVFXVolumn;
        userSettings.musicVFXVolume = volume;
        SaveLoadManager.Instance.SaveSettings(userSettings);
    }

    private void TurnOffMusicBG()
    {
        bgAudioSource.Pause();
        vfxAudioSource.loop = false;
    }

    public void StopAlertMusic()
    {
        alertAudioSource.Stop();
    }
    public void PlayAlertSound(bool isOneShot)
    {
        if (isOneShot)
        {
            alertAudioSource.PlayOneShot(soundVFXDict[SoundVFXType.SoundAlert]);
        }
        else
        {
            alertAudioSource.Stop();
            alertAudioSource.Play();
        }
    }
    private void OnApplicationQuit()
    {
        SaveSoundSettings();
    }
    private void SaveSoundSettings()
    {
        SaveLoadManager.Instance.SaveSettings(userSettings);
    }

}
