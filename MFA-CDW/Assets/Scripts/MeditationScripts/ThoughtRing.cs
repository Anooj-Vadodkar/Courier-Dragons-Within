using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtRing : MonoBehaviour
{
    [SerializeField] public GameObject thoughtSprite;

    void Update()
    {
        if (thoughtSprite) {
            transform.localScale = thoughtSprite.transform.localScale;
        }
    }
}
