using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMediSequence : MonoBehaviour
{
    [SerializeField] private GameObject fadeInScreen;
    [SerializeField] private float fadeInWaitTime;
    [SerializeField] private ExternalController externalController;
    [SerializeField] private NewBikeController bicycleController;
    [SerializeField] private bool startOnBike;

    private void Start() {
        if(startOnBike) {
            // start on bike
            bicycleController.SetIsPaused(false);
        } else {
            // exit medi sequence
            StartCoroutine(FadeIn());
            bicycleController.SetIsPaused(true);
        }
    }

    private IEnumerator FadeIn() {
        fadeInScreen.SetActive(true);
        yield return new WaitForSeconds(fadeInWaitTime);
        externalController.PlayExitMedCutscene();
    }
}
