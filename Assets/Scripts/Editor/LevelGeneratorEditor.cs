using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    LevelGenerator Generation { get { return target as LevelGenerator; } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Spawn Level"))
        {
            if (EditorApplication.isPlaying)
            {
                if (Generation.ShowPresentationSphere)
                {
                    Generation.VisualGenerateLevel();
                }
                else
                {
                    Generation.GenerateLevel();
                }
            }
            else
            {
                Generation.GenerateLevel();
            }
        }
        if (GUILayout.Button("Clear Level"))
        {
            Generation.ClearLevel();
        }
    }
}
