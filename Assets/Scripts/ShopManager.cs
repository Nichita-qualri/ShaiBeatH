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

    [Header("Спрайты скинов")]
    public Sprite[] skinSprites;

    [Header("Цвета скинов (если нет спрайта)")]
    public Color[] skinColors;

    [Header("Панели")]
    public GameObject skinPanel;
    public GameObject spicePanel;

    private int _currentSkin = 0;
    private int _selectedSkin = 0;

    private string[] _names = {
        "Standard",
        "Atreides",
        "Harkonnen",
        "Fremen"
    };

    private string[] _descriptions = {
        "Standard spice harvesting machine",
        "House Atreides harvester. 10% faster",
        "Harkonnen harvester. 10% more durable",
        "Fremen harvester. Silent and stealthy"
    };

    private string[] _prices = {
        "FREE",
        "0.99$",
        "0.99$",
        "1.99$"
    };

    private bool[] _unlocked = { true, false, false, false };

    void Start()
    {
        ShowHarvesters();
        _selectedSkin = PlayerPrefs.GetInt("SelectedSkin", 0);
        _currentSkin = _selectedSkin;
        UpdateUI();
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
        // Спрайт или цвет
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
            _selectedSkin = _currentSkin;
            PlayerPrefs.SetInt("SelectedSkin", _selectedSkin);
            PlayerPrefs.Save();
            UpdateUI();
        }
        else
        {
            ShowPurchasePopup();
        }
    }

    void ShowPurchasePopup()
    {
        buyButtonText.text = "Coming Soon!";
        Invoke(nameof(ResetButton), 2f);
    }

    void ResetButton()
    {
        UpdateUI();
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BuySmallPack()
    {
        ShowSpicePurchasePopup(500, "1.99$");
    }

    public void BuyMediumPack()
    {
        ShowSpicePurchasePopup(1500, "4.99$");
    }

    public void BuyLargePack()
    {
        ShowSpicePurchasePopup(4000, "9.99$");
    }

    void ShowSpicePurchasePopup(int amount, string price)
    {
        Debug.Log("Purchase: " + amount + " spice for " + price);
        skinName.text = "Coming Soon!";
        skinDescription.text = "In-app purchases will be\navailable in full release";
        Invoke(nameof(ResetAfterPopup), 2f);
    }

    void ResetAfterPopup()
    {
        UpdateUI();
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