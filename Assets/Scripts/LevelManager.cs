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

    [Header("Backgrounds")]
    public SpriteRenderer backgroundRenderer;
    public Sprite bgDay;
    public Sprite bgNight;
    public Sprite bgUnderground;

    [Header("Worm Sprites")]
    public SpriteRenderer wormRenderer;
    public Sprite wormDay;
    public Sprite wormNight;
    public Sprite wormUnderground;

    [Header("Endless Mode")]
    public int maxLevels = 15;

    [HideInInspector] public int currentLevel = 1;
    [HideInInspector] public bool isEndlessMode = false;

    private int _spiceGoal;
    private bool _levelComplete = false;
    private int _endlessMultiplier = 1;

    void Awake()
    {
        Instance = this;
        victoryScreenPanel.SetActive(false);

        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        // Миграция старых сохранений: если MaxUnlockedLevel ещё нет,
        // берём максимум из CurrentLevel, чтобы не потерять прогресс
        int maxUnlocked = PlayerPrefs.GetInt("MaxUnlockedLevel", 1);
        if (currentLevel > maxUnlocked)
        {
            PlayerPrefs.SetInt("MaxUnlockedLevel", currentLevel);
            PlayerPrefs.Save();
        }

        // Endless Mode если прошли все уровни
        if (currentLevel > maxLevels)
        {
            isEndlessMode = true;
            _endlessMultiplier = currentLevel - maxLevels;
        }

        _spiceGoal = GetGoalForLevel(currentLevel);
        UpdateGoalUI();

        if (levelText != null)
        {
            if (isEndlessMode)
                levelText.text = "ENDLESS " + _endlessMultiplier;
            else
                levelText.text = "LEVEL " + currentLevel;
        }

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

        // В Endless Mode сложность продолжает расти
        int effectiveLevel = isEndlessMode ? maxLevels + _endlessMultiplier : level;

        if (rhythm != null)
            rhythm.bpm = Mathf.Min(40f + (effectiveLevel - 1) * 4f, 120f);

        if (worm != null)
        {
            worm.normalSpeed = 0.15f + (level - 1) * 0.03f;
            worm.angrySpeed = 0.5f + (level - 1) * 0.15f;
        }

        if (spawner != null)
            spawner.maxMarkersOnScreen = effectiveLevel < 5 ? 2 : effectiveLevel < 10 ? 3 : 4;
    }

    int GetGoalForLevel(int level)
    {
        if (isEndlessMode)
            return 500 + _endlessMultiplier * 300;
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

        int nextLevel = currentLevel + 1;

        // Обновляем МАКСИМАЛЬНЫЙ открытый уровень, только если nextLevel больше
        int maxUnlocked = PlayerPrefs.GetInt("MaxUnlockedLevel", 1);
        if (nextLevel > maxUnlocked)
        {
            PlayerPrefs.SetInt("MaxUnlockedLevel", nextLevel);
        }

        // CurrentLevel двигаем вперёд, чтобы "Next Level" вёл на новый уровень
        PlayerPrefs.SetInt("CurrentLevel", nextLevel);
        PlayerPrefs.Save();

        if (victoryScreenPanel != null)
            victoryScreenPanel.SetActive(true);

        if (victoryScoreText != null)
        {
            if (isEndlessMode)
                victoryScoreText.text = "ENDLESS " + _endlessMultiplier + " COMPLETE!\n\nSPICE COLLECTED: " + GameManager.Instance.GetSpice();
            else
                victoryScoreText.text = "LEVEL " + currentLevel + " COMPLETE!\n\nSPICE COLLECTED: " + GameManager.Instance.GetSpice();
        }

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
            if (isEndlessMode || level > 10)
                backgroundRenderer.sprite = bgUnderground;
            else if (level <= 5)
                backgroundRenderer.sprite = bgDay;
            else
                backgroundRenderer.sprite = bgNight;
        }

        if (wormRenderer != null)
        {
            if (isEndlessMode || level > 10)
                wormRenderer.sprite = wormUnderground;
            else if (level <= 5)
                wormRenderer.sprite = wormDay;
            else
                wormRenderer.sprite = wormNight;
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}