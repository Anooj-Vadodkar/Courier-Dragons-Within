using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikePrompt : MonoBehaviour
{
    public GameObject exitPrompt;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            exitPrompt.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
