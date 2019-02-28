using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    public GameObject center;
    public NodeScript left;
    public NodeScript right;
    private LineRenderer lineRenderer;
    private Collider collider;
    internal Vector3 directionFromCenter;
    public Material materialForNodes;
    MeshFilter filter;
    List<Vector3> vertices = new List<Vector3>();

    public void CreateLineBetweenNodes()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = materialForNodes;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, right.transform.position);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material.color = Color.green;
        CreateMesh();
    }
    public void CreateColliderLineBetweenNodes()
    {
       // MeshCollider s = new MeshCollider();
        //s.
    }
    public void CreateMesh()
    {
        Mesh mesh = new Mesh();
        vertices.Add(Vector3.zero);
        vertices.Add((right.transform.position - transform.position));
        vertices.Add(Vector3.up * 0.2f);
        vertices.Add((right.transform.position - transform.position) + Vector3.up * 0.2f);

        mesh.SetVertices(vertices);
        mesh.triangles = new int[]
        {
            1, 0, 2,  // Front Down
            3, 1, 2, // Front Up
            2, 0, 1,  //Back Down
            2, 1, 3, // Back Up
        };
        mesh.RecalculateBounds();

        filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>();
    }

    private void Update()
    {
        if (lineRenderer)
        {
            UpdateLine();
        }

        Ray ray = new Ray(transform.position + (Vector3.up*0.1f), right.transform.position - transform.position);
        foreach (var item in Physics.RaycastAll(ray, (right.transform.position - transform.position).magnitude))
        {
            if(item.transform != transform && item.transform.parent != transform.parent)
            {
                Debug.Log( transform.name +" is Hitting: " + item.transform.name);
                lineRenderer.material.color = Color.red;
                break;
            }
            else
            {
                lineRenderer.material.color = Color.green;
            }
        }
        
    }
    void UpdateLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, right.transform.position);
    }
    public void UpdateMesh()
    {
        vertices[1] = right.transform.position - transform.position;
        vertices[3] = (right.transform.position - transform.position) + Vector3.up * 0.2f;
        filter.mesh.SetVertices(vertices);
    }
}
