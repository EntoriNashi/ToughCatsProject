using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public void SetFfxVolume(float volume)
    {
        audioMixer.SetFloat("volumeSfx", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("volumeMusic", volume);
    }
}
