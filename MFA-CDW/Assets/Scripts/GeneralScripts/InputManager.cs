using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class InputManager : MonoBehaviour
{
    [SerializeField] [Range(0.1f,0.8f)]
    private float controllerHorizDeadzone = 0.3f;

    [SerializeField]
    private float controllerXSensitivity = 100;
    [SerializeField]
    private float controllerYSensitivity = 1;
    [SerializeField]
    private float mouseXSensitivity = 75;
    [SerializeField]
    private float mouseYSensitivity = 0.4f;

    private float currentSensitivity;

    private static InputManager _instance;
    private CameraController cameraController;

    public bool isUsingController = false;
    private bool usingControllerLastFrame = false;

    public static InputManager Instance {
        get {
            return _instance;
        }
    }

    private PlayerInputScheme playerInput;

    private void Awake() {
        cameraController = CameraController.Instance;
        playerInput = new PlayerInputScheme();
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        UpdateSensitivity();
    }

    private void FixedUpdate() {
        GetMouseDelta();

        if(isUsingController != usingControllerLastFrame) {
            usingControllerLastFrame = isUsingController;
            UpdateSensitivity();
        }
    }

    private void OnEnable() {
        playerInput.Enable();
    }

    private void OnDisable() {
        playerInput.Disable();
    }

    // Update sensitivity based on control method
    public void UpdateSensitivity() {
        if(!GameManager.Instance.isInMeditation()) {
            if(cameraController == null) {
                cameraController = CameraController.Instance;
            }
            if(cameraController.GetCurrentCam() != null) {
                if(cameraController.GetCurrentCam().GetComponent<CinemachineFreeLook>() != null) {
                    if(isUsingController) {
                        cameraController.GetCurrentCam().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = controllerXSensitivity;
                        cameraController.GetCurrentCam().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = controllerYSensitivity;
                    } else {
                        cameraController.GetCurrentCam().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = mouseXSensitivity;
                        cameraController.GetCurrentCam().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = mouseYSensitivity;
                    }
                }
            }
        }
    }

    // Return movement input vector
    public Vector2 GetPlayerMovement() {
        Vector2 returnVec = playerInput.Player.Move.ReadValue<Vector2>();
        CheckControllerInput(playerInput.Player.Move);
        Vector2 returnVecCopy = returnVec;
        if(returnVec.x <= controllerHorizDeadzone && returnVec.x >= -controllerHorizDeadzone) {
            returnVec = new Vector2(0, returnVec.y);
        }
        if(returnVec.x != 0 && !(returnVec.x >= 0.99f) && !(returnVec.x <=-0.99f)) {
            if(returnVec.x > 0) {
                returnVec.x = 1 - (1 - returnVec.x) / (1 - controllerHorizDeadzone);
            } else if(returnVec.x < 0) {
                returnVec.x = -1 - (-1 - returnVec.x) / (1 - controllerHorizDeadzone);
            }
        }
        return returnVec;
    }

    // How much has the mouse moved
    public Vector2 GetMouseDelta() {
        CheckControllerInput(playerInput.Player.Look);
        return playerInput.Player.Look.ReadValue<Vector2>();
    }

    public bool GetLeftClick() {
        bool returnVal = playerInput.Player.Click.ReadValue<float>() == 1;
        if(returnVal)
            isUsingController = false;
        return returnVal;
    }

    // Sprint Input value
    public float GetSprintInput() {
        CheckControllerInput(playerInput.Player.Sprint);
        return playerInput.Player.Sprint.ReadValue<float>();
    }

    // Sprint Input pressed this frame
    public bool GetSprintInputPressed() {
        CheckControllerInput(playerInput.Player.Sprint);
        return playerInput.Player.Sprint.triggered;
    }

    // Sprint Input released this frame
    public bool GetSprintInputReleased() {
        CheckControllerInput(playerInput.Player.Sprint);
        return playerInput.Player.Sprint.WasReleasedThisFrame();
    }

    // Hop input pressed
    public float GetHopInput() {
        CheckControllerInput(playerInput.Player.BunnyHop);
        return playerInput.Player.BunnyHop.ReadValue<float>();
    }

    // Hop input released this frame
    public bool PlayerHopReleasedThisFrame() {
        CheckControllerInput(playerInput.Player.BunnyHop);
        return playerInput.Player.BunnyHop.WasReleasedThisFrame();
    }

    // Restart pressed this frame
    public bool PlayerRestartedThisFrame() {
        CheckControllerInput(playerInput.Player.Restart);
        return playerInput.Player.Restart.triggered;
    }

    // Quit Input pressed this frame
    public bool GetQuitGamePressed() {
        CheckControllerInput(playerInput.Debug.Quit);
        return playerInput.Debug.Quit.triggered;
    }

    // Breath input value
    public float GetBreathInput() {
        CheckControllerInput(playerInput.Player.Breath);
        return playerInput.Player.Breath.ReadValue<float>();
    }

    // Breath input pressed this frame (Biking)
    public bool GetBreathInputPressed() {
        CheckControllerInput(playerInput.Player.Breath);
        return playerInput.Player.Breath.triggered;
    }

    // Breath Input released this frame (Biking)
    public bool GetBreathInputReleased() {
        CheckControllerInput(playerInput.Player.Breath);
        return playerInput.Player.Breath.WasReleasedThisFrame();
    }

    // Dismount input presssed
    public bool GetDismountInputPressed() {
        CheckControllerInput(playerInput.Player.Dismount);
        return playerInput.Player.Dismount.triggered;
    }

    // Reload debug pressed
    public bool GetReloadPressed() {
        CheckControllerInput(playerInput.Debug.Reload);
        return playerInput.Debug.Reload.triggered;
    }

    public bool GetConfrontPressed() {
        return playerInput.Debug.Confront.triggered;
    }

    // Trasition to meditation scene debug input pressed
    public bool GetMediScenePressed() {
        CheckControllerInput(playerInput.Debug.MediScene);
        return playerInput.Debug.MediScene.triggered;
    }

    // Transition to convorsation scene debut input pressed
    public bool GetBikingConvoScenePressed() {
        CheckControllerInput(playerInput.Debug.MediScene);
        return playerInput.Debug.BikingConvoScene.triggered;
    }

    public bool GetPathTestPressed() {
        CheckControllerInput(playerInput.Debug.PathTest);
        return playerInput.Debug.PathTest.triggered;
    }

    public float GetSensitivity() {
        return currentSensitivity;
    }

    private void CheckControllerInput(InputAction action) {
        if(action.activeControl != null) {
            // Debug.Log(action.activeControl.path);
            if(action.activeControl.path.Substring(1,8) == "Keyboard" || action.activeControl.path.Substring(1,5) == "Mouse") {
                // using Keyboard
                isUsingController = false;
            } else {
                // using Controller
                isUsingController = true;
            }
        }
    }
}
