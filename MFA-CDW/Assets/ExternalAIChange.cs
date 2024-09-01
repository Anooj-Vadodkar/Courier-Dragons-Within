using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalAIChange : MonoBehaviour
{
    [SerializeField]
    private ExternalTagController tagAI;
    [SerializeField]
    private Transform companion;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AI")
        {
            Debug.Log("hi");
            tagAI.SetDestination(companion.position);
        }
            
    }
}
