using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwap : MonoBehaviour
{
    public string newTrack;

    private bool playerInsideTrigger = false;

    private void Update()
    {
        if (playerInsideTrigger && AudioManager.instance.IsEnemyAttacking())
        {
            AudioManager.instance.SwapTrackString("Battle");
        }

        Debug.Log($"is enemy attacking -> {AudioManager.instance.IsEnemyAttacking()}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !AudioManager.instance.IsEnemyAttacking())
        {
            playerInsideTrigger = true;
            newTrack = "Ambience2";
            AudioManager.instance.SwapTrackString(newTrack);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = false;
            if (!AudioManager.instance.IsEnemyAttacking())
            {
                AudioManager.instance.ReturnToDefault();
            }
        }
        //if (other.CompareTag("Player") && !AudioManager.instance.IsEnemyAttacking())
        //{
        //    AudioManager.instance.ReturnToDefault();
        //}
    }
}
