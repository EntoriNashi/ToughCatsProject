using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] MeshFilter gunModel;
    [SerializeField] MeshRenderer gunMaterial;

    [Header("----- Player Attributes -----")]
    [SerializeField][Range(1, 10)] int HP;
    [SerializeField][Range(1, 10)] int maxHP;
    [SerializeField][Range(1, 5)] float playerSpeed;
    [SerializeField][Range(2, 5)] float sprintMod;
    [SerializeField][Range(10, 50)] float jumpHeight;
    [SerializeField][Range(9.81f, 20)] float gravityValue;
    [SerializeField][Range(1, 3)] int jumpMax;

    [Header("----- Weapon Attributes -----")]
    public List<GunStats> gunList = new List<GunStats>();
    [SerializeField][Range(2, 300)] int shootDistance;
    [SerializeField][Range(0.1f, 3)] float shootRate;
    [SerializeField][Range(1, 10)] int shootDamage;

    private Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private int jumpedTimes;
    private bool isSprinting;
    private bool isShooting;
    private int selectedGun;

    private void Start()
    {
        SpawnPlayer();
        HP = maxHP;
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
        }

        Sprint();        
    }

    private void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpMax)
        {
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

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;

        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f,0.5f)), out hit, shootDistance))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();
            if(damageable != null)
            {
                damageable.takeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    public void Heal(int amount)
    {
        HP += amount;
    }

    public void takeDamage(int damage)
    {
        HP -= damage;

        if(HP <= 0)
        {
            //kill player and respawn
            gameManager.instance.youLose();
        }
    }

    public void SpawnPlayer()
    {
        controller.enabled = false;
        HP = maxHP;
        transform.position = gameManager.instance.playerSpawnPOS.transform.position;
        controller.enabled = true;
    }

    public void GunPickUp(GunStats gunStat)
    {
        gunList.Add(gunStat);

        ChangeGunStats(gunStat);
        selectedGun = gunList.Count - 1;
    }

    private void ChangeGunStats(GunStats gunStat)
    {
        shootDamage = gunStat.shootDamage;
        shootRate = gunStat.shootRate;
        shootDistance = gunStat.shootDistance;

        gunModel.mesh = gunStat.model.GetComponent<MeshFilter>().sharedMesh;
        gunMaterial.material = gunStat.model.GetComponent<MeshRenderer>().sharedMaterial;
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
}
