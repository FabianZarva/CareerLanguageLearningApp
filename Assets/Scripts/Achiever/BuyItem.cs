using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyItem : MonoBehaviour
{
    public TMP_Text Text;
    public Button button;  // Reference to the button component

    private string originalText;
    private BackgroundItem item;
    private bool backgroundPurchased = false; // Flag to check if background was purchased

    void Start()
    {
        Text = GetComponentInChildren<TMP_Text>();
        button = GetComponent<Button>();  // Get the button component attached to this GameObject

        if (Text != null)
        {
            originalText = Text.text;
        }

        item = ActiveCareerController.ActiveCareer.Shopitem1;

        // Check if the item is already purchased when the game starts
        if (IsItemPurchased())
        {
            // If already purchased, change button to "Apply"
            Text.text = "Apply";
            button.onClick.RemoveAllListeners();  // Remove previous listeners
            button.onClick.AddListener(ApplyBackground);  // Add ApplyBackground method
        }
        else
        {
            // If not purchased, show the price and enable the Buy functionality
            Text.text = originalText;
            button.onClick.RemoveAllListeners();  // Remove previous listeners
            button.onClick.AddListener(BuyBackground);  // Add BuyBackground method
        }
    }

    public void BuyBackground()
    {
        if (CurrencyManager.Instance.CurrencyCount >= CurrencyManager.Instance.CurrencyPrice)
        {
            // Disable the button after successful purchase
            button.interactable = false;

            // Check if player has enough currency
            CurrencyManager.Instance.PayCurrency();
            CurrencyManager.Instance.UpdateCurrencyText();

            // Change Text to show purchase confirmation
            Text.text = "Purchased";

            // Get the career ID from the active career
            string careerId = ActiveCareerController.ActiveCareer.name;

            // Save the background ID for the current career
            PlayerPrefs.SetString($"SelectedBackground_{careerId}", item.Id);
            PlayerPrefs.Save();

            // Add background to the registry for the current career
            BackgroundRegistry.AddOrUpdateBackground(careerId, item);

            // Update the live background (e.g., update the current scene)
            BackgroundManager.Instance.SetBackground(item);

            // Mark the item as purchased (store it in PlayerPrefs)
            PlayerPrefs.SetInt($"Purchased_{item.Id}", 1);
            PlayerPrefs.Save();

            // After purchase, change button to "Apply"
            Text.text = "Apply";
            button.onClick.RemoveAllListeners();  // Remove previous listeners
            button.onClick.AddListener(ApplyBackground);  // Add ApplyBackground method

            // Set the flag to indicate background is purchased
            backgroundPurchased = true;
        }
        else
        {
            StartCoroutine(NoMoney());
        }
    }

    IEnumerator NoMoney()
    {
        // Don't show "Not Enough Opportunities" if background is already purchased
        if (!backgroundPurchased)
        {
            if (Text != null)
            {
                Text.text = "Not Enough Opportunities";
            }

            yield return new WaitForSeconds(2);

            if (Text != null)
            {
                Text.text = originalText;
            }
        }
    }

    // Check if the item has already been purchased (via PlayerPrefs)
    private bool IsItemPurchased()
    {
        string careerId = ActiveCareerController.ActiveCareer.name;
        return PlayerPrefs.GetInt($"Purchased_{item.Id}", 0) == 1;
    }

    // Apply the background after purchase
    private void ApplyBackground()
    {
        // Set the background as the current one
        BackgroundManager.Instance.SetBackground(item);

        // Optionally, disable button after applying if needed
        button.interactable = false;
        Text.text = "Applied";  // You can update text to indicate it’s applied
    }
}





