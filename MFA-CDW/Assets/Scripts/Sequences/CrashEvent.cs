using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrashEvent : MonoBehaviour
{
    [SerializeField] private GameObject crashScreen;
    [SerializeField] private NewBikeController bikeController;
    [SerializeField] private Transform crashPoint;

    public void Crash() {
        StartCoroutine(CrashRoutine());
        StartCoroutine(bikeController.CrashDismountRoutine(crashPoint));
    }

    private IEnumerator CrashRoutine() {
        crashScreen.SetActive(true);

        // Play crash sound

        yield return new WaitForSeconds(5);

        crashScreen.SetActive(false);
    }
}
