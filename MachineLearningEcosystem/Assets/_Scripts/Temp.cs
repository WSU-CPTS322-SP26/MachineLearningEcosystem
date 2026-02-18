using System;
using UnityEngine;

public class Temp : MonoBehaviour
{
    String name;
    // Run before start as the scene is loaded
    void Awake()
    {
        name = "bob";   
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(name);
    }

    void FixedUpdate()
    {
        
    }
}
