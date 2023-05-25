using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Animator anim;

    [SerializeField] bool IsRoomEnterence;
    [SerializeField] bool IsRoomExit;
    [SerializeField] GameObject UnarmedSpawnPos;
    [SerializeField] GameObject Unarmed;

    GameObject currentUnarmed;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            anim.SetBool("character_nearby", true);
            if(IsRoomEnterence)
            {
                gameManager.instance.unarmed = Instantiate(Unarmed, UnarmedSpawnPos.transform.position, UnarmedSpawnPos.transform.rotation);
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("character_nearby", false);
            if(IsRoomExit)
            {
                currentUnarmed = gameManager.instance.unarmed;
                gameManager.instance.unarmed = null;
                Destroy(currentUnarmed);
            }
        }
    }
}
