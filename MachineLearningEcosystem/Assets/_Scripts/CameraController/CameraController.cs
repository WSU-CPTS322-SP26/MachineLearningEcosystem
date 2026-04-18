using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

    private Vector3 CameraPosition;
    private Camera cam;


    [Header("Camera Settings")]
    public float CameraSpeed = 180f; // when fully zoomed in


    [Header("Zoom Settings")]
    public float Speed = 5f;
    public float MinZoom = 2f;
    public float MaxZoom = 20f;
    public float zoomSpeedFactor = 1f;


    void Start()
    {
        CameraPosition = transform.position;
        cam = GetComponent<Camera>();
    }



    void Update()
    {
        float adjustedSpeed = CameraSpeed * (1 + (cam.orthographicSize - MinZoom) * zoomSpeedFactor);

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
        Vector2[] corners = MapManager.instance.GetCorners(); // bot left, bot right, top left, top right
        CameraPosition = new Vector3(Mathf.Clamp(CameraPosition.x, corners[0].x, corners[1].x),
                Mathf.Clamp(CameraPosition.y, corners[0].y, corners[3].y), CameraPosition.z);
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


