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
}
