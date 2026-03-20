using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    private Vector3 CameraPosition;


    [Header("Camera Settings")]
    public float CameraSpeed = 5f;


    [Header("Zoom Settings")]
    public float Speed = 5f;
    public float MinZoom = 2f;
    public float MaxZoom = 20f;
    public float zoomSpeedFactor = 0.0001f;


    void Start()
    {
        CameraPosition = transform.position;
    }



    void Update()
    {
        Camera cam = GetComponent<Camera>();
        float adjustedSpeed = CameraSpeed * (1 + (cam.orthographicSize - 5f) * zoomSpeedFactor);

        // CAMERA MOVEMENT
        if (Keyboard.current.wKey.isPressed)
        {
            CameraPosition.y += adjustedSpeed * Time.deltaTime;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            CameraPosition.y -= adjustedSpeed * Time.deltaTime;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            CameraPosition.x -= adjustedSpeed * Time.deltaTime;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            CameraPosition.x += adjustedSpeed * Time.deltaTime;
        }

        transform.position = CameraPosition;

        //ZOOM

        if (cam.orthographic)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll != 0)
            {
                cam.orthographicSize -= scroll * Speed;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MinZoom, MaxZoom);
            }
        }

        transform.position = new Vector3(CameraPosition.x, CameraPosition.y, transform.position.z);
    }
}


