using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWorld : MonoBehaviour
{
    [Header("Tweaks")]
    [SerializeField] public Transform lookat;
    [SerializeField] public Vector3 offset;

    [Header("Logic")]
    private Camera cam;

    // Use this for initialization 
    private void Start()
    {
        cam = Camera.main;

    }

    // Update is called once per frame 
    private void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(lookat.position + offset);

        if (transform.position != pos) 
            transform.position = pos;

    }
}
