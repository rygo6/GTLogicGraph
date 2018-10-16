using System;
using System.Collections;
using System.Collections.Generic;
using GeoTetra.GTGenericGraph;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[CustomEditor(typeof(GraphLogic))]
public class GraphLogicEditor : Editor
{
    private SerializedProperty _graphLogicDataProperty;
    private SerializedProperty _inputsProperty;
    private SerializedProperty _outputsProperty;
    

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        
        GraphLogic graphLogic = (GraphLogic) target;

        _graphLogicDataProperty = serializedObject.FindProperty("_graphLogicData");
        _inputsProperty = serializedObject.FindProperty("_inputs");
        _outputsProperty = serializedObject.FindProperty("_outputs");

        EditorGUILayout.PropertyField(_graphLogicDataProperty);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Inputs");

        for (int i = 0; i < _inputsProperty.arraySize; ++i)
        {
            SerializedProperty displayName = _inputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("DisplayName");
            SerializedProperty floatValue = _inputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("FloatValue");
            SerializedProperty componentValue = _inputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("ComponentValue");           
            
            EditorGUI.indentLevel = 2;
            EditorGUILayout.LabelField(displayName.stringValue);
            EditorGUILayout.PropertyField(floatValue);
            EditorGUILayout.ObjectField(componentValue, typeof(Transform));
        }
        
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Outputs");
        
        for (int i = 0; i < _outputsProperty.arraySize; ++i)
        {
            SerializedProperty displayName = _outputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("DisplayName");
            EditorGUI.indentLevel = 2;
            EditorGUILayout.LabelField(displayName.stringValue);
            
            Debug.Log(graphLogic.Outputs[i]);
            Debug.Log(graphLogic.Outputs[i].OutputType);
            if (graphLogic.Outputs[i].OutputType == typeof(Single))
            {
                SerializedProperty eventProperty = _outputsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_updatedFloat");
                EditorGUILayout.PropertyField(eventProperty);
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}