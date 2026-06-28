using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI spiceText;
    public TextMeshProUGUI comboText;

    [Header("Настройки")]
    public int spicePerMarker = 100;

    [HideInInspector] public bool isGameOver = false;

    private int _totalSpice = 0;

    void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void AddSpice(int multiplier)
    {
        int amount = spicePerMarker * multiplier;
        _totalSpice += amount;

        if (spiceText != null)
            spiceText.text = "SPICE: " + _totalSpice;

        if (comboText != null)
        {
            comboText.text = "x" + multiplier + "!";
            if (multiplier >= 4)
                comboText.color = new Color(1f, 0.2f, 0f);
            else if (multiplier >= 3)
                comboText.color = new Color(1f, 0.7f, 0f);
            else if (multiplier >= 2)
                comboText.color = new Color(0.9f, 1f, 0f);
            else
                comboText.color = new Color(0.78f, 0.63f, 0.38f);

            CancelInvoke(nameof(HideCombo));
            comboText.alpha = 1f;
            Invoke(nameof(HideCombo), 1.5f);
        }
    }

    void HideCombo()
    {
        StartCoroutine(FadeCombo());
    }

    System.Collections.IEnumerator FadeCombo()
    {
        while (comboText.alpha > 0f)
        {
            comboText.alpha -= Time.deltaTime * 1f;
            yield return null;
        }
        comboText.alpha = 0f;
    }

    public int GetSpice() => _totalSpice;
}