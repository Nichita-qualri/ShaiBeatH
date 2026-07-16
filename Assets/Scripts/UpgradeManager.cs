using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("UI")]
    public GameObject upgradeScreen;
    public TextMeshProUGUI spiceText;
    public Button speedButton;
    public Button radiusButton;
    public Button armorButton;
    public TextMeshProUGUI speedButtonText;
    public TextMeshProUGUI radiusButtonText;
    public TextMeshProUGUI armorButtonText;

    [Header("Costs")]
    public int speedCost = 500;
    public int radiusCost = 500;
    public int armorCost = 800;

    [Header("Max Level")]
    public int maxUpgradeLevel = 5;

    private int _spiceBank;
    private int _speedLevel;
    private int _radiusLevel;
    private int _armorLevel;

    void Awake()
    {
        Instance = this;
        upgradeScreen.SetActive(false);

        // Load saved data
        _spiceBank = PlayerPrefs.GetInt("SpiceBank", 0);
        _speedLevel = PlayerPrefs.GetInt("SpeedLevel", 0);
        _radiusLevel = PlayerPrefs.GetInt("RadiusLevel", 0);
        _armorLevel = PlayerPrefs.GetInt("ArmorLevel", 0);
    }

    public void ShowUpgradeScreen(int earnedSpice)
    {
        // Add the earned spice to the bank
        _spiceBank += earnedSpice;
        PlayerPrefs.SetInt("SpiceBank", _spiceBank);
        PlayerPrefs.Save();

        upgradeScreen.SetActive(true);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (spiceText != null)
            spiceText.text = "SPICE: " + _spiceBank;

        UpdateButton(speedButton, speedButtonText, "⚡ SPEED", _speedLevel, speedCost);
        UpdateButton(radiusButton, radiusButtonText, "📡 RADIUS", _radiusLevel, radiusCost);
        UpdateButton(armorButton, armorButtonText, "🛡 ARMOR", _armorLevel, armorCost);
    }

    void UpdateButton(Button btn, TextMeshProUGUI txt, string label, int level, int cost)
    {
        if (btn == null || txt == null) return;

        if (level >= maxUpgradeLevel)
        {
            txt.text = label + "  MAX";
            btn.interactable = false;
            return;
        }

        txt.text = label + "  Lvl " + level + " → " + (level + 1) + "  [" + cost + "]";

        if (_spiceBank >= cost)
        {
            btn.interactable = true;
            txt.color = new Color(0.94f, 0.75f, 0f);
        }
        else
        {
            btn.interactable = false;
            txt.color = new Color(0.5f, 0.4f, 0.3f);
        }
    }

    public void UpgradeSpeed()
    {
        if (_spiceBank < speedCost || _speedLevel >= maxUpgradeLevel) return;
        _spiceBank -= speedCost;
        _speedLevel++;
        PlayerPrefs.SetInt("SpeedLevel", _speedLevel);
        PlayerPrefs.SetInt("SpiceBank", _spiceBank);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void UpgradeRadius()
    {
        if (_spiceBank < radiusCost || _radiusLevel >= maxUpgradeLevel) return;
        _spiceBank -= radiusCost;
        _radiusLevel++;
        PlayerPrefs.SetInt("RadiusLevel", _radiusLevel);
        PlayerPrefs.SetInt("SpiceBank", _spiceBank);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void UpgradeArmor()
    {
        if (_spiceBank < armorCost || _armorLevel >= maxUpgradeLevel) return;
        _spiceBank -= armorCost;
        _armorLevel++;
        PlayerPrefs.SetInt("ArmorLevel", _armorLevel);
        PlayerPrefs.SetInt("SpiceBank", _spiceBank);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void AddSpice(int amount)
    {
        _spiceBank += amount;
        PlayerPrefs.SetInt("SpiceBank", _spiceBank);
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void NextLevel()
    {
        upgradeScreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetSpeedLevel() => _speedLevel;
    public int GetRadiusLevel() => _radiusLevel;
    public int GetArmorLevel() => _armorLevel;
}