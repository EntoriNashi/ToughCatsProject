using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int viewCone;

    [Header("----- Enemy Weapon -----")]
    [Range(2,300)][SerializeField] int shootDist;
    [Range(0.1f, 3)][SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] int shootAngle;

    Color colorOrig;
    bool isShooting;
    Vector3 playerDir;
    bool playerInRange;
    float angleToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        agent.SetDestination(gameManager.instance.player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && canSeePlayer())
        {
            //StartCoroutine(shoot());
        }
        else
        {
            StartCoroutine(antiIdleEnemy());
        }
        
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        StartCoroutine(flashColor());

        if (HP <= 0) 
        {
            gameManager.instance.enemyDefeatedCounter();
            Destroy(gameObject);
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
        Instantiate(bullet,transform.position,transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(transform.position, playerDir);
        Debug.Log(angleToPlayer);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    facePlayer();
                }
                if (!isShooting && angleToPlayer <= shootAngle)
                    StartCoroutine(shoot());
                return true;
            }

        }
        return false;
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; ;
        }
    }

    IEnumerator antiIdleEnemy()
    {
        if(agent.destination != gameManager.instance.player.transform.position)
        {
            yield return new WaitForSeconds(10);
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
    }
}
