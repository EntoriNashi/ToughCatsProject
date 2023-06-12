using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensHor;
    [SerializeField] int sensVert;
    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;

    [SerializeField] bool invertY;

    float xRotation;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if(!GameManager.instance.GetIsPaused())
        {
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVert;
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHor;

            if (invertY)
            {
                xRotation += mouseY;
            }
            else
            {
                xRotation -= mouseY;
            }

            xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }
}
