using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UIManagerParameters
{
    [Header("Answers Options")]
    [SerializeField] private float margins;
    public float Margins => margins;

    [Header("Result Screen")]
    [SerializeField] private Color finalBGColor;
    public Color FinalBGColor => finalBGColor;
}

[System.Serializable]
public struct UIElements
{
    [SerializeField] private RectTransform answersContentArea;
    public RectTransform AnswersContentArea => answersContentArea;

    [SerializeField] private TextMeshProUGUI questionInfoTextObject;
    public TextMeshProUGUI QuestionInfoTextObject => questionInfoTextObject;

    [SerializeField] private TextMeshProUGUI progressText;
    public TextMeshProUGUI ProgressText => progressText;

    [Space]
    [SerializeField] private Animator resolutionScreenAnimator;
    public Animator ResolutionScreenAnimator => resolutionScreenAnimator;

    [SerializeField] private Image resolutionBG;
    public Image ResolutionBG => resolutionBG;

    [SerializeField] private TextMeshProUGUI resolutionStateInfoText;
    public TextMeshProUGUI ResolutionStateInfoText => resolutionStateInfoText;

    [SerializeField] private TextMeshProUGUI resolutionScoreText;
    public TextMeshProUGUI ResolutionScoreText => resolutionScoreText;

    [SerializeField] private Image resultBackgroundImage;
    public Image ResultBackgroundImage => resultBackgroundImage;

    [Space]
    [SerializeField] private CanvasGroup mainCanvasGroup;
    public CanvasGroup MainCanvasGroup => mainCanvasGroup;

    [SerializeField] private Button backButton;
    public Button BackButton => backButton;

