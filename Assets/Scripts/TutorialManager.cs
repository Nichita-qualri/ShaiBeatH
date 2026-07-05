using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText1;
    public TextMeshProUGUI tutorialText2;
    public TextMeshProUGUI tutorialText3;

    private bool _tutorialShown = false;

    void Start()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);

        // Показываем туториал только на первом уровне
        if (level == 1 && !_tutorialShown)
        {
            tutorialPanel.SetActive(true);
            StartCoroutine(ShowTutorial());
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    IEnumerator ShowTutorial()
    {
        // Показываем первый текст
        tutorialText1.gameObject.SetActive(true);
        tutorialText2.gameObject.SetActive(false);
        tutorialText3.gameObject.SetActive(false);

        yield return new WaitForSeconds(3f);

        // Показываем второй текст
        tutorialText1.gameObject.SetActive(false);
        tutorialText2.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        // Показываем третий текст
        tutorialText2.gameObject.SetActive(false);
        tutorialText3.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        // Скрываем туториал
        tutorialPanel.SetActive(false);
        _tutorialShown = true;
    }
}