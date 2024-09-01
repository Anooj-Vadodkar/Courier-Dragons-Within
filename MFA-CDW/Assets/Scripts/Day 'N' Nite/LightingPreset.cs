using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
[CreateAssetMenu(fileName = "Ligting Preset")]
public class LightingPreset : ScriptableObject
{
    public Color ambientColor;
    public Color directionalColor;
    [Range(0, 360)] public int directionalLightRotation;
    public Color fogColor;
    [Range(0, 0.1f)]public float fogIntensity;
    public Material skybox;
}
