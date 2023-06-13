using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private MeshRenderer meshRenderer;

    private void OnTriggerEnter(Collider other)
    {
        // if player touches object -> make changes and destroy object //
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.player != null)
            {
                audioSource.Play();
                GameManager.instance.playerScript.AddMag();
            }
            // make object invisible before destroying so audio can play //
            meshRenderer.enabled = false;
            StartCoroutine(Destroy());
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
