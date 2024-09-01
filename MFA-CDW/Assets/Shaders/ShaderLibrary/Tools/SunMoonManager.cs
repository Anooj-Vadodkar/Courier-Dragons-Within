using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class SunMoonManager : MonoBehaviour
{
    public GameObject sun;
    public GameObject moon;
    
    // Start is called before the first frame update
    void Start()
    {
        moon.transform.rotation = sun.transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        SetMoonOppositeSun();
        //moon.transform.Rotate(sun.transform.forward, Mathf.PI/2);
    }

    public void SetMoonOppositeSun()
    {
        Quaternion oppSun = Quaternion.LookRotation(-sun.transform.forward, sun.transform.up);
        moon.transform.rotation = oppSun;
    }
}
