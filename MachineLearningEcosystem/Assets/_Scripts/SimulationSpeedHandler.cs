using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class SimulationSpeedHandler : MonoBehaviour
{
    [SerializeField] private Slider timeSlider;


    private void Awake()
    {
        timeSlider.onValueChanged.AddListener(SliderChanged);
    }

    public void SliderChanged(float speedValue)
    {
        Time.timeScale = speedValue/100f;
    }

    private void OnDestroy()
    {
        timeSlider.onValueChanged.RemoveAllListeners();
    }
}
