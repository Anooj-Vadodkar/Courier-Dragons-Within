using System.Collections;
using System.Collections.Generic;
using SBPScripts;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;
using FMOD;
using System.Diagnostics;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;

    private EventInstance musicEvent;
    private EventInstance ambientEvent;
    private StudioEventEmitter ambientEventEmitter;

    public static AudioManager Instance {
        get {
            return _instance;
        }
    }

    private void Awake() {
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start() {
        // musicEvent = CreateEventInstance(FMODEvents.Instance.medi1Music);
    }

    private void Update() {
        /* if(!ambientEvent.IsUnityNull()) {
            FMOD.ATTRIBUTES_3D attributes = new ATTRIBUTES_3D();
            FMOD.VECTOR pos = new VECTOR();
            pos.x = Camera.main.transform.position.x;
            pos.y = Camera.main.transform.position.y;
            pos.z = Camera.main.transform.position.z;
     
            attributes.position = pos;
            ambientEvent.set3DAttributes(attributes);
        } */
    }

    public void SetMusicEvent(EventReference eventRef) {
        SetMusicEvent(eventRef, false);
    }

    public void SetMusicEvent(EventReference eventRef, bool playMusic) {
        StopMusic();
        musicEvent = CreateEventInstance(eventRef);
        if(playMusic) {
            PlayMusic();
        }
    }

    public void SetAmbientEvent(StudioEventEmitter eventEmitter) {
        ambientEventEmitter = eventEmitter;
        ambientEventEmitter.Play();
    }

    public void SetAmbientEvent(EventReference eventRef) {
        // SetAmbientEvent(eventRef, false);
    }

    public void SetAmbientEvent(EventReference eventRef, bool playAmbient) {
        // Stop ambient
        ambientEvent = CreateEventInstance(eventRef);
        if(playAmbient) {
            // play ambient
            PlayAmbient();
        }
    }

    public void PlayEvent(EventReference eventRef) {
        PlayEvent(eventRef, Camera.main.transform.position);
    }

    public void PlayEvent(EventReference eventRef, Vector3 worldPos) {
        RuntimeManager.PlayOneShot(eventRef, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference) {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        ATTRIBUTES_3D attributes = new ATTRIBUTES_3D();
        VECTOR pos = new VECTOR();
        pos.x = Camera.main.transform.position.x;
        pos.y = Camera.main.transform.position.y;
        pos.z = Camera.main.transform.position.z;
     
        attributes.position = pos;
        eventInstance.set3DAttributes(attributes);
        return eventInstance;
    }

    public void StopEvent(EventReference eventReference) {
        CreateEventInstance(eventReference).stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); 
    }

    public void SetMusicParam(float state) {
        musicEvent.setParameterByName("PAR_MEDI_MX_SWITCHER", state);
    }

    public float GetMusicParam() {
        musicEvent.getParameterByName("PAR_MEDI_MX_SWITCHER", out float result);
        return result;
    }

    public void PlayMusic() {
        musicEvent.start();
    }

    public void StopMusic() {
        musicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PlayAmbient() {
        ambientEvent.start();
        ambientEvent.release();
    }

    public void StopAmbient() {
        UnityEngine.Debug.Log("Hit Stop Ambient");
        // ambientEvent.release();
        // ambientEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ambientEventEmitter.Stop();
    }

}

public class MusicState {
    public static readonly float MEDITATION_BASE = 0;
    public static readonly float MEDITATION_CONF = 1;
    public static readonly float MEDITATION_SERENE = 2;
    public static readonly float CONVO_BASE = 3;
    public static readonly float CONVO_TALK = 4;
    public static readonly float BIKING = 5;
}

[System.Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
}
