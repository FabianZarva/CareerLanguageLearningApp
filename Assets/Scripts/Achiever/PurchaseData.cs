using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseData : MonoBehaviour
{
    /*public static PurchaseData Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // Ensure this persists between scenes
    }

    // Store purchased backgrounds (could be a list of indices or references to bought backgrounds)
    public bool HasPurchasedBackgroundPortrait(Sprite background)
    {
        // Example logic: You can store background names or indices
        return PlayerPrefs.GetInt("PurchasedBackground_" + background.name, 0) == 1;
    }

    public void PurchaseBackground(Sprite portrait, Sprite landscape)
    {
        // Update PlayerPrefs to store the purchased backgrounds
        PlayerPrefs.SetInt("PurchasedBackground_" + portrait.name, 1);
        PlayerPrefs.SetInt("PurchasedBackground_" + landscape.name, 1);
        
        // Optionally save the selected background for active session
        PlayerPrefs.SetString("ActivePortraitBackground", portrait.name);
        PlayerPrefs.SetString("ActiveLandscapeBackground", landscape.name);
    }

    public void LoadPurchaseData()
    {
        // Load active backgrounds if available
        string portraitName = PlayerPrefs.GetString("ActivePortraitBackground", "DefaultPortraitName");
        string landscapeName = PlayerPrefs.GetString("ActiveLandscapeBackground", "DefaultLandscapeName");

        // Apply logic to find and set the loaded backgrounds (based on names)
        // This could involve finding the actual Sprite assets or referencing your CareerSO objects
    }

    public void SavePurchaseData()
    {
        // Handle any manual saving logic here if needed (e.g., PlayerPrefs).
        PlayerPrefs.Save();
    }*/
}
