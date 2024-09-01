using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBPScripts;
public class AIChangeGate : MonoBehaviour
{
    [SerializeField]
    private CompanionAI tag;
    [SerializeField]
    private GameObject nextGate;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "AI")
        {
            tag.SetDestination(nextGate.transform);
            Debug.Log("AI gone");
        }
    }
}
