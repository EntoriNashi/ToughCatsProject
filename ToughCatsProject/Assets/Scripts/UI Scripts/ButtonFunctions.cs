using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.UnpausedState();
    }

    public void restart()
    {
        AudioManager.instance.isGameEnded = false;
        GameManager.instance.UnpausedState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void respawnPlayer()
    {
        AudioManager.instance.isGameEnded = false;
        GameManager.instance.UnpausedState();
        GameManager.instance.playerScript.SpawnPlayer();
        GameManager.instance.isDead = false;
    }

    public void openSettingsMenu()
    {
        GameManager.instance.activeMenu.SetActive(false);
        GameManager.instance.activeMenu = GameManager.instance.settingsMenu;
        GameManager.instance.activeMenu.SetActive(true);
        GameManager.instance.SetPrimaryButton(GameManager.instance.settingsMenu.transform.GetChild(3).gameObject);
    }

    public void closeSettingsMenu()
    {
        GameManager.instance.activeMenu.SetActive(false);
        GameManager.instance.activeMenu = null;
        GameManager.instance.activeMenu = GameManager.instance.pauseMenu;
        GameManager.instance.activeMenu.SetActive(true);
        GameManager.instance.SetPrimaryButton(GameManager.instance.pauseMenu.transform.GetChild(1).gameObject);
    }
}
