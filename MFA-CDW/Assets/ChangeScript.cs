using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SBPScripts;
using UnityEngine.Events;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;

public class ChangeScript : MonoBehaviour
{
    [System.Serializable]
    public class Dialogue
    {
        public string speaker;
        public string dialogue;
        public Emotes emote;
        public EventReference oneShotAudio;
    }
    [SerializeField]
    private Dialogue[] dialogue;
    [SerializeField]
    private float[] timeToSwitch;
    [SerializeField]
    private TextMeshProUGUI subtitlesDialogue;
    [SerializeField]
    private TextMeshProUGUI subtitlesSpeaker;
    [SerializeField]
    private ExternalTagController tagAI;
    [SerializeField]
    private ExternalTagController secondSlightlySmallerTagAI;
    [SerializeField]
    private ExternalController playerMovement;
    [SerializeField]
    private GameObject tagBike;
    [SerializeField]
    private CyclistAnimController getOnBike;
    [SerializeField]
    private CarouselController carousel;
    [SerializeField]
    private PromptSpawner getOnBikePrompt;
    [SerializeField]
    private GameObject WalkingZone;
    [SerializeField]
    private VistaEntranceZone vista;
    [SerializeField]
    private EmptyVista emptyVista;
    [SerializeField]
    private Image cursor;
    [SerializeField]
    private GameObject bikeCamera;
    private InputManager inputManager;
    private Vector3 newDestination = new Vector3(900.67f, 55.74f, 481.68f);
    public int index;
    private string speaker;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    public Color tagSpeakerTextColor;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    public Color chaseSpeakerTextColor;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    public Color dadSpeakerTextColor;
    [ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
    public Color samSpeakerTextColor;
    private string text;
    public float time;
    public bool paused = false;
    string[] commands;
    int closeBracket;

    public enum Emotes {
        NONE,
        CURIOUS_CHASE,
        EXCITED_CHASE,
        PENSIVE_CHASE,
        CYNICAL_CHASE,
        GRIM_TAG,
        DELIBERATE_TAG,
        SATISFIED_TAG,
        UNDERSTANDING_TAG
    }

    private EventInstance chaseEmotes;
    private EventInstance tagEmotes;

    private void Start() {
        chaseEmotes = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.chaseEmotes);
        tagEmotes = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.tagEmotes);
        chaseEmotes.start();
        tagEmotes.start();
    }

    void OnEnable()
    {
        paused = false;
    }
    //SetPaused(false);
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("0"))
        {
            DebugSkipDialogue();
        }
        if (!paused)
        {
            time -= Time.deltaTime;
            if (time <= 0 && index < dialogue.Length && dialogue[index].dialogue[0] != '[')
            {
                text = dialogue[index].dialogue;
                speaker = dialogue[index].speaker;
                time = timeToSwitch[index];
                PlayEmote(dialogue[index].emote);
                if(!dialogue[index].oneShotAudio.IsNull) {
                    PlayOneShot(dialogue[index].oneShotAudio);
                }
                subtitlesSpeaker.SetText(speaker);
                subtitlesDialogue.SetText(text);
                UpdateSpeakerColor(subtitlesSpeaker);
                if (dialogue[index].speaker[0] == '[')
                {
                    speaker = "";
                    time = timeToSwitch[index];
                    subtitlesSpeaker.SetText(speaker);
                    UpdateSpeakerColor(subtitlesSpeaker);
                }
                index++;
            }
            else if (time <= 0 &&  index < dialogue.Length && dialogue[index].dialogue[0] == '[')
            {
                text = "";
                time = timeToSwitch[index];
                if(!dialogue[index].oneShotAudio.IsNull) {
                    PlayOneShot(dialogue[index].oneShotAudio);
                }
                subtitlesDialogue.SetText(text);
                if (dialogue[index].speaker[0] == '[')
                {
                    speaker = "";
                    time = timeToSwitch[index];
                    subtitlesSpeaker.SetText(speaker);
                    UpdateSpeakerColor(subtitlesSpeaker);
                }
                index++;
            }

            if (dialogue[index].dialogue[0] == '[')
            {
                closeBracket = dialogue[index].dialogue.IndexOf(']');
                commands = dialogue[index].dialogue.Substring(1, closeBracket - 1).Split(',');
                for (int i = 0; i < commands.Length; i++)
                {
                    switch (commands[i])
                    {
                        case "externalaistart":
                            tagAI.GetUp();
                            tagAI.SetDestination(newDestination);
                            break;
                        case "pause":
                            text = "";
                            time = timeToSwitch[index];
                            subtitlesDialogue.SetText(text);
                            paused = true;
                            break;
                        case "mount":
                            getOnBike.canMount = true;
                            getOnBikePrompt.canMount = true;
                            break;
                        case "bikecameraon":
                            bikeCamera.SetActive(true);
                            break;
                        case "chasegetup":
                            // AudioManager._instance.PlayEvent(FMODEvents._instance.getUp);
                            playerMovement.GetUp();
                            break;
                        case "carouselend":
                            carousel.StopCarousel();
                            break;
                        case "continuevista":
                            vista.SetCanMove(true);
                            break;
                        case "leavevista":
                            emptyVista.SetAbleToLeave();
                            break;
                        case "cursor":
                            cursor.enabled = true;
                            break;
                        case "externalaidisable":
                            tagBike.SetActive(false);
                            tagAI.gameObject.SetActive(false);
                            break;
                        case "turnoff":
                            text = "";
                            time = timeToSwitch[index];
                            subtitlesDialogue.SetText(text);
                            this.gameObject.SetActive(false);
                            break;
                    }
                }
            }
            
        } 
    }

    private void UpdateSpeakerColor(TextMeshProUGUI subtitlesSpeaker)
    {

        switch(subtitlesSpeaker.text)
        {
            case "Tag":
                subtitlesSpeaker.color = tagSpeakerTextColor;
                Debug.Log("COLOR CHANGED!");
                break;
            case "Chase":
                subtitlesSpeaker.color = chaseSpeakerTextColor;
                Debug.Log("COLOR CHANGED!");
                break;
            case "Chase's Dad":
                subtitlesSpeaker.color = dadSpeakerTextColor;
                Debug.Log("COLOR CHANGED!");
                break;
            case "Sam":
                subtitlesSpeaker.color = samSpeakerTextColor;
                Debug.Log("COLOR CHANGED!");
                break;
        }
    }

    private void PlayEmote(Emotes emote) {
        switch(emote) {
            case Emotes.NONE:
                break;
            // Chase
            case Emotes.CURIOUS_CHASE:
                chaseEmotes.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
                chaseEmotes.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 0);
                chaseEmotes.start();
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.chaseEmotes);
                // RuntimeManager.StudioSystem.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 0);
                // chaseEmotes.start();
                // chaseEmotes.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 0);
                break;
            case Emotes.EXCITED_CHASE:
                chaseEmotes.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
                chaseEmotes.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 2);
                chaseEmotes.start();
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.chaseEmotes);
                // RuntimeManager.StudioSystem.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 2);
                // chaseEmotes.start();
                // chaseEmotes.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 2);
                break;
            case Emotes.PENSIVE_CHASE:
                chaseEmotes.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
                chaseEmotes.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 3);
                chaseEmotes.start();
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.chaseEmotes);
                // RuntimeManager.StudioSystem.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 3);
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.chaseEmotes);
                // chaseEmotes.start();
                // chaseEmotes.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 3);
                break;
            case Emotes.CYNICAL_CHASE:
                chaseEmotes.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
                chaseEmotes.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 1);
                chaseEmotes.start();
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.chaseEmotes);
                // RuntimeManager.StudioSystem.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 1);
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.chaseEmotes);
                // chaseEmotes.start();
                // chaseEmotes.setParameterByName("PAR_DX_CHASE_EMOTE_SWITCHER", 1);
                break;
            // Tag
            case Emotes.GRIM_TAG:
                /* EventInstance grimInstance = RuntimeManager.CreateInstance(FMODEvents.Instance.tagEmotes);
                grimInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
                grimInstance.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 1);
                grimInstance.start(); */
                tagEmotes.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
                tagEmotes.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 1);
                tagEmotes.start();
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.tagEmotes, tagAI.transform.position);
                // RuntimeManager.StudioSystem.setParameterByID()
                // RuntimeManager.StudioSystem.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 1);
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.tagEmotes, tagAI.transform.position);
                // tagEmotes.start();
                // tagEmotes.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 1);
                break;
            case Emotes.DELIBERATE_TAG:
                tagEmotes.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
                tagEmotes.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 0);
                tagEmotes.start();
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.tagEmotes, tagAI.transform.position);
                // RuntimeManager.StudioSystem.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 0);
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.tagEmotes, tagAI.transform.position);
                // tagEmotes.start();
                // tagEmotes.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 0);
                break;
            case Emotes.SATISFIED_TAG:
                tagEmotes.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes((Camera.main.transform)));
                tagEmotes.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 2);
                tagEmotes.start();
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.tagEmotes, tagAI.transform.position);
                // RuntimeManager.StudioSystem.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 2);
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.tagEmotes, tagAI.transform.position);
                // tagEmotes.start();
                // tagEmotes.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 2);
                break;
            case Emotes.UNDERSTANDING_TAG:
                tagEmotes.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
                tagEmotes.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 3);
                tagEmotes.start();
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.tagEmotes, tagAI.transform.position);
                // RuntimeManager.StudioSystem.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 3);
                // AudioManager.Instance.PlayEvent(FMODEvents.Instance.tagEmotes, tagAI.transform.position);
                // tagEmotes.start();
                // tagEmotes.setParameterByName("PAR_DX_TAG_EMOTE_SWITCHER", 3);
                // tagEmotes.start();
                break;
            default:
                break;
        }
    }

    private void PlayOneShot(EventReference reference) {
        AudioManager.Instance.PlayEvent(reference);
    }

    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }
        

    public void DebugSkipDialogue()
    {
        for (int i = 0; i < dialogue.Length; i++)
        {
            timeToSwitch[i] = 0;

        }
        WalkingZone.SetActive(false);
        playerMovement.walkingZone = false;
    }

    public void SetPause(bool val) { 
        paused = val;
    }

    public void Unpause()
    {
        paused = false;
        index++;
    }
}
