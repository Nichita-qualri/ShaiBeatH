using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI pageIndicatorText; // optional, e.g. "1/4"

    [Header("Buttons")]
    public Button nextButton;
    public TextMeshProUGUI nextButtonLabel; // text inside nextButton, e.g. "NEXT" / "GOT IT"
    public Button closeButton; // optional separate close (X) button

    [Header("Steps")]
    [TextArea(2, 4)]
    public string[] steps = new string[]
    {
        "TAP the glowing spice markers in rhythm to move your harvester.",
        "SWIPE to dodge the worm when it gets close. You have a limited number of dodges per level.",
        "The worm chases you and speeds up if you miss a beat. Don't fall behind!",
        "SHIELD: upgrade Armor in the shop to get up to 2 shields per level. A shield saves you from one worm bite."
    };

    private int _currentStep = 0;

    void Awake()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    public void OpenTutorial()
    {
        _currentStep = 0;
        tutorialPanel.SetActive(true);
        ShowStep();
    }

    public void NextStep()
    {
        _currentStep++;
        if (_currentStep >= steps.Length)
        {
            CloseTutorial();
            return;
        }
        ShowStep();
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    void ShowStep()
    {
        if (tutorialText != null && _currentStep < steps.Length)
            tutorialText.text = steps[_currentStep];

        if (pageIndicatorText != null)
            pageIndicatorText.text = (_currentStep + 1) + "/" + steps.Length;

        if (nextButtonLabel != null)
            nextButtonLabel.text = (_currentStep == steps.Length - 1) ? "GOT IT" : "NEXT";
    }
}