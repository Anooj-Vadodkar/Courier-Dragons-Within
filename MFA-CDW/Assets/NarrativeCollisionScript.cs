using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using FMOD.Studio;
using Unity.VisualScripting;
using FMODUnity;
using UnityEngine.Events;

public class NarrativeCollisionScript : MonoBehaviour
{
    // private MeditationSubtitleManager subtitleManager;
    private SubtitleScroller subtitleScroller;
    // [SerializeField] private MeditationSubtitleManager.Subtitle[] storyText;
    [SerializeField] private SubtitleScroller.NewSub[] storyText; 
    [SerializeField] private bool requireBreath = false;
    [SerializeField] private bool lockedInPlace = false;
    [SerializeField] private EventReference oneShotAudio; 
    [SerializeField] private UnityEvent _onPass;
    [SerializeField] private UnityEvent _onSubtitleEnds;

    private List<EventReference> audioEvents;
    
    private void Start()
    {
        // subtitleManager = FindObjectOfType<MeditationSubtitleManager>();
        subtitleScroller = FindObjectOfType<SubtitleScroller>();
        Debug.Log("SubtitleScroller Name: " + subtitleScroller.name);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("We hit the " + GetComponent<Collider>().name);
        if (collider.CompareTag("Player"))//for this to work, we need to 'tag' gameObjects in unity
                                                                   //that are meant to be obstacles as such by adding a new tag
                                                                   //to the inspector under the object name name
        {
            Debug.Log("The Player hit " + gameObject);
            if (!requireBreath)
            {
                _onPass.Invoke();
                CalculateEndOfSubtitles();
                StartSubtitles();
                GetComponent<Collider2D>().enabled = false;
                if(!oneShotAudio.IsUnityNull()) {
                    AudioManager.Instance.PlayEvent(oneShotAudio, Camera.main.transform.position);
                }
            }
        }
    }

    public void OnBreathe()
    {
        StartSubtitles();
        CalculateEndOfSubtitles();

        _onPass.Invoke();
    }

    private void CalculateEndOfSubtitles() {
        float subtitleTime = 0.0f;
        foreach(SubtitleScroller.NewSub sub in storyText) {
            subtitleTime += sub.voTime;
        }

        Invoke("OnSubtitleEnds", subtitleTime);
    }

    private void StartSubtitles() {
        AudioManager.Instance.PlayEvent(oneShotAudio);
        subtitleScroller.InitializeSubtitleBlock(storyText);
    }

    public void OnSubtitleEnds() {
        Debug.Log("Hit Subtitle Ended");
        _onSubtitleEnds.Invoke();
    }
}
