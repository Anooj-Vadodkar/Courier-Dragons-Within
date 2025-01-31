using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using SBPScripts;
public class CycleSetup : MonoBehaviour
{
    [SerializeField] CyclistAnimController player;
    [SerializeField] TagBikingAI newTagBike;
    [SerializeField] TagBikingAI oldTagBike;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oldTagBike.gameObject.SetActive(false);
            player.SetBikingAI(newTagBike);
            newTagBike.gameObject.SetActive(true);
        }
            
    }
}
