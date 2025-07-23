using UnityEngine;
public class SoundManager : SingletonMono<SoundManager>
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    [Range(0f, 1f), SerializeField] private float musicBGVolumn, musicVFXVolumn;
    
    public void SetMusicBGVolumn(float volume)
    {
        musicBGVolumn = volume;
        //Debug.Log(musicBGVolumn);
    }

    public void SetMusicVFXVolumn(float volume)
    {
        musicVFXVolumn = volume;
        Debug.Log(musicVFXVolumn);
    }
}
