using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;
    [SerializeField] AudioSource aud;
    
    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    private int maxHP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int viewCone;
    [SerializeField] int roamDist;
    [SerializeField] int roamWaitTime;
    [SerializeField] int animTransSpeed;
    [SerializeField] bool armed;

    [Header("----- Health Bar -----")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthLeft;

    [Header("----- Enemy Weapon -----")]
    [Range(2,300)][SerializeField] int shootDist;
    [Range(0.1f, 3)][SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] int shootAngle;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] audSteps;
    [SerializeField] AudioClip[] audHit;
    [SerializeField] AudioClip audShot;
    [SerializeField][Range(0, 1)] float audStepsVol;
    [SerializeField][Range(0, 1)] float audHitVol;
    [SerializeField][Range(0, 1)] float audShotVol;

    Color colorOrig;
    bool isShooting;
    bool destinationChosen;
    bool playerInRange;
    bool alerting;
    Vector3 playerDir;
    Vector3 startingPos;
    Vector3 unarmedDir;
    float angleToPlayer;
    float stoppingDistOrg;
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        stoppingDistOrg = agent.stoppingDistance;
        startingPos = transform.position;
        GameManager.instance.UpdateEnemyCout(1);

        // health bar setup //
        maxHP = HP;
        healthSlider.maxValue = maxHP;
        healthSlider.value = HP;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            speed = Mathf.Lerp(speed, agent.velocity.normalized.magnitude, Time.deltaTime * animTransSpeed);
            animator.SetFloat("Speed", speed);
            if (playerInRange && !canSeePlayer())
            {
                StartCoroutine(roam());
            }
            else if (agent.destination != GameManager.instance.player.transform.position)
            {
                StartCoroutine(roam());
            }
        }
    }

    IEnumerator roam()
    {
        if(!destinationChosen && agent.remainingDistance < .05f)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamWaitTime);
            destinationChosen = false;

            Vector3 randPos = UnityEngine.Random.insideUnitSphere * roamDist;
            randPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randPos, out hit, roamDist, 1);

            agent.SetDestination(hit.position);
        }
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;

        // health bar update //
        healthSlider.value = HP;
        healthLeft.fillAmount = (float)HP / maxHP;

        if (HP <= 0) 
        {
            GameManager.instance.EnemyDefeatedCounter();
            animator.SetBool("Dead", true);
            agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            StopAllCoroutines();
            Destroy(gameObject, 10);

            // hide health bar //
            healthSlider.gameObject.SetActive(false);
        }
        else
        {
            animator.SetTrigger("Damage");
            agent.SetDestination(GameManager.instance.player.transform.position);
            StartCoroutine(flashColor());
            playerInRange = true;
        }
    }
    
    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        if (GameManager.instance.unarmed != null)
        {
            unarmedDir = GameManager.instance.unarmed.transform.position - headPos.position;
            Debug.DrawRay(headPos.position, unarmedDir);
        }
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir);
        Debug.Log(angleToPlayer);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone || hit.collider.CompareTag("Player Alert"))
            {
                agent.stoppingDistance = stoppingDistOrg;
                agent.SetDestination(GameManager.instance.player.transform.position);
 
                if (agent.remainingDistance <= agent.stoppingDistance)
                    facePlayer();
                if (!isShooting && angleToPlayer <= shootAngle)
                    if (armed)
                        StartCoroutine(shoot());
                    else
                        StartCoroutine(alert());
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    IEnumerator alert()
    {
        alerting = true;
        GameManager.instance.IsPlayerDetected = true;
        gameObject.tag = "Player Alert";
        yield return new WaitForSeconds(3);
        alerting = false;
        GameManager.instance.IsPlayerDetected = false;
        gameObject.tag = "Unarmed";
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    void step()
    {
        aud.PlayOneShot(audSteps[UnityEngine.Random.Range(0, audSteps.Length)],audStepsVol);
    }
    void hit()
    {
        aud.PlayOneShot(audHit[UnityEngine.Random.Range(0,audHit.Length)],audHitVol);
    }
    void gunShot()
    {
        aud.PlayOneShot(audShot, audShotVol);
    }

    public void playerEnteredRange()
    {
        playerInRange = true;
    }

    public void playerExitedRange()
    {
        playerInRange = false;
        agent.stoppingDistance = 0;
    }
}
