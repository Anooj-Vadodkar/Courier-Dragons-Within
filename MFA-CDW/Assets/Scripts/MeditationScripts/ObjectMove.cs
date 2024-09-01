using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    private void Start()
    {
        LockToMouse.instance.AddObject(this);
    }

    public void MoveObject(Vector3 offset)
    {
        transform.position += offset;
    }

    private void OnDestroy()
    {
        LockToMouse.instance.RemoveObject(this);
    }
}
