using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.unPausedState();
    }

    public void restart()
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
        gameManager.instance.unPausedState();
    }
}
