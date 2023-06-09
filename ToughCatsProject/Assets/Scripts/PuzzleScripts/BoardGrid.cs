using System.Collections.Generic;
using UnityEngine;

public class BoardGrid : MonoBehaviour
{
    public PieceStorage pieceStorage;
    public int Columns = 0;
    public int Rows = 0;
    public float SquaresGap = 0.1f;
    public GameObject GridSquare;
    public Vector2 StartPos = new Vector2(0.0f, 0.0f);
    public float SquareScale = 0.5f;
    public float EverySquareOffset = 0.0f;
    public int BoardSquaresFilled;
    public int BoardSquaresRemaining;
    public int TotalBoardSquares;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();




    private void OnEnable()
    {
        PuzzleEvents.CheckIfPieceCanBePlaced += CheckIfPieceCanBePlaced;
    }

    private void OnDisable()
    {
        PuzzleEvents.CheckIfPieceCanBePlaced -= CheckIfPieceCanBePlaced;
    }

    private void Start()
    {
        TotalBoardSquares = Columns * Rows;
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPos();
    }

    private void SpawnGridSquares()
    {
        int square_index = 0;

        for (var row = 0; row < Rows; ++row)
        {
            for (var column = 0; column < Columns; ++column)
            {
                _gridSquares.Add(Instantiate(GridSquare) as GameObject);

                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;

                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(SquareScale, SquareScale, SquareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(square_index % 2 == 0);
                square_index++;
            }
        }
    }

    private void SetGridSquaresPos()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + EverySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + EverySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (column_number + 1 > Columns)
            {
                square_gap_number.x = 0;

                column_number = 0;
                row_number++;
                row_moved = false;
            }

            var pos_x_offset = _offset.x * column_number + (square_gap_number.x * SquaresGap);
            var pos_y_offset = _offset.x * row_number + (square_gap_number.y * SquaresGap);

            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += SquaresGap;
            }

            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += SquaresGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(StartPos.x + pos_x_offset, StartPos.y + pos_y_offset);

            square.GetComponent<RectTransform>().localPosition = new Vector3(StartPos.x + pos_x_offset, StartPos.y + pos_y_offset, 0.0f);

            column_number++;
        }
    }

    private void CheckIfPieceCanBePlaced()
    {
        var squareIndexes = new List<int>();
        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();

            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
            }
        }

        var currentSelectedPiece = pieceStorage.GetCurrentSelectedPiece();
        if (currentSelectedPiece == null)
        {
            return;
        }

        if (currentSelectedPiece.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlacePieceOnBoard();
            }
            currentSelectedPiece.DeactivatePiece();
            UpdateBoardState();
        }
        else
        {
            //PuzzleEvents.MovePieceToStartPosition();
        }

    }

    public void UpdateBoardState()
    {
        BoardSquaresFilled = 0;
        BoardSquaresRemaining = 0;

        foreach (var square in _gridSquares)
        {
            if (square.GetComponent<GridSquare>().SquareOccupied == true)
            {
                    BoardSquaresFilled++;
            }
            else
            {
                BoardSquaresRemaining++;
            }

        }

        if (BoardSquaresFilled == TotalBoardSquares)
        {
            Debug.Log("YOU WIN!");
            //wincondition
        }
        
        
    }
}