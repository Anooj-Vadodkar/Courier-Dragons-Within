using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class LoopingAudioInstance : MonoBehaviour
{
    [SerializeField] private EventReference loopingReference;

    private EventInstance instance;

    private void Start() {
        instance = AudioManager.Instance.CreateEventInstance(loopingReference);
    }

    public void StartLoopingTrack() {
        instance.start();
    }

    public void StopLoopingTrack() {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
