using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseButton : MonoBehaviour
{

    [SerializeField] private GameObject pausePanel;


    void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        //esc pressed and not already shown: show
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !pausePanel.activeSelf)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        //if esc pressed and shown: unshow
        else if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }

    }
}
