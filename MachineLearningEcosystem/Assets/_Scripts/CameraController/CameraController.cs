using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 CameraPosition;


    [Header("Camera Settings")]
    public float CameraSpeed = 5f;

    void Start()
    {
        CameraPosition = transform.position;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            CameraPosition.y += CameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            CameraPosition.y -= CameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            CameraPosition.x -= CameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            CameraPosition.x += CameraSpeed * Time.deltaTime;
        }

        transform.position = CameraPosition;
        
    }
}
