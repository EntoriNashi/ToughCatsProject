using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] Camera mainCamera;
    //[SerializeField] LayerMask PlayerMask;

    [Header("----- Player Attributes -----")]
    [SerializeField][Range(1, 10)] int HP;
    [SerializeField][Range(1, 10)] int maxHP;
    [SerializeField][Range(1, 5)] float playerSpeed;
    [SerializeField][Range(2, 5)] float sprintMod;
    [SerializeField][Range(1, 50)] float jumpHeight;
    [SerializeField][Range(9.81f, 20)] float gravityValue;
    [SerializeField][Range(1, 3)] int jumpMax;
    [SerializeField][Range(0.1f, 1f)] float crouchHeight = 0.5f;
    [SerializeField][Range(1f, 3f)] float standinghHeight = 2f;
    [SerializeField][Range(0.1f, 1f)] float timeToCrouch = 0.25f;
    [SerializeField] Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] public float crouchOffset;
    [SerializeField] Vector3 standingCenter = Vector3.zero;
    [SerializeField] public bool isCrouching;

    [Header("----- Weapon Attributes -----")]
    public List<GunStats> gunList = new List<GunStats>();
    [SerializeField][Range(2, 300)] int shootDistance;
    [SerializeField][Range(0.1f, 3)] float shootRate;
    [SerializeField][Range(1, 10)] int shootDamage;
    [SerializeField] MeshFilter gunModel;
    [SerializeField] MeshRenderer gunMaterial;
    [SerializeField] public GameObject gunPos;
    [SerializeField] public Vector3 gunLowerPos = new Vector3(0, -1, 0);
    [SerializeField] public Vector3 gunOrigPos;
    [SerializeField] private float reloadTime;
    [SerializeField] Transform muzzle;

    [Header("*----- Grenade Attributes -----*")]
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Transform throwPoint;
    [SerializeField][Range(1, 3)] int totalGrenades;
    [SerializeField][Range(0.2f, 5)] float throwCooldown;
    [SerializeField][Range(1, 15)] int throwForce;
    [SerializeField][Range(1, 15)] int throwUpwardForce;

    [Header("*----- Audio -----*")]
    [SerializeField] AudioClip[] audJump;
    [SerializeField][Range(0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audDmg;
    [SerializeField][Range(0, 1)] float audDmgVol;
    [SerializeField] AudioClip[] audSteps;
    [SerializeField][Range(0, 1)] float audStepsVol;


    float crouchSpeed;
    private Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private int jumpedTimes;
    private bool isSprinting;
    private bool isShooting;
    private int selectedGun;
    bool stepIsPlaying;
    //int currentMag;
    //int currentAmmo;
    bool isReloading;
    bool isThrowing = false;
    int currGrenadeAmount;
    bool canCrouch = true;

    private void Start()
    {
        SpawnPlayer();
        HP = maxHP;
        currGrenadeAmount = totalGrenades;
        crouchSpeed = playerSpeed / 2;
        if (gunList.Count != 0)
        {
            gunList[selectedGun].currentAmmo = gunList[selectedGun].magazineSize;
            gunList[selectedGun].currentMag = gunList[selectedGun].numOfMag;
        }

        // save gun original pos //
        gunOrigPos = gunPos.transform.localPosition;
    }

    void Update()
    {
        if (GameManager.instance.activeMenu == null)
        {
            Movement();
            SelectGun();
            if (Input.GetButton("Shoot") && !isShooting && gunList.Count > 0)
            {
                StartCoroutine(Shoot());
            }
            if (Input.GetButton("Reload") && !isReloading && gunList[selectedGun].numOfMag > 0 && gunList.Count > 0)
            {
                StartCoroutine(Reload());
            }
            if (Input.GetButton("Grenade") && !isThrowing && currGrenadeAmount > 0)
            {
                StartCoroutine(ThrowGrenade());
            }
            if (Input.GetButtonDown("Crouch") && canCrouch && controller.isGrounded)
            {
                StartCoroutine(CrouchStand());
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

        if (isCrouching)
        {
            move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
            controller.Move(move * Time.deltaTime * crouchSpeed);
        }
        else
        {
            move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
            controller.Move(move * Time.deltaTime * playerSpeed);
        }

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
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            playerSpeed /= sprintMod;
        }
    }

    IEnumerator CrouchStand()
    {
        //prevent player from standing while crouched underneath objects
        if (isCrouching && Physics.Raycast(mainCamera.transform.position, Vector3.up, 1f))
        {
            yield break;
        }

        canCrouch = false;
        float timeElapsed = 0;
        float targetHeight = isCrouching ? standinghHeight : crouchHeight;
        float currentHeight = controller.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = controller.center;

        while (timeElapsed < timeToCrouch)
        {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //prevent weird behaviors
        controller.height = targetHeight;
        controller.center = targetCenter;

        isCrouching = !isCrouching;

        canCrouch = true;
    }

    IEnumerator playSteps()
    {

        stepIsPlaying = true;

        if (isCrouching)
        {
            aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol / 5);
        }
        else
        {
            aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        }

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

        if (gunList[selectedGun].currentAmmo > 0 && !isReloading)
        {
            gunList[selectedGun].currentAmmo--;
            aud.PlayOneShot(gunList[selectedGun].gunShotAud, gunList[selectedGun].gunShotAudVol);

            RaycastHit hit;

            int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
            GameObject muzzleFlash = ObjectPooler.instance.SpawnFromPool("MuzzleFlash", muzzle.position, muzzle.transform.rotation);

            // Move muzzle flash under the map after a delay.
            StartCoroutine(MoveObjectUnderMap(muzzleFlash, 0.01f));

            if (gunList[selectedGun].isTranqGun)
            {
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance, enemyLayerMask)) // PlayerMask
                {
                    ISleep sleepable = hit.collider.GetComponent<ISleep>();
                    if (sleepable != null)
                    {
                        sleepable.ReduceStamina(shootDamage);
                    }
                }
            }
            else
            {
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance, enemyLayerMask)) // PlayerMask
                {
                    IDamage damageable = hit.collider.GetComponent<IDamage>();
                    if (damageable != null)
                    {
                        damageable.takeDamage(shootDamage);
                    }
                }
            }

            GameObject hitEffect = ObjectPooler.instance.SpawnFromPool("HitEffect", hit.point, Quaternion.LookRotation(hit.normal));

            // Move hit effect under the map after a delay.
            StartCoroutine(MoveObjectUnderMap(hitEffect, 0.1f));


            yield return new WaitForSeconds(gunList[selectedGun].shootRate);
        }

        isShooting = false;






        //    isShooting = true;

        //    if(gunList[selectedGun].currentAmmo > 0 && !isReloading)
        //    {
        //        gunList[selectedGun].currentAmmo--;
        //        aud.PlayOneShot(gunList[selectedGun].gunShotAud, gunList[selectedGun].gunShotAudVol);

        //        RaycastHit hit;

        //        int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        //        GameObject muzzleFlash = Instantiate(gunList[selectedGun].muzzleFlash, muzzle.position, muzzle.transform.rotation);
        //        yield return new WaitForSeconds(0.01f);
        //        Destroy(muzzleFlash);

        //        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance, enemyLayerMask)) // PlayerMask
        //        {
        //            IDamage damageable = hit.collider.GetComponent<IDamage>();
        //            if (damageable != null)
        //            {
        //                damageable.takeDamage(shootDamage);
        //            }

        //            //GameObject hitEffect = Instantiate(gunList[selectedGun].hitEffect, hit.point, gunList[selectedGun].hitEffect.transform.rotation);
        //            //yield return new WaitForSeconds(gunList[selectedGun].shootRate);
        //            //Destroy(hitEffect);
        //        }

        //        //UpdatePlayerUI();
        //    }
        //    yield return new WaitForSeconds(gunList[selectedGun].shootRate);

        //    isShooting = false;
    }

    // Coroutine to move an object under the map and deactivate it after a delay.
    IEnumerator MoveObjectUnderMap(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Arbitrary position under the map.
        obj.transform.position = new Vector3(0, -1000, 0);
        obj.SetActive(false);
    }

    IEnumerator Reload()
    {
        if (gunList[selectedGun].currentMag > 0)
        {
            isReloading = true;

            reloadTime = gunList[selectedGun].reloadSpeed;

            // Store the original pitch
            float originalPitch = aud.pitch;

            // Increase the pitch to make the sound play faster
            aud.pitch = 1.8f;  // Change this value to adjust the speed

            aud.PlayOneShot(gunList[selectedGun].reloadAud);

            StartCoroutine(MoveGunDownAndUp());

            yield return new WaitForSeconds(gunList[selectedGun].reloadSpeed);

            //decrease mag amount
            gunList[selectedGun].currentMag--;
            //set current to mag max size
            gunList[selectedGun].currentAmmo = gunList[selectedGun].magazineSize;

            // Reset the pitch back to the original value
            aud.pitch = originalPitch;

            isReloading = false;
        }
    }

    IEnumerator MoveGunDownAndUp()
    {
        // Animate lowering the gun
        float startTime = Time.time;
        while (Time.time < startTime + reloadTime / 2)
        {
            float t = (Time.time - startTime) / (reloadTime / 2);  // normalized time between 0 and 1
            gunPos.transform.localPosition = Vector3.Lerp(gunOrigPos, gunLowerPos, t);
            yield return null;  // wait until the next frame
        }

        // Animate raising the gun
        startTime = Time.time;
        while (Time.time < startTime + reloadTime / 2)
        {
            float t = (Time.time - startTime) / (reloadTime / 2);  // normalized time between 0 and 1
            gunPos.transform.localPosition = Vector3.Lerp(gunLowerPos, gunOrigPos, t);
            yield return null;  // wait until the next frame
        }

        // Make sure the gun ends up in exactly the right place
        gunPos.transform.localPosition = gunOrigPos;
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
            GameManager.instance.YouLose();
        }
    }

    IEnumerator DamageFlash()
    {
        GameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.playerDamageFlash.SetActive(false);
    }

    public void SpawnPlayer()
    {
        controller.enabled = false;
        HP = maxHP;
        UpdatePlayerUI();
        transform.position = GameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void GunPickUp(GunStats gunStat)
    {
        gunList.Add(gunStat);

        ChangeGunStats(gunStat);
        selectedGun = gunList.Count - 1;
        gunList[selectedGun].currentMag = gunList[selectedGun].numOfMag;
        gunList[selectedGun].currentAmmo = gunList[selectedGun].magazineSize;

        aud.PlayOneShot(gunStat.pickupAud);

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
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
        }

        if (gunList.Count > 0)
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
        GameManager.instance.playerHpBar.fillAmount = (float)HP / maxHP;
        if (gunList.Count > 0)
        {
            GameManager.instance.playerCurrentAmmo.text = $"{gunList[selectedGun].currentAmmo}";
            GameManager.instance.playerMagazineAmount.text = $"{gunList[selectedGun].currentMag}";
            GameManager.instance.playerMagazineSize.text = $"/{gunList[selectedGun].magazineSize}";
        }
    }
}
