using EasyRoads3Dv3;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EntryController : MonoBehaviour

{
    public enum EntryState { Inactive, Active, Written }
    public EntryState CurrentState = EntryState.Inactive;

    public EntryActivationTestScript EntryActivationScript;

    public GameObject SelectorObject;
    public GameObject breathIcon;
    public GameObject breathFillIcon;
    public AnimationCurve iconSpeed;
    public GameObject CursorSelectorObject;
    public GameObject EntryPrompt;
    public GameObject FinalEntry;
    public Button entryButton;
    public Button.ButtonClickedEvent onClick;

    [SerializeField] private float breathMax = 1;
    private float breathAmt = 0;
    private bool entryActivated;
    private bool isEnabled = true;

    private Vector3 startingBreathScale;



    // Start is called before the first frame update
    void Start()
    {
        // entryButton.onClick.AddListener(OnEntryClick);
        startingBreathScale = breathFillIcon.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();

        if(isEnabled) {
            if(InputManager.Instance.GetBreathInput() != 0) {
                // player is breathing
                if(breathAmt != breathMax) {
                    breathAmt += Time.deltaTime;
                    if(breathAmt > breathMax) {
                        entryActivated = true;
                        breathAmt = breathMax;
                    }
                }
            } else {
                // player is not breathing
                if(entryActivated) {
                    OnEntryClick();
                    entryActivated = false;
                    isEnabled = false;
                }
                if(breathAmt != 0) {
                    breathAmt -= Time.deltaTime * 2;
                    if(breathAmt < 0) {
                        breathAmt = 0;
                    }
                }
            }
        }

        if(breathIcon != null) {
            breathIcon.transform.Rotate(Vector3.forward * iconSpeed.Evaluate(breathAmt));
        }
        if(breathFillIcon) {
            breathFillIcon.transform.localScale = Vector3.Lerp(Vector3.zero, startingBreathScale, breathAmt);
        }

/* 
        // Increase scale of breath icon
        breathIcon.transform.localScale = Vector3.Lerp(Vector3.zero, startingBreathScale, breath);
        // Icon rotation
        icon.transform.Rotate(Vector3.forward * iconSpeed.Evaluate(breath)); */
    }

    public void UpdateUI()
    {

        switch (CurrentState)
        {
            case EntryState.Inactive:
                SetInactiveState();
                break;
            case EntryState.Active:
                SetActiveState();
                break;
            case EntryState.Written:
                SetWrittenState();
                break;
        }
    }

    void OnEntryClick()
    {
        if (CurrentState == EntryState.Active)
        {
            CurrentState = EntryState.Written;
        }
    }

    public void ActivateEntry()
    {
        CurrentState = EntryState.Active;
;    }

    void SetInactiveState()
    {
        entryButton.interactable = false;
        EntryPrompt.SetActive(false);
    }

    public void SetActiveState()
    {
        entryButton.interactable = true;
        EntryPrompt.SetActive(true);

    }

    void SetWrittenState()
    {
        entryButton.interactable = true;
        entryButton.onClick.RemoveAllListeners();
        FinalEntry.SetActive(true);
        EntryPrompt.SetActive(false);
        CursorSelectorObject.SetActive(false);
        GetComponent<Image>().color = Color.clear;
    }


}
