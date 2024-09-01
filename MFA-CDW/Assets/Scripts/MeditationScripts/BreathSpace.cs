using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathSpace : MonoBehaviour
{
    [SerializeField]
    private Thought thought;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")) {
            Debug.Log("Hit Player");
            StartCoroutine(thought.StartConfrontation());
            Destroy(gameObject);
        }
    }
}
