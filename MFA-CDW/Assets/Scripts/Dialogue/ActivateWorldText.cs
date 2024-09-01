using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWorldText : MonoBehaviour
{
    [SerializeField] private GameObject worldText;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            worldText.SetActive(true);
        }
    }
}
