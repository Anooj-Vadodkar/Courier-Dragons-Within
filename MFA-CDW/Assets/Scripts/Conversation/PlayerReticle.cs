using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReticle : MonoBehaviour
{
    [SerializeField] private Image reticle;
    [SerializeField] private Image breathImage;
    [SerializeField] private AnimationCurve accelerationCurve;
    private Vector3 maxScale;

    private void Start()
    {
        maxScale = breathImage.rectTransform.localScale;
        breathImage.rectTransform.localScale = maxScale * 0.0f;
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

    public void SetBreathOn()
    {
        breathImage.color = new Color(breathImage.color.r, breathImage.color.g, breathImage.color.b, 1.0f);
    }

    public void Disable()
    {
        reticle.enabled = false;
        breathImage.enabled = false;
    }
}
