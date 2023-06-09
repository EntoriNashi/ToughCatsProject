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
    [HideInInspector] public bool isDead = false;
    public bool isInPuzzle;
    public bool isInCinematic;

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
    public TextMeshProUGUI playerGrenadeAmount;
    public TextMeshProUGUI introText;
    public TextMeshProUGUI pistolText;
    public TextMeshProUGUI rifleText;



    [Header("*----- Other -----*")]
    public bool IsPlayerDetected;
    public GameObject unarmed;
    [SerializeField][Range(0, 10)] float WinDelay;
    [SerializeField][Range(0, 10)] float introTextDisplayDuration;
    [SerializeField][Range(0, 10)] float introTextFadeDuration;
    [SerializeField] public float introTextTypeSpeed;

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

        // intro text //
        StartCoroutine(ShowIntroText());

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && (activeMenu == null || activeMenu==pauseMenu))
        {
            PauseTheGame();
        }
    }

    public void PauseTheGame()
    {
        isPaused = true;
        activeMenu = pauseMenu;
        activeMenu.SetActive(true);
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
        Time.timeScale = timeScaleOrig;
        activeMenu?.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        activeMenu = null;
    }

    IEnumerator YouWin()
    {
        yield return new WaitForSeconds(WinDelay);
        AudioManager.instance.PlayEndGameTrack("Win");
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
        isDead = false;
        DestroyAllBullets();
        AudioManager.instance.MuteTracks();
        AudioManager.instance.PlayEndGameTrack("Lose");
        AudioManager.instance.isGameEnded = true;
        GameObject[] allPuzzle = GameObject.FindGameObjectsWithTag("Puzzle");
        foreach (GameObject puzzle in allPuzzle)
        {
            puzzle.SetActive(false);
        }

        activeMenu = LoseMenu;
        activeMenu.SetActive(true);
        SetPrimaryButton(activeMenu.transform.GetChild(1).gameObject);
        PauseState();
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
    public void PuzzleActivate(GameObject puzzle)
    {
        activeMenu = puzzle;
        activeMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        isPaused = true;
    }

    public bool GetIsPaused()
    {
        return isPaused;
    }

    IEnumerator ShowIntroText()
    {
        // Fade in //
        //yield return StartCoroutine(FadeTextIn(introTextFadeDuration, introText));
        yield return StartCoroutine(TypeText(introText));
        // Wait //
        yield return new WaitForSeconds(introTextDisplayDuration);
        // Fade out //
        yield return StartCoroutine(FadeTextOut(introTextFadeDuration, introText));
    }
    

    public IEnumerator TypeText(TMP_Text textComponent)
    {
        string fullText = textComponent.text;
        textComponent.text = "";

        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(introTextTypeSpeed);  // You can adjust this delay for faster or slower typing
        }
    }

    public IEnumerator FadeTextOut(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    public void DestroyAllBullets()
    {
        Bullet[] bullets = FindObjectsOfType<Bullet>();

        foreach (Bullet bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
    }

    public void ShowPistolText()
    {
        StartCoroutine(ShowAndHideWeaponText(pistolText));
    }

    public void ShowRifleText()
    {
        StartCoroutine(ShowAndHideWeaponText(rifleText));
    }

    IEnumerator ShowAndHideWeaponText(TMP_Text textComponent)
    {
        // activate the text object
        textComponent.gameObject.SetActive(true);

        // Show the text like a typewriter
        yield return StartCoroutine(TypeText(textComponent));

        // Wait for some time (you can customize this delay)
        yield return new WaitForSeconds(3.0f);

        // Fade out the text
        yield return StartCoroutine(FadeTextOut(introTextFadeDuration, textComponent));

        // Deactivate the text object
        textComponent.gameObject.SetActive(false);
    }
}
