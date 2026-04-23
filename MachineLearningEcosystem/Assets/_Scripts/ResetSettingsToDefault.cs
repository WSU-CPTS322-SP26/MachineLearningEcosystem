using UnityEngine;
using UnityEngine.UI;

public class ResetSettingsToDefault : MonoBehaviour
{
    private const float SCANLINE_SPEED = 0.08f;
    private const float SCANLINE_COUNT = 175f;
    private const float BLOOM = 0.5f;

    [SerializeField] private Slider mainVolume;
    [SerializeField] private Slider sfxVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider scanlineSpeed;
    [SerializeField] private Slider scanlineCount;
    [SerializeField] private Slider bloom;
    [SerializeField] private Button button;
    private void Awake()
    {
        button.onClick.AddListener(ResetSettings);
    }

    private void ResetSettings()
    {
        mainVolume.value = mainVolume.maxValue;
        sfxVolume.value = sfxVolume.maxValue;
        musicVolume.value = musicVolume.maxValue;
        scanlineSpeed.value = SCANLINE_SPEED;
        scanlineCount.value = SCANLINE_COUNT;
        bloom.value = BLOOM;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
