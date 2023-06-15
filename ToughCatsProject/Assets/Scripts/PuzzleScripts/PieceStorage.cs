using System.Collections.Generic;
using UnityEngine;

public class PieceStorage : MonoBehaviour
{
    public List<PuzzlePiece> pieceList;

    void Start()
    {
        foreach (var piece in pieceList)
        {
            piece.CreatePiece(piece.pieceData);
        }
    }

    public PuzzlePiece GetCurrentSelectedPiece()
    {
        foreach (var piece in pieceList)
        {
            if (piece.IsOnStartPosition() == false && piece.IsAnyOfPieceSquareActive())
            {

                return piece;

            }
        }

        //Debug.LogError("There is no Piece Selected!");
        return null;
    }

    public void ResetPieces()
    {
        foreach (var piece in pieceList)
        {
            piece.GetComponent<PuzzlePiece>().ResetPiece();
        }
    }


}
