using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBPScripts;
public class DeactivateParkingIcon : MonoBehaviour
{
    [SerializeField] private CyclistAnimController playerBike;
    [SerializeField] private BicycleController bike;

    [Header("Variables for new bike implementation")]
    [SerializeField] private NewBikeController newBike;
    [SerializeField] private Transform haltPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!newBike) {
                bike.SetHalt(true);
                playerBike.SetParkWaypoint(true);
            } else {
                // start halting the new bike controller
                newBike.SetHaltingPoint(haltPoint);
            }
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bike.GetComponent<Rigidbody>().drag >= 2.0f && bike.GetComponent<BicycleStatus>().onBike)
            {
                if(!newBike) {
                    playerBike.DismountPromptTrigger();
                } else {
                    // show dismount prompt on new bike
                }
                this.gameObject.SetActive(false);
            }
               
        }
    }
}
