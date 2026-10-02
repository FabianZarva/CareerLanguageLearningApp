using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class Minigame1Manager : MonoBehaviour
{
    private Minigame1Question[] _Questions;
    private static List<Minigame1Question> _UnansweredQuestions;
    public static List<Minigame1Question> UnansweredQuestions
    {
        get { return _UnansweredQuestions; }
    }

    private static string _CurrentExplanation;
    public static string CurrentExplanation
    {
        get { return _CurrentExplanation; }
    }

    private Minigame1Question _CurrentQuestion;

    [SerializeField]
    private string[] _AllNyms;

    [SerializeField]
    private TMP_Text _Term1Text, _Term2Text;
    [SerializeField]
    private TMP_Text[] _NymText;
    [SerializeField]
    private Button[] _NymButtons; 
    private Button _CorrectButton;

    // ── How long to wait after playing the sting before loading next scene ──
    [SerializeField] private float _CorrectDelay = 0.6f;
    [SerializeField] private float _WrongDelay   = 0.6f;

    void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    void Start()
    {
        _Questions = ActiveCareerController.ActiveCareer.Minigame1Questions;

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

        _Term1Text.text = _CurrentQuestion.Term1;
        _Term2Text.text = _CurrentQuestion.Term2;

        _CurrentExplanation = _CurrentQuestion.Explanation;

        string correctNym = _CurrentQuestion.CorrectNym;
        List<string> incorrectNyms = new List<string>(_AllNyms);

        incorrectNyms.Remove(correctNym); 
        incorrectNyms.Shuffle();

        int correctSlotIndex = Random.Range(0, _NymText.Length);
        _NymText[correctSlotIndex].text = correctNym;

        int incorrectIndex = 0;
        for (int i = 0; i < _NymText.Length; i++)
        {
            if (i == correctSlotIndex)
                continue;

            _NymText[i].text = incorrectNyms[incorrectIndex];
            incorrectIndex++;
        }

        for (int i = 0; i < _NymButtons.Length; i++)
        {
            _NymButtons[i].onClick.RemoveAllListeners();

            if (_NymText[i].text == correctNym)
            {
                _NymButtons[i].onClick.AddListener(() => OnCorrectAnswer(randomQuestionIndex));
            }
            else
            {
                _NymButtons[i].onClick.AddListener(() => OnWrongAnswer());
            }
        }
    }

    // ── Answer handlers ───────────────────────────────────────────────────────

    void OnCorrectAnswer(int questionIndex)
    {
        // Disable buttons so the player can't tap again during delay
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

        StartCoroutine(LoadAfterDelay("Minigame 1 Success", _CorrectDelay));
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

        StartCoroutine(LoadAfterDelay("Minigame 1 Failure", _WrongDelay));
    }

    void SetButtonsInteractable(bool interactable)
    {
        foreach (Button btn in _NymButtons)
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