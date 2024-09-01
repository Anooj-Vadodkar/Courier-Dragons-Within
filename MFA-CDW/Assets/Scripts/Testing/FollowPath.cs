using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Cinemachine;

public class FollowPath : MonoBehaviour
{
    [SerializeField] private PathCreator path;
    [SerializeField] private float speed = 6f;
    [SerializeField] private Vector3 pathOffset;
    private float posOnPath = 0;

    private bool started = false;

    private void Update() {
        if(started) {
            transform.position = path.path.GetPointAtDistance(posOnPath) + pathOffset;
            Quaternion pathRotation = path.path.GetRotationAtDistance(posOnPath);
            pathRotation *= Quaternion.Euler(0, 0, 90);
            transform.rotation = pathRotation;
            posOnPath += speed * Time.deltaTime;
        }
        else if(InputManager.Instance.GetPathTestPressed()) {
            GetComponent<CinemachineVirtualCamera>().enabled = true;
            started = true;
        }
    }
}
