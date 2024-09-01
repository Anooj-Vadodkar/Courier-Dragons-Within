using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using TMPro;
using FMODUnity;
using FMOD.Studio;

public class VistaEntranceZone : MonoBehaviour
{
    [SerializeField] private int _vistaNum;
    [SerializeField] private float _delay = 1;
    [SerializeField] private float _delayReplacement = 1;
    [SerializeField] private float _selectRange = 25.0f;

    [SerializeField] private float _startingFov = 29.0f;
    [SerializeField] private float _fovIncrement = 4.0f;

    Vector2 centerScreen;
    float distFromCenter = 200.0f;

    private float breath;
    private bool breathedIn = false;
    [SerializeField] private float breathTime = 1.5f;

    [SerializeField] private LayerMask _mask;

    [SerializeField] private VistaController _vistaController;
    [SerializeField] private TopTextObserver _breathObserver;
    [SerializeField] private DisappearingText[] _disappearingText;
    [SerializeField] private MeshRenderer[] _dialogueTexts;
    //[SerializeField] private GameObject _explorePrompt;
    [SerializeField] private GameObject _reflectPrompt;
    [SerializeField] private CinemachineVirtualCamera _cam;
    [SerializeField] private VistaReticle[] _reticles;
    [SerializeField] private PlayerReticle playerReticle;
    [SerializeField] private CanvasGroup cursorAlpha;
    [SerializeField] private DisappearingText[] _replacementTexts;
    [SerializeField] private GameObject _exitPrompt;
    [SerializeField] private GameObject _breathePrompt;
    [SerializeField] private ChangeScript convoNav;
    [SerializeField] private GameObject finishedCam;
    [SerializeField] private bool canMove = false;
    [SerializeField] private LoopingAudioInstance cnavAudioLoop;
    [SerializeField] private float timeToDisplay = 2.0f;
    [SerializeField] private List<GameObject> finalShotCams;
    [SerializeField] private GameObject externalAIOne;
    [SerializeField] private GameObject externalAITwo;

    private InputManager _inputManager;
    private GraphicRaycaster _gr;
    private bool textFinished = false;
    private bool musicTriggered = false;

    private int _thoughtsCompleted = 0;

    private bool _firstTimeTriggered = false;
    private bool[] _triggered;
    private float _targetFov;
    private bool _canLeaveVista = false;
    private bool breathingIn = false;
    private EventInstance breathIn;

