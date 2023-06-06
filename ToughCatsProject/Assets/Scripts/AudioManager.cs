using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip defaultAudio;
    private AudioSource track1, track2;
    private bool isPlayingtrack1;
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        instance = this;
    }

    private void Start()
    {
        track1 = gameObject.AddComponent<AudioSource>();
        track2 = gameObject.AddComponent<AudioSource>();
        isPlayingtrack1 = true;

        SwapTrack(defaultAudio);
    }

    public void SwapTrack(AudioClip newClip)
    {
        if (isPlayingtrack1)
        {
            track2.clip = newClip;
            track2.Play();
            track1.Stop();
        }
        else
        {
            track1.clip = newClip;
            track1.Play();
            track2.Stop();
        }

        isPlayingtrack1 = !isPlayingtrack1;
    }
}
