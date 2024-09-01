using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SBPScripts;
public class ConversationSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject firstButton;
    [SerializeField]
    private GameObject secondButton;
    [SerializeField]
    private GameObject thirdButton;
    [SerializeField]
    private GameObject originalCamera;
    [SerializeField]
    private GameObject newCamera;
    [SerializeField]
    private Transform cameraOriginalPoint;
    [SerializeField]
    private GameObject breathCanvasPrompt;

    private InputManager inputManager;
    public CyclistAnimController bikeCheck;
    public ExternalController player;
    public GameObject conversationCanvas;
    //public GameObject breathePrompt;
    public GameObject promptSpawner;
    public DialogueWrapper dialogueWrapper;
    private Vector3 cameraFront;
    public TextMeshProUGUI response2;
    void Start()
    {
        inputManager = InputManager.Instance;
    }

    public void DialogueSpawn()
    {
        // AudioManager.Instance.SetMusicParam(MusicState.CONVO_TALK);
        //PlayerCursor.Instance.BaseRetical();

        //breathePrompt.SetActive(false);
        //breathingInstructionPromptActive();
        originalCamera.SetActive(!originalCamera.activeSelf);
        if (originalCamera.activeSelf)
        {
            originalCamera.transform.position = cameraOriginalPoint.position;
        }
        newCamera.SetActive(true);
        conversationCanvas.SetActive(true);
        firstButton.SetActive(true);
        secondButton.SetActive(true);
        thirdButton.SetActive(true);
        dialogueWrapper.DelayedRefreshConversation();
        this.gameObject.SetActive(false);

        //AudioManager.Instance.PlayEvent(FMODEvents.Instance.SamQuestionVO, Camera.main.transform.position);
    }

    public void SetPromptActive()
    {
        Cursor.visible = false;
        BreathingInstructionInactive();
        promptSpawner.SetActive(true);
    }

    public void BreathingInstructionInactive()
    {
        breathCanvasPrompt.SetActive(false);
    }
    public void delayedBreathingInstructionPrompt()
    {
        Invoke("breathingInstructionPromptActive", 5.0f);
    }

    public void breathingInstructionPromptActive()
    {
         breathCanvasPrompt.SetActive(true);
    }
}
