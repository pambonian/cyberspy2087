using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource backgroundMusic;

    public AudioSource[] SFXs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // This line ensures the AudioManager isn't destroyed on scene load.
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayerSFX(int sfxNumber)
    {
        Debug.Log("Playing SFX number: " + sfxNumber);
        Debug.Log($"Playing SFX number: {sfxNumber} - {SFXs[sfxNumber].name}");
        SFXs[sfxNumber].Stop();
        SFXs[sfxNumber].Play();
    }

    public void StopBackgroundMusic()
    {
        backgroundMusic.Stop();
    }

    public void SetVolume(float volume)
    {
        if (backgroundMusic != null && backgroundMusic.isActiveAndEnabled)
        {
            backgroundMusic.volume = volume;
        }

        foreach (AudioSource sfx in SFXs)
        {
            if (sfx != null && sfx.isActiveAndEnabled)
            {
                sfx.volume = volume;
            }
        }
    }
}
