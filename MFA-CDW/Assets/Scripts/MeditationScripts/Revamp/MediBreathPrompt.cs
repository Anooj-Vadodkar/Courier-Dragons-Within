using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;

public class MediBreathPrompt : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI breathPromptText;
    [SerializeField]
    private TextMeshProUGUI inputPromptText;
    [SerializeField]
    private Image inputPromptImage;
    [SerializeField]
    private GameObject blackScreen;
    [SerializeField]
    private GameObject contextScreen;
    [SerializeField]
    private GameObject contextBlackScreen;
    [SerializeField]
    private GameObject fogScreen;
    [SerializeField]
    private float fadeInSpeed = 5.0f;

    [SerializeField]
    private MediMovement player;

    [SerializeField]
    float timeToStart = 1;

    [SerializeField]
    private GameObject sphereFov;
    [SerializeField]
    private float scaleIncSpeed = 1.0f;
    [SerializeField] private TextMeshProUGUI subtitles;
    [SerializeField] private string startingText;

    [SerializeField] private EventReference musicEvent;
    [SerializeField] private bool promptAtStart = true;
    [SerializeField] private UnityEvent beforePromptEvent;
    [SerializeField] private UnityEvent afterPromptEvent;
 
    private float startingScale;

    private bool breathIsPlaying = false;

    private bool ready = false;

    private InputManager inputManager;
    private AudioManager audioManager;

    private EventInstance natureAmbientInstnace;
    private EventInstance breathInstance;

    private void Awake() {
        startingScale = sphereFov.transform.localScale.x;
        sphereFov.transform.localScale = Vector3.zero;
        blackScreen.SetActive(true);
    }

    private void Start() {
        inputManager = InputManager.Instance;
        audioManager = AudioManager.Instance;

        natureAmbientInstnace = audioManager.CreateEventInstance(FMODEvents.Instance.natureAmbient);
        natureAmbientInstnace.start();

        breathInstance = audioManager.CreateEventInstance(FMODEvents.Instance.breathIn);

        natureAmbientInstnace.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        if(promptAtStart) {
            StartCoroutine(PromptPlayer());
        } else {
            StartCoroutine(PrePromptEvent());
            // beforePromptEvent.Invoke(timeToStart);
        }
    }

    private void Update() {
        if(ready) {
            if(inputManager.GetBreathInput() == 1) {
                if(sphereFov.transform.localScale.x < startingScale) {
                    if(!breathIsPlaying) {
                        breathInstance.start();
                        breathIsPlaying = true;
                    }
                    sphereFov.transform.localScale += (new Vector3(scaleIncSpeed, scaleIncSpeed, scaleIncSpeed) * Time.deltaTime);
                    if(sphereFov.transform.localScale.x > startingScale) {
                        sphereFov.transform.localScale = new Vector3(startingScale, startingScale, startingScale);
                        ready = false;

                        // Start Game
                        breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        audioManager.PlayEvent(FMODEvents.Instance.breathOut, Camera.main.transform.position);
                        breathPromptText.gameObject.SetActive(false);
                        inputPromptText.gameObject.SetActive(false);
                        inputPromptImage.color = new Color(inputPromptImage.color.r, inputPromptImage.color.g, inputPromptImage.color.b, 1);
                        inputPromptImage.gameObject.SetActive(false);
                        fogScreen.SetActive(true);
                        // fov.SetPaused(false);
                        StopAllCoroutines();
                        ready = false;
                        StartCoroutine(FadeOutBlackScreen(blackScreen.GetComponent<SpriteRenderer>()));
                    }
                }
            } else {
                if(breathIsPlaying) {
                    //audioManager.StopSFX();
                    breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    breathIsPlaying = false;
                }
                if(sphereFov.transform.localScale.x > 0) {
                    sphereFov.transform.localScale -= (new Vector3(scaleIncSpeed, scaleIncSpeed, scaleIncSpeed) * Time.deltaTime);
                    if(sphereFov.transform.localScale.x < 0) {
                        sphereFov.transform.localScale = Vector3.zero;
                    }
                }
            }
        }
    }

    private IEnumerator ContextScreen() {
        contextScreen.SetActive(true);
        while(inputManager.GetBreathInput() != 1) {
            yield return new WaitForEndOfFrame();
        }
        float alphaValue = 0;
        RawImage screen = contextBlackScreen.GetComponent<RawImage>();
        Color startingColor = screen.color;
        while(alphaValue < 1) {
            screen.color = new Color(startingColor.r, startingColor.g, startingColor.b, alphaValue);
            alphaValue += fadeInSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(2.0f);
        contextScreen.SetActive(false);
        StartCoroutine(PromptPlayer());
    }

    public void TriggerPrompt() {
        StartCoroutine(PromptPlayer());
    }

    private IEnumerator PrePromptEvent() {
        yield return new WaitForSeconds(timeToStart);
        beforePromptEvent.Invoke();
    }

    private IEnumerator PromptPlayer() {
        yield return new WaitForSeconds(timeToStart);
        float alphaValue = 0;
        while(alphaValue < 1) {
            breathPromptText.alpha = alphaValue;
            alphaValue += fadeInSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ready = true;
        alphaValue = 0;
        yield return new WaitForSeconds(5.0f);
        Color startingColor = inputPromptImage.color;
        while(alphaValue < 1) {
            // inputPromptText.alpha = alphaValue;
            inputPromptImage.color = new Color(startingColor.r, startingColor.g, startingColor.b, alphaValue);
            alphaValue += fadeInSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator FadeOutBlackScreen(SpriteRenderer screen) {
        audioManager.SetMusicEvent(musicEvent, true);
        // audioManager.SetMusicParam(MusicState.MEDITATION_BASE);
        float alphaValue = 1.0f;
        Color startingColor = screen.color;
        while(alphaValue > 0) {
            screen.color = new Color(startingColor.r, startingColor.g, startingColor.b, alphaValue);
            alphaValue -= (fadeInSpeed / 3) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        player.SetPaused(false);
        if(subtitles) {
            subtitles.text = startingText;
        }

        sphereFov.SetActive(false);
        player.gameObject.layer = LayerMask.NameToLayer("Black");

        yield return new WaitForSeconds(1);

        afterPromptEvent.Invoke();
    }
}
