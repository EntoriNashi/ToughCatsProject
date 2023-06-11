using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public void PuzzleExit()
    {
        GameManager.instance.UnpausedState();
    }
    public void PuzzleRestart()
    {

    }
    public void PuzzleUndo()
    {

    }
}
