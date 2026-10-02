using UnityEngine;
using UnityEngine.UI;

public class RestartConfirmPopup : MonoBehaviour
{
    [Header("Popup Root - the whole panel to show/hide")]
    [SerializeField] private GameObject popupPanel;

    [Header("Buttons inside the popup")]
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button cancelNoButton;

    [Header("The restart logic script")]
    [SerializeField] private ReturnToQuestionnaire returnScript;

    private void Awake()
    {
        popupPanel.SetActive(false);

        confirmYesButton.onClick.AddListener(OnConfirmYes);
        cancelNoButton.onClick.AddListener(OnCancelNo);
    }

    // Called by your existing back/restart button in the Quiz scene
    public void ShowPopup()
    {
        // ── AUDIO ──────────────────────────────────────────────────────────
        AudioManager.Instance?.PlaySFX("UIClick");
        // ───────────────────────────────────────────────────────────────────

        popupPanel.SetActive(true);
    }

    private void OnConfirmYes()
    {
        // ── AUDIO ──────────────────────────────────────────────────────────
        // CareerConfirm feels appropriate here — the player is confirming
        // a major decision (restarting the whole game).
        AudioManager.Instance?.PlaySFX("CareerConfirm");
        // ───────────────────────────────────────────────────────────────────

        popupPanel.SetActive(false);
        returnScript.ResetAndReturnToQuestionnaire();
    }

    private void OnCancelNo()
    {
        // ── AUDIO ──────────────────────────────────────────────────────────
        AudioManager.Instance?.PlaySFX("UIClick");
        // ───────────────────────────────────────────────────────────────────

        popupPanel.SetActive(false);
    }
}