using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
public class ExitGateCollisionScript : MonoBehaviour

{

    [SerializeField] GameObject targetCamera;
    //[SerializeField] Camera FirstPersonCam;
    //[SerializeField] Camera ThirdPersonCam;
    //[SerializeField] Camera ZoomCam;
    //[SerializeField] Camera WakingUpCam;
    [SerializeField] GameObject BikeCam;

    [SerializeField] GameObject breathSlider;
    [SerializeField] GameObject endCardText;

    //public PlayerMovement movement;
    private void OnTriggerEnter(Collider collisionInfo)
    {
        //Debug.Log("We hit the " + GetComponent<Collider>().name);
        if (collisionInfo.GetComponent<Collider>().tag == "Player")//for this to work, we need to 'tag' gameObjects in unity
                                                     //that are meant to be obstacles as such by adding a new tag
                                                     //to the inspector under the object name name
        {
            //movement.enabled = false;
            Debug.Log("We hit the player!");
            targetCamera.SetActive(true);
            breathSlider.SetActive(false);
            endCardText.SetActive(true);
            //FirstPersonCam.enabled = false;
            //ThirdPersonCam.enabled = false;
            //ZoomCam.enabled = false;
           // WakingUpCam.enabled = false;  
            BikeCam.SetActive(false);
            //Debug.Log("Quitting the App!!");
            //Application.Quit();
        }
    }

    /*private void OnCollisionStay(Collision collisionInfo)
    {
        Debug.Log("We hit the " + collisionInfo.collider.name);
        if (collisionInfo.collider.tag == "Player")//for this to work, we need to 'tag' gameObjects in unity
                                                   //that are meant to be obstacles as such by adding a new tag
                                                   //to the inspector under the object name name
        {
            //movement.enabled = false;
            Debug.Log("We hit the player!");
        }
    }*/
}