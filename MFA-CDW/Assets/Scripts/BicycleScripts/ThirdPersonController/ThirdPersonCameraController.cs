using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCameraController : MonoBehaviour
{
    private CinemachineFreeLook freeLookCam;

    [Header("FOV Settings")]
    [SerializeField]
    [Range(10, 100)]
    private float defaultFOV = 60.0f;
    [SerializeField]
    [Range(10, 100)]
    private float zoomFOV = 30.0f;
    [SerializeField]
    private float FOVIncrementAmount = 2.0f;

    private float currentFOV;
    private float targetFOV;

    [Header("Noise Settings")]
    [SerializeField]
    private float defaultNoise = 1;
    [SerializeField]
    private float increasedNoise = 2;

    private int topRigIndex = 0;
    private int middleRigIndex = 1;
    private int bottomRigIndex = 2;
    private CinemachineBasicMultiChannelPerlin topRigNoiseSettings;
    private CinemachineBasicMultiChannelPerlin middleRigNoiseSettings;
    private CinemachineBasicMultiChannelPerlin bottomRigNoiseSettings;

    private InputManager inputManager;

    private void Awake()
    {
        if (freeLookCam == null)
        {
            freeLookCam = GetComponent<CinemachineFreeLook>();
        }

        if (freeLookCam != null && freeLookCam.m_Orbits.Length > bottomRigIndex)
        {
            if (freeLookCam.m_Orbits.Length > topRigIndex)
            {
                var topRig = freeLookCam.GetRig(topRigIndex);
                if (topRig != null)
                {
                    topRigNoiseSettings = topRig.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                }
            }

            if (freeLookCam.m_Orbits.Length > middleRigIndex)
            {
                var middleRig = freeLookCam.GetRig(middleRigIndex);
                if (middleRig != null)
                {
                    middleRigNoiseSettings = middleRig.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                }
            }

            var bottomRig = freeLookCam.GetRig(bottomRigIndex);
            if (bottomRig != null)
            {
                bottomRigNoiseSettings = bottomRig.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        ResetShake();

        currentFOV = defaultFOV;
        targetFOV = defaultFOV;
        freeLookCam.m_Lens.FieldOfView = currentFOV;

        inputManager = InputManager.Instance;
    }

    private void Update()
    {
        if (currentFOV != targetFOV)
        {
            if (currentFOV > targetFOV)
            {
                currentFOV -= FOVIncrementAmount * Time.deltaTime;
                if (currentFOV < targetFOV)
                    currentFOV = targetFOV;
            }
            else if (currentFOV < targetFOV)
            {
                currentFOV += FOVIncrementAmount * Time.deltaTime;
                if (currentFOV > targetFOV)
                {
                    currentFOV = targetFOV;
                }
            }
            freeLookCam.m_Lens.FieldOfView = currentFOV;
        }
    }

    public void ZoomIn()
    {
        targetFOV = zoomFOV;
    }

    public void ResetZoom()
    {
        targetFOV = defaultFOV;
    }

    public void IncreaseShake()
    {
        topRigNoiseSettings.m_AmplitudeGain = increasedNoise;
        topRigNoiseSettings.m_FrequencyGain = increasedNoise;

        middleRigNoiseSettings.m_AmplitudeGain = increasedNoise;
        middleRigNoiseSettings.m_FrequencyGain = increasedNoise;

        bottomRigNoiseSettings.m_AmplitudeGain = increasedNoise;
        bottomRigNoiseSettings.m_FrequencyGain = increasedNoise;
    }

    public void ResetShake()
    {
        topRigNoiseSettings.m_AmplitudeGain = defaultNoise;
        topRigNoiseSettings.m_FrequencyGain = defaultNoise;

        middleRigNoiseSettings.m_AmplitudeGain = defaultNoise;
        middleRigNoiseSettings.m_FrequencyGain = defaultNoise;

        bottomRigNoiseSettings.m_AmplitudeGain = defaultNoise;
        bottomRigNoiseSettings.m_FrequencyGain = defaultNoise;
    }
}
