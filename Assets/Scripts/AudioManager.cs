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

    private void Update()
    {
        // None
    }

    public void StopBackgroundMusic()
    {
        backgroundMusic.Stop();
    }
}
