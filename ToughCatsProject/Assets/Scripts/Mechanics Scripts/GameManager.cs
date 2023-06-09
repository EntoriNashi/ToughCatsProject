using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    // Variables //
    public static GameManager instance;


    [Header("*----- Player Stuff -----*")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawnPos;

    [Header("*----- UI Stuff -----*")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject LoseMenu;
    public GameObject WinMenu;
    public GameObject settingsMenu;
    public Image playerHpBar;
    public GameObject playerDamageFlash;
    public TextMeshProUGUI KillCountText;
    public TextMeshProUGUI TotalEnemiesText;
    public TextMeshProUGUI playerCurrentAmmo;
    public TextMeshProUGUI playerMagazineSize;
    public TextMeshProUGUI playerMagazineAmount;
    

    [Header("*----- Other -----*")]
    public bool IsPlayerDetected;
    public GameObject unarmed;
    [SerializeField][Range(0,10)] float WinDelay;

    int enemiesKilled;
    int totalEnemies;
    bool isPaused;
    float timeScaleOrig;

    void Awake()
    {
        instance = this;
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn POS");
        player = GameObject.FindGameObjectWithTag("Player");
        unarmed = GameObject.FindGameObjectWithTag("Unarmed");
        playerScript = player.GetComponent<PlayerController>();
        timeScaleOrig = Time.timeScale;
        IsPlayerDetected = false;
        totalEnemies = 0;
        enemiesKilled = 0;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            PauseTheGame();
        }
    }

    public void PauseTheGame()
    {
        isPaused = !isPaused;
        activeMenu = pauseMenu;
        activeMenu.SetActive(isPaused);
        SetPrimaryButton(activeMenu.transform.GetChild(1).gameObject);

        PauseState();
    }

    public void SetPrimaryButton(GameObject primaryButton)
    {
        if(primaryButton == null)
        {
            return;
        }
        ButtonSelect.primaryButton = primaryButton.GetComponent<Button>();
    }

    public void PauseState()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UnpausedState()
    {
        activeMenu.SetActive(false);
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = !isPaused;
        activeMenu = null;
    }

    IEnumerator YouWin()
    {
        yield return new WaitForSeconds(WinDelay);
        AudioManager.instance.PlayWinTrack("Win");
        AudioManager.instance.isGameEnded = true;

        activeMenu = WinMenu;
        activeMenu.SetActive(true);
        SetPrimaryButton(activeMenu.transform.GetChild(1).gameObject);
        PauseState();
    }
    
    public void UpdateWinCondition()
    {
        StartCoroutine(YouWin());
    }

    public void YouLose()
    {
        PauseState();
        activeMenu = LoseMenu;
        AudioManager.instance.isGameEnded = true;
        activeMenu.SetActive(true);
        SetPrimaryButton(activeMenu.transform.GetChild(1).gameObject);
    }

    
    public void EnemyDefeatedCounter()
    {
        enemiesKilled += 1;
        UpdateUI();
    }

    public void UpdatePlayerSpawnPos(Vector3 position, Quaternion oriantation)
    {
        playerSpawnPos.transform.position = position;
        playerSpawnPos.transform.rotation = oriantation;
    }

    public void UpdateEnemyCout(int amount)
    {
        totalEnemies += amount;
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        KillCountText.text = enemiesKilled.ToString("F0");
        TotalEnemiesText.text = totalEnemies.ToString("F0");
    }
}
