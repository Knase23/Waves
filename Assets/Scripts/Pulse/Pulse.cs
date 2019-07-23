using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    private int degreesPerSegment = 45;
    private float distanceFromCenter = 1f;
    public List<NodeScript> listOfNodes = new List<NodeScript>();

    private bool expand;
    private float speed;
    private float maxDistance = 10;
    private float previousDistanceFromCenter = 0;

    public static void CreatePulse(float speed, float strength, float maxDistance, Transform maker, Color makerColor)
    {
        GameObject gameObject = new GameObject("Pulse");
        gameObject.transform.position = maker.position;
        Pulse pulse = gameObject.AddComponent<Pulse>();
        pulse.Init(speed, strength, maxDistance, maker, makerColor);
    }
    // Start is called before the first frame update
    public void Init(float speed,float strength, float maxDistance, Transform maker, Color makerColor, int degreesPerSegment = 45,float distanceFromCenter = 1f, bool expand = true)
    {
        Statistics.instance.numberOfShips++;
        this.distanceFromCenter = distanceFromCenter;
        this.degreesPerSegment = degreesPerSegment;
        GameObject node;
        float curAngle = 0;
        NodeScript firstNode = null;
        NodeScript previousNode = null;
        this.speed = speed;
        this.maxDistance = maxDistance;
        for (int i = 0, j = 0; i < 360/ degreesPerSegment; i++, j++)
        {
            node = new GameObject("Node: " + i);
            node.transform.parent = transform;
            node.transform.localPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad *curAngle) * distanceFromCenter, 0, Mathf.Sin(Mathf.Deg2Rad * curAngle) * distanceFromCenter);            

            curAngle += degreesPerSegment;

            NodeScript  nS = node.AddComponent<NodeScript>();
            nS.strength = strength;
            nS.makerColor = makerColor;
            nS.center = gameObject;
            nS.directionFromCenter = new Vector3(node.transform.localPosition.x, node.transform.localPosition.y, node.transform.localPosition.z);
            nS.directionFromCenter.Normalize();
            
            nS.makerOfPulse = maker;
            if (previousNode != null)
            {
                nS.left = previousNode;
                previousNode.right = nS;
            }
            if (firstNode == null)
            {
                firstNode = nS;
            }
            previousNode = nS;
            listOfNodes.Add(nS);
        }

        previousNode.right = firstNode;
        firstNode.left = previousNode;

        foreach (NodeScript item in listOfNodes)
        {
            item.CreateLineBetweenNodes();
            item.CreateColliderLineBetweenNodes();
        }
        this.expand = expand;
    }

    // Update is called once per frame
    void Update()
    {
        if(expand)
        {
            distanceFromCenter += speed * Time.deltaTime;
        }
        if(distanceFromCenter > maxDistance)
        {
            expand = false;
            Destroy(gameObject);
        }

        if (previousDistanceFromCenter != distanceFromCenter)
        {
            foreach (var item in listOfNodes)
            {
                if (item)
                {
                    item.transform.localPosition = item.directionFromCenter * distanceFromCenter;
                }
            }
            foreach (var item in listOfNodes)
            {
                if (item)
                {
                    item.UpdateMesh();
                    item.UpdateLine();
                }
            }
            previousDistanceFromCenter = distanceFromCenter;
        }
    }
}