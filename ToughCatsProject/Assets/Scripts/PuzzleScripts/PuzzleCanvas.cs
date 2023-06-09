using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        this.GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