    private void Start()
    {
        _gr = GetComponent<GraphicRaycaster>();
        _triggered = new bool[_disappearingText.Length];
        _cam = GetComponent<CinemachineVirtualCamera>();
        _cam.m_Lens.FieldOfView = _startingFov;
        _targetFov = _startingFov;
        _inputManager = InputManager.Instance;
        Invoke("DisableTexts", 1.0f);
        foreach(DisappearingText text in _replacementTexts)
        {
            text.SetVisibility(true); // This is backwards lol
        }
        _exitPrompt.gameObject.SetActive(false);
        _breathePrompt.gameObject.SetActive(false);

        breathIn = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.breathInOutside);
    }

    private void DisableTexts()
    {
        for (int i = 0; i < _disappearingText.Length; i++)
        {
            _dialogueTexts[i].enabled = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!_firstTimeTriggered && other.gameObject.CompareTag("Player") && _inputManager.GetDismountInputPressed())
        {
            _reflectPrompt.SetActive(false);
            _firstTimeTriggered = true;
            _vistaController.EnterVista(_vistaNum);
            _disappearingText[_thoughtsCompleted].EnableText(8.5f);
            if(externalAIOne != null && externalAITwo != null)
            {
                externalAIOne.SetActive(false);
                externalAITwo.SetActive(true);
            }
            
            // _dialogueTexts[_thoughtsCompleted].enabled = true;
            // _disappearingText[_thoughtsCompleted].PlayAudioDelayed();
            if (convoNav != null)
                convoNav.Unpause();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            if(!_firstTimeTriggered) {
                if(!musicTriggered) {
                    cnavAudioLoop.StartLoopingTrack();
                    musicTriggered = true;
                }
                _reflectPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            _reflectPrompt.SetActive(false);
    }

    private void Update()
    {
        if(_firstTimeTriggered && _thoughtsCompleted < _disappearingText.Length)
        {
            //Camera panning with cursor stuff
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            Vector3 updatedPos = _vistaController._crosshair.rectTransform.localPosition + input * _vistaController._crosshairMoveSpeed * Time.deltaTime;
            _vistaController._crosshair.rectTransform.localPosition = new Vector3(Mathf.Clamp(updatedPos.x, -_vistaController._xMaxRange, _vistaController._xMaxRange), Mathf.Clamp(updatedPos.y, -_vistaController._yMaxRange, _vistaController._yMaxRange), 0);
            float xDist = _vistaController._xMaxRange - Mathf.Abs(updatedPos.x);
            float yDist = _vistaController._yMaxRange - Mathf.Abs(updatedPos.y);
            CinemachinePOV pov = _cam.GetCinemachineComponent<CinemachinePOV>();
            if (xDist < _vistaController._distFromEdgeToMove && Mathf.Abs(input.x) / input.x == Mathf.Abs(updatedPos.x) / updatedPos.x)
            {
                pov.m_HorizontalAxis.m_InputAxisValue = input.x * .75f;
            }
            else
            {
                pov.m_HorizontalAxis.m_InputAxisValue = 0;
            }
            if(yDist < _vistaController._distFromEdgeToMove && Mathf.Abs(input.y)/input.y == Mathf.Abs(updatedPos.y)/updatedPos.y)
            {
                pov.m_VerticalAxis.m_InputAxisValue = input.y * .75f;
            }
            else
            {
                pov.m_VerticalAxis.m_InputAxisValue = 0;
            }

            //The below handles checking if we are over and have breathed on a text, as well as the associated juice

            //Changing this from center of screen to cursor position
            centerScreen = _vistaController._crosshair.rectTransform.position;
            if(!_triggered[_thoughtsCompleted] && _dialogueTexts[_thoughtsCompleted].enabled)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(_disappearingText[_thoughtsCompleted].transform.position);
                Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);
                distFromCenter = Vector2.Distance(centerScreen, screenPos2D);
                if (distFromCenter <= _selectRange)
                {
                    _reticles[_thoughtsCompleted].SetCursorDistance(Mathf.Clamp((_selectRange - distFromCenter) / (_selectRange - 10f), 0f, 1f));
                    if(!_breathePrompt.gameObject.activeSelf)
                    {
                        _breathePrompt.gameObject.SetActive(true);
                        cursorAlpha.alpha = 0f;
                    }
                    if (_breathObserver.IsOverThreshold && _breathObserver.hasJumped)
                    {
                        _breathePrompt.gameObject.SetActive(false);
                        cursorAlpha.alpha = 0.5f;
                        _triggered[_thoughtsCompleted] = true;
                        StartCoroutine("StartFade", _thoughtsCompleted);
                        _reticles[_thoughtsCompleted].Disable();
                        _thoughtsCompleted++;
                        canMove = false;
                        if(_thoughtsCompleted < _dialogueTexts.Length)
                        {
                            // _dialogueTexts[_thoughtsCompleted].enabled = true;
                            _disappearingText[_thoughtsCompleted].EnableText(20.0f);
                        }
                        _targetFov += _fovIncrement;
                        if (_thoughtsCompleted == _disappearingText.Length && !textFinished)
                        {
                            StartCoroutine(TextFinished());
                            _vistaController._crosshair.enabled = false;
                            _vistaController._breathFilled.enabled = false;
                            textFinished = true;
                            /* AudioManager.Instance.PlayEvent(FMODEvents.Instance.riffsTogether);
                            _vistaController._crosshair.enabled = false;
                            finishedCam.SetActive(true);
                            _canLeaveVista = true;
                            _exitPrompt.gameObject.SetActive(true); */
                        }
                    }
                }
                else
                {
                    _reticles[_thoughtsCompleted].SetCursorDistance(0);
                    _breathePrompt.gameObject.SetActive(false);
                    cursorAlpha.alpha = 0.5f;
                }
                if (Input.GetKeyDown(KeyCode.O))
                {
                    StartCoroutine("LeaveVista");
                }
            }
        }

        if(_canLeaveVista && _inputManager.GetDismountInputPressed())
        {
            Invoke("DisableTexts", 1.0f);
            StartCoroutine("LeaveVista");
        }


        // Check if holding breath
        if (InputManager.Instance.GetBreathInput() == 1)
        {
            if(!breathingIn) {
                breathingIn = true;
                breathIn.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
                breathIn.start();
            }
            // if haven't started beathing in then start playing breath sound.
            if (breath < breathTime)
            {
                breath += Time.deltaTime;
                if (breath >= breathTime)
                {
                    breath = breathTime;

                    // fully breathed in, wait for release
                    breathedIn = true;
                }
            }
            if (_thoughtsCompleted < _reticles.Length && distFromCenter <= _selectRange)
            {
/*                playerReticle.SetBreathAmount(Mathf.Clamp(breath / breathTime, 0f, 1f));*/
                _reticles[_thoughtsCompleted].SetBreathAmount(Mathf.Clamp(breath / breathTime, 0f, 1f));
            }
        }
        else
        {
            if(breathingIn) {
                breathingIn = false;
                /* AudioManager.Instance.PlayEvent(FMODEvents.Instance.breathOutOutside); */
                breathIn.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            // if the player has fully breathed in ...
            if (breathedIn)
            {
                breathedIn = false;
                AudioManager.Instance.PlayEvent(FMODEvents.Instance.breathOutOutside);
                // breathIn.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            if (breath > 0)
            {
                breath -= Time.deltaTime;
                if (breath <= 0)
                {
                    breath = 0;
                }
            }

            if (_thoughtsCompleted < _reticles.Length && distFromCenter <= _selectRange)
            {
                playerReticle.SetBreathAmount(Mathf.Clamp(breath / breathTime, 0f, 1f));
                _reticles[_thoughtsCompleted].SetBreathAmount(Mathf.Clamp(breath / breathTime, 0f, 1f));
            }
        }

        if (_cam.m_Lens.FieldOfView != _targetFov)
        {
            _cam.m_Lens.FieldOfView = Mathf.Lerp(_cam.m_Lens.FieldOfView, _targetFov, 0.25f * Time.deltaTime);
        }
    }

    private IEnumerator TextFinished() {
        yield return new WaitForSeconds(12.0f);
        AudioManager.Instance.PlayEvent(FMODEvents.Instance.riffsTogether);
        _vistaController._crosshair.enabled = false;

        // Loops through final shot cams, should be nested in while loop for final implementation
        if(finalShotCams.Count == 3) {
            Debug.Log("Hit final shot cams");
            finalShotCams[0].SetActive(true);
            yield return new WaitForSeconds(6.75f); // dialogue 1
            finalShotCams[0].SetActive(false);
            finalShotCams[1].SetActive(true);
            yield return new WaitForSeconds(7.55f); // dialogue 2
            finalShotCams[1].SetActive(false);
            finalShotCams[2].SetActive(true);
            yield return new WaitForSeconds(10.7f); // dialogue 3
            finalShotCams[2].SetActive(false);
            AudioManager.Instance.PlayEvent(FMODEvents.Instance.dragonSFX);
        }
        finishedCam.SetActive(true);
        _canLeaveVista = true;
        _exitPrompt.gameObject.SetActive(true);
    }

    private IEnumerator StartFade(int textNum)
    {
        yield return new WaitForSeconds(_delay);
        if (_disappearingText != null)
        {
            _disappearingText[textNum].StartFade();
            StartCoroutine("FadeReplacementText", textNum);
        }
    }

    private IEnumerator FadeReplacementText(int textNum)
    {
        if (_replacementTexts[textNum] != null)
        {
            yield return new WaitForSeconds(_delayReplacement);
            if (convoNav != null && convoNav.gameObject.activeInHierarchy)
            {
                convoNav.Unpause();
            }
            _replacementTexts[textNum].SetVisibility(false);
            _replacementTexts[textNum].StartAppear();
        }
    }

    public void ShowNextThought()
    {
        _dialogueTexts[_thoughtsCompleted].enabled = true;
    }

    private IEnumerator LeaveVista()
    {
        _exitPrompt.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        DisableTexts();
        // cnavAudioLoop.StopLoopingTrack();
        _vistaController.LeaveVista();
        if (convoNav != null && convoNav.gameObject.activeInHierarchy)
        {
            convoNav.Unpause();
        }
    }

    public void SetCanMove(bool val)
    {
        canMove = val;
    }
    private IEnumerator InstaLeaveVista()
    {
        yield return new WaitForSeconds(0.0f);
        _vistaController.LeaveVista();
    }
}
