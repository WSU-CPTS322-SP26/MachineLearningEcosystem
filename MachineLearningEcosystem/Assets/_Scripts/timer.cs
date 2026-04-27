using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(TextMeshProUGUI))]
public class GenerationTimer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float elapsedTime;
    private bool started = false;
    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        timerText.text = "Awaiting Start";
    }

    private void Update()
    {
        if (started)
        {
            elapsedTime += Time.deltaTime;

            timerText.text = (elapsedTime / 100f).ToString("N3") + " years";
        }
    }

    public void StartTimer()
    {
        started = true;
    }

    public void PauseTimer()
    {
        started = false;
    }

    public float StopTimer()
    {
        started = false;
        return elapsedTime;
    }
}
