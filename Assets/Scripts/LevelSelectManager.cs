using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("UI")]
    public Transform levelsGrid;
    public GameObject levelButtonPrefab;

    [Header("Настройки")]
    public int totalLevels = 15;

    private int _unlockedLevels;

    void Start()
    {
        // Читаем МАКСИМАЛЬНЫЙ открытый уровень, а не текущий выбранный
        _unlockedLevels = PlayerPrefs.GetInt("MaxUnlockedLevel", 1);
        CreateLevelButtons();
    }

    void CreateLevelButtons()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject btn = Instantiate(levelButtonPrefab, levelsGrid);
            btn.name = "Level_" + i;
            TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
            Button button = btn.GetComponent<Button>();
            Image image = btn.GetComponent<Image>();
            int levelNum = i;

            if (i <= _unlockedLevels)
            {
                text.text = i.ToString();
                button.interactable = true;
                image.color = new Color(0.78f, 0.63f, 0.38f);
                button.onClick.AddListener(() => SelectLevel(levelNum));
            }
            else
            {
                text.text = "?";
                button.interactable = false;
                image.color = new Color(0.16f, 0.09f, 0.03f);
            }
        }
    }

    void AddChapterLabel(string title, Transform parent, int index)
    {
        GameObject label = new GameObject(title);
        label.transform.SetParent(parent);
        label.transform.SetSiblingIndex(index);
        TextMeshProUGUI text = label.AddComponent<TextMeshProUGUI>();
        text.text = title;
        text.fontSize = 28;
        text.color = new Color(0.94f, 0.75f, 0f);
        text.alignment = TextAlignmentOptions.Center;
        RectTransform rt = label.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(900, 50);
    }

    void SelectLevel(int level)
    {
        // Тут только выбираем, какой уровень ЗАГРУЗИТЬ.
        // MaxUnlockedLevel не трогаем — прогресс не теряется
        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}