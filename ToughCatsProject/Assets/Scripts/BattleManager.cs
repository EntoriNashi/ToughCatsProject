using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private int enemiesInBattle = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void EnemyStartedBattle()
    {
        enemiesInBattle++;
        if (enemiesInBattle == 1) // if it's the first enemy starting a battle
        {
            AudioManager.instance.SwapTrackString("Battle");
        }
    }

    public void EnemyStoppedBattle()
    {
        enemiesInBattle--;
        if (enemiesInBattle == 0) // if it's the last enemy stopping the battle
        {
            AudioManager.instance.SwapTrackString("Ambience1");
        }
    }

    public bool IsInBattle()
    {
        return enemiesInBattle > 0;
    }
}
