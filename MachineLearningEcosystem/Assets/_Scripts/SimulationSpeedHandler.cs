using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationSpeedHandler : MonoBehaviour
{
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI simulationSpeedText = null;


    private void Awake()
    {
        timeSlider.onValueChanged.AddListener(SliderChanged);
    }

    public void SliderChanged(float speedValue)
    {
        Time.timeScale = speedValue/100f;
        simulationSpeedText.text = "x" + (speedValue/20).ToString();
    }

    private void OnDestroy()
    {
        timeSlider.onValueChanged.RemoveAllListeners();
    }
}
