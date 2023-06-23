using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] GunStats gun;

    MeshFilter model;
    MeshRenderer material;

    private void Start()
    {
        model = gun.model.GetComponent<MeshFilter>();
        material = gun.model.GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerScript.GunPickUp(gun);

            if (this.gameObject.CompareTag("Pistol"))
            {
                GameManager.instance.ShowPistolText();
            }
            else if (this.gameObject.CompareTag("Rifle"))
            {
                GameManager.instance.ShowRifleText();
            }

            Destroy(gameObject);
        }
    }
}
