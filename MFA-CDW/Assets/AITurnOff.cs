using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurnOff : MonoBehaviour
{
    [SerializeField]
    private ExternalTagController tag;

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "AI")
        {
            tag.ResetDestination();
        }
    }
}
