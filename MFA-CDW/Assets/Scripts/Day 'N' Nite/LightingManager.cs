using System.Collections;
using System.Collections.Generic;
using OccaSoftware.SuperSimpleSkybox.Runtime;
//using UnityEditor.Presets;
using UnityEngine;

[ExecuteInEditMode]
public class LightingManager : MonoBehaviour
{
    // References
    [SerializeField] private Light directionalLight;
    // [SerializeField] private LightingPreset preset;
    [SerializeField] private LightingPreset morningPreset;
    [SerializeField] private LightingPreset dayPreset;
    [SerializeField] private LightingPreset eveningPreset;
    [SerializeField] private LightingPreset nightPreset;
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material eveningSkybox;
    [SerializeField] private Material nightSkybox;
    [SerializeField] private Material morningSkybox;
    // Variables
    [SerializeField, Range(0, 24)] private float timeofDay;

    private void Update() {
        /* if(!preset) {
            return;
        } */

        if(Application.isPlaying) {
            // timeofDay += Time.deltaTime;
            // timeofDay %= 24; // Clamp between 0-24
            UpdateLighting(timeofDay / 24);
        } else {
            UpdateLighting(timeofDay / 24);
        }
    }

    public void SetTimeOfDay(float time) {
        timeofDay = time;
        UpdateLighting(timeofDay / 24);
    }

    public void UpdateLighting(float timePercent) {
        if((timeofDay >= 0 || timeofDay == 24) && timeofDay < 6) {
            // morning
            SetLightPreset(morningPreset, timePercent);
        } else if(timeofDay >= 6 && timeofDay < 12) {
            // day
            SetLightPreset(dayPreset, timePercent);
        } else if(timeofDay >= 12 && timeofDay < 18) {
            // evening
            SetLightPreset(eveningPreset, timePercent);
        } else if(timeofDay >= 18 && timeofDay < 24) {
            // night
            SetLightPreset(nightPreset, timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((0.5f * 360f) - 90f, 170, 0));
        }
    }

    private void SetLightPreset(LightingPreset preset, float timePercent) {
        RenderSettings.skybox = preset.skybox;
        RenderSettings.ambientLight = preset.ambientColor;
        RenderSettings.fogColor = preset.fogColor;
        RenderSettings.fogDensity = preset.fogIntensity;
        if(directionalLight) {
            directionalLight.color = preset.directionalColor;
            directionalLight.transform.localRotation = Quaternion.Euler(preset.directionalLightRotation - 90f, 170, 0);
        }
    }

    private void OnValidate() {
        if(directionalLight)
            return;

        if(RenderSettings.sun) {
            directionalLight = RenderSettings.sun;
        } else {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights) {
                if(light.type == LightType.Directional) {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}
