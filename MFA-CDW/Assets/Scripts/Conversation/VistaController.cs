using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class VistaController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] _vistaCams;

    [SerializeField] private DialogueWrapper _dialogueWrapper;
    [SerializeField] private ExternalController _externalController;
    [SerializeField] private GameObject meditationGate;
    [SerializeField] private ChangeScript convoNav;
    [SerializeField] private GameObject finishedCamera;
    [SerializeField] private List<GameObject> finalShotCams;
    [SerializeField] private GameObject vistaDisable;
    [SerializeField] private PlayerReticle reticle;
    // [SerializeField] private LoopingAudioInstance cnavAudioLoop;
    public float _crosshairMoveSpeed;
    public float _yMaxRange;
    public float _xMaxRange;
    public float _distFromEdgeToMove;

    public Image _crosshair;
    public Image _breathFilled;

    private int _currVista = 0;
    private int _completedVistas = 0;

    private void Start()
    {
        _crosshair.enabled = false;
    }

    public void EnterVista(int vistaNum)
    {
        _currVista = vistaNum;
        _crosshair.enabled = true;
/*        if (_breathFilled != null)
            _breathFilled.color = new Color(_breathFilled.color.r, _breathFilled.color.g, _breathFilled.color.b, 1.0f);*/
        if(reticle != null)
        {
            reticle.SetBreathOn();
        }
        if(convoNav != null)
        {
            convoNav.Unpause();
        }
        for(int i = 0; i < _vistaCams.Length; i++)
        {
            if(i == vistaNum)
            {
                _vistaCams[vistaNum].Priority = 101;
                //_vistaCams[vistaNum].m_Lens.FieldOfView = _startingFov;
                _externalController.paused = true;
                _externalController.StopWalkAnimation();
                if (convoNav != null && convoNav.gameObject.activeInHierarchy)
                {
                    convoNav.SetIndex(13);
                }
            }
            else
            {
                _vistaCams[i].Priority = 5;
            }
        }
    }

    public void LeaveVista()
    {
        _externalController.paused = false;
        _crosshair.enabled = false;
        if(_breathFilled != null)
            _breathFilled.color = new Color(_breathFilled.color.r, _breathFilled.color.g, _breathFilled.color.b, 0.0f);
        if (_breathFilled != null)
            _breathFilled.enabled = false;
        if(_vistaCams.Length > 0)
            _vistaCams[_currVista].Priority = 5;
        finishedCamera.SetActive(false);
        _currVista = -1;
        _completedVistas++;
        _externalController.StartWalkAnimation();
        if (convoNav != null && convoNav.gameObject.activeInHierarchy)
        {
            convoNav.SetIndex(32);
            convoNav.Unpause();
        }
        if (_completedVistas == _vistaCams.Length)
        {
            _dialogueWrapper.UnhideOptions();
            _dialogueWrapper.SelectFirstOption();
        }
        if(meditationGate) {
            meditationGate.SetActive(true);
        }
        if (vistaDisable != null)
            vistaDisable.SetActive(false);
    }

}
