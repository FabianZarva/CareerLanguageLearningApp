using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Minigame3Manager : MonoBehaviour
{
    private Minigame3Question[] _Questions;
    private static List<Minigame3Question> _UnansweredQuestions;
    public static List<Minigame3Question> UnansweredQuestions
    {
        get { return _UnansweredQuestions; }
    }

    private static string _CurrentHint;
    public static string CurrentHint
    {
        get { return _CurrentHint; }
    }

    private Minigame3Question _CurrentQuestion;

    [SerializeField]
    private TMP_Text _InstructionText;

    [SerializeField]
    private GameObject _DraggableWordObject;

    [SerializeField]
    private GameObject[] _ButtonObjects;

    private DraggableWord  _draggableWord;
    private DropTarget[]   _dropTargets;
    private int            _currentQuestionIndex;
    private Camera         _uiCamera;

    // ── How long to wait after playing the sting before loading next scene ──
    [SerializeField] private float _CorrectDelay = 0.6f;
    [SerializeField] private float _WrongDelay   = 0.6f;

    // Prevent double-firing if OnWordDropped is somehow called twice
    private bool _answered = false;

    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Start()
    {
        Canvas rootCanvas = _DraggableWordObject.GetComponentInParent<Canvas>();
        _uiCamera = (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? rootCanvas.worldCamera
            : null;

        _draggableWord = _DraggableWordObject.GetComponent<DraggableWord>();
        if (_draggableWord == null)
            _draggableWord = _DraggableWordObject.AddComponent<DraggableWord>();
        _draggableWord.Manager = this;

        _dropTargets = new DropTarget[_ButtonObjects.Length];
        for (int i = 0; i < _ButtonObjects.Length; i++)
        {
            DropTarget dt = _ButtonObjects[i].GetComponent<DropTarget>();
            if (dt == null)
                dt = _ButtonObjects[i].AddComponent<DropTarget>();
            _dropTargets[i] = dt;

            Button btn = _ButtonObjects[i].GetComponent<Button>();
            if (btn != null)
                btn.onClick.RemoveAllListeners();
        }

        _Questions = ActiveCareerController.ActiveCareer.Minigame3Questions;
        if (_UnansweredQuestions == null || _UnansweredQuestions.Count == 0)
            ResetQuestions();

        SetCurrentQuestion();
    }

    void ResetQuestions()
    {
        _UnansweredQuestions = _Questions.ToList();
    }

    void SetCurrentQuestion()
    {
        _answered = false;
        _currentQuestionIndex = Random.Range(0, _UnansweredQuestions.Count);
        _CurrentQuestion      = _UnansweredQuestions[_currentQuestionIndex];

        _InstructionText.text = _CurrentQuestion.Instruction;
        _CurrentHint          = _CurrentQuestion.Hint;

        string correctObjectName = _CurrentQuestion.CorrectObjectName;

        foreach (DropTarget dt in _dropTargets)
            dt.IsCorrect = (dt.gameObject.name == correctObjectName);
    }

    public void OnWordDropped(Vector2 screenPosition)
    {
        if (_answered) return;

        foreach (DropTarget dt in _dropTargets)
        {
            if (dt.ContainsScreenPoint(screenPosition, _uiCamera))
            {
                _answered = true;

                if (dt.IsCorrect)
                    OnCorrectAnswer(_currentQuestionIndex);
                else
                    OnWrongAnswer();

                return;
            }
        }
    }

    // ── Answer handlers ───────────────────────────────────────────────────────

    void OnCorrectAnswer(int questionIndex)
    {
        if (GameManager.Instance.ScoreEnabled)
        {
            Timer timer = FindObjectOfType<Timer>();
            float timeLeft = timer != null ? timer.CurrentTime : 0f;
            ScoreManager.Instance.AddScore(timeLeft);
        }

        _UnansweredQuestions.RemoveAt(questionIndex);

        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("AnswerCorrect");

        StartCoroutine(LoadAfterDelay("Minigame 3 Success", _CorrectDelay));
    }

    void OnWrongAnswer()
    {
        if (GameManager.Instance.ScoreEnabled)
            ScoreManager.Instance.RemoveScore();

        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("AnswerWrong");

        StartCoroutine(LoadAfterDelay("Minigame 3 Failure", _WrongDelay));
    }

    IEnumerator LoadAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}