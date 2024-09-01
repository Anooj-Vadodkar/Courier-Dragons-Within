using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ContinuousShakeVisuals : MonoBehaviour
{
    [SerializeField] private float shakeAmt;
    [SerializeField] private CinemachineVirtualCamera vc;
    private CinemachineBasicMultiChannelPerlin noiseSettings;

    private bool isShaking = false;

    private void Start() {
        noiseSettings = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void StartNoise() {
        Debug.Log("Hit Start Noise");
        StartCoroutine(StartNoiseRoutine());
    }

    private IEnumerator StartNoiseRoutine() {
        float amt = 0;
        while(amt < shakeAmt) {
            amt += Time.deltaTime;
            noiseSettings.m_AmplitudeGain = amt;
            noiseSettings.m_FrequencyGain = amt;
            yield return null;
        }
        isShaking = true;
    }

    public void StopNoise() {
        StartCoroutine(StopNoiseRoutine());
    }

    private IEnumerator StopNoiseRoutine() {
        float amt = noiseSettings.m_AmplitudeGain;
        while(amt > 0) {
            amt -= Time.deltaTime;
            noiseSettings.m_AmplitudeGain = amt;
            noiseSettings.m_FrequencyGain = amt;
            yield return null;
        }
        isShaking = false;
    }

    public bool GetIsShaking() {
        return isShaking;
    }

    public float GetShakeAmt() {
        return shakeAmt;
    }

}
