using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBPScripts;
public class TagBikeStop : MonoBehaviour
{
    [SerializeField] TagBikingAI tagBike;
    [SerializeField] GameObject cyclingPath;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            tagBike.SetIsBiking(false);
            cyclingPath.SetActive(false);
        }
    }

}
