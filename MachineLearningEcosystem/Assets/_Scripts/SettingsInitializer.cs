using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsInitializer : MonoBehaviour
{
    [SerializeField] private Material crtShader;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private AudioMixer targetMixer;
    private Bloom bloom;

    void Start()
    {
        if (PlayerPrefs.GetInt("New") == 0)
        {
            PlayerPrefs.SetFloat("mainVolume", 1f);
            PlayerPrefs.SetFloat("sfxVolume", 1f);
            PlayerPrefs.SetFloat("musicVolume", 1f);
            PlayerPrefs.SetFloat("bloomIntensity", 0.5f);
            PlayerPrefs.SetFloat("scanlineSpeed", 0.08f);
            PlayerPrefs.SetFloat("scanlineCount", 175f);
            PlayerPrefs.SetInt("New", 1);
        }
        // Volume settings
        targetMixer.SetFloat("mainVolume", GetVolumeInDb(PlayerPrefs.GetFloat("mainVolume")));
        targetMixer.SetFloat("sfxVolume", GetVolumeInDb(PlayerPrefs.GetFloat("sfxVolume")));
        targetMixer.SetFloat("musicVolume", GetVolumeInDb(PlayerPrefs.GetFloat("musicVolume")));

        // Graphix
        if (globalVolume != null && globalVolume.profile.TryGet(out bloom))
        {
            // Set initial intensity
            bloom.intensity.value = PlayerPrefs.GetFloat("bloomIntensity");
        }
        crtShader.SetFloat("_Scanlines_Speed", Mathf.Clamp(PlayerPrefs.GetFloat("scanlineSpeed"), -0.1f, 0.1f));
        crtShader.SetFloat("_Scanline_Count", Mathf.Clamp(PlayerPrefs.GetFloat("scanlineCount"), -0.1f, 0.1f));

    }

    public static float GetVolumeInDb(float value)
    {
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f; // convert 0 to 1 slider value to db
    }
}
