using UnityEngine;
using UnityEngine.UI;

public class PieceSquare : MonoBehaviour
{
    public Image occupiedImage;
    private void Start()
    {
        occupiedImage.gameObject.SetActive(false);
    }
    public void DeactivatePiece()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(false);
    }

    public void ActivePiece()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(true);
    }

    public void SetOccupied()
    {
        occupiedImage.gameObject.SetActive(true);
    }

    public void UnSetOccupied()
    {
        occupiedImage.gameObject.SetActive(false);
    }
}
