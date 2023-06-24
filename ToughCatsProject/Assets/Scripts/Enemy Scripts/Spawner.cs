using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectToSpawn;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] int numberToSpawn;
    [SerializeField] bool IsUsingPlayerDetected;


    int numberSpawned;
    bool playerInRange;
    bool isSpawning;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsUsingPlayerDetected && GameManager.instance.IsPlayerDetected)
        {
            //Debug.Log("Spawner is using player detection");
            if (playerInRange && !isSpawning && numberSpawned < numberToSpawn)
            {
                StartCoroutine(spawn());
            }
        }
        else if (!IsUsingPlayerDetected && playerInRange && !isSpawning && numberSpawned < numberToSpawn)
        {
            //Debug.Log("Spawner is NOT using player detection");
            StartCoroutine(spawn());
        }
    }

    IEnumerator spawn()
    {
        isSpawning = true;

        Instantiate(objectToSpawn[Random.Range(0, objectToSpawn.Length)], spawnPos[Random.Range(0, spawnPos.Length)].position, transform.rotation);
        numberSpawned++;
        yield return new WaitForSeconds(timeBetweenSpawns);

        isSpawning = false;
    }
}
