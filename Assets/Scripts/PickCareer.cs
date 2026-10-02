using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickCareer : MonoBehaviour
{
    [SerializeField]
    private CareerSO[] _Careers;

    private string SceneName;

    public void LoadGame(CareerSO input)
    {
        ActiveCareerController.ActiveCareer = input;
               
        // Load the saved background for this career
        string careerId = input.name;
        string savedBackgroundId = PlayerPrefs.GetString($"SelectedBackground_{careerId}", string.Empty);

        if (!string.IsNullOrEmpty(savedBackgroundId))
        {
            BackgroundItem background = BackgroundRegistry.GetBackgroundById(careerId, savedBackgroundId);
            if (background != null)
            {
                BackgroundManager.Instance.SetBackground(background);
            }
        }

        // ── AUDIO: switch to this career's music track ────────────────────
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(input.MusicTrackName))
        {
            AudioManager.Instance.PlayMusic(input.MusicTrackName);
        }
        // ─────────────────────────────────────────────────────────────────

        if (GameManager.Instance.WorldEnabled)
        {
            SceneName = ActiveCareerController.ActiveCareer.ExplorerWorld;
        }
        else
        {
            SceneName = "Testing";
        }

        SceneManager.LoadScene(SceneName);
        Debug.Log("Career is now: " + ActiveCareerController.ActiveCareer);
    }
}