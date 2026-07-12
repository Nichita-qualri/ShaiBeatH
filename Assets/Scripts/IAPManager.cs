using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class SpicePack
{
    public string id;              // unique key, e.g. "pack_small"
    public string priceLabel;      // fake price shown to player, e.g. "$0.99"
    public int spiceAmount;        // how much spice this pack grants
    public Button buyButton;
    public TextMeshProUGUI buttonLabel;
}

public class IAPManager : MonoBehaviour
{
    [Header("Spice Packs")]
    public SpicePack[] packs;

    [Header("Purchase Feedback")]
    public TextMeshProUGUI purchaseToastText; // e.g. "PURCHASE SUCCESSFUL!"
    public float toastDuration = 1.5f;

    void Start()
    {
        RefreshAllButtons();
        if (purchaseToastText != null)
            purchaseToastText.alpha = 0f;
    }

    void RefreshAllButtons()
    {
        foreach (var pack in packs)
            RefreshButton(pack);
    }

    void RefreshButton(SpicePack pack)
    {
        bool purchased = PlayerPrefs.GetInt("IAP_" + pack.id, 0) == 1;

        if (pack.buttonLabel != null)
            pack.buttonLabel.text = purchased
                ? "PURCHASED"
                : pack.spiceAmount + " SPICE  " + pack.priceLabel;

        if (pack.buyButton != null)
            pack.buyButton.interactable = !purchased;
    }

    // Hook this up to each pack's Buy button, passing the pack id
    public void BuyPack(string packId)
    {
        SpicePack pack = System.Array.Find(packs, p => p.id == packId);
        if (pack == null) return;

        bool alreadyPurchased = PlayerPrefs.GetInt("IAP_" + pack.id, 0) == 1;
        if (alreadyPurchased) return;

        // Fake payment confirmed instantly — no real transaction
        PlayerPrefs.SetInt("IAP_" + pack.id, 1);
        PlayerPrefs.Save();

        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.AddSpice(pack.spiceAmount);

        RefreshButton(pack);
        ShowPurchaseToast();
    }

    void ShowPurchaseToast()
    {
        if (purchaseToastText == null) return;
        StopAllCoroutines();
        StartCoroutine(ToastRoutine());
    }

    IEnumerator ToastRoutine()
    {
        purchaseToastText.text = "PURCHASE SUCCESSFUL!";
        purchaseToastText.alpha = 1f;
        yield return new WaitForSeconds(toastDuration);
        while (purchaseToastText.alpha > 0f)
        {
            purchaseToastText.alpha -= Time.deltaTime * 2f;
            yield return null;
        }
        purchaseToastText.alpha = 0f;
    }
}