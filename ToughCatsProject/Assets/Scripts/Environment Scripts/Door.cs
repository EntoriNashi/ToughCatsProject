using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Animator anim;
    public GameObject Puzzle;

    public bool IsGoal;
    [SerializeField] bool IsRoomEnterence;
    [SerializeField] bool IsRoomExit;
    [SerializeField] GameObject UnarmedSpawnPos;
    [SerializeField] GameObject Unarmed;
    public bool IsLocked { get; set; }
    GameObject attachedPuzzle;
    bool hasSpawnedUnarmed;

    private void Awake()
    {
        if(Puzzle != null)
        {
            IsLocked = true;
            attachedPuzzle = Instantiate(Puzzle); //add parent as UI?
            attachedPuzzle.SetActive(false);
            attachedPuzzle.GetComponentInChildren<BoardGrid>().SetAttachedDoor(this.gameObject);
        }
        if(IsGoal)
        {
            gameObject.tag = "Goal Door";
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") && !IsLocked)
        {
            anim.SetBool("character_nearby", true);
            if (IsRoomEnterence && !hasSpawnedUnarmed)
            {
                GameManager.instance.unarmed = Instantiate(Unarmed, UnarmedSpawnPos.transform.position, UnarmedSpawnPos.transform.rotation);

            }
        }
        else if (other.CompareTag("Player") && IsLocked)
        {
            GameManager.instance.PuzzleActivate(attachedPuzzle);
            GameManager.instance.isInPuzzle = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsGoal)
        {
            if (other.CompareTag("Player"))
            {
                anim.SetBool("character_nearby", false);
            }
        }
    }
}
