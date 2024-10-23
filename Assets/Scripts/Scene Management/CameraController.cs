using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : Singleton<CameraController>
{
    private CinemachineVirtualCamera vcam;

    private void Start()
    {
        SetPlayerCameraFollow();
    }

    public void SetPlayerCameraFollow()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        vcam.Follow = PlayerController.Instance.transform;
    }
}
