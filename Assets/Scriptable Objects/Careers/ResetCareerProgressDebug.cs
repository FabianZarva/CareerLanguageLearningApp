using UnityEngine;

public class ResetCareerProgressDebug : MonoBehaviour
{
    public void ResetCurrentCareerProgress()
    {
        CareerSO career = ActiveCareerController.ActiveCareer;
        CareerProgress.ResetCareer(career);
        Debug.Log("Reset progress for current career.");
    }
}