using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class DialogueSpawner : MonoBehaviour
{
    [SerializeField] FadeableText fadeableText;
    [SerializeField] private TopTextObserver breathObserver;
    [SerializeField]
    private GameObject dialogueObject;
    [SerializeField]
    private GameObject exploreObject;
    [SerializeField] DisappearingText disappearingText;
    [SerializeField] private float delay = 3;
    [SerializeField] private float UnhideDelay = 3;
    [SerializeField] private GameObject lastObject;
    private bool triggered = false;
    private bool firsttriggered = false;

    private void OnTriggerEnter(Collider other) {
        if(!firsttriggered && other.gameObject.CompareTag("Player")) {
            //AudioManager.Instance.PlayEvent(FMODEvents.Instance.dialogueStinger, Camera.main.transform.position);
            Debug.Log("Spawn dialogue on Player");
            Transform spawnOrigin = other.gameObject.transform;
            dialogueObject.SetActive(true);
            // this.gameObject.SetActive(false);
            //GetComponent<BoxCollider>().enabled = false;
            firsttriggered = true;
            if(fadeableText != null && breathObserver.hasJumped)
            {
                triggered = true;
                Invoke("StartFade", delay);
                Invoke("HandleFadeFinished", UnhideDelay + delay);
            }
            if(lastObject != null && exploreObject != null)
            {
                exploreObject.SetActive(false);
                lastObject.SetActive(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !triggered && fadeableText != null && breathObserver.hasJumped)
        {
            triggered = true;
            Invoke("StartFade", delay);
            Invoke("HandleFadeFinished", UnhideDelay + delay);
        }
    }
    private void StartFade()
    {
        if(disappearingText != null) 
        {
            disappearingText.StartFade();
        }
        
    }
    public void HandleFadeFinished()
    {
        fadeableText.Unhide();
    }
    private void OnEnable()
    {
        //disappearingText.FadeFinishedEvent += HandleFadeFinished;
        
    }
    private void OnDisable()
    {
        //disappearingText.FadeFinishedEvent -= HandleFadeFinished;
    }
}
