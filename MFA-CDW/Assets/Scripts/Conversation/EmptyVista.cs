using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using TMPro;

public class EmptyVista : MonoBehaviour
{
    [SerializeField] private int _vistaNum = 0;
    [SerializeField] private float _delay = 1;
    [SerializeField] private float _delayReplacement = 1;
    [SerializeField] private float _selectRange = 25.0f;

    [SerializeField] private float _startingFov = 29.0f;
    [SerializeField] private float _fovIncrement = 4.0f;

    [SerializeField] private VistaController _vistaController;
    //[SerializeField] private GameObject _explorePrompt;
    [SerializeField] private GameObject _reflectPrompt;
    [SerializeField] private CinemachineVirtualCamera _cam;
    [SerializeField] private GameObject _exitPrompt;
    [SerializeField] private GameObject _breathePrompt;
    [SerializeField] private ChangeScript convoNav;
    [SerializeField] private GameObject finishedCam;
    private InputManager _inputManager;

    private bool _firstTimeTriggered = false;
    private float _targetFov;
    private bool _canLeaveVista = false;

    private void Start()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
        _cam.m_Lens.FieldOfView = _startingFov;
        _targetFov = _startingFov;
        _inputManager = InputManager.Instance;
        _exitPrompt.gameObject.SetActive(false);
        _breathePrompt.gameObject.SetActive(false);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!_firstTimeTriggered && other.gameObject.CompareTag("Player") && _inputManager.GetDismountInputPressed())
        {
            _reflectPrompt.SetActive(false);
            _firstTimeTriggered = true;
            _vistaController.EnterVista(_vistaNum);
            _vistaController._crosshair.enabled = false;
            convoNav.gameObject.SetActive(true);
        }
        //_explorePrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            _reflectPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            _reflectPrompt.SetActive(false);
    }

    private void Update()
    {
        if (_firstTimeTriggered)
        {
            //Camera panning with cursor stuff
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            //Vector3 updatedPos = _vistaController._crosshair.rectTransform.localPosition + input * _vistaController._crosshairMoveSpeed * Time.deltaTime;
            //_vistaController._crosshair.rectTransform.localPosition = updatedPos;
            CinemachinePOV pov = _cam.GetCinemachineComponent<CinemachinePOV>();
            pov.m_HorizontalAxis.m_InputAxisValue = input.x * .75f;
            pov.m_VerticalAxis.m_InputAxisValue = input.y * .75f;
            if (Input.GetKeyDown(KeyCode.O))
            {
                StartCoroutine("InstaLeaveVista");
            }
        }

        if (_canLeaveVista && _inputManager.GetDismountInputPressed())
        {
            StartCoroutine("LeaveVista");
        }

        if (_cam.m_Lens.FieldOfView != _targetFov)
        {
            _cam.m_Lens.FieldOfView = Mathf.Lerp(_cam.m_Lens.FieldOfView, _targetFov, 0.25f * Time.deltaTime);
        }
    }

    public void SetAbleToLeave()
    {
        _canLeaveVista = true;
        _exitPrompt.gameObject.SetActive(true);
    }

    private IEnumerator LeaveVista()
    {
        yield return new WaitForSeconds(0.5f);
        _exitPrompt.gameObject.SetActive(false);
        _vistaController.LeaveVista();
        if (convoNav != null && convoNav.gameObject.activeInHierarchy)
        {
            convoNav.Unpause();
        }
    }

    private IEnumerator InstaLeaveVista()
    {
        yield return new WaitForSeconds(0.0f);
        _vistaController.LeaveVista();
    }
}
