using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private SceneAsset nextScene;
    [SerializeField] private SceneAsset menuScene;

    public void PlayNextScene()
    {
        SceneManager.LoadScene(nextScene.name);
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(menuScene.name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
