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
        GameManager.instance.UnpausedState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void respawnPlayer()
    {
        GameManager.instance.UnpausedState();
        GameManager.instance.playerScript.SpawnPlayer();
    }

    public void openSettingsMenu()
    {
        GameManager.instance.activeMenu.SetActive(false);
        GameManager.instance.activeMenu = GameManager.instance.SettingsMenu;
        GameManager.instance.activeMenu.SetActive(true);
    }

    public void closeSettingsMenu()
    {
        GameManager.instance.activeMenu.SetActive(false);
        GameManager.instance.activeMenu = GameManager.instance.pauseMenu;
        GameManager.instance.activeMenu.SetActive(true);
    }
}
