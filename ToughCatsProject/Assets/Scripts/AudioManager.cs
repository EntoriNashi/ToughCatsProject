using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class NamedAudioClip
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public List<NamedAudioClip> clipsToAssignInEditor;

    public AudioClip defaultAudio;
    public AudioMixerGroup outputAudio;
    public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private AudioSource track1, track2;
    private bool isPlayingtrack1;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        instance = this;

        foreach (var namedClip in clipsToAssignInEditor)
        {
            audioClips[namedClip.name] = namedClip.clip;
        }
    }

    private void Start()
    {
        track1 = gameObject.AddComponent<AudioSource>();
        track1.outputAudioMixerGroup = outputAudio;

        track2 = gameObject.AddComponent<AudioSource>();
        track2.outputAudioMixerGroup = outputAudio;

        isPlayingtrack1 = true;

        SwapTrack(defaultAudio);
    }

    public void AddAudioClip(string name, AudioClip clip)
    {
        audioClips[name] = clip;
    }

    public void SwapTrack(AudioClip newClip)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTrack(newClip));

        isPlayingtrack1 = !isPlayingtrack1;
    }

    public void SwapTrackString(string clipName)
    {
        if (audioClips.ContainsKey(clipName))
            SwapTrack(audioClips[clipName]);
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


