#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SDFCreator : EditorWindow
{
    public GameObject focusedObject;

    [MenuItem("Tools/Shaders/SDF Creator")]
    public static void OpenWindow()
    {
        SDFCreator wnd = GetWindow<SDFCreator>();
        wnd.titleContent = new GUIContent("SDF Creator");
    }

    void OnGUI()
    {
        // focused object
        focusedObject = (GameObject)EditorGUILayout.ObjectField("Object: ", 
            focusedObject, typeof(GameObject), true);
    }

    public void CreateSDF()
    {
    }
}

#endif
