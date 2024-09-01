using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using FMOD.Studio;

public class ScreenShakeScript : MonoBehaviour
{
    // Transform of the GameObject you want to shake
    [SerializeField] CinemachineVirtualCamera VC;

    // Desired duration of the shake effect
    [SerializeField] private float shakeDuration = 0f;
    [SerializeField] private float shakeMagnitude = 0f;


    // A measure of how quickly the shake effect should evaporate
    [SerializeField] private float dampingSpeed = .50f;

    // The initial position of the GameObject
    [SerializeField] Vector3 initialPosition;
    [SerializeField] private ContinuousShakeVisuals continuousShake;

    private float initialIntensity = 0;

    private bool isShaking = false;

    public void TriggerShake() {
        TriggerShake(0.3f);
    }

    public void TriggerShake(float duration)
    {
        initialIntensity = Mathf.Clamp(VC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain, 0, 2.5f);
        shakeDuration = duration;
        isShaking = true;
    }
    

    private void OnEnable()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isShaking) {
            if (shakeDuration > 0f)
            {
                VC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = initialIntensity + shakeMagnitude;
                shakeDuration -= Time.deltaTime * dampingSpeed;
            }
            else
            {
                if(continuousShake) {
                    if(continuousShake.GetIsShaking()) {
                        VC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = continuousShake.GetShakeAmt();
                    } else {
                        VC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;    
                    }
                } else {
                    VC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                }
                isShaking = false;
                
            }
        }
    }
}
