using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainGenerationUI : MonoBehaviour
{
    [SerializeField] private Button GenerateButton;
    [SerializeField] private Button PlayButton;
    [SerializeField] private TMP_InputField xDimInput;
    [SerializeField] private TMP_InputField yDimInput;

    void Start()
    {
        GenerateButton.onClick.AddListener(OnGenerateButtonClicked);
        PlayButton.onClick.AddListener(OnPlayButtonClicked);
        xDimInput.text = MapManager.GetXDim().ToString();
        yDimInput.text = MapManager.GetYDim().ToString();
    }
    private void OnGenerateButtonClicked()
    {
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
        gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        GenerateButton.onClick.RemoveListener(OnGenerateButtonClicked);
        PlayButton.onClick.RemoveListener(OnPlayButtonClicked);
    }
}
