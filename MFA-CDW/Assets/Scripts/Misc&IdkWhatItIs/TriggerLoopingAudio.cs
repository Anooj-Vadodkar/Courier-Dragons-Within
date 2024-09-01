using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLoopingAudio : MonoBehaviour
{
    [SerializeField] private LoopingAudioInstance loopingAudio;
    [SerializeField] private bool startAudio = false;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            if(startAudio) {
                Debug.Log("Start Music Track");
                loopingAudio.StartLoopingTrack();
            } else {
                Debug.Log("Stop Music Track");
                loopingAudio.StopLoopingTrack();
            }

            GetComponent<Collider>().enabled = false;
        }
    }
}
