using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string volumeMusicParameter = "volumeMusic";
    [SerializeField] string volumeSfxParameter = "volumeSfx";
    [SerializeField] Slider sliderMusic;
    [SerializeField] Slider sliderSfx;
    [SerializeField] float multiplier;
    public void playGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        sliderMusic.value = PlayerPrefs.GetFloat(volumeMusicParameter);
        sliderSfx.value = PlayerPrefs.GetFloat(volumeSfxParameter);
    }

    public void rollCredits()
    {
        SceneManager.LoadScene(2);
    }
}
