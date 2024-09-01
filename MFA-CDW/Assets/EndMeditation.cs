using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class EndMeditation : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vc;
    [SerializeField] private MediMovement player;
    [SerializeField] private bool isShaking = true;
    [SerializeField] private RawImage fadeOutBackground;
    [SerializeField] private float endingShakeAmt = 4.0f;
    [SerializeField] private float shakeIncreaseAmount = 0.2f;
    [SerializeField] private int meditationNum;
    [SerializeField] private LoopingAudioInstance shakingSound;

    public void Exit() {
        // fade out the maze

        // Pause player
        player.SetPaused(true);

        // play scream & dragon sound
        // AudioManager.Instance.PlayEvent(FMODEvents.Instance.wsMeditation, Camera.main.transform.position);

        // shake the screen a lot
        StartCoroutine(EndMeditationWS());
    }

    private IEnumerator EndMeditationWS() {
        // AudioManager.Instance.SetMusicParam(MusicState.MEDITATION_SERENE);
        if(isShaking) {
            CinemachineBasicMultiChannelPerlin noiseSettings = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            float i = 1.0f;
            while(i < endingShakeAmt) {
                i += Time.deltaTime * shakeIncreaseAmount;
                if(i >= endingShakeAmt) {
                    i = endingShakeAmt;
                }
                noiseSettings.m_AmplitudeGain = i;
                noiseSettings.m_FrequencyGain = i;
                yield return null;
            }
        }

        yield return new WaitForSeconds(2.0f);

        AudioManager.Instance.StopMusic();
        shakingSound.StopLoopingTrack();
        // fade to white
        float alpha = 0;
        while(alpha < 1) {
            fadeOutBackground.color = new Color(fadeOutBackground.color.r, fadeOutBackground.color.g, fadeOutBackground.color.b, alpha);
            alpha += 1.0f * Time.deltaTime;
            if(alpha > 1) {
                alpha = 1;
                fadeOutBackground.color = new Color(fadeOutBackground.color.r, fadeOutBackground.color.g, fadeOutBackground.color.b, alpha);
            }
            
            yield return new WaitForEndOfFrame();
        }

/*         while(AudioManager.Instance.GetMusicParam() != MusicState.MEDITATION_SERENE) {
            Debug.Log(AudioManager.Instance.GetMusicParam());
            yield return null;
        } */

        // Debug.Log(AudioManager.Instance.GetMusicParam());

        // transition to new scene
        // AudioManager.Instance.SetMusicParam(MusicState.CONVO_BASE);
        GameManager.Instance.SetMeditationNum(meditationNum);
        GameManager.Instance.SceneTranstition("Day1Scene_MWHVersion");
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if(collider.gameObject.CompareTag("Player")) {
            Exit();
        }
    }
}
