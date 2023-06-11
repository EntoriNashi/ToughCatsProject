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
        ISleep damagable = other.GetComponent<ISleep>();

        if (damagable != null)
        {
            damagable.ReduceStamina(damage);
        }
    }
}
