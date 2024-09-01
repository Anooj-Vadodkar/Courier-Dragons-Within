using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PixelCrushers.DialogueSystem;
using SBPScripts;
using Cinemachine;
using TMPro;
using UnityEngine.InputSystem;
public class DialogueWrapper : MonoBehaviour
{
    public static DialogueWrapper _instance;

    [SerializeField]
    private FadeableText promptText;
    [SerializeField]
    private FadeableText[] responseTexts;
    [SerializeField]
    private GameObject dialogueCanvas;
    [SerializeField]
    private GameObject exploreCanvas;
    [SerializeField]
    private ChangeScript finalConvo;
    [SerializeField]
    private GameObject firstButton;
    [SerializeField]
    private GameObject secondButton;
    [SerializeField]
    private GameObject thirdButton;
    [SerializeField]
    private CyclistAnimController bikeController;
    [SerializeField]
    private GameObject originalCamera;
    [SerializeField]
    private Transform cameraOriginalPoint;
    [SerializeField]
    private CinemachineBrain cameraBrain;
    [SerializeField]
    private ConversationSpawner conversationSpawner;
    [SerializeField]
    private ExternalTagController tagAI;
    [SerializeField]
    private GameObject firstResponse;
    [SerializeField]
    private GameObject secondResponse;
    [SerializeField]
    private GameObject thirdResponse;
    public ExternalController player;
    private Conversation _currentConversation;

    [SerializeField]
    private string[] emotionStringsArray; //DESIGNER SETUP ONLY, DO NOT ACCESS AFTER DICTIONARY IS BUILT
    [SerializeField]
    private TMP_FontAsset[] emotionFontsArray; //DESIGNER SETUP ONLY, DO NOT ACCESS AFTER DICTIONARY IS BUILT

    private Dictionary<string, TMP_FontAsset> fontDictionary;

