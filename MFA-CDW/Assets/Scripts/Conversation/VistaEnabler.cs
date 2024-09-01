using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBPScripts;
public class VistaEnabler : MonoBehaviour
{
    [SerializeField] private ExternalTagController externalAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            externalAI.GetUp();
            externalAI.SetFollowPlayer(true);
        }
            
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            externalAI.SetFollowPlayer(false);
        }
            
    }
}
