using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI finalScoreText;
    public GameObject deathScreenPanel;

    public static DeathScreen Instance;

    void Awake()
    {
        Instance = this;
        deathScreenPanel.SetActive(false);
    }

    public void ShowDeathScreen(int score)
    {
        deathScreenPanel.SetActive(true);
        if (finalScoreText != null)
            finalScoreText.text = "SPICE: " + score;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}