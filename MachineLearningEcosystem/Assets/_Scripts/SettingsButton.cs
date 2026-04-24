using UnityEngine;
using UnityEngine.UIElements;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        //settingsPanel.SetActive(false);
    }
    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }
    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
}
