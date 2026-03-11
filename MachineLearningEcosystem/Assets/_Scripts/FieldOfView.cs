using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    // private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    private Vector3 origin = Vector3.zero;
    private float viewAngle = 90f;
    private float viewDistance = 20f;
    private int rayCount = 22;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[rayCount + 2];
        uv = new Vector2[vertices.Length];
        triangles = new int[rayCount * 3];
        vertices[0] = origin;

        float angle = 0f;
        float angleInc = viewAngle / rayCount;
        int triangleIndex = 0;
        int vertexIndex = 1;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = origin + (GetVectorFromAngle(angle) * viewDistance);
            vertices[vertexIndex] = vertex; // vertices are >0 bc 0 is the origin
            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleInc;
        }
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void CreateWedge()
    {
        
    }

    public void SetViewAngle(int newAngle)
    {
        viewAngle = newAngle;
        rayCount = (int)(viewAngle / 4);
    }

    public void SetOrigin(Vector3 newOrigin)
    {
        origin = newOrigin;
    }

    private static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}
