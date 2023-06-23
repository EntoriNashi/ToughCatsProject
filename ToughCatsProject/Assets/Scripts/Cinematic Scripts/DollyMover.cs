using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyMover : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float speed = 0.2f;
    private CinemachineTrackedDolly dolly;

    void Start()
    {
        dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    void Update()
    {
        dolly.m_PathPosition += speed * Time.deltaTime;
    }
}
