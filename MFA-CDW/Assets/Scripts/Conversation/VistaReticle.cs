using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VistaReticle : MonoBehaviour
{
    [SerializeField] private Image reticle;
    [SerializeField] private Image breathImage;
    [SerializeField] private AnimationCurve accelerationCurve;
    private Vector3 maxScale;

    private void Start()
    {
        maxScale = breathImage.rectTransform.localScale;
        breathImage.rectTransform.localScale = maxScale * 0.0f;
        Color color = new Color(reticle.color.r, reticle.color.g, reticle.color.b, 0f);
        reticle.color = color;
    }

    // Cursor distance relative to max (0-1)
    public void SetCursorDistance(float val)
    {
        Color color = new Color(reticle.color.r, reticle.color.g, reticle.color.b, accelerationCurve.Evaluate(val));
        reticle.color = color;

        reticle.rectTransform.Rotate(Vector3.forward * accelerationCurve.Evaluate(val));
    }

    // Cursor distance relative to max (0-1)
    public void SetBreathAmount(float val)
    {
        breathImage.rectTransform.localScale = maxScale * accelerationCurve.Evaluate(val);
    }

    public void Disable()
    {
        reticle.enabled = false;
        breathImage.enabled = false;
    }
}
