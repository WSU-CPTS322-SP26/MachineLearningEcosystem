using UnityEngine;
using UnityEngine.UI;

public class ResetSettingsToDefault : MonoBehaviour
{
    public const float SCANLINE_SPEED = 0.08f;
    public const float SCANLINE_COUNT = 175f;
    public const float BLOOM = 0.5f;

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
        mainVolume.onValueChanged.Invoke(mainVolume.maxValue);
        sfxVolume.value = sfxVolume.maxValue;
        sfxVolume.onValueChanged.Invoke(sfxVolume.maxValue);
        musicVolume.value = musicVolume.maxValue;
        musicVolume.onValueChanged.Invoke(musicVolume.maxValue);
        scanlineSpeed.value = SCANLINE_SPEED;
        scanlineSpeed.onValueChanged.Invoke(SCANLINE_SPEED);
        scanlineCount.value = SCANLINE_COUNT;
        scanlineCount.onValueChanged.Invoke(SCANLINE_COUNT);
        bloom.value = BLOOM;
        bloom.onValueChanged.Invoke(BLOOM);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
