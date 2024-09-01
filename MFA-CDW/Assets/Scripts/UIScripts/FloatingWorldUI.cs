using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingWorldUI : MonoBehaviour
{
    Transform mainCam;
    Transform theObject;
    public Canvas worldSpaceCanvas;
    

    public Vector3 offset;

 void Start()
    {
        mainCam = Camera.main.transform;
        theObject = transform.parent;

        transform.SetParent(worldSpaceCanvas.transform);
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        transform.position = theObject.position + offset;

    }
}
