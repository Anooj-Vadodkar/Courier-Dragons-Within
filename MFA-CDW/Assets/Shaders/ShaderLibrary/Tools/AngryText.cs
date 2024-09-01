using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AngryText : MonoBehaviour
{
    // Start is called before the first frame update
    private MeshRenderer mr;
    private CanvasRenderer cr;
    private Material myMat;
    private Material dropMat;
    
    private float vibrationTimer = 0.0f;
    private Transform dropText;
    
    [SerializeField] private float dropRange = 0.5f;
    [SerializeField] private float vibrateEvery = 0.2f;
    [SerializeField] private float dropAlphaMin = 0.2f;
    [SerializeField] private float dropAlphaMax = 0.6f;
    [SerializeField] private float blurDistance = 0.00225f;

    private void Start()
    {
        InitializeMaterial();
        dropText = Instantiate(this).GetComponent<Transform>();
        Destroy(dropText.GetComponent<AngryText>());
        dropText.parent = transform;
        MeshRenderer dropRenderer = dropText.GetComponent<MeshRenderer>();
        dropRenderer.material = new Material(myMat);
        dropMat = dropRenderer.material;
    }

    public void InitializeMaterial()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            if (!mr) mr = GetComponent<MeshRenderer>();
        }
        else
        {
            if (!cr) cr = GetComponent<CanvasRenderer>();
        }
        
        if (!myMat)
        {
            if (mr)
            {
                if (!mr.material) return;
                myMat = new Material(mr.material);
                mr.material = myMat;
            }
            else if (cr)
            {
                myMat = new Material(cr.GetMaterial());
                if (!myMat) return;
                cr.SetMaterial(myMat, 0);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!myMat)
        {
            InitializeMaterial();
        }
        
        vibrationTimer += Time.deltaTime;
        if (vibrationTimer > vibrateEvery)
        {
            dropText.position = transform.position +
                                dropText.right * UnityEngine.Random.Range(-dropRange, dropRange) +
                                dropText.up * UnityEngine.Random.Range(-dropRange, dropRange);
            vibrationTimer = 0.0f;
            Color oldColor = dropMat.GetColor("_FaceColor");
            dropMat.SetColor("_FaceColor", new Color(oldColor.r, oldColor.g, oldColor.b,
                UnityEngine.Random.Range(dropAlphaMin, dropAlphaMax)));
            dropMat.SetFloat("_BlurDistance", blurDistance);
        }
    }
}
