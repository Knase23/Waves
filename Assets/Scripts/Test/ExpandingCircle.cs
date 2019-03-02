using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingCircle : MonoBehaviour
{
    public int degreesPerSegment = 10;

    [Range(0f,20f)]
    public float distanceFromCenter = 5f;

    public Material materialForNodes;

    public List<NodeScript> listOfNodes = new List<NodeScript>();

    public bool expand;
    public float speed;
    public float strength = 100;
    public float maxDistance = 10;


    private float previousDistanceFromCenter = 0;
    // Start is called before the first frame update
    void Start()
    {
        Statistics.instance.numberOfShips++;
        GameObject node;
        float curAngle = 0;
        NodeScript firstNode = null;
        NodeScript previousNode = null;
        for (int i = 0, j = 0; i < 360/ degreesPerSegment; i++, j++)
        {
            node = new GameObject("Node: " + i);
            node.transform.parent = transform;
            node.transform.localPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad *curAngle) * distanceFromCenter, 0, Mathf.Sin(Mathf.Deg2Rad * curAngle) * distanceFromCenter);

            

            curAngle += degreesPerSegment;
            NodeScript  nS = node.AddComponent<NodeScript>();
            nS.materialForNodes = materialForNodes;
            nS.center = gameObject;
            nS.directionFromCenter = new Vector3(node.transform.localPosition.x, node.transform.localPosition.y, node.transform.localPosition.z);
            nS.directionFromCenter.Normalize();
            nS.strength = strength;
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
            gameObject.SetActive(false);
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
