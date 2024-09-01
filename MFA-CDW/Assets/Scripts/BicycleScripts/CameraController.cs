using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using FMODUnity;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CinemachineBrain))]
public class CameraController : MonoBehaviour
{
    private static CameraController _instance;

    public static CameraController Instance{
        get{
            return _instance;
        }
    }

    private InputManager inputManager;

    [SerializeField]
    private GameObject firstPersonCam;
    [SerializeField]
    private GameObject thirdPersonCam;
    [SerializeField]
    private GameObject playerModel;
    private bool firstPersonActive = false;

    private CinemachineBrain brain;

    private void Awake() {
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        brain = GetComponent<CinemachineBrain>();
    }

    private void Start() {
        inputManager = InputManager.Instance;

        thirdPersonCam.SetActive(true);
        firstPersonCam.SetActive(false);
        playerModel.SetActive(true);

        if(GetComponent<StudioEventEmitter>()) {
            AudioManager.Instance.SetAmbientEvent(GetComponent<StudioEventEmitter>());
        } else {
            Debug.Log("No Studio Event Emitter on the Camera object for ambient sounds");
        }
    }

    private void Update() {
/*         if(inputManager.SwitchCamPressedThisFrame()) {
            // Switch to ThirdPersonCamera
            if(firstPersonActive) {
                firstPersonCam.SetActive(false);
                thirdPersonCam.SetActive(true);
                playerModel.SetActive(true);
            // Switch to FirstPersonCamera
            } else {
                firstPersonCam.SetActive(true);
                thirdPersonCam.SetActive(false);
                playerModel.SetActive(false);
            }
            firstPersonActive = !firstPersonActive;
        } */
    }

    public bool isInFirstPersonMode() {
        return firstPersonActive;
    }

    public bool isInThirdPersonMode() {
        return !firstPersonActive;
    }

    public GameObject GetThirdPersonCam() {
        return thirdPersonCam;
    }

    public GameObject GetFirstPersonCam() {
        return firstPersonCam;
    }

    public GameObject GetCurrentCam() {
        if(brain.ActiveVirtualCamera != null) {
            return brain.ActiveVirtualCamera.VirtualCameraGameObject;
        } else {
            return null;
        }
    }
}
