using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    [SerializeField] private Color failColor = Color.red;
    [SerializeField] private Color goodColor = Color.blue;
    [SerializeField] private Color midColor = Color.magenta;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void UpdateColor(float mental)
    {
        mental *= 2;

        if (mental >= 1)
        {
            image.color = Color.Lerp(midColor, goodColor, mental - 1);
        }
        else
        {
            image.color = Color.Lerp(failColor, midColor, mental);
        }
    }
}