    [SerializeField] private RectTransform finishUIElements;
    public RectTransform FinishUIElements => finishUIElements;
}

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameEvents events = null;

    [Header("UI Elements (Prefabs)")]
    [SerializeField] private AnswerData answerPrefab = null;
    [SerializeField] private UIElements uIElements = new UIElements();
    [SerializeField] private UIManagerParameters parameters = new UIManagerParameters();

    [Header("Result Backgrounds By Job")]
    [SerializeField] private Sprite lawyerBackground = null;
    [SerializeField] private Sprite surgeonBackground = null;
    [SerializeField] private Sprite cookBackground = null;

    private readonly List<AnswerData> currentAnswers = new List<AnswerData>();
    private int resStateParaHash = 0;

    private void OnEnable()
    {
        if (events != null)
        {
            events.UpdateQuestionUI += UpdateQuestionUI;
            events.QuestionnaireCompleted += ShowQuestionnaireResult;

            // ── AUDIO: hook into answer selection event ──
            events.UpdateQuestionAnswer += OnAnswerSelected;
        }
    }

    private void OnDisable()
    {
        if (events != null)
        {
            events.UpdateQuestionUI -= UpdateQuestionUI;
            events.QuestionnaireCompleted -= ShowQuestionnaireResult;

            // ── AUDIO: unhook ──
            events.UpdateQuestionAnswer -= OnAnswerSelected;
        }
    }

    private void Start()
    {
        resStateParaHash = Animator.StringToHash("ScreenState");

        if (uIElements.FinishUIElements != null)
        {
            uIElements.FinishUIElements.gameObject.SetActive(false);
        }

        if (uIElements.BackButton != null)
            uIElements.BackButton.onClick.AddListener(() => events.RequestGoBack?.Invoke());
    }

    // ── AUDIO: called whenever the player taps an answer ─────────────────────
    private void OnAnswerSelected(AnswerData answer)
    {
        AudioManager.Instance?.PlaySFX("UIClick");
    }

    private void UpdateQuestionUI(QuestionData question, int currentIndex, int totalQuestions)
    {
        if (uIElements.QuestionInfoTextObject != null)
        {
            uIElements.QuestionInfoTextObject.text = question.QuestionText;
        }

        if (uIElements.ProgressText != null)
        {
            uIElements.ProgressText.text = $"Question {currentIndex + 1}/{totalQuestions}";
        }

        CreateAnswers(question);
        if (uIElements.BackButton != null)
            uIElements.BackButton.transform.parent.gameObject.SetActive(currentIndex > 0);
    }

    private void CreateAnswers(QuestionData question)
    {
        EraseAnswers();

        float startOffset = GetStartOffset(question);
        float offset = -startOffset;

        for (int i = 0; i < question.Answers.Count; i++)
        {
            AnswerData newAnswer = Instantiate(answerPrefab, uIElements.AnswersContentArea);
            newAnswer.UpdateData(question.Answers[i].AnswerText, i);

            bool isRankedJobAnswer =
                question.Category == QuestionCategory.JobPreference &&
                IsRankedJobAnswer(question.Answers[i].AnswerText);

            newAnswer.SetSelectedVisual(isRankedJobAnswer);

            newAnswer.Rect.anchoredPosition = new Vector2(0, offset);
            offset -= (newAnswer.Rect.sizeDelta.y + parameters.Margins);

            uIElements.AnswersContentArea.sizeDelta = new Vector2(
                uIElements.AnswersContentArea.sizeDelta.x,
                offset * -1
            );

            currentAnswers.Add(newAnswer);
        }
    }

    private float GetStartOffset(QuestionData question)
    {
        if (question.Category == QuestionCategory.JobPreference)
            return 500f;
        return 480f;
    }

    private bool IsRankedJobAnswer(string answerText)
    {
        return answerText.StartsWith("1.") ||
               answerText.StartsWith("2.") ||
               answerText.StartsWith("3.");
    }

    private void EraseAnswers()
    {
        foreach (var answer in currentAnswers)
        {
            if (answer != null)
            {
                Destroy(answer.gameObject);
            }
        }

        currentAnswers.Clear();
    }

    private void ShowQuestionnaireResult(GamerTypeResult result)
    {
        if (uIElements.ResolutionBG != null)
        {
            uIElements.ResolutionBG.color = parameters.FinalBGColor;
        }

        if (uIElements.ResolutionStateInfoText != null)
        {
            uIElements.ResolutionStateInfoText.text = "YOUR RESULT";
        }

        if (uIElements.ResultBackgroundImage != null)
        {
            uIElements.ResultBackgroundImage.sprite = GetJobBackground(result.FinalJobType);
            uIElements.ResultBackgroundImage.enabled = uIElements.ResultBackgroundImage.sprite != null;
        }

        if (uIElements.ResolutionScoreText != null)
        {
            string typeNames = GetGamerTypeNames(result);
            string typeTitles = GetGamerTypeTitles(result);
            string typeDescription = GetCombinedGamerTypeDescription(result);
            string jobDescription = GetJobDescription(result.FinalJobType);

            uIElements.ResolutionScoreText.text =
                $"Your Gamer Type: {typeNames} ({typeTitles})\n\n" +
                $"{typeDescription}\n\n" +
                $"{jobDescription}";
        }

        if (uIElements.FinishUIElements != null)
        {
            uIElements.FinishUIElements.gameObject.SetActive(true);
        }

        if (uIElements.ResolutionScreenAnimator != null)
        {
            uIElements.ResolutionScreenAnimator.SetInteger(resStateParaHash, 2);
        }

        if (uIElements.MainCanvasGroup != null)
        {
            uIElements.MainCanvasGroup.blocksRaycasts = false;
        }

        // ── AUDIO: play result fanfare ──
        AudioManager.Instance?.PlaySFX("QuestionnaireResult");
    }


    private string GetGamerTypeTitle(GamerType gamerType)
    {
        switch (gamerType)
        {
            case GamerType.Killer:      return "Result-Oriented";
            case GamerType.Socializer:  return "People-Oriented";
            case GamerType.Achiever:    return "Goal-Oriented";
            case GamerType.Explorer:    return "Discovery-Oriented";
            default:                    return "Player";
        }
    }

    private string GetGamerTypeDescription(GamerType gamerType)
    {
        switch (gamerType)
        {
            case GamerType.Killer:
                return "Your answers suggest that you prefer quick decisions, and learning through immediate situations and challenges.";
            case GamerType.Socializer:
                return "Your answers suggest that you prefer emotion, connection, and learning through situations that involve people and shared experience.";
            case GamerType.Achiever:
                return "Your answers suggest that you prefer structure, progress, and improving step by step through clear goals and tasks.";
            case GamerType.Explorer:
                return "Your answers suggest that you prefer curiosity, discovery, and learning by exploring how things work.";
            default:
                return "Your answers show a balanced Gamer Type.";
        }
    }

    private string GetJobDescription(JobType jobType)
    {
        switch (jobType)
        {
            case JobType.Lawyer:
                return "Based on your career choice, the game will begin with the Lawyer path. In this role, you will work with legal vocabulary, documents, rules, and courtroom-based situations.";
            case JobType.Surgeon:
                return "Based on your career choice, the game will begin with the Doctor path. In this role, you will work with medical vocabulary, tools, procedures, and hospital-based situations.";
            case JobType.Cook:
                return "Based on your career choice, the game will begin with the Cook path. In this role, you will work with kitchen vocabulary, ingredients, tools, and step-by-step preparation tasks.";
            default:
                return "The game will now begin with a recommended career path.";
        }
    }

    private Sprite GetJobBackground(JobType jobType)
    {
        switch (jobType)
        {
            case JobType.Lawyer:    return lawyerBackground;
            case JobType.Surgeon:   return surgeonBackground;
            case JobType.Cook:      return cookBackground;
            default:                return null;
        }
    }

    private List<GamerType> GetActiveGamerTypes(GamerTypeResult result)
    {
        if (result.FinalGamerTypes != null && result.FinalGamerTypes.Count > 0)
            return result.FinalGamerTypes;
        return new List<GamerType> { result.FinalGamerType };
    }

    private string GetGamerTypeNames(GamerTypeResult result)
    {
        List<GamerType> activeTypes = GetActiveGamerTypes(result);
        List<string> names = new List<string>();
        foreach (GamerType type in activeTypes)
            names.Add(type.ToString());
        return string.Join(" / ", names);
    }

    private string GetGamerTypeTitles(GamerTypeResult result)
    {
        List<GamerType> activeTypes = GetActiveGamerTypes(result);
        List<string> titles = new List<string>();
        foreach (GamerType type in activeTypes)
            titles.Add(GetGamerTypeTitle(type));
        return string.Join(" / ", titles);
    }

    private string GetCombinedGamerTypeDescription(GamerTypeResult result)
    {
        List<GamerType> activeTypes = GetActiveGamerTypes(result);

        if (activeTypes.Count == 1)
            return GetGamerTypeDescription(activeTypes[0]);

        List<string> parts = new List<string>();
        foreach (GamerType type in activeTypes)
            parts.Add(GetShortGamerTypeDescription(type));

        return "Your answers suggest multiple Gamer Types. " + string.Join(" ", parts);
    }

    private string GetShortGamerTypeDescription(GamerType gamerType)
    {
        switch (gamerType)
        {
            case GamerType.Killer:      return "You value direct action and fast decisions.";
            case GamerType.Socializer:  return "You enjoy people, emotion, and shared experiences.";
            case GamerType.Achiever:    return "You like progress, goals, and clear improvement.";
            case GamerType.Explorer:    return "You enjoy discovery, curiosity, and figuring things out.";
            default:                    return string.Empty;
        }
    }
}