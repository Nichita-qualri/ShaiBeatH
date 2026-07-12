using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public Image skinImage;
    public TextMeshProUGUI skinName;
    public TextMeshProUGUI skinDescription;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;
    public TextMeshProUGUI dotsText;

    [Header("Skin Sprites")]
    public Sprite[] skinSprites;

    [Header("Skin Colors (fallback if no sprite)")]
    public Color[] skinColors;

    [Header("Panels")]
    public GameObject skinPanel;
    public GameObject spicePanel;

    [Header("Purchase Feedback")]
    public TextMeshProUGUI purchaseToastText; // optional, brief "Purchased!" popup
    public float toastDuration = 1.5f;

    [Header("Spice Pack Buttons")]
    public Button smallPackButton;
    public TextMeshProUGUI smallPackButtonText;
    public Button mediumPackButton;
    public TextMeshProUGUI mediumPackButtonText;
    public Button largePackButton;
    public TextMeshProUGUI largePackButtonText;

    private int _currentSkin = 0;
    private int _selectedSkin = 0;

    private string[] _names = {
    "Standard",
    "Vanguard",
    "Ironclad",
    "Nomad"
};

    private string[] _descriptions = {
    "Standard spice harvesting machine",
    "Vanguard harvester. 10% faster",
    "Ironclad harvester. 10% more durable",
    "Nomad harvester. Silent and stealthy"
};

    private string[] _prices = {
        "FREE",
        "0.99$",
        "0.99$",
        "1.99$"
    };

    private bool[] _unlocked;

    void Start()
    {
        // Load unlock states from PlayerPrefs (Standard is unlocked by default)
        _unlocked = new bool[_names.Length];
        for (int i = 0; i < _names.Length; i++)
            _unlocked[i] = i == 0 || PlayerPrefs.GetInt("SkinUnlocked_" + i, 0) == 1;

        ShowHarvesters();
        _selectedSkin = PlayerPrefs.GetInt("SelectedSkin", 0);
        _currentSkin = _selectedSkin;
        UpdateUI();

        if (purchaseToastText != null)
            purchaseToastText.alpha = 0f;

        RefreshSpicePackButtons();
    }

    public void NextSkin()
    {
        _currentSkin = (_currentSkin + 1) % _names.Length;
        UpdateUI();
    }

    public void PrevSkin()
    {
        _currentSkin = (_currentSkin - 1 + _names.Length) % _names.Length;
        UpdateUI();
    }

    void UpdateUI()
    {
        // Sprite or color
        if (skinImage != null)
        {
            if (skinSprites != null && skinSprites.Length > _currentSkin && skinSprites[_currentSkin] != null)
            {
                skinImage.sprite = skinSprites[_currentSkin];
                skinImage.color = Color.white;
            }
            else if (skinColors != null && skinColors.Length > _currentSkin)
            {
                skinImage.sprite = null;
                Color c = skinColors[_currentSkin];
                skinImage.color = new Color(c.r, c.g, c.b, 1f);
            }
        }

        if (skinName != null)
            skinName.text = _names[_currentSkin];

        if (skinDescription != null)
            skinDescription.text = _descriptions[_currentSkin];

        if (buyButtonText != null)
        {
            if (_currentSkin == _selectedSkin)
            {
                buyButtonText.text = "SELECTED";
                buyButton.interactable = true;
                buyButton.GetComponent<Image>().color = new Color(0.78f, 0.63f, 0.38f);
            }
            else if (_unlocked[_currentSkin])
            {
                buyButtonText.text = "SELECT";
                buyButton.interactable = true;
                buyButton.GetComponent<Image>().color = new Color(0.30f, 0.18f, 0.06f);
            }
            else
            {
                buyButtonText.text = "BUY " + _prices[_currentSkin];
                buyButton.interactable = true;
                buyButton.GetComponent<Image>().color = new Color(0.30f, 0.18f, 0.06f);
            }
        }

        if (dotsText != null)
        {
            string dots = "";
            for (int i = 0; i < _names.Length; i++)
                dots += (i == _currentSkin) ? "● " : "○ ";
            dotsText.text = dots.Trim();
        }
    }

    public void OnBuyButton()
    {
        if (_currentSkin == _selectedSkin) return;

        if (_unlocked[_currentSkin])
        {
            SelectSkin(_currentSkin);
        }
        else
        {
            // Fake purchase — no real transaction, instantly unlocks
            _unlocked[_currentSkin] = true;
            PlayerPrefs.SetInt("SkinUnlocked_" + _currentSkin, 1);
            PlayerPrefs.Save();

            SelectSkin(_currentSkin);
            ShowPurchaseToast();
        }
    }

    void SelectSkin(int index)
    {
        _selectedSkin = index;
        PlayerPrefs.SetInt("SelectedSkin", _selectedSkin);
        PlayerPrefs.Save();
        UpdateUI();
    }

    void ShowPurchaseToast()
    {
        if (purchaseToastText == null) return;
        CancelInvoke(nameof(HideToast));
        purchaseToastText.text = "PURCHASED!";
        purchaseToastText.alpha = 1f;
        Invoke(nameof(HideToast), toastDuration);
    }

    void HideToast()
    {
        if (purchaseToastText != null)
            purchaseToastText.alpha = 0f;
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BuySmallPack()
    {
        BuyPack("small", 500, "1.99$", smallPackButton, smallPackButtonText);
    }

    public void BuyMediumPack()
    {
        ShowComingSoon(mediumPackButtonText);
    }

    public void BuyLargePack()
    {
        ShowComingSoon(largePackButtonText);
    }

    void ShowComingSoon(TextMeshProUGUI label)
    {
        if (label == null) return;
        string original = label.text;
        label.text = "COMING SOON";
        CancelInvoke(nameof(ResetComingSoonLabels));
        Invoke(nameof(ResetComingSoonLabels), 1.5f);
    }

    void ResetComingSoonLabels()
    {
        // Restore Medium/Large labels to their normal price text (they stay locked either way)
        if (mediumPackButtonText != null)
            mediumPackButtonText.text = "1500 Spice - 4.99$";
        if (largePackButtonText != null)
            largePackButtonText.text = "4000 Spice - 9.99$";
    }

    void BuyPack(string id, int amount, string price, Button btn, TextMeshProUGUI label)
    {
        bool alreadyPurchased = PlayerPrefs.GetInt("SpicePackBought_" + id, 0) == 1;
        if (alreadyPurchased) return;

        // Fake purchase — no real transaction, one-time only
        PlayerPrefs.SetInt("SpicePackBought_" + id, 1);
        PlayerPrefs.Save();

        // Write directly to PlayerPrefs so this works even if UpgradeManager
        // hasn't been created yet in this scene (e.g. the Shop scene).
        // UpgradeManager will pick up the correct value on its next Awake().
        int currentBank = PlayerPrefs.GetInt("SpiceBank", 0);
        currentBank += amount;
        PlayerPrefs.SetInt("SpiceBank", currentBank);
        PlayerPrefs.Save();

        Debug.Log("Purchase: " + amount + " spice for " + price);
        ShowPurchaseToast();
        RefreshPackButton(id, amount, price, btn, label);
    }

    void RefreshSpicePackButtons()
    {
        RefreshPackButton("small", 500, "1.99$", smallPackButton, smallPackButtonText);
        RefreshPackButton("medium", 1500, "4.99$", mediumPackButton, mediumPackButtonText);
        RefreshPackButton("large", 4000, "9.99$", largePackButton, largePackButtonText);
    }

    void RefreshPackButton(string id, int amount, string price, Button btn, TextMeshProUGUI label)
    {
        bool purchased = PlayerPrefs.GetInt("SpicePackBought_" + id, 0) == 1;

        if (label != null)
            label.text = purchased ? "PURCHASED" : amount + " Spice - " + price;

        if (btn != null)
            btn.interactable = !purchased;
    }

    public void ShowHarvesters()
    {
        skinPanel.SetActive(true);
        spicePanel.SetActive(false);
    }

    public void ShowSpicePacks()
    {
        skinPanel.SetActive(false);
        spicePanel.SetActive(true);
    }
}