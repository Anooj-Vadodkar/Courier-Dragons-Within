using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    float fps;
    float updateTimer = 0.2f;

    [SerializeField]
    TextMeshProUGUI fpsTitle;

    private void updateFPSDisplay()
    {
        updateTimer -= Time.deltaTime;
        if(updateTimer <= 0)
        {
            fps = 1f / Time.unscaledDeltaTime;
            fpsTitle.text = "FPS: " + Mathf.Round(fps);
            updateTimer = 0.2f;
        }
    }
    void Update()
    {
        updateFPSDisplay();
    }
}
