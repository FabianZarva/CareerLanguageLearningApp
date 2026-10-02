using UnityEngine;

public class MinigameCompletion : MonoBehaviour
{
    [SerializeField] private int completedStage; // 1 = Object, 2 = Instructions, 3 = Nym
    [SerializeField] private GoBack goBack;

    public void CompleteAndReturnToMenu()
    {
        CareerSO career = ActiveCareerController.ActiveCareer;

        if (career == null)
        {
            Debug.LogError("MinigameCompletion: No active career.");
            return;
        }

        CareerProgress.CompleteStage(career, completedStage);

        if (goBack != null)
        {
            goBack.LoadMenu();
        }
        else
        {
            Debug.LogError("MinigameCompletion: GoBack reference missing.");
        }
    }
}