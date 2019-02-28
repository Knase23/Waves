using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingCircle : MonoBehaviour
{
    private GameObject circleCenter;
    public int degreesPerSegment = 10;
    [Range(0f,20f)]public float distanceFromCenter = 5f;
    private float previousDistanceFromCenter = 0;
    public bool centerHaveLineRenderer = false;
    public Material materialForNodes;
    private List<NodeScript> listOfNodes = new List<NodeScript>();
    // Start is called before the first frame update
    void Start()
    {
        circleCenter = new GameObject("Circle Center");
        circleCenter.transform.parent = transform;
        circleCenter.transform.localPosition = Vector3.zero;
        GameObject node;
        float curAngle = 0;
        NodeScript firstNode = null;
        NodeScript previousNode = null;
        

        LineRenderer lineRenderer = null;
        if(centerHaveLineRenderer)
            lineRenderer = circleCenter.AddComponent<LineRenderer>();

        if (lineRenderer)
        {
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.positionCount = 360 / degreesPerSegment + 1;
        }

        for (int i = 0, j = 0; i < 360/ degreesPerSegment + 1; i++, j++)
        {
            node = new GameObject("Node: " + i);
            node.transform.parent = circleCenter.transform;
            node.transform.localPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad *curAngle) * distanceFromCenter, 0, Mathf.Sin(Mathf.Deg2Rad * curAngle) * distanceFromCenter);

            

            curAngle += degreesPerSegment;
            NodeScript  nS = node.AddComponent<NodeScript>();
            nS.materialForNodes = materialForNodes;
            nS.center = circleCenter;
            nS.directionFromCenter = new Vector3(node.transform.localPosition.x, node.transform.localPosition.y, node.transform.localPosition.z);
            nS.directionFromCenter.Normalize();
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

            if (lineRenderer)
                lineRenderer.SetPosition(j, previousNode.transform.position);
            
        }

        if (lineRenderer)
            lineRenderer.loop = true;

        previousNode.right = firstNode;
        firstNode.left = previousNode;

        foreach (NodeScript item in listOfNodes)
        {
            item.CreateLineBetweenNodes();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (previousDistanceFromCenter != distanceFromCenter)
        {
            foreach (var item in listOfNodes)
            {
                item.transform.localPosition = item.directionFromCenter * distanceFromCenter;
                
            }
            foreach (var item in listOfNodes)
            {
                item.UpdateMesh();
            }
            previousDistanceFromCenter = distanceFromCenter;
        }
    }
}
