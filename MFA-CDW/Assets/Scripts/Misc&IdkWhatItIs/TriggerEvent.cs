using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _event;
    [SerializeField] private bool onlyOnce = false;
    private bool _triggered = false;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            if(onlyOnce && !_triggered) {
                _event.Invoke();
            }
            if(!onlyOnce) {
                _event.Invoke();
            }
            _triggered = true;
        }
    }
}
