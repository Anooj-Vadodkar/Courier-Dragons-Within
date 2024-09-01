using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;

public class DisappearingText : MonoBehaviour
{
    public delegate void FadeFinishedEventDelegate();
    public FadeFinishedEventDelegate FadeFinishedEvent;

    [SerializeField] private EventReference audioEvent;

    private MeshRenderer mr;
    private CanvasRenderer cr;
    private Material newMat;
    private float revealValue = 1.0f;
    //private float warpValue = 0.0f;
    
    [SerializeField] private float effectDuration = 1.0f;
    [SerializeField] private float scaleStrength = 1.15f;
    [SerializeField] private float warpStrength = 0.3f;
    
    void Awake()
    {
        InitializeMaterial();
    }

    public void InitializeMaterial()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            if (!mr) mr = GetComponent<MeshRenderer>();
        }
        else
        {
            if (!cr) cr = GetComponent<CanvasRenderer>();
        }
        if (!newMat)
        {
            if (mr)
            {
                if (!mr.material) return;
                newMat = new Material(mr.material);
                mr.material = newMat;
            }
            else if (cr)
            {
                Material mat = cr.GetMaterial();
                if (!mat) return;
                newMat = Instantiate(mat);
                cr.SetMaterial(newMat, 0);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!cr && !mr)
        {
            InitializeMaterial();
        }
        
        if (Input.GetKey(KeyCode.Z))
        {
            StartFade();
        }

        if (Input.GetKey(KeyCode.X))
        {
            StartAppear();
        }
    }

    public void EnableText(float time) {
        StartCoroutine(EnableTextDelayed(time));
    }

    private IEnumerator EnableTextDelayed(float time) {
        yield return new WaitForSeconds(time);
        AudioManager.Instance.PlayEvent(audioEvent);
        mr.enabled = true;
    }

    public void StartFade()
    {
        StartCoroutine(Countdown());
    }
    
    public void StartAppear()
    {
        AudioManager.Instance.PlayEvent(audioEvent);
        StartCoroutine(Reappear());
    }

    public void SetVisibility(bool invisible)
    {
        float value = (invisible) ? -0.2f : 1.0f;
        newMat.SetFloat("_RevealValue", value);
    }

    private IEnumerator Countdown()
    {
        //AudioManager.Instance.PlayEvent(FMODEvents.Instance.breathOutOutside);
        float duration = effectDuration;
        Vector3 currentScale = transform.localScale;
        float warpValue = 0f;
        float timer = 0f;
        while (timer <= duration)
        {
            float val = timer / duration;
            warpValue = Mathf.SmoothStep(0, warpStrength, val);
            revealValue = Mathf.Lerp(1.0f, 0f, val);
            transform.localScale = Vector3.Lerp(currentScale, currentScale * scaleStrength, 
                Mathf.SmoothStep(0, 1, val));
            newMat.SetFloat("_RevealValue", revealValue);
            newMat.SetFloat("_WarpValue", warpValue);
            timer += Time.deltaTime;
            yield return null;
        }
        newMat.SetFloat("_RevealValue", -0.2f);
        transform.localScale = currentScale;
        FadeFinishedEvent?.Invoke();
    }

    private IEnumerator Reappear()
    {
        float duration = effectDuration;
        Vector3 currentScale = transform.localScale;
        float warpValue = warpStrength;
        float timer = 0f;
        while (timer <= duration)
        {
            float val = timer / duration;
            warpValue = Mathf.SmoothStep(warpStrength, 0, val);
            revealValue = Mathf.Lerp(0.0f, 1f, val);
            transform.localScale = Vector3.Lerp(currentScale, currentScale / scaleStrength, 
                Mathf.SmoothStep(0, 1, val));
            newMat.SetFloat("_RevealValue", revealValue);
            newMat.SetFloat("_WarpValue", warpValue);
            timer += Time.deltaTime;
            yield return null;
        }
        newMat.SetFloat("_RevealValue", 1.0f);
        transform.localScale = currentScale;
        FadeFinishedEvent?.Invoke();
    }
}
