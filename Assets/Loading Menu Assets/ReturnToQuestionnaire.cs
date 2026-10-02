using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToQuestionnaire : MonoBehaviour
{
    [Header("All careers in the game - drag all CareerSO assets here")]
    [SerializeField] private CareerSO[] allCareers;

    [Header("Scene to load (your questionnaire scene name)")]
    [SerializeField] private string questionnaireSceneName = "Test Portrait";

    public void ResetAndReturnToQuestionnaire()
    {
        // 1 - Reset progress for every career
        foreach (CareerSO career in allCareers)
        {
            if (career != null)
                CareerProgress.ResetCareer(career);
        }

        // 2 - Reset minigame question state
        Minigame1Manager.UnansweredQuestions?.Clear();
        Minigame2Manager.UnansweredQuestions?.Clear();
        Minigame3Manager.UnansweredQuestions?.Clear();

        // 3 - Reset all Eki "seen" flags so she reappears on next visit
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 4 - Reset screen orientation in case we're returning from landscape
        Screen.orientation = ScreenOrientation.Portrait;

        // 5 - Load questionnaire
        SceneManager.LoadScene(questionnaireSceneName);
    }
}