using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Mathematics;

public class TransitionStates : MonoBehaviour
{
    [SerializeField]
    private BackgroundChanger bc;
    [SerializeField]
    private CinemachineVirtualCamera vc;
    [SerializeField]
    private float shakeAmount = 0.5f;
    [SerializeField]
    private float shakeIncreaseAmount = 1.0f;

    CinemachineBasicMultiChannelPerlin noiseSettings;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            AudioManager.Instance.SetMusicParam(MusicState.MEDITATION_CONF);
            // start shaking the screen
            noiseSettings = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            StartCoroutine(increaseNoise());
            bc.TransitionToCacophonous();
        }
    }

    private IEnumerator increaseNoise() {
        float i = 0;
        while(i < shakeAmount) {
            i += Time.deltaTime * shakeIncreaseAmount;
            if(i >= shakeAmount) {
                i = shakeAmount;
            }
            noiseSettings.m_AmplitudeGain = i;
            noiseSettings.m_FrequencyGain = i;
            yield return null;
        }
    }
}
