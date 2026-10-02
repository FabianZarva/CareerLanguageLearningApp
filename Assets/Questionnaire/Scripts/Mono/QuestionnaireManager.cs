using System.Collections.Generic;
using UnityEngine;

public class QuestionnaireManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameEvents events = null;

    private Data data = new Data();
    private readonly List<AnswerData> pickedAnswers = new List<AnswerData>();
    // History: stores (questionIndex, selectedAnswerIndex, weightsSnapshot) per question
    private readonly Stack<(int questionIndex, int answerIndex, GamerTypeResult weightsBefore)> answerHistory = new Stack<(int, int, GamerTypeResult)>();
    private int currentQuestionIndex = 0;

    private GamerTypeResult currentProfile = new GamerTypeResult();

    // NEW: ranking state for final job preference question
    private readonly List<JobType> rankedJobs = new List<JobType>();

    private void OnEnable()
    {
        if (events != null)
        {
            events.UpdateQuestionAnswer += UpdateAnswers;
            events.RequestGoBack += GoBack;
        }
    }

    private void OnDisable()
    {
        if (events != null)
        {
            events.UpdateQuestionAnswer -= UpdateAnswers;
            events.RequestGoBack -= GoBack;
        }
    }

    private void Start()
    {
        LoadData();

        if (data.Questions == null || data.Questions.Length == 0)
        {
            Debug.LogError("No questionnaire questions were loaded.");
            return;
        }

        DisplayCurrentQuestion();
    }

     private void LoadData()
    {
        TextAsset questionnaireAsset = Resources.Load<TextAsset>(GameUtility.QuestionnaireResourcePath);

        if (questionnaireAsset == null)
        {
            Debug.LogError("Questionnaire XML not found in Resources/Questionnaire/IntroQuestionnaire.xml");
            data = new Data();
            return;
        }

        data = Data.FetchFromXmlString(questionnaireAsset.text);
    }

    private void DisplayCurrentQuestion()
    {
        if (events == null) return;

        QuestionData question = data.Questions[currentQuestionIndex];

        // SPECIAL UI for final ranking question
        if (question.Category == QuestionCategory.JobPreference)
        {
            QuestionData rankingQuestion = BuildJobRankingQuestion();
            events.UpdateQuestionUI?.Invoke(rankingQuestion, currentQuestionIndex, data.Questions.Length);
        }
        else
        {
            events.UpdateQuestionUI?.Invoke(question, currentQuestionIndex, data.Questions.Length);
        }
    }

    private QuestionData BuildJobRankingQuestion()
    {
        QuestionData rankingQuestion = new QuestionData
        {
            QuestionId = "JobRanking",
            Category = QuestionCategory.JobPreference,
            Answers = new List<AnswerOption>()
        };

        if (rankedJobs.Count == 0)
        {
            rankingQuestion.QuestionText = "Pick your 1st job choice.";
        }
        else if (rankedJobs.Count == 1)
        {
            rankingQuestion.QuestionText = "Pick your 2nd job choice.";
        }
        else
        {
            rankingQuestion.QuestionText = "Review ranking. Tap Next, or tap a ranked job to change.";
        }

        foreach (JobType job in GetAllJobs())
        {
            rankingQuestion.Answers.Add(new AnswerOption
            {
                AnswerText = GetJobAnswerLabel(job)
            });
        }

        return rankingQuestion;
    }

    private List<JobType> GetAllJobs()
    {
        return new List<JobType>
        {
            JobType.Lawyer,
            JobType.Surgeon,
            JobType.Cook
        };
    }

    private string GetJobAnswerLabel(JobType job)
    {
        int rankIndex = rankedJobs.IndexOf(job);

        if (rankIndex >= 0)
        {
            return $"{rankIndex + 1}. {GetJobDisplayName(job)}";
        }

        return GetJobDisplayName(job);
    }

    private string GetJobDisplayName(JobType job)
    {
        switch (job)
        {
            case JobType.Lawyer: return "Lawyer";
            case JobType.Surgeon: return "Doctor";
            case JobType.Cook: return "Cook";
            default: return job.ToString();
        }
    }

    public void UpdateAnswers(AnswerData newAnswer)
    {
        if (data.Questions == null || data.Questions.Length == 0)
            return;

        QuestionData currentQuestion = data.Questions[currentQuestionIndex];

        // SPECIAL behavior for ranking question
        if (currentQuestion.Category == QuestionCategory.JobPreference)
        {
            HandleJobRankingTap(newAnswer.AnswerIndex);
            return;
        }

        // Normal single-choice behavior for other questions
        foreach (var answer in pickedAnswers)
        {
            if (answer != null && answer != newAnswer)
            {
                answer.Reset();
            }
        }

        pickedAnswers.Clear();
        pickedAnswers.Add(newAnswer);
    }

    private void HandleJobRankingTap(int answerIndex)
    {
        List<JobType> allJobs = GetAllJobs();

        if (answerIndex < 0 || answerIndex >= allJobs.Count)
        {
            Debug.LogError("Selected job ranking index is out of range.");
            return;
        }

        JobType selectedJob = allJobs[answerIndex];
        int existingRankIndex = rankedJobs.IndexOf(selectedJob);

        // If player taps an already ranked job, undo from that point
        if (existingRankIndex >= 0)
        {
            rankedJobs.RemoveRange(existingRankIndex, rankedJobs.Count - existingRankIndex);
            DisplayCurrentQuestion();
            return;
        }

        // If 1st and 2nd are already chosen, ignore extra taps until changed
        if (rankedJobs.Count >= 2)
        {
            return;
        }

        rankedJobs.Add(selectedJob);

        // After 2nd choice, auto-assign the remaining job as 3rd
        if (rankedJobs.Count == 2)
        {
            foreach (JobType job in allJobs)
            {
                if (!rankedJobs.Contains(job))
                {
                    rankedJobs.Add(job);
                    break;
                }
            }
        }

        DisplayCurrentQuestion();
    }

    public void Accept()
    {
        if (data.Questions == null || data.Questions.Length == 0)
            return;

        QuestionData currentQuestion = data.Questions[currentQuestionIndex];

        // SPECIAL confirm logic for final ranking question
        if (currentQuestion.Category == QuestionCategory.JobPreference)
        {
            if (rankedJobs.Count < 3)
            {
                Debug.LogWarning("Complete the job ranking first.");
                return;
            }

            // ── AUDIO ──────────────────────────────────────────────────────
            AudioManager.Instance?.PlaySFX("UIClick");
            // ───────────────────────────────────────────────────────────────

            ApplyRankedJobWeights();
            rankedJobs.Clear();
                   
            currentQuestionIndex++;

            if (currentQuestionIndex >= data.Questions.Length)
            {
                FinishQuestionnaire();
            }
            else
            {
                DisplayCurrentQuestion();
            }

            return;
        }

        // Normal behavior for standard questions
        if (pickedAnswers.Count == 0)
        {
            Debug.LogWarning("No answer selected.");
            return;
        }

        int selectedAnswerIndex = pickedAnswers[0].AnswerIndex;

        if (selectedAnswerIndex < 0 || selectedAnswerIndex >= currentQuestion.Answers.Count)
        {
            Debug.LogError("Selected answer index is out of range.");
            return;
        }

        // ── AUDIO ──────────────────────────────────────────────────────────
        AudioManager.Instance?.PlaySFX("UIClick");
        // ───────────────────────────────────────────────────────────────────

        AnswerOption selectedAnswer = currentQuestion.Answers[selectedAnswerIndex];
        ApplyAnswerWeights(selectedAnswer);
        answerHistory.Push((currentQuestionIndex, selectedAnswerIndex, currentProfile.Clone()));

        ClearPickedAnswers();
        currentQuestionIndex++;

        if (currentQuestionIndex >= data.Questions.Length)
        {
            FinishQuestionnaire();
        }
        else
        {
            DisplayCurrentQuestion();
        }
    }

    public void GoBack()
    {
        if (answerHistory.Count == 0) return; // Already on first question

        // ── AUDIO ──────────────────────────────────────────────────────────
        AudioManager.Instance?.PlaySFX("UIClick");
        // ───────────────────────────────────────────────────────────────────

        var (prevIndex, _, weightsBefore) = answerHistory.Pop();

        currentQuestionIndex = prevIndex;
        currentProfile = weightsBefore; // Restore scores to before that answer
        ClearPickedAnswers();
        DisplayCurrentQuestion();
    }

    private void ApplyRankedJobWeights()
    {
        if (rankedJobs.Count != 3)
        {
            Debug.LogError("Job ranking is incomplete.");
            return;
        }

        AddJobWeight(rankedJobs[0], 3);
        AddJobWeight(rankedJobs[1], 2);
        AddJobWeight(rankedJobs[2], 1);
    }

    private void AddJobWeight(JobType job, int weight)
    {
        switch (job)
        {
            case JobType.Lawyer:
                currentProfile.LawyerScore += weight;
                break;
            case JobType.Surgeon:
                currentProfile.SurgeonScore += weight;
                break;
            case JobType.Cook:
                currentProfile.CookScore += weight;
                break;
        }
    }

    private void ApplyAnswerWeights(AnswerOption selectedAnswer)
    {
        currentProfile.KillerScore += selectedAnswer.KillerWeight;
        currentProfile.SocializerScore += selectedAnswer.SocializerWeight;
        currentProfile.AchieverScore += selectedAnswer.AchieverWeight;
        currentProfile.ExplorerScore += selectedAnswer.ExplorerWeight;

        currentProfile.LawyerScore += selectedAnswer.LawyerWeight;
        currentProfile.SurgeonScore += selectedAnswer.SurgeonWeight;
        currentProfile.CookScore += selectedAnswer.CookWeight;
    }

    private void ClearPickedAnswers()
    {
        foreach (var answer in pickedAnswers)
        {
            if (answer != null)
            {
                answer.Reset();
            }
        }

        pickedAnswers.Clear();
    }

    private void FinishQuestionnaire()
    {
        CalculateFinalResult();
        ActiveCareerController.LastQuestionnaireResult = currentProfile;

        if (events != null)
        {
            events.QuestionnaireCompleted?.Invoke(currentProfile);
        }
    }

    private void CalculateFinalResult()
    {
        float maxGamerScore = Mathf.Max(
            currentProfile.KillerScore,
            currentProfile.SocializerScore,
            currentProfile.AchieverScore,
            currentProfile.ExplorerScore
        );

        currentProfile.FinalGamerTypes.Clear();

        const float epsilon = 0.0001f;

        if (Mathf.Abs(currentProfile.KillerScore - maxGamerScore) < epsilon)
            currentProfile.FinalGamerTypes.Add(GamerType.Killer);

        if (Mathf.Abs(currentProfile.SocializerScore - maxGamerScore) < epsilon)
            currentProfile.FinalGamerTypes.Add(GamerType.Socializer);

        if (Mathf.Abs(currentProfile.AchieverScore - maxGamerScore) < epsilon)
            currentProfile.FinalGamerTypes.Add(GamerType.Achiever);

        if (Mathf.Abs(currentProfile.ExplorerScore - maxGamerScore) < epsilon)
            currentProfile.FinalGamerTypes.Add(GamerType.Explorer);

        if (currentProfile.FinalGamerTypes.Count > 0)
            currentProfile.FinalGamerType = currentProfile.FinalGamerTypes[0];

        int maxJobScore = Mathf.Max(
            currentProfile.LawyerScore,
            currentProfile.SurgeonScore,
            currentProfile.CookScore
        );

        if (maxJobScore == currentProfile.LawyerScore)
            currentProfile.FinalJobType = JobType.Lawyer;
        else if (maxJobScore == currentProfile.SurgeonScore)
            currentProfile.FinalJobType = JobType.Surgeon;
        else
            currentProfile.FinalJobType = JobType.Cook;
    }
}