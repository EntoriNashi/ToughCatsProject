using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] int timer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float radius;
    [SerializeField] private int damage;

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
        // check if enemies or player is nearby //
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        Debug.Log(distanceToPlayer);

        if (distanceToPlayer <= radius)
        {
            GameManager.instance.playerScript.takeDamage(damage);
        }
    }
}
