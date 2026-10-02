using System.Collections.Generic;
using UnityEngine;

public class QuestionnaireResultRouter : MonoBehaviour
{
    [Header("Career Mapping")]
    [SerializeField] private CareerSO lawyerCareer;
    [SerializeField] private CareerSO surgeonCareer;
    [SerializeField] private CareerSO cookCareer;

    [Header("References")]
    [SerializeField] private PickCareer pickCareer;

    public void ContinueFromQuestionnaire()
    {
        GamerTypeResult result = ActiveCareerController.LastQuestionnaireResult;

        if (result == null)
        {
            Debug.LogError("No questionnaire result found.");
            return;
        }

        ApplyGamerTypeSettings(result);

        CareerSO mappedCareer = GetCareerFromJob(result.FinalJobType);

        if (mappedCareer == null)
        {
            Debug.LogError("No CareerSO mapped for questionnaire result.");
            return;
        }

        pickCareer.LoadGame(mappedCareer);
    }

    private CareerSO GetCareerFromJob(JobType jobType)
    {
        switch (jobType)
        {
            case JobType.Lawyer:
                return lawyerCareer;
            case JobType.Surgeon:
                return surgeonCareer;
            case JobType.Cook:
                return cookCareer;
            default:
                return null;
        }
    }

    private void ApplyGamerTypeSettings(GamerTypeResult result)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Main project GameManager.Instance is missing.");
            return;
        }

        // Reset all feature toggles first
        GameManager.Instance.ScoreEnabled = false;
        GameManager.Instance.CurrencyEnabled = false;
        GameManager.Instance.DialogueEnabled = false;
        GameManager.Instance.WorldEnabled = false;

        List<GamerType> activeTypes = new List<GamerType>();

        if (result.FinalGamerTypes != null && result.FinalGamerTypes.Count > 0)
        {
            activeTypes.AddRange(result.FinalGamerTypes);
        }
        else
        {
            // fallback for old data
            activeTypes.Add(result.FinalGamerType);
        }

        foreach (GamerType gamerType in activeTypes)
        {
            switch (gamerType)
            {
                case GamerType.Achiever:
                    GameManager.Instance.CurrencyEnabled = true;
                    break;

                case GamerType.Socializer:
                    GameManager.Instance.DialogueEnabled = true;
                    break;

                case GamerType.Explorer:
                    GameManager.Instance.WorldEnabled = true;
                    break;

                case GamerType.Killer:
                    GameManager.Instance.ScoreEnabled = true;
                    break;
            }
        }
    }
}