using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(VisualLevelGenerator))]
public class VisualLevelGeneratorEditor : Editor
{
    VisualLevelGenerator Generation { get { return target as VisualLevelGenerator; } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Spawn Level"))
            {

                Generation.GenerateLevel();

            }
        }
    }
}
