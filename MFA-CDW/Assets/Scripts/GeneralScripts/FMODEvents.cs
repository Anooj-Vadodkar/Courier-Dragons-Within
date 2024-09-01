using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Runtime.InteropServices;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{
    [field: SerializeField] public EventReference natureAmbient {get; private set;}
    [field: SerializeField] public EventReference medi1Music {get; private set;}
    [field: SerializeField] public EventReference medi2Music {get; private set;}
    [field: SerializeField] public EventReference medi3Music {get; private set;}
    [field: SerializeField] public EventReference convoMusic {get; private set;}
    [field: SerializeField] public EventReference bikingMusic {get; private set;}
    [field: SerializeField] public EventReference dragonSFX {get; private set;}
    [field: SerializeField] public EventReference SamQuestionVO {get; private set;}
    [field: SerializeField] public EventReference ChaseOption1 {get; private set;}
    [field: SerializeField] public EventReference ChaseOption2 {get; private set;}
    [field: SerializeField] public EventReference ChaseOption3 {get; private set;}
    [field: SerializeField] public EventReference breathIn {get; private set;}
    [field: SerializeField] public EventReference breathOut {get; private set;}
    [field: SerializeField] public EventReference breathInOutside {get; private set;}
    [field: SerializeField] public EventReference breathOutOutside {get; private set;}
    [field: SerializeField] public EventReference bikeGearSFX {get; private set;}
    [field: SerializeField] public EventReference bikeGravelSFX {get; private set;}
    [field: SerializeField] public EventReference dialogueStinger {get; private set;}
    [field: SerializeField] public EventReference chaseMonologue {get; private set;}
    [field: SerializeField] public EventReference footsteps {get; private set;}
    [field: SerializeField] public EventReference wsMeditation {get; private set;}
    [field: SerializeField] public EventReference bikeSounds {get; private set;}
    [field: SerializeField] public EventReference getOffBike {get; private set;}
    [field: SerializeField] public EventReference getOnBike {get; private set;}
    [field: SerializeField] public EventReference getUp {get; private set;}
    [field: SerializeField] public EventReference sitDown {get; private set;}

    [field: Header("Conversation Navigation")]
    [field: SerializeField] public EventReference riffsTogether {get; private set;}
    [field: Header("Emotes")]
    [field: SerializeField] public EventReference chaseEmotes {get; private set;}
    [field: SerializeField] public EventReference tagEmotes {get; private set;}

    public static FMODEvents _instance;

    public static FMODEvents Instance {
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
}
