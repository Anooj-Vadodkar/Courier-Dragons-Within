#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SunMoonManager))]
public class SunMoonManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SunMoonManager sunMoon = (SunMoonManager)target;
        sunMoon.sun = (GameObject) EditorGUILayout.ObjectField("Sun", sunMoon.sun, typeof(GameObject), true);
        sunMoon.moon = (GameObject) EditorGUILayout.ObjectField("Moon", sunMoon.moon, typeof(GameObject), true);

        if (GUILayout.Button("Set Moon in Opposition to Sun"))
        {
            sunMoon.SetMoonOppositeSun();
        }
    }
}

#endif