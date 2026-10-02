using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;

    [Header("Scene Background Settings")]
    [SerializeField] private Image backgroundImage; 
    [SerializeField] private bool isPortrait; // toggle in inspector

    private BackgroundItem currentBackground;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        backgroundImage = GetComponent<Image>();
        LoadSelectedBackground();
        ApplyBackground();
    }

    /// Sets a new background (called when buying/selecting in shop).
    public void SetBackground(BackgroundItem item)
    {
        currentBackground = item;
        SaveSelectedBackground(item);
        // Save the background to the active career registry
        string careerId = ActiveCareerController.ActiveCareer.name;  // Get the current career ID
        BackgroundRegistry.AddOrUpdateBackground(careerId, item);
        ApplyBackground();
    }

    /// Applies the current background to the scene image.
    private void ApplyBackground()
    {
        if (currentBackground == null || backgroundImage == null) return;

        backgroundImage.sprite = isPortrait ? currentBackground.Portrait : currentBackground.Landscape;
    }

    /// Loads the saved background for the current career, otherwise defaults to CareerSO's background.
    private void LoadSelectedBackground()
    {
        string careerId = ActiveCareerController.ActiveCareer.name;  // Get the current career ID
        string savedBackgroundId = PlayerPrefs.GetString($"SelectedBackground_{careerId}", string.Empty);

        // If a saved background exists for this career, use it
        if (!string.IsNullOrEmpty(savedBackgroundId))
        {
            BackgroundItem savedBackground = BackgroundRegistry.GetBackgroundById(careerId, savedBackgroundId);
            if (savedBackground != null)
            {
                currentBackground = savedBackground;
            }
            else
            {
                UseCareerDefault();
            }
        }
        else
        {
            // If no saved background exists for this career, use the default
            UseCareerDefault();
        }

        // Apply the background
        ApplyBackground();
    }

    private void UseCareerDefault()
    {
        // Fallback to the default background of the current career
        currentBackground = ScriptableObject.CreateInstance<BackgroundItem>();
        currentBackground.Portrait = ActiveCareerController.ActiveCareer.BackgroundPortrait;
        currentBackground.Landscape = ActiveCareerController.ActiveCareer.BackgroundLandscape;
    }

    /// Saves the currently selected background ID for the current career.
    private void SaveSelectedBackground(BackgroundItem item)
    {
        string careerId = ActiveCareerController.ActiveCareer.name;  // Get the current career ID
        PlayerPrefs.SetString($"SelectedBackground_{careerId}", item.Id);
        PlayerPrefs.Save();
        Debug.Log("Background saved for career: " + careerId);
    }
}
