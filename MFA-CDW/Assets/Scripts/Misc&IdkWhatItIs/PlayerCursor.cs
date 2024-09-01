using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerCursor : MonoBehaviour
{
    private RectTransform cursorSprite;
    private RectTransform canvas;

    [SerializeField]
    private float transitionTime = 2;
    [SerializeField]
    private Image reticle1;
    [SerializeField]
    private Image reticle2;
    [SerializeField]
    private Image reticle3;

    private float currentSpeed;

    private static PlayerCursor _instance;

    public static PlayerCursor Instance {
        get {
            return _instance;
        }
    }

    private void Awake() {
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start() {
        cursorSprite = GetComponent<RectTransform>();
        canvas = transform.parent.GetComponent<RectTransform>();

        DisableCursor();
    }

    private void Update() {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        cursorSprite.anchoredPosition = mousePos - canvas.sizeDelta / 2f;

        // Rotate the cursor
        transform.Rotate(Vector3.forward * 20 * currentSpeed * Time.deltaTime);
    }

    public void BaseRetical() {
        reticle1.DOFade(0, transitionTime);
        reticle2.DOFade(0, transitionTime);
        reticle3.DOFade(0, transitionTime);
        SetSpeed(0);
    }

    public void PromptRetical() {
        reticle1.DOFade(0, transitionTime);
        reticle2.DOFade(1, transitionTime);
        reticle3.DOFade(0, transitionTime);
        SetSpeed(0);
    }

    public void BreathingReticle() {
        reticle1.DOFade(0, transitionTime);
        reticle2.DOFade(0, transitionTime);
        reticle3.DOFade(1, transitionTime);
    }

    public void SetSpeed(float speed) {
        currentSpeed = speed;
    }

    public void ResetRotation() {
        transform.eulerAngles = Vector3.zero;
    }

    public void DisableCursor() {
        reticle1.DOFade(0, transitionTime);
        reticle2.DOFade(0, transitionTime);
        reticle3.DOFade(0, transitionTime);
    }
}
