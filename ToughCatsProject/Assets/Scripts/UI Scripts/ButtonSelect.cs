using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelect : MonoBehaviour
{
    public static Button primaryButton;

    private void Start()
    {
        primaryButton.Select();
    }

}
