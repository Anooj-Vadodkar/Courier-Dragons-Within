using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float texMoveSpeed = 1f;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    public void Scroll(Vector2 moveAmount)
    {
        GetComponent<Image>().material.mainTextureOffset = cam.transform.position * texMoveSpeed;
    }
}
