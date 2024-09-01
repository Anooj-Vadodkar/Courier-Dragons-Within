using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowResponse : MonoBehaviour
{
    [SerializeField]
    private FadeableText _fadeableText;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _fadeableText.Unhide();
        }
    }
}
