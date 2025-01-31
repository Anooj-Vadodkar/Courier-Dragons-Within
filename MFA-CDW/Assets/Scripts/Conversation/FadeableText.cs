 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;
using UnityEngine.EventSystems;

public class FadeableText : MonoBehaviour
{
    [SerializeField]
    private float timeBeforeShow = 0.25f;
    [SerializeField]
    private float resetTime = 0.25f;

    [SerializeField]
    protected TextMeshProUGUI onDeckText;
    [SerializeField]
    protected TextMeshProUGUI currentText;
    [SerializeField]
    protected TextMeshProUGUI speakerName;
    [SerializeField]
    protected ClickableText clickableTextOption;
    [SerializeField]
    public ButtonWrapper button;

    private string currentFontTag;

    [SerializeField]
    private bool isPrompt;
    [SerializeField]
    private int responseNumber;

    public bool IsPrompt => isPrompt;

    private Queue<string> _textQueue = new Queue<string>();
    private bool _isReadyToRefresh = true;

    private TextFader _textFader;
    private bool _hidden = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(_isReadyToRefresh && _textQueue.Count > 0)
        {
            ShowNextText();
        }
    }

    public bool IsPointerOver()
    {
        return button.IsItHighlighted();
    }

    public void SetFont(TMP_FontAsset font)
    {
        currentFontTag = "<font=\"" + font.name + "\">";
    }

    public void ClearFont()
    {
        currentFontTag = "";
    }

    public void QueueText(string text)
    {
        _textQueue.Enqueue(text);
    }

    public void SetSpeaker(string name)
    {
        speakerName.text = name + ":";
    }
    private void ShowNextText()
    {
        if(!_hidden)
        {
            _isReadyToRefresh = false;
            string toShow = _textQueue.Dequeue();
            toShow = currentFontTag + toShow + "</font>";
            onDeckText.text = toShow;
            _textFader.FadeText(currentText, false);
            Invoke("ShowOnDeck", timeBeforeShow);
        }
    }

    public void SetHidden()
    {
        _hidden = true;
        TurnOffTexts();
        if (button != null)
        {
            button.enabled = false;
            button.SetImageVisibility(false);
        }
    }

    private void TurnOffTexts()
    {
        currentFontTag = "";
        currentText.text = " ";
        onDeckText.text = " ";
        currentText.color = new Color(currentText.color.r, currentText.color.g, currentText.color.b, 0);
        onDeckText.color = new Color(onDeckText.color.r, onDeckText.color.g, onDeckText.color.b, 0);
        currentText.ClearMesh();
    }

    public void SetButtonEnabled(bool val)
    {
        if(button != null)
        {
            button.enabled = val;
            button.SetImageVisibility(val);
        }
    }
    public void Unhide()
    {
        if(_hidden)
        {
            
            _hidden = false;
            if(button != null)
            {
                button.enabled = true;
                button.SetImageVisibility(true);
            }

            if (InputManager.Instance.isUsingController)
            {
                button.Select();
            }
            ShowNextText();
        }
    }

    private void HandleFadeOutFinished(TextMeshProUGUI TMProbject, bool fadingIn)
    {

    }
    private void HandleFadeInFinished(TextMeshProUGUI TMProbject, bool fadingIn)
    {
        if (onDeckText == TMProbject)
        {
            currentText.text = onDeckText.text;
            currentText.color = new Color(currentText.color.r, currentText.color.g, currentText.color.b, 1);
            onDeckText.color = new Color(onDeckText.color.r, onDeckText.color.g, onDeckText.color.b, 0);
            Invoke("ReadyToRefresh", resetTime);
        }
    }

    private void HandleTextClicked(ClickableText clickable, PointerEventData eventData)
    {
        if(!_isReadyToRefresh) { return; }
    }

    public void ClearText()
    {
        _hidden = false;
        currentText.text = " ";
        onDeckText.text = " ";
        currentText.color = new Color(currentText.color.r, currentText.color.g, currentText.color.b, 1);
        onDeckText.color = new Color(onDeckText.color.r, onDeckText.color.g, onDeckText.color.b, 0);
        currentText.ClearMesh();
        ReadyToRefresh();
    }
    private void ShowOnDeck()
    {
        if(!_hidden)
        {
            _textFader.FadeText(onDeckText, true);
        }
    }
    private void ReadyToRefresh()
    {
        _isReadyToRefresh = true;
    }

    private void OnEnable()
    {
        _textFader = TextFader._instance;
        _textFader.FadeOutFinished += HandleFadeOutFinished;
        _textFader.FadeInFinished += HandleFadeInFinished;
        if (clickableTextOption)
        {
            clickableTextOption.TextClicked += HandleTextClicked;
        }
    }
    private void OnDisable()
    {
        if (_textFader)
        {
            _textFader.FadeOutFinished -= HandleFadeOutFinished;
            _textFader.FadeInFinished -= HandleFadeInFinished;
            if (clickableTextOption)
            {
                clickableTextOption.TextClicked -= HandleTextClicked;

            }
        }
    }
}
