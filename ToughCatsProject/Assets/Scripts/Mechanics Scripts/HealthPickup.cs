using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // variables //
    [SerializeField] int healthKit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.player != null)
            {
                GameManager.instance.playerScript.Heal(healthKit);
            }
            Destroy(gameObject);
        }
    }
}
