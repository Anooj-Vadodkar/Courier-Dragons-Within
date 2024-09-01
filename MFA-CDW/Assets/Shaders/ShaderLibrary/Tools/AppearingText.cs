using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AppearingText : MonoBehaviour
{
    //public delegate void FadeFinishedEventDelegate();
    //public FadeFinishedEventDelegate FadeFinishedEvent;
    // Start is called before the first frame update
    private MeshRenderer mr;
    private CanvasRenderer cr;
    private float revealValue = 1.0f;
    private float warpValue = 0.0f;
    private TextMeshPro tmPro;
    private string oldText;

    [SerializeField] private float effectDuration = 1.0f;
    
    void Awake()
    {
        tmPro = GetComponent<TextMeshPro>();
        oldText = tmPro.text;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (Input.GetKey(KeyCode.Z))
        {
            StartReveal();
        }
    }

    public void StartReveal()
    {
        StartCoroutine(Countdown());
    }


    private IEnumerator Countdown()
    {
        tmPro.text = "";
        float duration = effectDuration;
        float timer = 0f;
        while (timer <= duration)
        {
            float val = timer / duration;
            int charNumber = (int) Mathf.Ceil(val * oldText.Length);
            tmPro.text = oldText.Substring(0, charNumber);
            
            timer += Time.deltaTime;
            yield return null;
        }

        tmPro.text = oldText;
        //FadeFinishedEvent?.Invoke();
    }
}
