using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButtons : MonoBehaviour
{
    public GameObject Storage;
    public GameObject PuzzleGrid;

    public void PuzzleExit()
    {
        GameManager.instance.UnpausedState();
    }
    public void PuzzleRestart()
    {
        Storage.GetComponent<PieceStorage>().ResetPieces();
        PuzzleGrid.GetComponent<BoardGrid>().ResetGrid();
    }
    public void PuzzleUndo()
    {

    }
}
