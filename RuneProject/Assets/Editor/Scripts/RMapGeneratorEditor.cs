using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RuneProject.EnvironmentSystem;

[CustomEditor(typeof(RMapGenerator))]
public class RMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {        
        DrawDefaultInspector();
        //EditorGUILayout.Space();

        //RMapGenerator generator = (RMapGenerator)target;
        //if (GUILayout.Button("Generate Level"))
        //{
            
        //}
        //if (GUILayout.Button("Reset Level"))
        //{

        //}
    }
}
