using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip defaultAudio;
    private AudioSource track1, track2;
    private bool isPlayingtrack1;
    public static AudioManager instance;

    //[SerializeField] private float fadeTime;
    //[SerializeField] private float timePassed;

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
        StopAllCoroutines();
        StartCoroutine(FadeTrack(newClip));

        isPlayingtrack1 = !isPlayingtrack1;
    }

    public void ReturnToDefault()
    {
        SwapTrack(defaultAudio);
    }

    private IEnumerator FadeTrack(AudioClip newClip)
    {
        float fadeTime = 1.25f;
        float timePassed = 0;

        if (isPlayingtrack1)
        {
            track2.clip = newClip;
            track2.Play();

            while(timePassed < fadeTime)
            {
                track2.volume = Mathf.Lerp(0, 1, timePassed / fadeTime);
                track1.volume = Mathf.Lerp(1, 0, timePassed / fadeTime);
                timePassed += Time.deltaTime;
                yield return null;
            }

            track1.Stop();
        }
        else
        {
            track1.clip = newClip;
            track1.Play();

            while (timePassed < fadeTime)
            {
                track1.volume = Mathf.Lerp(0, 1, timePassed / fadeTime);
                track2.volume = Mathf.Lerp(1, 0, timePassed / fadeTime);
                timePassed += Time.deltaTime;
                yield return null;
            }

            track2.Stop();
        }
    }
}


