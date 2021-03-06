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
        EditorGUILayout.Space();

        RMapGenerator generator = (RMapGenerator)target;
        if (GUILayout.Button("(Re-)Generate Level"))
        {
            generator.Delete();
            generator.Create();
        }
        if (GUILayout.Button("Reset Level"))
        {
            generator.Delete();
        }
        if (GUILayout.Button("(Re-)Generate Level from string"))
        {
            generator.Delete();
            generator.Create(generator.InputMapLayout);
        }
        if (GUILayout.Button("Show / Hide Props"))
        {
            generator.TogglePropVisibility();
        }
    }
}
