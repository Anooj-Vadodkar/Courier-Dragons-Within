using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
[CustomPropertyDrawer(typeof(StreamLayout))]
public class CustPropertyDrawer : PropertyDrawer
{
    int sc = 10;
    int bc = 10;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.PrefixLabel(position, label);

        Rect newPosition = position;
        newPosition.y += 18f;
        newPosition.height = 18f;

        EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("sc"), GUIContent.none);
        newPosition.y += 18f;

        EditorGUI.PropertyField(newPosition, property.FindPropertyRelative("bc"), GUIContent.none);
        newPosition.y += 18f;
        
        SerializedProperty data = property.FindPropertyRelative("rows");

        sc = property.FindPropertyRelative("sc").intValue;
        bc = property.FindPropertyRelative("bc").intValue;

        for(int n = 0; n < data.arraySize; n++) {
            SerializedProperty row = data.GetArrayElementAtIndex(n).FindPropertyRelative("row");
                
            if(row.arraySize != bc) {
                row.arraySize = bc;
            }

            newPosition.width = position.width / bc;
            newPosition.height = 18f;

            for(int i = 0; i < bc; i++) {
                EditorGUI.PropertyField(newPosition, row.GetArrayElementAtIndex(i), GUIContent.none);
                newPosition.x += newPosition.width;
            }

            newPosition.x = position.x;
            newPosition.y += 18f;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return 18f * (sc + 3);
    }
}
