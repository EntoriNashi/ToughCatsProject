using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] int damage;

    private void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamage damagable = other.GetComponent<IDamage>();

        if (damagable != null)
        {
            damagable.takeDamage(damage);
        }
    }
}
