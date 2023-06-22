using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorCinematic : MonoBehaviour
{
    [SerializeField] float RotationSpeed;
    [SerializeField] float WalkSpeed;
    GameObject player;
    GameObject door;
    bool isLevelEnding;
    bool isFacingDoor1;
    bool isFacingDoor2;
    bool isAtDoor;
    bool isFacingCenter;
    bool isAtCenter;
    bool isLookAtDoor1Set;
    bool isLookAtDoor2Set;
    bool isLookAtCenterSet;
    Quaternion lookAtDoor1;
    Quaternion lookAtDoor2;
    Quaternion lookAtCenter;

    private void Start()
    {
        player = GameManager.instance.player;
        door = GameObject.FindGameObjectWithTag("Goal Door");
        isLevelEnding = false;
    }
    private void Update()
    {
        if(isLevelEnding)
        {
            
            if (!isFacingDoor1)
            {
                if (!isLookAtDoor1Set)
                {
                    Vector3 doorDist = door.transform.position - player.transform.position;
                    doorDist.y = 0;
                    lookAtDoor1 = Quaternion.LookRotation(doorDist);
                    isLookAtDoor1Set = true;
                }
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookAtDoor1, RotationSpeed * Time.deltaTime);
                if (Quaternion.Angle(lookAtDoor1, player.transform.rotation) < .5)
                {
                    isFacingDoor1 = true;
                }
                
            }
            else if (!isAtDoor)
            {
                player.transform.position = Vector3.Lerp(player.transform.position, new Vector3(door.transform.position.x, 1, door.transform.position.z), WalkSpeed * Time.deltaTime);
                if (Vector3.Distance(player.transform.position, new Vector3(door.transform.position.x, 1, door.transform.position.z)) < .5)
                {
                    isAtDoor = true;
                }
            }
            else if (!isFacingCenter)
            {
                if (!isLookAtCenterSet)
                {
                    Vector3 centerDist = this.transform.position - player.transform.position;
                    centerDist.y = 0;
                    lookAtCenter = Quaternion.LookRotation(centerDist);
                    isLookAtCenterSet = true;

                }
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookAtCenter, RotationSpeed * Time.deltaTime);
                if (Quaternion.Angle(lookAtCenter, player.transform.rotation) < .5)
                {
                    isFacingCenter = true;
                }

            }
            else if (!isAtCenter)
            {
                player.transform.position = Vector3.Lerp(player.transform.position, new Vector3(this.transform.position.x, 1, this.transform.position.z), WalkSpeed * Time.deltaTime);
                if (Vector3.Distance(player.transform.position, new Vector3(this.transform.position.x, 1, this.transform.position.z)) < .5)
                {
                    isAtCenter = true;
                }
            }
            else if (!isFacingDoor2)
            {
                if (!isLookAtDoor2Set)
                {
                    Vector3 doorDist = door.transform.position - player.transform.position;
                    doorDist.y = 0;
                    lookAtDoor2 = Quaternion.LookRotation(doorDist);
                }
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookAtDoor2, RotationSpeed * Time.deltaTime);
                if (Quaternion.Angle(lookAtDoor2, player.transform.rotation) < .5)
                {
                    isFacingDoor2 = true;
                }

            }
            else
            {
                door.GetComponent<Animator>().SetBool("character_nearby", false);
                StartCoroutine(EndDelay());
            }
        }
    }

    public void StartLevelEnd()
    {
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponentInChildren<CameraController>().enabled = false;
        GameManager.instance.isInCinematic = true;
        isLevelEnding = true;
    }
    IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(3);
        GameManager.instance.isInCinematic = false;
        SceneManager.LoadScene(2);
    }
}
