using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateVista1 : MonoBehaviour
{
    [SerializeField] GameObject vista1;

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vista1.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
