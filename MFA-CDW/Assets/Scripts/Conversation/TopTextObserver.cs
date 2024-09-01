using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;


/// <summary>
/// This script observes the slider values, and at key thresholds enables/disables the surrounding text.
/// </summary>
public class TopTextObserver : MonoBehaviour
{

    //Declare references for the Slider
    [SerializeField] GameObject Slider;
    [SerializeField] GameObject TopText;
    [SerializeField] GameObject BottomText;
    [SerializeField] ConversationSpawner conversationSpawner;
    [SerializeField] Text _toptext;
    [SerializeField] Text _bottomtext;
    [SerializeField] public double _value; //TODO change to doubles and add casts
    [SerializeField] public double _changePerSecond;
    [SerializeField] private float _breatheThreshold = 50f;
    [SerializeField] private string _conversationName = "New Conversation 2";
    private InputManager inputManager;
    public bool hasJumped = false;

    public bool IsOverThreshold => _value > 25.0f;

    //Note to self: Do I need this reference? Test!
    //[SerializeField] DialogueSystemController DialogueSystemController;

    //The conversation we'd like to reference?
    //[ConversationPopup] public string conversation;

    // Start is called before the first frame update
    void Start()
    {
       
        
        //Set Slider Value to 0.
        //_slider = GetComponent<Slider>();
        _toptext = TopText.GetComponent<Text>();
        _bottomtext= BottomText.GetComponent<Text>();
        inputManager = InputManager.Instance;
        //Debug.Log("the slider's value is " + _value);

    }

    // Update is called once per frame
    void Update()
    {

        _value = Slider.GetComponent<Slider>().value;

        if (inputManager.GetBreathInput() > 0) 
        {
            // Debug.Log("increasing value!");

            Slider.GetComponent<Slider>().value += (float)_changePerSecond * Time.deltaTime;
        }
        else
        {
            Slider.GetComponent<Slider>().value -= (float)_changePerSecond * Time.deltaTime; 
        }


        //If Value >= 50, enable TopText; else, disable TopText.
        // Debug.Log("The slider's value is " + _value);
        if (_value <= 50)
        {
            // Debug.Log("Turning off TopText");
            _toptext.enabled = false;
        }
        else
        {
            // Debug.Log("Turning On TopText");
            _toptext.enabled= true;

            //TESTING: Will it update the responses at runtime? What variables do I need?
            //DialogueManager.UpdateResponses();
            if (!hasJumped)
            {
                //GoToLastOption();
                //hasJumped = true;
            }
            // Debug.Log("Responses Updated!");
        }

        if(_value < 25)
        {
            // Debug.Log("Turning Off BottomText");
            _bottomtext.enabled= false;
        }
        else
        {
            // Debug.Log("MediumBreath achieved!");

            //update variable in Conversation

            // Debug.Log("Turning On BottomText");
            _bottomtext.enabled= true;

            //TESTING: Will it update the responses at runtime? What variables do I need?
            //DialogueManager.UpdateResponses();
            if (!hasJumped)
            {
               // GoToLastOption();
                hasJumped = true;
            }
            // Debug.Log("Responses Updated!");
            
        }
    }

    void OnEnable()
    {
        Lua.RegisterFunction(nameof(IsSliderHighEnough), this, SymbolExtensions.GetMethodInfo(() => IsSliderHighEnough()));
    }

    void OnDisable()
    {
        // Note: If this script is on your Dialogue Manager & the Dialogue Manager is configured
        // as Don't Destroy On Load (on by default), don't unregister Lua functions.
        Lua.UnregisterFunction(nameof(IsSliderHighEnough)); // <-- Only if not on Dialogue Manager.
    }
    public bool IsSliderHighEnough()
    {
        return _value > _breatheThreshold;
    }

    private void GoToLastOption()
    {
        conversationSpawner.BreathingInstructionInactive();
        Conversation convo = DialogueManager.databaseManager.defaultDatabase.GetConversation(DialogueManager.LastConversationStarted);
        DialogueEntry targetEntry = convo.GetDialogueEntry(
        DialogueManager.instance.currentConversationState.subtitle.dialogueEntry.outgoingLinks[DialogueManager.instance.currentConversationState.subtitle.dialogueEntry.outgoingLinks.Count - 1].destinationDialogueID);
        ConversationState targetState = DialogueManager.conversationModel.GetState(targetEntry);
        DialogueManager.conversationController.GotoState(targetState);
        DialogueManager.UpdateResponses();
        DialogueWrapper._instance.SetBreathTaken(true);
        DialogueWrapper._instance.RefreshTextState();
        Debug.LogError("Moved to last option");
    }

}
