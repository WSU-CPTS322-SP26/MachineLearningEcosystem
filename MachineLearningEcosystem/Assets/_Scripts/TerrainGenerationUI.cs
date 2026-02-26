using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class TerrainGenerationUI : MonoBehaviour
{
    [SerializeField] private Button generateButton;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_InputField xDimInput;
    [SerializeField] private TMP_InputField yDimInput;
    [SerializeField] private Toggle animationToggle;
    [SerializeField] private Slider animationSpeedSlider;
    private Color originalColor;

    void Start()
    {
        generateButton.onClick.AddListener(OnGenerateButtonClicked);
        playButton.onClick.AddListener(OnPlayButtonClicked);
        animationToggle.onValueChanged.AddListener(OnAnimationToggleChanged);
        animationSpeedSlider.onValueChanged.AddListener(OnAnimationSpeedChanged);
        animationSpeedSlider.value = (float)ProceduralGeneration.GetAnimationSpeed() / ProceduralGeneration.GetMaxAnimationSpeed();
        animationToggle.isOn = ProceduralGeneration.GetAnimated();
        xDimInput.text = MapManager.GetXDim().ToString();
        yDimInput.text = MapManager.GetYDim().ToString();
        originalColor = generateButton.image.color;
    }
    private void OnGenerateButtonClicked()
    {
        if (ProceduralGeneration.IsGenerating())
        {
            StartCoroutine(ButtonFlash(generateButton));
            return;
        }
        int xDim = int.Parse(xDimInput.text);
        int yDim = int.Parse(yDimInput.text);
        xDim = Mathf.Clamp(xDim, 40, 200);
        yDim = Mathf.Clamp(yDim, 40, 200);
        xDimInput.text = xDim.ToString();
        yDimInput.text = yDim.ToString();
        MapManager.instance.SetXDim(xDim);
        MapManager.instance.SetYDim(yDim);
        MapManager.instance.GenerateMap();
    }
    private void OnPlayButtonClicked()
    {
        if (ProceduralGeneration.IsGenerating())
        {
            StartCoroutine(ButtonFlash(playButton));
            return;
        }
        gameObject.SetActive(false);
    }
    private void OnAnimationToggleChanged(bool isOn)
    {
        ProceduralGeneration.SetAnimated(isOn);
    }
    private void OnAnimationSpeedChanged(float value)
    {
        ProceduralGeneration.SetAnimationSpeed(Math.Max((int)(value * ProceduralGeneration.GetMaxAnimationSpeed()), 1));
    }
    void OnDestroy()
    {
        generateButton.onClick.RemoveListener(OnGenerateButtonClicked);
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        animationToggle.onValueChanged.RemoveListener(OnAnimationToggleChanged);
        animationSpeedSlider.onValueChanged.RemoveListener(OnAnimationSpeedChanged);
    }
    private IEnumerator ButtonFlash(Button button)
    {
        button.image.color = Color.red;
        float flashDuration = 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            button.image.color = Color.Lerp(Color.red, originalColor, elapsedTime / flashDuration);
            yield return null;
        }
        button.image.color = originalColor;
    }
}
