using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RWorldItemEditor))]
public class RWorldItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Sp�ter: Blende die Weapon-Values nur ein, wenn nicht 'cannotBePickedUp' true ist
    }
}
