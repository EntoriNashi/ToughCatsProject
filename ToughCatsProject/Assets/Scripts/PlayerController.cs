using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] LayerMask PlayerMask;

    [Header("----- Player Attributes -----")]
    [SerializeField][Range(1, 10)] int HP;
    [SerializeField][Range(1, 10)] int maxHP;
    [SerializeField][Range(1, 5)] float playerSpeed;
    [SerializeField][Range(2, 5)] float sprintMod;
    [SerializeField][Range(1, 50)] float jumpHeight;
    [SerializeField][Range(9.81f, 20)] float gravityValue;
    [SerializeField][Range(1, 3)] int jumpMax;

    [Header("----- Weapon Attributes -----")]
    public List<GunStats> gunList = new List<GunStats>();
    [SerializeField][Range(2, 300)] int shootDistance;
    [SerializeField][Range(0.1f, 3)] float shootRate;
    [SerializeField][Range(1, 10)] int shootDamage;
    [SerializeField] MeshFilter gunModel;
    [SerializeField] MeshRenderer gunMaterial;

    [Header("*----- Grenade Attributes -----*")]
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Transform throwPoint;
    [SerializeField][Range(1, 3)] int totalGrenades;
    [SerializeField][Range(0.2f, 5)] float throwCooldown;
    [SerializeField][Range(1, 100)] int throwForce;
    [SerializeField][Range(1, 100)] int throwUpwardForce;

    [Header("*----- Audio -----*")]
    [SerializeField] AudioClip[] audJump;
    [SerializeField] [Range(0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audDmg;
    [SerializeField] [Range(0, 1)] float audDmgVol;
    [SerializeField] AudioClip[] audSteps;
    [SerializeField] [Range(0, 1)] float audStepsVol;

    

    private Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private int jumpedTimes;
    private bool isSprinting;
    private bool isShooting;
    private int selectedGun;
    bool stepIsPlaying;
    int currentMag;
    int currentAmmo;
    bool isReloading;
    bool isThrowing = false;
    int currGrenadeAmount;

    private void Start()
    {
        SpawnPlayer();
        HP = maxHP;
        currGrenadeAmount = totalGrenades;
    }

    void Update()
    {
        if(gameManager.instance.activeMenu == null)
        {
            Movement();
            SelectGun();
            if (Input.GetButton("Shoot") && !isShooting && gunList.Count > 0)
            {
                StartCoroutine(Shoot());
            }
            if(Input.GetButton("Reload") && !isReloading && gunList[selectedGun].numOfMag > 0 && gunList.Count > 0)
            {
                StartCoroutine(Reload());
            }
            if(Input.GetButton("Grenade") && !isThrowing && currGrenadeAmount > 0)
            {
                StartCoroutine(ThrowGrenade());
            }
        }

        Sprint();        
    }

    private void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer)
        {
            if (!stepIsPlaying && move.normalized.magnitude > 0.5f)
            {
                StartCoroutine(playSteps());
            }

            if (playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
                jumpedTimes = 0;
            }
        }

        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpMax)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            jumpedTimes++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            playerSpeed *= sprintMod;
        }
        else if(Input.GetButtonUp("Sprint"))
        { 
            isSprinting= false;
            playerSpeed /= sprintMod;
        }
    }

    IEnumerator playSteps()
    {
        stepIsPlaying = true;

        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (!isSprinting)
        {
            yield return new WaitForSeconds(.5f);
        }
        else
        {
            yield return new WaitForSeconds(.3f);
        }


        stepIsPlaying = false;
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        currentAmmo--;
        aud.PlayOneShot(gunList[selectedGun].gunShotAud, gunList[selectedGun].gunShotAudVol);

        RaycastHit hit;

        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f,0.5f)), out hit, shootDistance, PlayerMask))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();
            if(damageable != null)
            {
                damageable.takeDamage(shootDamage);
            }

            GameObject hitEffect = Instantiate(gunList[selectedGun].hitEffect, hit.point, gunList[selectedGun].hitEffect.transform.rotation);
            yield return new WaitForSeconds(0.5f);
            Destroy(hitEffect);
        }

        yield return new WaitForSeconds(shootRate);
        //UpdatePlayerUI();

        isShooting = false;
    }

    IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(gunList[selectedGun].reloadSpeed);

        //decrease mag amount
        currentMag--;
        //set current to mag max size
        currentAmmo = gunList[selectedGun].magazineSize;

        isReloading = false;
    }

    public void Heal(int amount)
    {
        HP += amount;
        UpdatePlayerUI();
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        aud.PlayOneShot(audDmg[Random.Range(0, audDmg.Length)], audDmgVol);
        UpdatePlayerUI();
        StartCoroutine(DamageFlash());

        if (HP <= 0)
        {
            //kill player and respawn
            gameManager.instance.youLose();
        }
    }

    IEnumerator DamageFlash()
    {
        gameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        gameManager.instance.playerDamageFlash.SetActive(false);
    }

    public void SpawnPlayer()
    {
        controller.enabled = false;
        HP = maxHP;
        UpdatePlayerUI();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void GunPickUp(GunStats gunStat)
    {
        gunList.Add(gunStat);

        ChangeGunStats(gunStat);
        selectedGun = gunList.Count - 1;
        currentMag = gunList[selectedGun].numOfMag;
        currentAmmo = gunList[selectedGun].magazineSize;

        UpdatePlayerUI();
    }

    private void ChangeGunStats(GunStats gunStat)
    {
        shootDamage = gunStat.shootDamage;
        shootRate = gunStat.shootRate;
        shootDistance = gunStat.shootDistance;

        gunModel.mesh = gunStat.model.GetComponent<MeshFilter>().sharedMesh;
        gunMaterial.material = gunStat.model.GetComponent<MeshRenderer>().sharedMaterial;

        UpdatePlayerUI();
    }

    void SelectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
        }

        if(gunList.Count > 0)
            ChangeGunStats(gunList[selectedGun]);
    }

    IEnumerator ThrowGrenade()
    {
        isThrowing = true;
        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, grenadePrefab.transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        Vector3 forceToAdd = GetComponentInChildren<Camera>().transform.forward * throwForce + transform.up * throwUpwardForce;

        rb.AddForce(forceToAdd, ForceMode.Impulse);
        currGrenadeAmount--;

        yield return new WaitForSeconds(throwCooldown);
        isThrowing = false;
    }

    void UpdatePlayerUI()
    {
        gameManager.instance.playerHpBar.fillAmount = (float)HP / maxHP;
        if (gunList.Count > 0)
        {
            gameManager.instance.playerCurrentAmmo.text = $"{currentAmmo}";
            gameManager.instance.playerMagazineAmount.text = $"{gunList[selectedGun].numOfMag}";
            gameManager.instance.playerMagazineSize.text = $"/{gunList[selectedGun].magazineSize}";
        }
    }
}
