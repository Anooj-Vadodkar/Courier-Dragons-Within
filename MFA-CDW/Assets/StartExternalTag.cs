using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartExternalTag : MonoBehaviour
{
    [SerializeField] ExternalTagController tagAI;
    [SerializeField] Transform gateStart;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<ExternalController>() != null)
        {
            tagAI.gameObject.SetActive(true);
            tagAI.SetDestination(gateStart.position);
            tagAI.GetUp();
        }
            
    }
}
