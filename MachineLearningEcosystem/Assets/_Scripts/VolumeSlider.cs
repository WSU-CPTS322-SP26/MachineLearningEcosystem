using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer targetMixer;
    [SerializeField] private string subMixer = "mainVolume";
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(ChangeMixerVolume);
        slider.value = PlayerPrefs.GetFloat(subMixer);
    }

    private void ChangeMixerVolume(float value)
    {
        float volumeInDb = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f; // convert 0 to 1 slider value to db
        targetMixer.SetFloat(subMixer, volumeInDb);
        PlayerPrefs.SetFloat(subMixer, slider.value);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveAllListeners();
    }
}
