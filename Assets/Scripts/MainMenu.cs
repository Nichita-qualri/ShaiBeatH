using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelSelect");
    }
    public void OpenShop()
    {
        SceneManager.LoadScene("Shop");
    }
    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    public void OpenTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }
    public void CloseTutorial()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}