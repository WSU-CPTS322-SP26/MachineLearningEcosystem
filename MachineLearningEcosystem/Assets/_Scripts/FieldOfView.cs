using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider col;
    public List<GameObject> detectedObjects = new();
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    private Vector3 origin = Vector3.zero;
    private float viewAngle = 90f;
    private float viewDistance = 20f;
    private int rayCount = 40;
    private float baseAngle = 0f;
    private float direction = 0f;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        CreateWedge();
        DetectObjects();
    }

    private void DetectObjects()
    {
        detectedObjects.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewDistance);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;

            if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                // Debug.Log("Target Detected: " + target.name);
                detectedObjects.Add(target.gameObject);
            }
        }
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
    
    public List<GameObject> GetDetectedObjects()
    {
        return detectedObjects;
    }

    public void SetViewAngle(float newAngle)
    {
        viewAngle = Math.Clamp(newAngle, 0, 360);
    }

    public float GetViewAngle()
    {
        return viewAngle;
    }

    public void SetViewDistance(float newDistance)
    {
        viewDistance = newDistance;
    }

    public float GetViewDistance()
    {
        return viewDistance;
    }

    public void UpdateViewDirection(float newDirection, float angle)
    {
        SetViewAngle(angle);
        direction = newDirection;
        baseAngle = direction + (angle / 2);
    }

    public float GetViewDirection()
    {
        return direction;
    }

    private static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    // private void SpinFov()
    // {
    //     if (viewAngle > 0)
    //     {
    //         viewAngle -= 0.1f;
    //     }
    //     else {
    //         viewAngle = 350;
    //     }

    //     if (direction < 360f)
    //     {
    //         direction += 0.1f;
    //     }
    //     else
    //     {
    //         direction = 0f;
    //     }

    //     if (viewDistance > 50)
    //     {
    //         viewDistance = 1;
    //     }
    //     else
    //     {
    //         viewDistance += 0.1f;
    //     }
    //     UpdateViewDirection(direction, viewAngle);;
    // }
}
