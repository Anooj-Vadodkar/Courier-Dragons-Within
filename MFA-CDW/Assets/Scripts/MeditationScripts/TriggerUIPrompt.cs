using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TriggerUIPrompt : MonoBehaviour
{
    /* [SerializeField]
    private TextMeshProUGUI promptField; */

    private TextMeshPro promptField;

    [SerializeField]
    private float fadeInSpeed = 2.0f;

    private void Start() {
        promptField = GetComponent<TextMeshPro>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")) {
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(FadeInPrompt());
        }
    }

    private IEnumerator FadeInPrompt() {
        if(promptField == null){
            Debug.Log("Hit");
        }
        float alpha = 0;
        while(alpha < 1) {
            alpha += fadeInSpeed * Time.deltaTime;
            promptField.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        alpha = 1;
        promptField.alpha = alpha;
        yield return new WaitForSeconds(3.0f);
        while(alpha > 0) {
            alpha -= fadeInSpeed * Time.deltaTime;
            promptField.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        alpha = 0;
        promptField.alpha = alpha;

    }
}
