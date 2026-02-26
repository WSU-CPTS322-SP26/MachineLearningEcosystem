using UnityEngine;
using UnityEngine.InputSystem;

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
        if (Keyboard.current.wKey.isPressed)
        {
            CameraPosition.y += CameraSpeed * Time.deltaTime;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            CameraPosition.y -= CameraSpeed * Time.deltaTime;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            CameraPosition.x -= CameraSpeed * Time.deltaTime;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            CameraPosition.x += CameraSpeed * Time.deltaTime;
        }

        transform.position = CameraPosition;
        
    }
}
