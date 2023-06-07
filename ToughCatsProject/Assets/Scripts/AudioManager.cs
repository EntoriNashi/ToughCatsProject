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
    private bool isPlayingTrack1;

    public static AudioManager instance;

    private int attackingEnemiesCount = 0;

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

        isPlayingTrack1 = true;

        SwapTrack(defaultAudio);
    }

    public void SwapTrack(AudioClip newClip)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTrack(newClip));

        isPlayingTrack1 = !isPlayingTrack1;
    }

    public void SwapTrackString(string clipName)
    {
        if (audioClips.ContainsKey(clipName))
            SwapTrack(audioClips[clipName]);

        if (clipName == "Battle")
            attackingEnemiesCount++;
    }

    public void ReturnToDefault()
    {
        if (attackingEnemiesCount > 0)
            attackingEnemiesCount--;

        if (attackingEnemiesCount == 0)
            SwapTrack(defaultAudio);


    }

    private IEnumerator FadeTrack(AudioClip newClip)
    {
        AudioSource currentTrack = isPlayingTrack1 ? track1 : track2;

        if (currentTrack.clip == newClip)
            yield break;

        float fadeTime = 1.25f;
        float timePassed = 0;

        if (isPlayingTrack1)
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


