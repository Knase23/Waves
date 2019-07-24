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
        //Read-only
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((PlayerShip)target), typeof(PlayerShip), false);
        GUILayout.Label("Ship Stats");
        EditorGUILayout.FloatField("Current Health", ship.hp.GetHealth());
        //Editable fields
        GUI.enabled = true;
        ship.speed = EditorGUILayout.IntField("Speed", ship.speed);

        GUILayout.Label("Ships Pulse");
        EditorGUI.indentLevel++;
        ship.pulsePower = EditorGUILayout.IntField("Pulse Power", ship.pulsePower);
        ship.pulseSpeed = EditorGUILayout.IntField("Pulse Speed", ship.pulseSpeed);
        ship.pulseMaxDistance = EditorGUILayout.IntField("Pulse Max Distance", ship.pulseMaxDistance);
        ship.pulseCooldown = EditorGUILayout.FloatField("Pulse Cooldown", ship.pulseCooldown);
        GUI.enabled = false;
        //Read-only
        EditorGUILayout.FloatField("Pulse Cooldown Time", ship.pulseCooldownTimer);
        GUI.enabled = true;

        EditorGUI.indentLevel--;
        //Editable fields
        GUILayout.Label("Controlling the Ship");
        ship.VerticalControllAxis = EditorGUILayout.TextField("Vertical Axis",ship.VerticalControllAxis);
        ship.HorizontalControllAxis = EditorGUILayout.TextField("Horizontal Axis",ship.HorizontalControllAxis);
        ship.FirePulseControllAxis = EditorGUILayout.TextField("Fire Pulse Axis", ship.FirePulseControllAxis);

        ship.shipColor = EditorGUILayout.ColorField("Ship/Player Color", ship.shipColor);        
    }
}
