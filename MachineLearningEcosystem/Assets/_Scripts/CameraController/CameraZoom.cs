using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{

    [Header("Zoom Settings")]
    public float Speed = 5f;
    public float MinZoom = 2f;
    public float MaxZoom = 20f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = GetComponent<Camera>();
        if (!cam.orthographic) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if(scroll != 0)
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

            cam.orthographicSize -= scroll * Speed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MinZoom, MaxZoom);

            Vector3 diff= mouseWorldPos - cam.transform.position;
            cam.transform.position += diff * scroll * 0.5f;
        }
        
    }
}
