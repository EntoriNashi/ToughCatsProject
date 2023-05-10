using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyType;
    [SerializeField] [Range(0, 5)] int enemyCount;
    [SerializeField] [Range(0, 10)] int secBetweenSpawns;

    IEnumerator SpawnEnemy()
    {
        for(int spawnCount = 0; spawnCount < enemyCount; spawnCount++)
        {
            Instantiate(enemyType);
            yield return new WaitForSeconds(secBetweenSpawns);
        }
    }

    Vector3 RandomPos()
    {
        float x = Random.Range(-10, 10);
        float y = 1;
        float z = Random.Range(-10, 10);

        Vector3 rngPos = new Vector3(x, y, z);

        return rngPos;
    }
}
