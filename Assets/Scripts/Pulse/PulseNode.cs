﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseNode : MonoBehaviour
{
    public GameObject center;
    private PulseNode left;
    private PulseNode right;
    private LineRenderer lineRenderer;
    private CapsuleCollider capsuleCollider;
    internal Vector3 directionFromCenter;
    public Transform makerOfPulse;
    public Color makerColor = Color.green;

    MeshFilter filter;
    List<Vector3> vertices = new List<Vector3>();
    Rigidbody rigidBody;

    /// <summary>
    /// The power/strength it will deal when hitting a enemy or a another pulse
    /// </summary>
    public float strength;

    public static PulseNode CreateNode(Pulse pulse, float curAngle, float distanceFromCenter, float strength, Transform maker, Color makerColor)
    {
        GameObject gameObject = new GameObject("Node");
        gameObject.transform.parent = pulse.transform;
        PulseNode node = gameObject.AddComponent<PulseNode>();
        node.transform.localPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * curAngle) * distanceFromCenter, 0, Mathf.Sin(Mathf.Deg2Rad * curAngle) * distanceFromCenter);
        node.InitNode(gameObject, strength, maker, makerColor);
        return node;
    }

    private void Start()
    {
        Statistics.instance.numberOfNodes++;
    }
    public void InitNode(GameObject center, float strength, Transform makerOfPulse, Color makerColor)
    {
        this.center = center;
        directionFromCenter = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        directionFromCenter.Normalize();
        this.strength = strength;
        this.makerOfPulse = makerOfPulse;
        this.makerColor = makerColor;
    }
    public void CreateLineBetweenNodes()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.InitilizeLine(transform, right.transform, makerColor);
    }
    public void CreateColliderLineBetweenNodes()
    {
        transform.LookAt(right.transform);
        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.InitiLineCollider(right.transform);
       
        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rigidBody.isKinematic = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        PulseNode nodeScript = other.GetComponent<PulseNode>();
        if (nodeScript != null && other.transform.parent != transform.parent)
        {
            if (nodeScript.makerOfPulse != makerOfPulse && !(strength <= 0 || nodeScript.strength <= 0))
            {
                float tempStrength = strength;
                strength -= nodeScript.strength;
                nodeScript.strength -= tempStrength;
                if (nodeScript.strength <= 0)
                {
                    nodeScript.gameObject.SetActive(false);
                }
                if (strength <= 0)
                {
                    gameObject.SetActive(false);
                }
            }

        }
        else if (other.transform != makerOfPulse && nodeScript == null)
        {
            IDamagable damagable = other.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(strength);
            }
            gameObject.SetActive(false);

        }

    }
    private void Update()
    {
        if (capsuleCollider.height > (Statistics.instance ? Statistics.instance.maxLengthBetweenNodes : 2))
        {
            CreateAdditionalNode();
        }
    }
    public void UpdateLineRender()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, right.transform.position);
    }
    public void UpdateCollider()
    {
        transform.LookAt(right.transform);
        capsuleCollider.height = Vector3.Distance(right.transform.position, transform.position);
        capsuleCollider.center = Vector3.forward * capsuleCollider.height * 0.5f;

    }
    public void CreateAdditionalNode()
    {
        //Creates a gameobject where it should be in the circle
        GameObject node = new GameObject(transform.name + " : Node : ");
        node.transform.parent = transform.parent;
        node.transform.position = (right.transform.position + transform.position) * 0.5f;

        //Make the gameobject into a node. 
        PulseNode newNodeScript = node.AddComponent<PulseNode>();
        // Sets all values to the correct values based on the Node that creates the new node.
        newNodeScript.InitNode(center, strength, makerOfPulse, makerColor);

        //Add in the nodes neighbours. And change the 
        newNodeScript.SetNeighbours(right, this);
        right.SetNeighbours(left: newNodeScript);
        SetNeighbours(newNodeScript);

        //Add the node into the list of the pulse
        GetComponentInParent<Pulse>().listOfNodes.Add(newNodeScript);
        newNodeScript.CreateLineBetweenNodes();
        newNodeScript.CreateColliderLineBetweenNodes();

        // For Debug purposes, should be false when playing or deploying game
        if (Statistics.instance.showWhereCircleSplits)
        {
            ShowPulseSplit();
        }
    }
    public void SetNeighbours(PulseNode right = null, PulseNode left = null)
    {
        if (right)
        {
            this.right = right;
        }
        if (left)
        {
            this.left = left;
        }
    }
    /// <summary>
    /// Leaving a box, where a node spilt.
    /// </summary>
    void ShowPulseSplit()
    {
        //Collector, holds all remnents.
        GameObject obj = Time.frameCount == Statistics.instance.lastFrameCount ? GameObject.Find((Statistics.instance.numberOfSplitsLevels).ToString()) : GameObject.Find((Statistics.instance.numberOfSplitsLevels + 1).ToString());
        if (obj == null || Statistics.instance.numberOfSplitsLevels == 0)
        {
            Statistics.instance.numberOfSplitsLevels++;
            Statistics.instance.lastFrameCount = Time.frameCount;
            Statistics.instance.latestColor = Random.ColorHSV();

            obj = new GameObject(Statistics.instance.numberOfSplitsLevels.ToString());
        }
        //Remnent when called.
        GameObject remnent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        remnent.transform.parent = obj.transform;
        remnent.transform.position = transform.position + (Vector3.up * (5 * Statistics.instance.numberOfSplitsLevels));
        remnent.GetComponent<Renderer>().material.color = Statistics.instance.latestColor;
    }
}
