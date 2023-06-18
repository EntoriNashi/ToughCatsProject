using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButtons : MonoBehaviour
{
    public GameObject Storage;
    public GameObject PuzzleGrid;
    public GameObject PuzzlePieces;
    public GameObject HelpMenu;
    

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
    public void PuzzleHelp()
    {
        HelpMenu.SetActive(true);
        PuzzleGrid.SetActive(false);
        PuzzlePieces.SetActive(false);
    }
    public void CloseHelp()
    {
        HelpMenu.SetActive(false);
        PuzzleGrid.SetActive(true);
        PuzzlePieces.SetActive(true);
    }
}
