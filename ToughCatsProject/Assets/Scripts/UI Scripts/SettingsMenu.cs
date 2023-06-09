using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string volumeMusicParameter = "volumeMusic";
    [SerializeField] string volumeSfxParameter = "volumeSfx";
    [SerializeField] Slider sliderMusic;
    [SerializeField] Slider sliderSfx;
    [SerializeField] float multiplier = 30f;

    private void Awake()
    {
        sliderMusic.onValueChanged.AddListener(HandleSliderMusicChanged);
        sliderSfx.onValueChanged.AddListener(HandleSliderSfxChanged);
    }

    private void HandleSliderMusicChanged(float value)
    {
        audioMixer.SetFloat(volumeMusicParameter, value:Mathf.Log10(value) * multiplier);
    }

    private void HandleSliderSfxChanged(float value)
    {
        audioMixer.SetFloat(volumeSfxParameter, value:Mathf.Log10(value) * multiplier);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeMusicParameter, sliderMusic.value);
        PlayerPrefs.SetFloat(volumeSfxParameter, sliderSfx.value);
    }

    private void Start()
    {
        
    }
}
