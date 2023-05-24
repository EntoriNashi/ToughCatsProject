using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;


    [Header("----- Player Stuff -----")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawnPos;

    [Header("*----- UI Stuff -----*")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject LoseMenu;
    public GameObject WinMenu;

    [Header("*----- Other -----*")]
    public bool IsPlayerDetected;

    int enemiesRemaining;
    bool isPaused;
    float timeScaleOrig;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        timeScaleOrig = Time.timeScale;
        IsPlayerDetected = false;
        Instantiate(playerSpawnPos, player.transform.position, player.transform.rotation);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            isPaused = !isPaused;
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);

            pauseState();
        }
    }

    public void pauseState()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void unPausedState()
    {
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = !isPaused;
        activeMenu.SetActive(false);
        activeMenu = null;
    }

    IEnumerator youWin()
    {
        yield return new WaitForSeconds(3);
        activeMenu = WinMenu;
        activeMenu.SetActive(true);
        pauseState();
    }
    
    public void UpdateWinCondition()
    {

    }

    public void youLose()
    {
        pauseState();
        activeMenu = LoseMenu;
        activeMenu.SetActive(true);
    }

    
    public void enemyDefeatedCounter()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
        }
    }

    public void UpdatePlayerSpawnPos(Vector3 position, Quaternion oriantation)
    {
        playerSpawnPos.transform.position = position;
        playerSpawnPos.transform.rotation = oriantation;
    }
    
    
}
