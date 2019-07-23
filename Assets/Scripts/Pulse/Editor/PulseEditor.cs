using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Pulse))]
public class PulseEditor : Editor
{
    Pulse inspectedPulse { get { return target as Pulse; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}