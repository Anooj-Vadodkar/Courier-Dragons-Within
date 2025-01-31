using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZone : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    private int _startingPriority;

    void Start()
    {
        _startingPriority = _virtualCamera.Priority;
    }

    private void OnTriggerEnter(Collider other)
    {
        _virtualCamera.Priority += 500;

    }

    private void OnTriggerExit(Collider other)
    {
        _virtualCamera.Priority = _startingPriority;
    }
}
