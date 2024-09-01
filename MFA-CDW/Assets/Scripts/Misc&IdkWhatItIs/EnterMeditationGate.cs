using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnterMeditationGate : MonoBehaviour
{
    [SerializeField] private string mediSceneName;
    [SerializeField] private GameObject enterMediPrompt;
    [SerializeField] private float breathingTime;
    [SerializeField] private ParticleSystem particles;
    private float breathTimer;

    private bool canEnter = false;
    private bool isBreathing = false;
    private bool isDone;
    private EventInstance breathInstance;

    private InputManager inputManager;
    private AudioManager audioManager;
    private ExternalController player;

    private void Start() {
        inputManager = InputManager.Instance;
        audioManager = AudioManager.Instance;

        breathInstance = audioManager.CreateEventInstance(FMODEvents.Instance.breathIn);
    }

    private void Update() {
        if(canEnter && !isDone) {
            if(inputManager.GetBreathInput() == 1) {
                if(breathTimer == 0) {
                    // play breathing sound
                    breathInstance.start();
                    isBreathing = true;
                }
                breathTimer += Time.deltaTime;

                if(breathTimer >= breathingTime) {
                    if(player) {
                        audioManager.PlayEvent(FMODEvents._instance.breathOut);
                        breathInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                        player.PlayEnterMedCutscene(mediSceneName);
                        enterMediPrompt.SetActive(false);
                        isDone = true;
                    }
                }
            } else {
                // check if previously breathing in
                if(isBreathing) {
                    // stop breathing sound
                    breathInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    isBreathing = false;
                    breathTimer = 0;
                }
            }
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            if(other.gameObject.GetComponent<ExternalController>()) {
                // Display input prompt
                enterMediPrompt.SetActive(true);

                player = other.gameObject.GetComponent<ExternalController>();
                canEnter = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")) {
            if(other.gameObject.GetComponent<ExternalController>()) {
                // Hide input prompt
                enterMediPrompt.SetActive(false);

                player = null;
                canEnter = false;
            }
        }
    }
}
