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

    [Header("----- Enemy Weapon -----")]
    [Range(2,300)][SerializeField] int shootDist;
    [Range(0.1f, 3)][SerializeField] float shootRate;
    [SerializeField] GameObject bullet;

    Color colorOrig;
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
       agent.SetDestination(gameManager.instance.player.transform.position);
        if (!isShooting)
            StartCoroutine(shoot());
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        StartCoroutine(flashColor());

        if (HP <= 0) 
        {
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
}
