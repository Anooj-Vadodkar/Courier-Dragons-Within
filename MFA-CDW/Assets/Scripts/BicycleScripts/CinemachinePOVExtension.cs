using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtension : CinemachineExtension
{
    [SerializeField]
    private float horizontalSpeed = 10.0f;
    [SerializeField]
    private float verticalSpeed = 10.0f;
    [SerializeField]
    private float clampXAngle = 80.0f;
    [SerializeField]
    private float clampYAngle = 80.0f;

    private InputManager inputManager;
    private Vector3 startingRotation;

    protected override void Awake() {
        startingRotation = transform.localRotation.eulerAngles;
        // startingRotation = transform.localEulerAngles;
        // startingRotation = transform.rotation.eulerAngles;
        // startingRotation = transform.loca
        inputManager = InputManager.Instance;
        base.Awake();
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
        if (vcam.Follow) {
            if (stage == CinemachineCore.Stage.Aim) {
                Vector2 deltaInput = inputManager.GetMouseDelta();
                startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
                startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime;
                startingRotation.x = Mathf.Clamp(startingRotation.x, -clampXAngle, clampXAngle);
                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampYAngle, clampYAngle);
                // state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f), 0.5f);
            }
        }
    }
}
