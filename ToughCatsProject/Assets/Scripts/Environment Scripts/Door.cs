using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Animator anim;
    public GameObject Puzzle;

    [SerializeField] bool IsGoal;
    [SerializeField] bool IsRoomEnterence;
    [SerializeField] bool IsRoomExit;
    [SerializeField] GameObject UnarmedSpawnPos;
    [SerializeField] GameObject Unarmed;
    public bool IsLocked { get; set; }
    GameObject currentUnarmed;
    GameObject attachedPuzzle;

    private void Awake()
    {
        if(Puzzle != null)
        {
            IsLocked = true;
            attachedPuzzle = Instantiate(Puzzle); //add parent as UI?
            attachedPuzzle.SetActive(false);
            attachedPuzzle.GetComponentInChildren<BoardGrid>().SetAttachedDoor(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") && !IsLocked)
        {
            anim.SetBool("character_nearby", true);
            if (IsRoomEnterence)
            {
                GameManager.instance.unarmed = Instantiate(Unarmed, UnarmedSpawnPos.transform.position, UnarmedSpawnPos.transform.rotation);

            }
            if (IsGoal)
            {
                GameManager.instance.UpdateWinCondition();
            }
        }
        else if (other.CompareTag("Player") && IsLocked)
        {
            GameManager.instance.PuzzleActivate(attachedPuzzle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("character_nearby", false);
            if(IsRoomExit)
            {
                currentUnarmed = GameManager.instance.unarmed;
                GameManager.instance.unarmed = null;
                Destroy(currentUnarmed);
            }
        }
    }
}
