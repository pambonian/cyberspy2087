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
        instance = this;
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
}
