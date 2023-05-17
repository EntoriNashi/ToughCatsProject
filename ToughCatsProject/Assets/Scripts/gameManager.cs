using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;


    [Header("----- Player Stuff -----")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawnPOS;

    [Header("*----- UI Stuff -----*")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject LoseMenu;
    public GameObject WinMenu;

    [Header("*----- Wave Stuff -----*")]
    [SerializeField] GameObject enemyType;
    [SerializeField] GameObject enemySpawner;
    [SerializeField] [Range(0, 5)] int enemyCount;
    [SerializeField] [Range(0, 10)] float secBetweenSpawns;
    [SerializeField] [Range(1, 30)] float waveDelay;
    

    int enemiesRemaining;
    public bool isPaused;
    public float timeScaleOrig;
    int WaveCout;
    int numOfWaves;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPOS = GameObject.FindGameObjectWithTag("Player Spawn POS");
        timeScaleOrig = Time.timeScale;
        startWave();

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

    public void youLose()
    {
        pauseState();
        activeMenu = LoseMenu;
        activeMenu.SetActive(true);
    }

    //Updated enemiesRemaining and if there are no more enemies, starts the advanceWave routine
    public void enemyDefeatedCounter()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            StartCoroutine(advanceWave());
        }
    }
    
    //initialize sets numOfWaves and WaveCount, and starts the advanceWave routine
    void startWave()
    {
        numOfWaves = 5;
        WaveCout = 0;

        StartCoroutine(advanceWave());
    }

    //Spawns an amount (enemyCount) of objects (enemyType)  with a delay between spawning (secBetweenSpans)
    IEnumerator SpawnEnemy()
    {
        for (int spawnCount = 0; spawnCount < enemyCount; spawnCount++)
        {
            Instantiate(enemyType, enemySpawner.transform);
            yield return new WaitForSeconds(secBetweenSpawns);

        }
    }


    //Delays wave advacement(waveDelay), increase WaveCount, and decides if to start another wave or start the win routine
    IEnumerator advanceWave()
    {
        yield return new WaitForSeconds(waveDelay);
        WaveCout++;
        if (WaveCout <= numOfWaves)
        {
            enemiesRemaining = enemyCount;

            StartCoroutine(SpawnEnemy());
        }
        else
        {
            StartCoroutine(youWin());
        }
    }
}
