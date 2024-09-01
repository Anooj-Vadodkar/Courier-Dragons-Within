using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBPScripts;
public class DeactivateParkingIcon : MonoBehaviour
{
    [SerializeField] private CyclistAnimController playerBike;
    [SerializeField] private BicycleController bike;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bike.SetHalt(true);
            playerBike.SetParkWaypoint(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bike.GetComponent<Rigidbody>().drag >= 2.0f && bike.GetComponent<BicycleStatus>().onBike)
            {
                playerBike.DismountPromptTrigger();
                this.gameObject.SetActive(false);
            }
               
        }
    }
}
