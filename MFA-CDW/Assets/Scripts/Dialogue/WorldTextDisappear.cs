using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTextDisappear : MonoBehaviour
{
    [SerializeField] DisappearingText text;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            text.StartFade();
            this.GetComponent<BoxCollider>().enabled = false;
        }
            
    }
}
