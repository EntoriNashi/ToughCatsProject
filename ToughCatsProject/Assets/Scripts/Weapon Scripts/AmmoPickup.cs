using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.player != null)
            {
                GameManager.instance.playerScript.AddMag();
            }
            Destroy(gameObject);
        }
    }
}
