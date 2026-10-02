using System.Collections.Generic;
using UnityEngine;

public class EkiLines : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MainMenuIntroController introController;

    [Header("Career Lines")]
    [TextArea(2, 5)] public string[] lawyerLines;
    [TextArea(2, 5)] public string[] chefLines;
    [TextArea(2, 5)] public string[] surgeonLines;

    [Header("Gamer Type Lines")]
    [TextArea(2, 5)] public string[] achieverLines;
    [TextArea(2, 5)] public string[] explorerLines;
    [TextArea(2, 5)] public string[] socializerLines;
    [TextArea(2, 5)] public string[] killerLines;

    [Header("Fallback")]
    [TextArea(2, 5)] public string[] fallbackLines;

    private void Awake()
    {
        if (introController == null)
        {
            Debug.LogError("TestingEkiLines: introController is missing.");
            return;
        }

        List<string> finalLines = new List<string>();

        // 1) Career-specific block
        finalLines.AddRange(GetCareerLines());

        // 2) Gamer-type-specific block
        finalLines.AddRange(GetGamerTypeLines());

        // 3) Fallback if somehow nothing exists
        if (finalLines.Count == 0 && fallbackLines != null && fallbackLines.Length > 0)
        {
            finalLines.AddRange(fallbackLines);
        }

        introController.introLines = finalLines.ToArray();
    }

    private string[] GetCareerLines()
    {
        if (ActiveCareerController.ActiveCareer == null)
        {
            Debug.LogWarning("TestingEkiLines: No active career found.");
            return fallbackLines;
        }

        string careerName = ActiveCareerController.ActiveCareer.name.ToLower();

        if (careerName.Contains("lawyer"))
            return lawyerLines;

        if (careerName.Contains("chef") || careerName.Contains("cook"))
            return chefLines;

        if (careerName.Contains("doctor") || careerName.Contains("surgeon"))
            return surgeonLines;

        return fallbackLines;
    }

    private List<string> GetGamerTypeLines()
    {
        List<string> lines = new List<string>();

        GamerTypeResult result = ActiveCareerController.LastQuestionnaireResult;

        if (result == null)
        {
            lines.Add("I cannot see your questionnaire result right now, so I will give you the general version.");
            return lines;
        }

        List<GamerType> activeTypes = new List<GamerType>();

        if (result.FinalGamerTypes != null && result.FinalGamerTypes.Count > 0)
        {
            activeTypes.AddRange(result.FinalGamerTypes);
        }
        else
        {
            activeTypes.Add(result.FinalGamerType);
        }

        if (activeTypes.Count > 1)
        {
            lines.Add("Your questionnaire suggests a mixed player profile, so your path may combine different strengths.");
        }

        foreach (GamerType type in activeTypes)
        {
            switch (type)
            {
                case GamerType.Achiever:
                    if (achieverLines != null) lines.AddRange(achieverLines);
                    break;

                case GamerType.Explorer:
                    if (explorerLines != null) lines.AddRange(explorerLines);
                    break;

                case GamerType.Socializer:
                    if (socializerLines != null) lines.AddRange(socializerLines);
                    break;

                case GamerType.Killer:
                    if (killerLines != null) lines.AddRange(killerLines);
                    break;
            }
        }

        return lines;
    }
}