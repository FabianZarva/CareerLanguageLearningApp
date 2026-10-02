using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class Minigame2Manager : MonoBehaviour
{
    private Minigame2Question[] _Questions;
    private static List<Minigame2Question> _UnansweredQuestions;
    public static List<Minigame2Question> UnansweredQuestions
    {
        get { return _UnansweredQuestions; }
    }

    private static string _CurrentHint;
    public static string CurrentHint
    {
        get { return _CurrentHint; }
    }
    
    private Minigame2Question _CurrentQuestion;

    [SerializeField]
    private string[] _AllTerms;

    [SerializeField]
    private TMP_Text _InstructionText;
    [SerializeField]
    private TMP_Text[] _TermText;
    [SerializeField]
    private Button[] _ThoughtButtons;
    private Button _CorrectButton;

    // ── How long to wait after playing the sting before loading next scene ──
    [SerializeField] private float _CorrectDelay = 0.6f;
    [SerializeField] private float _WrongDelay   = 0.6f;

    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Start()
    {
        _Questions = ActiveCareerController.ActiveCareer.Minigame2Questions;
        _AllTerms  = ActiveCareerController.ActiveCareer.TermList;

        if (_UnansweredQuestions == null || _UnansweredQuestions.Count == 0)
        {
            ResetQuestions();
        }

        SetCurrentQuestion();
    }

    void ResetQuestions()
    {
        _UnansweredQuestions = _Questions.ToList();
    }

    void SetCurrentQuestion()
    {
        int randomQuestionIndex = Random.Range(0, _UnansweredQuestions.Count);
        _CurrentQuestion = _UnansweredQuestions[randomQuestionIndex];

        _InstructionText.text = _CurrentQuestion.Instruction;
        _CurrentHint = _CurrentQuestion.Minigame2Hint;

        string CorrectTerm = _CurrentQuestion.CorrectTerm;
        List<string> incorrectTerms = new List<string>(_AllTerms);

        incorrectTerms.Remove(CorrectTerm);
        incorrectTerms.Shuffle();

        int correctSlotIndex = Random.Range(0, _TermText.Length);
        _TermText[correctSlotIndex].text = CorrectTerm;

        int incorrectIndex = 0;
        for (int i = 0; i < _TermText.Length; i++)
        {
            if (i == correctSlotIndex)
                continue;

            _TermText[i].text = incorrectTerms[incorrectIndex];
            incorrectIndex++;
        }

        for (int i = 0; i < _ThoughtButtons.Length; i++)
        {
            _ThoughtButtons[i].onClick.RemoveAllListeners();

            if (_TermText[i].text == CorrectTerm)
            {
                _ThoughtButtons[i].onClick.AddListener(() => OnCorrectAnswer(randomQuestionIndex));
            }
            else
            {
                _ThoughtButtons[i].onClick.AddListener(() => OnWrongAnswer());
            }
        }
    }

    // ── Answer handlers ───────────────────────────────────────────────────────

    void OnCorrectAnswer(int questionIndex)
    {
        SetButtonsInteractable(false);

        if (GameManager.Instance.ScoreEnabled)
        {
            Timer timer = FindObjectOfType<Timer>();
            float timeLeft = timer != null ? timer.CurrentTime : 0f;
            ScoreManager.Instance.AddScore(timeLeft); 
        }

        _UnansweredQuestions.RemoveAt(questionIndex);

        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("AnswerCorrect");

        StartCoroutine(LoadAfterDelay("Minigame 2 Success", _CorrectDelay));
    }

    void OnWrongAnswer()
    {
        SetButtonsInteractable(false);

        if (GameManager.Instance.ScoreEnabled)
        {
            ScoreManager.Instance.RemoveScore();
        }

        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("AnswerWrong");

        StartCoroutine(LoadAfterDelay("Minigame 2 Failure", _WrongDelay));
    }

    void SetButtonsInteractable(bool interactable)
    {
        foreach (Button btn in _ThoughtButtons)
        {
            if (btn != null) btn.interactable = interactable;
        }
    }

    IEnumerator LoadAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}