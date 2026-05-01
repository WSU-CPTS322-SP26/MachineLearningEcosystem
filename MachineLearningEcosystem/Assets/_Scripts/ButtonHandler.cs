using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    // [SerializeField] private string nextScene;
    // [SerializeField] private string menuScene;
    // [SerializeField] private string creditsScene;

    public void PlayNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void GoToCredits()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
