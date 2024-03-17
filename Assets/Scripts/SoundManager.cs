using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private bool isBackgroundMusicPlaying;
    private AudioSource source;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        source = GetComponent<AudioSource>();

        ToggleBackgroundMusic();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClip(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }

    public void StopClip(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Stop();
    }


    public void ToggleBackgroundMusic()
    {
        if (isBackgroundMusicPlaying || source.isPlaying)
        {
            source.Stop();
            isBackgroundMusicPlaying = true;
        }
        else
        {
            source.Play();
            isBackgroundMusicPlaying = false;
        }
    }
}
