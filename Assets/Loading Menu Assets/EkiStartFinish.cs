using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EkiStartFinish : MonoBehaviour
{
    [Header("Intro References")]
    public GameObject overlayRoot;
    public MainMenuIntroController introController;
    public MainMenuIntroFinish introFinish;

    [Header("Remember")]
    public bool showOnlyOncePerScene = true;
    public bool useActiveCareerInKey = false;
    public string playerPrefsKeyOverride = "";

    [Header("Disable until Eki is gone")]
    public Behaviour[] behavioursToDisableUntilIntroEnds;
    public Button[] buttonsToDisableUntilIntroEnds;

    [Header("Start only after Eki is gone")]
    public UIFadeInOut[] fadeScriptsToStartAfterIntro;

    private string PrefKey
    {
        get
        {
            string baseKey = !string.IsNullOrWhiteSpace(playerPrefsKeyOverride)
                ? playerPrefsKeyOverride
                : SceneManager.GetActiveScene().name;

            if (useActiveCareerInKey)
            {
                string careerKey = "NoCareer";

                if (ActiveCareerController.ActiveCareer != null)
                    careerKey = CleanKeyPart(ActiveCareerController.ActiveCareer.name);

                return baseKey + "_" + careerKey + "_EkiSeen";
            }

            return baseKey + "_EkiSeen";
        }
    }

    private string CleanKeyPart(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Unknown";

        return value.Replace(" ", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("/", "_")
                    .Replace("\\", "_");
    }

    private void Awake()
    {
        if (showOnlyOncePerScene && PlayerPrefs.GetInt(PrefKey, 0) == 1)
        {
            SkipIntroCompletely();
            return;
        }

        LockScene();
        PrepareFadeScripts();
    }

    private void LockScene()
    {
        foreach (Behaviour behaviour in behavioursToDisableUntilIntroEnds)
        {
            if (behaviour != null)
                behaviour.enabled = false;
        }

        foreach (Button button in buttonsToDisableUntilIntroEnds)
        {
            if (button != null)
            {
                // Prevent Unity's disabled tint from changing the button's appearance
                var colors = button.colors;
                colors.disabledColor = Color.white;
                button.colors = colors;

                button.interactable = false;
            }
        }
    }

    private void UnlockScene()
    {
        foreach (Behaviour behaviour in behavioursToDisableUntilIntroEnds)
        {
            if (behaviour != null)
                behaviour.enabled = true;
        }

        foreach (Button button in buttonsToDisableUntilIntroEnds)
        {
            if (button != null)
                button.interactable = true;
        }
    }

    private void PrepareFadeScripts()
    {
        foreach (UIFadeInOut fade in fadeScriptsToStartAfterIntro)
        {
            if (fade != null)
                fade.playOnStart = false;
        }
    }

    private void StartDelayedFades()
    {
        foreach (UIFadeInOut fade in fadeScriptsToStartAfterIntro)
        {
            if (fade != null)
                fade.StartFadeSequence();
        }
    }

    public void HandleEkiFinished()
    {
        if (showOnlyOncePerScene)
        {
            PlayerPrefs.SetInt(PrefKey, 1);
            PlayerPrefs.Save();
        }

        // ── AUDIO ──────────────────────────────────────────────────────────
        // Plays when Eki's dialogue ends and the scene unlocks — used in
        // Explorer scenes, JobMenu, Testing, and any other scene with Eki.
        AudioManager.Instance?.PlaySFX("CareerConfirm");
        // ───────────────────────────────────────────────────────────────────

        UnlockScene();
        StartDelayedFades();
    }

    private void SkipIntroCompletely()
    {
        if (introFinish != null)
            introFinish.FinishIntro();

        if (overlayRoot != null)
            overlayRoot.SetActive(false);

        if (introController != null)
            introController.enabled = false;

        UnlockScene();
        StartDelayedFades();
    }

    [ContextMenu("Reset Intro Seen Flag")]
    public void ResetIntroSeenFlag()
    {
        PlayerPrefs.DeleteKey(PrefKey);
        PlayerPrefs.Save();
    }
}