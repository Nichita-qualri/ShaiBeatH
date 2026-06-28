using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI goalText;
    public GameObject victoryScreenPanel;
    public TextMeshProUGUI victoryScoreText;

    [Header("Фоны локаций")]
    public SpriteRenderer backgroundRenderer;
    public Sprite bgDay;
    public Sprite bgNight;
    public Sprite bgUnderground;

    [Header("Спрайты червя")]
    public SpriteRenderer wormRenderer;
    public Sprite wormDay;
    public Sprite wormNight;
    public Sprite wormUnderground;

    [HideInInspector] public int currentLevel = 1;
    private int _spiceGoal;
    private bool _levelComplete = false;

    void Awake()
    {
        Instance = this;
        victoryScreenPanel.SetActive(false);

        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        _spiceGoal = GetGoalForLevel(currentLevel);
        UpdateGoalUI();

        if (levelText != null)
            levelText.text = "LEVEL " + currentLevel;

        ApplyLevelDifficulty(currentLevel);
        ApplyBackground(currentLevel);
    }

    void Update()
    {
        if (_levelComplete) return;
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        if (GameManager.Instance != null &&
            GameManager.Instance.GetSpice() >= _spiceGoal)
        {
            LevelComplete();
        }
    }

    void ApplyLevelDifficulty(int level)
    {
        RhythmManager rhythm = FindObjectOfType<RhythmManager>();
        WormController worm = FindObjectOfType<WormController>();
        SpiceSpawner spawner = FindObjectOfType<SpiceSpawner>();

        if (rhythm != null)
            rhythm.bpm = 40f + (level - 1) * 4f;

        if (worm != null)
            worm.angrySpeed = 0.3f + (level - 1) * 0.1f;

        if (spawner != null)
            spawner.maxMarkersOnScreen = level < 5 ? 2 : level < 10 ? 3 : 4;
    }

    int GetGoalForLevel(int level)
    {
        return 300 + (level - 1) * 200;
    }

    void LevelComplete()
    {
        _levelComplete = true;

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();

        WormController worm = FindObjectOfType<WormController>();
        if (worm != null) worm.StopWorm();

        SpiceSpawner spawner = FindObjectOfType<SpiceSpawner>();
        if (spawner != null) spawner.StopSpawning();

        PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);
        PlayerPrefs.Save();

        if (victoryScreenPanel != null)
            victoryScreenPanel.SetActive(true);

        if (victoryScoreText != null)
            victoryScoreText.text = "LEVEL " + currentLevel + " COMPLETE!\n\n SPICE COLLECTED: " + GameManager.Instance.GetSpice();

        Invoke(nameof(ShowUpgrade), 2f);
    }

    void ShowUpgrade()
    {
        if (victoryScreenPanel != null)
            victoryScreenPanel.SetActive(false);

        int earned = GameManager.Instance != null ? GameManager.Instance.GetSpice() : 0;
        UpgradeManager.Instance?.ShowUpgradeScreen(earned);
    }

    void UpdateGoalUI()
    {
        if (goalText != null)
            goalText.text = "GOAL: " + _spiceGoal;
    }

    void ApplyBackground(int level)
    {
        if (backgroundRenderer != null)
        {
            if (level <= 5)
                backgroundRenderer.sprite = bgDay;
            else if (level <= 10)
                backgroundRenderer.sprite = bgNight;
            else
                backgroundRenderer.sprite = bgUnderground;
        }

        if (wormRenderer != null)
        {
            if (level <= 5)
                wormRenderer.sprite = wormDay;
            else if (level <= 10)
                wormRenderer.sprite = wormNight;
            else
                wormRenderer.sprite = wormUnderground;
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}