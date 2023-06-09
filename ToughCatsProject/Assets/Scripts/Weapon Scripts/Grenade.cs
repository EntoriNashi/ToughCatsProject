using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] int timer;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(timer);

        Instantiate(explosion, transform.position, explosion.transform.rotation);

        Destroy(gameObject);
    }
}
