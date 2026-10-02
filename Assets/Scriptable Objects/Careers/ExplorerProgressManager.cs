using UnityEngine;

[System.Serializable]
public class ExplorerTrialSlot
{
    public string debugName;
    [Range(1, 3)] public int requiredStage = 1;

    [Header("Trigger Side")]
    public Collider2D triggerCollider;      // the trigger area in the world
    public Behaviour showPopupScript;       // ShowMinigamePopup on that trigger

    [Header("Popup Side")]
    public GameObject popupObject;          // the popup object that appears
    public Behaviour popupClickScript;      // MinigamePopup on the popup object

    [Header("Optional")]
    public GameObject lockedVisual;         // optional "Locked" sign/sprite/text
}

public class ExplorerProgressManager : MonoBehaviour
{
    [Header("Trial Order")]
    [SerializeField] private ExplorerTrialSlot stage1_FindObject;
    [SerializeField] private ExplorerTrialSlot stage2_FollowInstructions;
    [SerializeField] private ExplorerTrialSlot stage3_FindNym;

    private void Start()
    {
        ApplyProgress();
    }

    public void ApplyProgress()
    {
        CareerSO career = ActiveCareerController.ActiveCareer;

        if (career == null)
        {
            Debug.LogError("ExplorerProgressManager: Active career is null.");
            return;
        }

        GamerTypeResult profile = ActiveCareerController.LastQuestionnaireResult;
       bool isAchiever = (profile != null && profile.FinalGamerTypes.Contains(GamerType.Achiever))
               || (profile == null && GameManager.Instance != null && GameManager.Instance.CurrencyEnabled);

        if (!isAchiever)
        {
            ApplySlot(stage1_FindObject, true);
            ApplySlot(stage2_FollowInstructions, true);
            ApplySlot(stage3_FindNym, true);
            return;
        }

        ApplySlot(stage1_FindObject, CareerProgress.IsUnlocked(career, 1));
        ApplySlot(stage2_FollowInstructions, CareerProgress.IsUnlocked(career, 2));
        ApplySlot(stage3_FindNym, CareerProgress.IsUnlocked(career, 3));
    }

    private void ApplySlot(ExplorerTrialSlot slot, bool unlocked)
    {
        if (slot.triggerCollider != null)
            slot.triggerCollider.enabled = unlocked;

        if (slot.showPopupScript != null)
            slot.showPopupScript.enabled = unlocked;

        if (slot.popupClickScript != null)
            slot.popupClickScript.enabled = unlocked;

        // Always start popup hidden. It should appear only when the trigger says so.
        if (slot.popupObject != null)
            slot.popupObject.SetActive(false);

        if (slot.lockedVisual != null)
            slot.lockedVisual.SetActive(!unlocked);
    }
}