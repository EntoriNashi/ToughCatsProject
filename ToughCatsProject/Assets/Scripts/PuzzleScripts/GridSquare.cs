using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image HooverImage;
    public Image ActiveImage;
    public Image NormalImage;
    public List<Sprite> NormalImages;

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }

    private void Start()
    {
        Selected = false;
        SquareOccupied = false;
    }

    public bool CanWeUseThisSquare()
    {
        return HooverImage.gameObject.activeSelf;
    }

    public void PlacePieceOnBoard()
    {
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        HooverImage.gameObject.SetActive(false);
        ActiveImage.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;
    }

    public void DeactivateSquare()
    {
        NormalImage.gameObject.SetActive(true);
        ActiveImage.gameObject.SetActive(false);
        Selected = false;
        SquareOccupied = false;
    }

    public void SetImage(bool setFirstImage)
    {
        NormalImage.GetComponent<Image>().sprite = setFirstImage ? NormalImages[1] : NormalImages[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            Selected = true;
            HooverImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<PieceSquare>() != null)
        {
            collision.GetComponent<PieceSquare>().SetOccupied();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;
        if (SquareOccupied == false)
        {
            HooverImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<PieceSquare>() != null)
        {
            collision.GetComponent<PieceSquare>().SetOccupied();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SquareOccupied == false)
        {
            Selected = false;
            HooverImage.gameObject.SetActive(false);
        }
        else if (collision.GetComponent<PieceSquare>() != null)
        {
            collision.GetComponent<PieceSquare>().UnSetOccupied();
        }
    }
}
