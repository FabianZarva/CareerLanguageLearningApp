using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour
{
    [Header("Stage Buttons")]
    [SerializeField] private Button stage1Button; // Find the Object
    [SerializeField] private Button stage2Button; // Follow the Instructions
    [SerializeField] private Button stage3Button; // Find the -Nym

    [Header("Labels (optional)")]
    [SerializeField] private TMP_Text stage1Label;
    [SerializeField] private TMP_Text stage2Label;
    [SerializeField] private TMP_Text stage3Label;

    [Header("Lock Objects (optional)")]
    [SerializeField] private GameObject stage2Lock;
    [SerializeField] private GameObject stage3Lock;

    [Header("Visuals")]
    [SerializeField] private float lockedAlpha = 0.45f;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        CareerSO career = ActiveCareerController.ActiveCareer;
        if (career == null)
        {
            Debug.LogError("ProgressUI: No active career.");
            return;
        }

        GamerTypeResult profile = ActiveCareerController.LastQuestionnaireResult;
        bool isAchiever = (profile != null && profile.FinalGamerTypes.Contains(GamerType.Achiever))
               || (profile == null && GameManager.Instance != null && GameManager.Instance.CurrencyEnabled);

        if (!isAchiever)
        {
            ApplyButtonState(stage1Button, true, null, stage1Label, "Find the Object");
            ApplyButtonState(stage2Button, true, stage2Lock, stage2Label, "Follow the Instructions");
            ApplyButtonState(stage3Button, true, stage3Lock, stage3Label, "Find the -Nym");
            return;
        }

        int unlockedStage = CareerProgress.GetUnlockedStage(career);

        ApplyButtonState(stage1Button, unlockedStage >= 1, null, stage1Label, "Find the Object");
        ApplyButtonState(stage2Button, unlockedStage >= 2, stage2Lock, stage2Label, "Follow the Instructions");
        ApplyButtonState(stage3Button, unlockedStage >= 3, stage3Lock, stage3Label, "Find the -Nym");
    }

    private void ApplyButtonState(Button button, bool unlocked, GameObject lockOverlay, TMP_Text label, string text)
    {
        if (button != null)
        {
            button.interactable = unlocked;

            CanvasGroup cg = button.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = button.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = unlocked ? 1f : lockedAlpha;
        }

        if (lockOverlay != null)
            lockOverlay.SetActive(!unlocked);

        if (label != null)
            label.text = text;
    }
}