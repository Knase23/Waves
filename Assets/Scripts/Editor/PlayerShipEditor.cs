using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PlayerShip))]
public class PlayerShipEditor : Editor
{
    PlayerShip ship { get { return target as PlayerShip; } }

    public override void OnInspectorGUI()
    {
        //Read-only fields
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((PlayerShip)target), typeof(PlayerShip), false);

        EditorGUILayout.FloatField("Current Health", ship.hp.GetHealth());
        //Editable fields
        GUI.enabled = true;
        EditorGUILayout.FloatField("Pulse Power", ship.pulsePower);
        EditorGUILayout.FloatField("Pulse Speed", ship.pulseSpeed);
        EditorGUILayout.FloatField("Pulse Max Distance", ship.pulseMaxDistance);

        GUILayout.Label("Controlling the Ship");
        EditorGUILayout.TextField("Vertical Axis",ship.VerticalControllAxis);
        EditorGUILayout.TextField("Horizontal Axis",ship.HorizontalControllAxis);

        EditorGUILayout.ColorField("Ship/Player Color", ship.shipColor);

        base.OnInspectorGUI();





        
    }
}
