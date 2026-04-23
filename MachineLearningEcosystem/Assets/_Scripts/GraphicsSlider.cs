using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;


[RequireComponent(typeof(Slider))]
public class GraphicsSlider : MonoBehaviour
{
    enum VFXSliderTarget {
        scanlineSpeed,
        scanlineCount,
        bloom,
    }
    private Slider slider;
    [SerializeField] private Material crtShader;
    [SerializeField] private Volume globalVolume;
    private Bloom bloom;
    [SerializeField] private VFXSliderTarget targetVal = VFXSliderTarget.scanlineSpeed;


    void Awake()
    {
        slider = GetComponent<Slider>();
        if (targetVal == VFXSliderTarget.bloom)
        {
            slider.onValueChanged.AddListener(ModifyBloom);
            if (globalVolume != null && globalVolume.profile.TryGet(out bloom))
            {
                // Set initial intensity
                bloom.intensity.value = PlayerPrefs.GetFloat("bloomIntensity");
                slider.SetValueWithoutNotify(PlayerPrefs.GetFloat("bloomIntensity"));
            }
        }
        else if (targetVal == VFXSliderTarget.scanlineCount)
        {
            slider.onValueChanged.AddListener(ModifyScanlineCount);
            ModifyScanlineCount(PlayerPrefs.GetFloat("scanlineCount"));
        }
        else
        {
            slider.onValueChanged.AddListener(ModifyScanlineSpeed);
            ModifyScanlineSpeed(PlayerPrefs.GetFloat("scanlineSpeed"));
        }

    }

    void ModifyBloom(float value)
    {
        if (bloom != null)
        {
            bloom.intensity.value = value;
        }
        PlayerPrefs.SetFloat("bloomIntensity", value);
    }

    void ModifyScanlineSpeed(float value)
    {
        if (crtShader != null)
        {
            crtShader.SetFloat("_Scanlines_Speed", Mathf.Clamp(value, -0.1f, 0.1f));
        }
        PlayerPrefs.SetFloat("scanlineSpeed", value);
    }

    void ModifyScanlineCount(float value)
    {
        if (crtShader != null)
        {
            crtShader.SetFloat("_Scanline_Count", Mathf.Clamp(value, 100, 200));
        }
        PlayerPrefs.SetFloat("scanlineCount", value);
    }

    void OnDestroy()
    {
        slider.onValueChanged.RemoveAllListeners();
    }
}
