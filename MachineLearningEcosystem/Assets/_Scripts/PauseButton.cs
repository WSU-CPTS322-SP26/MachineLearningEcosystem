using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseButton : MonoBehaviour
{

    [SerializeField] private GameObject pausePanel;
    private float currentTimeScale;


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
            currentTimeScale = Time.timeScale;
            Time.timeScale = 0f;

        }
        //if esc pressed and shown: unshow
        else if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pausePanel.SetActive(false);
            Time.timeScale = currentTimeScale;
        }

    }
}
