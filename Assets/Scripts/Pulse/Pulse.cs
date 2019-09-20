using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    private int degreesPerSegment = 45;
    private float distanceFromCenter = 1f;
    public List<PulseNode> listOfNodes = new List<PulseNode>();
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
        float curAngle = 0;
        PulseNode firstNode = null;
        PulseNode previousNode = null;
        this.speed = speed;
        this.maxDistance = maxDistance;
        for (int i = 0, j = 0; i < 360/ degreesPerSegment; i++, j++)
        {

            // Create a Node
            PulseNode node = PulseNode.CreateNode(this, curAngle, distanceFromCenter, strength, maker, makerColor);

            //Íncreasing this makes so the next node will have a diffrent position
            curAngle += degreesPerSegment;

            // Need to save the first node so we can connect it when all nodes are done
            if (firstNode == null)
            {
                firstNode = node;
            }

            //Sets the Neighours for the node
            //If we already made a node before this one then set make sure we are connected 
            if (previousNode != null)
            {
                node.SetNeighbours(left: previousNode); ;
                previousNode.SetNeighbours(node);
            }
            
            //Add it to the list for this pulse and then set the created one as the previous made node
            previousNode = node;
            listOfNodes.Add(node);
        }

        // Connect the first node and the last made node.
        previousNode.SetNeighbours(firstNode);
        firstNode.SetNeighbours(left:previousNode);

        foreach (PulseNode item in listOfNodes)
        {
            item.CreateLineBetweenNodes();
            item.CreateColliderLineBetweenNodes();
        }
        this.expand = expand;
    }

    // Update is called once per frame
    void Update()
    {
        // Should the pulse Expand
        if(expand)
        {
            distanceFromCenter += speed * Time.deltaTime;
        }
        // If the pulse is to big
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
                    item.UpdateCollider();
                    item.UpdateLineRender();
                }
            }
            previousDistanceFromCenter = distanceFromCenter;
        }
    }
}