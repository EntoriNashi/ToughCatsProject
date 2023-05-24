using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectToSpawn;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] int numberToSpawn;


    int numberSpawned;
    bool playerInRange;
    bool isSpawning;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && !isSpawning && numberSpawned < numberToSpawn)
        {
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
