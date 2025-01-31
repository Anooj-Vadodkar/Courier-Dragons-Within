using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChangeScript.Dialogue)), CanEditMultipleObjects]
public class DialougeEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Details");
    }
}
