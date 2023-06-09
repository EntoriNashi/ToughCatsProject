using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject SquarePieceImage;
    public PieceData pieceData;

    public Vector3 PieceSelectedScale;
    public Vector2 offset = new Vector2(0f, 0f);
    public float RotateRepeatDelay;

    [HideInInspector]
    public PieceData CurrentPieceData;

    public int TotalSquareNumber { get; set; }

    private List<GameObject> _currentPiece = new List<GameObject>();
    private Vector3 pieceStartScale;
    private RectTransform _transform;
    private bool isPieceDraggable = true;
    private Canvas _canvas;
    private Vector3 startPos;
    private bool pieceActive = true;

    public void Awake()
    {
        pieceStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        isPieceDraggable = true;
        startPos = transform.localPosition;
        pieceActive = true;
    }

    private void OnEnable()
    {
        PuzzleEvents.MovePieceToStartPosition += MovePieceToStartPosition;
    }

    private void OnDisable()
    {
        PuzzleEvents.MovePieceToStartPosition -= MovePieceToStartPosition;
    }



    private void Start()
    {

    }

    public bool IsOnStartPosition()
    {
        return _transform.localPosition == startPos;
    }

    public bool IsAnyOfPieceSquareActive()
    {
        foreach (var square in _currentPiece)
        {
            if (square.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    public void DeactivatePiece()
    {
        if (pieceActive)
        {
            foreach (var square in _currentPiece)
            {
                square?.GetComponent<PieceSquare>().DeactivatePiece();
            }
        }

        pieceActive = false;
    }

    public void ActivatePiece()
    {
        if (!pieceActive)
        {
            foreach (var square in _currentPiece)
            {
                square?.GetComponent<PieceSquare>().ActivePiece();
            }
        }

        pieceActive = true;
    }

    public void RequestNewPiece(PieceData pieceData)
    {
        transform.localPosition = startPos;
        CreatePiece(pieceData);
    }

    public void CreatePiece(PieceData pieceData)
    {
        CurrentPieceData = pieceData;
        TotalSquareNumber = GetNumberOfSquares(pieceData);

        while (_currentPiece.Count <= TotalSquareNumber)
        {
            _currentPiece.Add(Instantiate(SquarePieceImage, transform));
        }

        foreach (var square in _currentPiece)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = SquarePieceImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x, squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;

        for (var row = 0; row < pieceData.rows; row++)
        {
            for (var column = 0; column < pieceData.columns; column++)
            {
                if (pieceData.board[row].column[column])
                {
                    _currentPiece[currentIndexInList].SetActive(true);
                    _currentPiece[currentIndexInList].GetComponent<RectTransform>().localPosition = new Vector2(GetXPositionForPieceSquare(pieceData, column, moveDistance), GetYPositionForPieceSquare(pieceData, row, moveDistance));

                    currentIndexInList++;
                }
            }
        }
    }

    private float GetXPositionForPieceSquare(PieceData pieceData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;
        if (pieceData.columns > 1)
        {
            float startXPos;
            if (pieceData.columns % 2 != 0)
                startXPos = (pieceData.columns / 2) * moveDistance.x * -1;
            else
                startXPos = ((pieceData.columns / 2) - 1) * moveDistance.x * -1 - moveDistance.x / 2;
            shiftOnX = startXPos + column * moveDistance.x;

        }
        return shiftOnX;
    }

    private float GetYPositionForPieceSquare(PieceData pieceeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;
        if (pieceeData.rows > 1)
        {
            float startYPos;
            if (pieceeData.rows % 2 != 0)
                startYPos = (pieceeData.rows / 2) * moveDistance.y;
            else
                startYPos = ((pieceeData.rows / 2) - 1) * moveDistance.y + moveDistance.y / 2;
            shiftOnY = startYPos - row * moveDistance.y;
        }
        return shiftOnY;
    }

    private int GetNumberOfSquares(PieceData pieceData)
    {
        int number = 0;

        foreach (var rowData in pieceData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                {
                    number++;
                }
            }
        }
        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RotatePiece();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            this.GetComponent<RectTransform>().localScale = pieceStartScale;
            PuzzleEvents.CheckIfPieceCanBePlaced();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = PieceSelectedScale;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    private void MovePieceToStartPosition()
    {
        _transform.localPosition = startPos;
    }

    public void RotatePiece()
    {
        Vector3 rotate = new Vector3(0, 0, 90);
        this.transform.Rotate(rotate);
    }
}
