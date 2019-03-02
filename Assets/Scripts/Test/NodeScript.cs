using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    public GameObject center;
    public NodeScript left;
    public NodeScript right;
    private LineRenderer lineRenderer;
    private CapsuleCollider collider;
    internal Vector3 directionFromCenter;
    public Material materialForNodes;
    MeshFilter filter;
    List<Vector3> vertices = new List<Vector3>();
    Rigidbody r;
    public float strength;
    private void Start()
    {
        Statistics.instance.numberOfNodes++;
    }
    public void CreateLineBetweenNodes()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = materialForNodes;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, right.transform.position);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material.color = Color.green;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
    public void CreateColliderLineBetweenNodes()
    {
        transform.LookAt(right.transform);
        collider = gameObject.AddComponent<CapsuleCollider>();
        collider.direction = 2;
        collider.height = Vector3.Distance(transform.position, right.transform.position);
        collider.radius = 0.05f;
        collider.center = Vector3.forward * collider.height * 0.5f;
        collider.isTrigger = true;
        r = gameObject.AddComponent<Rigidbody>();
        r.useGravity = false;
        r.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        r.isKinematic = true;
        //collider.size = new Vector3(0.2f,1,(right.transform.position - transform.position).sqrMagnitude );
    }
    private void OnTriggerEnter(Collider other)
    {
        NodeScript nodeScript = other.GetComponent<NodeScript>();
        if (other.transform.parent != transform.parent && nodeScript != null && nodeScript.strength > 0 && strength > 0)
        {
            float tempStrength = strength;
            strength -= nodeScript.strength;
            nodeScript.strength -= tempStrength;
            if (strength <= 0)
            {
                gameObject.SetActive(false);
            }
            if (nodeScript.strength <= 0)
            {
                nodeScript.gameObject.SetActive(false);
            }
        }
        else if(other.transform.parent != transform.parent && nodeScript == null)
        {
            gameObject.SetActive(false);
        }
        
    }
    private void Update()
    {
        if(collider.height > (Statistics.instance ? Statistics.instance.maxLengthBetweenNodes:2))
        {
            CreateAdditionalNode();
        }
    }
    public void UpdateLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, right.transform.position);
    }
    public void UpdateMesh()
    {
        transform.LookAt(right.transform);
        collider.height = Vector3.Distance(right.transform.position, transform.position);
        collider.center = Vector3.forward * collider.height * 0.5f;

    }
    public void CreateAdditionalNode()
    {
        GameObject node = new GameObject(transform.name + " : Node : " );
        node.transform.parent = transform.parent;
        node.transform.position = (right.transform.position + transform.position) * 0.5f;

        NodeScript nS = node.AddComponent<NodeScript>();
        nS.materialForNodes = materialForNodes;
        nS.center = center;
        nS.directionFromCenter = new Vector3(node.transform.localPosition.x, node.transform.localPosition.y, node.transform.localPosition.z);
        nS.directionFromCenter.Normalize();
        nS.strength = strength;

        nS.right = right;
        right = nS;
        nS.left = this;
        nS.right.left = nS;
        GetComponentInParent<ExpandingCircle>().listOfNodes.Add(nS);
        nS.CreateLineBetweenNodes();
        nS.CreateColliderLineBetweenNodes();

        if (Statistics.instance.showWhereCircleSplits)
        {
            //Collector
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
}
