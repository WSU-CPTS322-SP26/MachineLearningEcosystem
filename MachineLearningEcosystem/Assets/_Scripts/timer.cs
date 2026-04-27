using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(TextMeshProUGUI))]
public class GenerationTimer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    [SerializeField ]private TextMeshProUGUI subTimerText;
    private float elapsedTime = 0f;
    private float generationTime = 0f;
    private bool started = false;
    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        timerText.text = "Awaiting Start";
        if (subTimerText != null)
        {
            subTimerText.text = "Timer Ready";
        }
    }

    private void Update()
    {
        if (started)
        {
            elapsedTime += Time.deltaTime;
            generationTime += Time.deltaTime;

            timerText.text = (elapsedTime / 100f).ToString("N3") + " years";
            if (subTimerText != null)
            {
                subTimerText.text = "Current Generation: " + (generationTime / 100f).ToString("N3") + " years";
            }
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

    public void NewGeneration()
    {
        generationTime = 0f;
    }
}
