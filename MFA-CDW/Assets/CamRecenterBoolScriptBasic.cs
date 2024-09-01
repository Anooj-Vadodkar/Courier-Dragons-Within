using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamRecenterBoolScriptBasic : MonoBehaviour
{
    //declare/assigned variables for enabling/disabling recentering. 
    [SerializeField] private bool callToEnable = false;
    //delcare counter to count down. 
    [SerializeField] private float timeToEnable;
    //declare counter default value.
    [SerializeField] private float enableTimeCounter;
    //declare variables for waiting (in seconds).
    [SerializeField] CinemachineFreeLook ourExternalPlayerCam;

    // Update is called once per frame
    void Update()
    {
        
        //start count down.
        timeToEnable -= Time.deltaTime;
        //detect movement from player (input mouse). Reset counter to default value.
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            //Debug.Log("Setting call bool false!");
            callToEnable = false;
            if(ourExternalPlayerCam.m_YAxisRecentering.m_enabled == true|| ourExternalPlayerCam.m_RecenterToTargetHeading.m_enabled == true)
            {
                ourExternalPlayerCam.m_YAxisRecentering.m_enabled = false;
                ourExternalPlayerCam.m_RecenterToTargetHeading.m_enabled = false;
            }
            //Debug.Log("Resetting counter!");
            timeToEnable = enableTimeCounter;
        }
        //if a certain amount of time passes, and no input has been detected, enable recentering on x and y axis. HINT: we'll need to call a coroutine.


        //if no movement is detected && a certain amount of time has passed, declare callToEnable true.

        if((Input.GetAxis("Vertical")==0 && Input.GetAxis("Horizontal") == 0) && timeToEnable <= 0)
        {
            //Debug.Log("Setting call bool true!");
            callToEnable = true;
        }

        //if callToEnable is true, run coroutine.
        if (callToEnable == true)
        {
            StartCoroutine(EnableRecentering());
        }
        
        //if input is detected, then disable recentering. 
    }

    IEnumerator EnableRecentering()
    {
        //Debug.Log("Starting Recentering Coroutine");
        ourExternalPlayerCam.m_YAxisRecentering.m_enabled = true;
        ourExternalPlayerCam.m_RecenterToTargetHeading.m_enabled = true;
        yield return null;
        //Debug.Log("Recentering Coroutine yielded!");
        
    }
}
