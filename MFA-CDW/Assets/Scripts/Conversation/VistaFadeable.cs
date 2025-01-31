using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VistaFadeable : MonoBehaviour
{
    [SerializeField] private bool _fadeIn = true;
    [SerializeField] private Image _image;
    [SerializeField] private AnimationCurve accelerationCurve;
    private Vector3 maxScale;

    private void Start()
    {
        maxScale = _image.rectTransform.localScale;
        if(_fadeIn)
        {
            _image.rectTransform.localScale = maxScale * 0.0f;
            Color color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
            _image.color = color;
        }
    }

    // Cursor distance relative to max (0-1)
    public void SetCursorDistance(float val)
    {
        float curveValue = accelerationCurve.Evaluate(val);
        if(!_fadeIn)
        {
            curveValue = accelerationCurve.Evaluate(1 - val); //Dexter: curve must go to 0 at 0 or this won't work properly
        }
        Color color = new Color(_image.color.r, _image.color.g, _image.color.b, curveValue);
        _image.color = color;
        _image.rectTransform.localScale = maxScale * curveValue;

        //_image.rectTransform.Rotate(Vector3.forward * accelerationCurve.Evaluate(val));
    }

    public void Disable()
    {
        _image.enabled = false;
    }
}
