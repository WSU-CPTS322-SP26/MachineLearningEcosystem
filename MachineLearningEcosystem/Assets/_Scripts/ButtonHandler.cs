using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private SceneAsset nextScene;
    [SerializeField] private SceneAsset menuScene;
    [SerializeField] private SceneAsset creditsScene;

    public void PlayNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextScene.name);
    }
    public void GoToCredits()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(creditsScene.name);
    }
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene.name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
