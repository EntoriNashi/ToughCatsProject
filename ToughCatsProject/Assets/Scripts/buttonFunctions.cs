using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        gameManager.instance.unPausedState();
    }

    public void Restart()
    {
        gameManager.instance.unPausedState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void respawnPlayer()
    {
        gameManager.instance.unPausedState();
        gameManager.instance.playerScript.SpawnPlayer();
    }
}
