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
    [SerializeField] private AudioSource bgAudioSource, vfxAudioSource;
    [SerializeField] List<SoundBG> bgSounds;
    [SerializeField] List<SoundVFX> vfsSounds;
    [SerializeField] private AudioClip soundInGame;
    private Dictionary<SoundVFXType, AudioClip> soundVFXDict;
    private Dictionary<SoundBGType, AudioClip> soundBGDict;
    [Range(0f, 1f), SerializeField] private float musicBGVolumn, musicVFXVolumn;


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
    }

    private void Start()
    {
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            vfxAudioSource.PlayOneShot(soundVFXDict[SoundVFXType.SoundOnClickMouse]);
        }
    }

    public void SetMusicBGVolumn(float volume)
    {
        musicBGVolumn = volume;
    }

    public void SetMusicVFXVolumn(float volume)
    {
        musicVFXVolumn = volume;
        Debug.Log(musicVFXVolumn);
    }
}
