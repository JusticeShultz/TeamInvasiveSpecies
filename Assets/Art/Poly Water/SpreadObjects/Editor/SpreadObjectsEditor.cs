using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SpreadObjects))]
public class SpreadObjectsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpreadObjects myScript = (SpreadObjects)target;

        if (GUILayout.Button("Spread Objects"))
        {
            myScript.Spread();
        }
    }
}