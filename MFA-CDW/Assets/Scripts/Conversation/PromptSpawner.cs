using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptSpawner : MonoBehaviour
{
    public GameObject prompt;
    public bool canMount = false;
    private void OnTriggerStay(Collider other)
    {
        if (canMount && other.gameObject.tag == "Player")
            prompt.SetActive(true);

    }
    private void OnTriggerExit(Collider other)
    {
        if (canMount && other.gameObject.tag == "Player")
            prompt.SetActive(false);
    }
}