    int responseMade;
    bool breathTaken;
    bool started = false;
    int dummyNum = -1;
    int failNum = -1;
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        SetupDictionary();
        //Invoke("RefreshConversation", 0.2f);
    }

    public void DelayedRefreshConversation()
    {
        Invoke("RefreshConversation", 0.1f);
    }
    public void RefreshConversation()
    {
        started = true;
        if(InputManager.Instance.isUsingController)
        {
            firstButton.GetComponent<ButtonWrapper>().Select();
        }

        _currentConversation = DialogueManager.databaseManager.defaultDatabase.GetConversation(DialogueManager.LastConversationStarted);
        RefreshText();
    }
    public void SetConversation(Conversation convo)
    {
        _currentConversation = convo;
        //TODO: actually switch out text if this happens
    }

    public bool ChooseResponse(int response)
    {
        exploreCanvas.SetActive(false);
        if(response < 0 || response >= DialogueManager.instance.currentConversationState.subtitle.dialogueEntry.outgoingLinks.Count) { return false; }
        DialogueEntry targetEntry = _currentConversation.GetDialogueEntry(
        DialogueManager.instance.currentConversationState.subtitle.dialogueEntry.outgoingLinks[response].destinationDialogueID);
        ConversationState targetState = DialogueManager.conversationModel.GetState(targetEntry);
        DialogueManager.conversationController.GotoState(targetState);
        DialogueManager.UpdateResponses();
        firstButton.SetActive(false);
        secondButton.SetActive(false);
        thirdButton.SetActive(false);
        RefreshText();
        return true;
    }
    public void ChooseFirstResponse()
    {
        if (failNum == 0)
        {
            RestartConversation();
            return;
        }
        ChooseResponse(0);
        secondResponse.SetActive(false);
        thirdResponse.SetActive(false);
        responseMade = 1;
        if(breathTaken) {
          //  AudioManager.Instance.PlayEvent(FMODEvents.Instance.ChaseOption1, Camera.main.transform.position);
        }
    }

    public void ChooseSecondResponse()
    {
        if (failNum == 1)
        {
            RestartConversation();
            return;
        }
        ChooseResponse(1);
        responseMade = 2;
        firstResponse.SetActive(false);
        thirdResponse.SetActive(false);
        if (breathTaken) {
          //  AudioManager.Instance.PlayEvent(FMODEvents.Instance.ChaseOption2, Camera.main.transform.position);
        }
    }

    public void ChooseThirdResponse()
    {
        if (failNum == 2)
        {
            RestartConversation();
            return;
        }
        ChooseResponse(2);
        responseMade = 3;
        firstResponse.SetActive(false);
        secondResponse.SetActive(false);
        if (breathTaken) {
            //AudioManager.Instance.PlayEvent(FMODEvents.Instance.ChaseOption3, Camera.main.transform.position);
        }
    }

    public void ChooseFourthResponse()
    {
        if (failNum == 3)
        {
            RestartConversation();
            return;
        }
        ChooseResponse(3);
    }

    public void SelectFirstOption()
    {
        firstButton.SetActive(true);
        secondButton.SetActive(true);
        thirdButton.SetActive(true);
        if (InputManager.Instance.isUsingController)
        {
            firstButton.GetComponent<ButtonWrapper>().Select();
        }
    }

    public void RefreshTextState()
    {
        Invoke("RefreshText", 0.25f);
    }
    protected void RefreshText()
    {
        promptText.QueueText(DialogueManager.currentConversationState.subtitle.dialogueEntry.DialogueText);
        if (fontDictionary.TryGetValue(DialogueManager.currentConversationState.subtitle.dialogueEntry.userScript, out TMP_FontAsset afont))
        {
            promptText.SetFont(afont);
        }
        else
        {
            promptText.ClearFont();
        }
        conversationSpawner.BreathingInstructionInactive();
        //promptText.SetSpeaker(DialogueManager.databaseManager.defaultDatabase.actors[DialogueManager.currentConversationState.subtitle.dialogueEntry.ActorID - 1].Name);
        int currResponse = 0;
        dummyNum = -1;
        failNum = -1;
        for (int i = 0; i <  DialogueManager.currentConversationState.subtitle.dialogueEntry.outgoingLinks.Count; i++)
        {
            DialogueEntry entry = _currentConversation.GetDialogueEntry(DialogueManager.instance.currentConversationState.subtitle.dialogueEntry.outgoingLinks[i].destinationDialogueID);
            if (Lua.IsTrue(entry.conditionsString))
            {
                responseTexts[currResponse].QueueText(entry.currentDialogueText);
                TMP_FontAsset font;
                string scriptText = entry.userScript;
                string fontText = "";
                string controlText = "";
                if (scriptText.Contains(','))
                {
                    string[] data = scriptText.Split(',');
                    fontText = data[0];
                    controlText = data[1];
                }
                else
                {
                    fontText = scriptText;
                }
                if(fontDictionary.TryGetValue(fontText, out font))
                {
                    responseTexts[currResponse].SetFont(font);
                }
                else
                {
                    responseTexts[currResponse].ClearFont();
                }
                if(controlText.Contains("hide", System.StringComparison.OrdinalIgnoreCase))
                {
                    responseTexts[currResponse].SetHidden();
                }
                else if (controlText.Contains("dummy", System.StringComparison.OrdinalIgnoreCase))
                {
                    dummyNum = currResponse;
                }
                else if (controlText.Contains("fail", System.StringComparison.OrdinalIgnoreCase))
                {
                    failNum = currResponse;
                }
                currResponse++;
            }
        }
        for(int i = currResponse; i < responseTexts.Length; i++)
        {
            responseTexts[i].ClearText();
            responseTexts[i].SetButtonEnabled(false);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            originalCamera.SetActive(!originalCamera.activeSelf);
            if (originalCamera.activeSelf)
            {
                originalCamera.transform.position = cameraOriginalPoint.position;
            }
            //zoomCamera.SetActive(!zoomCamera.activeSelf);
        }
        if(started && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)))
        {
            ChooseFirstResponse();
        }
        else if(started && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) && DialogueManager.currentConversationState.subtitle.dialogueEntry.outgoingLinks.Count >= 2)
        {
            ChooseSecondResponse();
        }
        else if(started && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) && DialogueManager.currentConversationState.subtitle.dialogueEntry.outgoingLinks.Count >= 3)
        {
            ChooseThirdResponse();
        }
        else if(started && (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) && DialogueManager.currentConversationState.subtitle.dialogueEntry.outgoingLinks.Count >= 4)
        {
            ChooseResponse(3);
        }
        else if(started && (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) && DialogueManager.currentConversationState.subtitle.dialogueEntry.outgoingLinks.Count >= 5)
        {
            ChooseResponse(4);
        }
        if(DialogueManager.currentConversationState != null) {
            if(started && !DialogueManager.currentConversationState.HasAnyResponses)
            {
                Debug.Log(responseMade);
                /*switch (responseMade)
                {
                    case 0:
                        Invoke("TurnOff", 7.0f);
                        break;
                    case 1:
                        Invoke("TurnOff", 10.0f);
                        break;
                    case 2:
                        Invoke("TurnOff", 7.0f);
                        break;
                }*/
                Invoke("TurnOff", 7.0f);
            }
        }
    }
    private float GetAxisCustom(string axisName)
    {
        if (cameraBrain.IsBlending)
            return 0;
        return Input.GetAxis(axisName);
    }
    private void TurnOff()
    {
        
        originalCamera.SetActive(true);
        if (originalCamera.activeSelf)
        {
            originalCamera.transform.position = cameraOriginalPoint.position;
        }
        //Cursor.visible = false;

        // zoomCamera.SetActive(false);
        exploreCanvas.SetActive(false);
       // finalConvo.gameObject.SetActive(true);
        //finalConvo.paused = false;
        breathTaken = false;
        player.SetPaused(false);
        dialogueCanvas.SetActive(false);
        conversationSpawner.SetPromptActive();
    }

    public void UnhideOptions()
    {
        foreach(FadeableText text in responseTexts)
        {
            text.Unhide();
        }
    }
    public void SetBreathTaken(bool value)
    {
        breathTaken = value;
        if (breathTaken)
        {
            if(dummyNum != -1 && responseTexts[dummyNum].IsPointerOver())
            {
                responseTexts[dummyNum].SetHidden();
                //UnhideOptions();
            }
        }
    }

    public void RestartConversation()
    {
        DialogueManager.StartConversation(_currentConversation.Name);
        RefreshText();
    }

    private void TurnCanvasOn()
    {
        dialogueCanvas.SetActive(true);
    }

    private void SetupDictionary()
    {
        fontDictionary = new Dictionary<string, TMP_FontAsset>();
        for (int i = 0; i < emotionFontsArray.Length; i++)
        {
            fontDictionary.Add(emotionStringsArray[i], emotionFontsArray[i]);
        }
    }
}
