using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    private Vector3 origin = Vector3.zero;
    private float viewAngle = 90f;
    private float viewDistance = 20f;
    private int rayCount = 40;
    private float baseAngle = 0f;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        if (viewAngle > 0)
        {
            UpdateViewDirection(0, viewAngle - 1);;
        }
        else {
            UpdateViewDirection(0, 350);
        }
        CreateWedge();
    }
    
    private void CreateWedge()
    {
        vertices = new Vector3[rayCount + 2];
        uv = new Vector2[vertices.Length];
        triangles = new int[rayCount * 3];
        vertices[0] = origin;
        float angleInc = viewAngle / rayCount;
        int triangleIndex = 0;
        int vertexIndex = 1;
        float angle = baseAngle;
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

    public void SetViewAngle(float newAngle)
    {
        viewAngle = Math.Clamp(newAngle, 0, 360);
    }

    public void SetViewDistance(float newDistance)
    {
        viewDistance = newDistance;
    }

    public void UpdateViewDirection(float direction, float angle)
    {
        SetViewAngle(angle);
        baseAngle = (direction + (angle / 2)) % 360;
    }

    private static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}
