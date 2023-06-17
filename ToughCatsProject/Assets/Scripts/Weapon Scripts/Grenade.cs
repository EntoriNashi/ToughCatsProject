using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] int timer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float radius;
    [SerializeField] private int damageToPlayer;
    [SerializeField] private int damageToEnemy;


    private List<GameObject> enemiesList = new List<GameObject>();

    IEnumerator Start()
    {
        yield return new WaitForSeconds(timer);

        Instantiate(explosion, transform.position, explosion.transform.rotation);

        audioSource.Play();

        Damage();

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    private void Damage()
    {
        // check if player is nearby //
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= radius)
            GameManager.instance.playerScript.takeDamage(damageToPlayer);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy <= radius)
                enemy.GetComponent<EnemyAI>().takeDamage(damageToEnemy);

            Debug.Log(distanceToEnemy);
        }
    }
}
