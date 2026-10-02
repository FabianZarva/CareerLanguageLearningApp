using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text DialogueText;
    public Image characterImage;
    [SerializeField] float _TextSpeed;
    [SerializeField] private string _MinigameName;

    [Header("Swipe Settings")]
    [SerializeField] private float _SwipeThreshold = 50f;  
    [SerializeField] private float _SwipeMaxVertical = 75f; 

    [Header("UI")]
    [SerializeField] private GameObject _SwipeHint;        

    private DialogueLine[] dialogueScript;
    private int DialogueIndex;

    private Vector2 _touchStart;
    private bool _tracking;

    private bool _isTyping;

    public enum MinigameScriptType
    {
        Minigame1Script,
        Minigame2Script,
        Minigame3Script
    }

    [Header("Minigame Selection")]
    public MinigameScriptType selectedMinigame;


    void Start()
    {
        dialogueScript = GetSelectedMinigameDialogue();

        if (dialogueScript != null && dialogueScript.Length > 0)
        {
            DialogueText.text = string.Empty;
            StartDialogue();
        }
        else
        {
            Debug.LogError("Dialogue not found for: " + selectedMinigame);
            LoadScene();
        }
    }


    private DialogueLine[] GetSelectedMinigameDialogue()
    {
        switch (selectedMinigame)
        {
            case MinigameScriptType.Minigame1Script:
                return ActiveCareerController.ActiveCareer.Minigame1Dialogue;
            case MinigameScriptType.Minigame2Script:
                return ActiveCareerController.ActiveCareer.Minigame2Dialogue;
            case MinigameScriptType.Minigame3Script:
                return ActiveCareerController.ActiveCareer.Minigame3Dialogue;
            default:
                return null;
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _touchStart = Input.mousePosition;
            _tracking = true;
        }

        if (_tracking && Input.GetMouseButtonUp(0))
        {
            _tracking = false;
            Vector2 delta = (Vector2)Input.mousePosition - _touchStart;
            EvaluateSwipe(delta);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _touchStart = touch.position;
                _tracking = true;
            }

            if (_tracking && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                _tracking = false;
                Vector2 delta = touch.position - _touchStart;
                EvaluateSwipe(delta);
            }
        }
    }

    private void EvaluateSwipe(Vector2 delta)
    {
        bool isRightSwipe = delta.x > _SwipeThreshold
                         && Mathf.Abs(delta.y) < _SwipeMaxVertical;

        if (!isRightSwipe) return;

        // ── AUDIO: play swipe sound on every advance ──
        AudioManager.Instance?.PlaySFX("DialogueSwipe");

        if (_isTyping)
        {
            StopAllCoroutines();
            _isTyping = false;

            if (dialogueScript != null && DialogueIndex < dialogueScript.Length)
            {
                DialogueText.text = dialogueScript[DialogueIndex].text;
                characterImage.sprite = dialogueScript[DialogueIndex].characterSprite;
            }
        }
        else
        {
            NextLine();
        }
    }


    private void StartDialogue()
    {
        DialogueIndex = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        if (dialogueScript == null || DialogueIndex >= dialogueScript.Length) yield break;

        _isTyping = true;

        if (_SwipeHint != null) _SwipeHint.SetActive(true);

        characterImage.sprite = dialogueScript[DialogueIndex].characterSprite;

        foreach (char c in dialogueScript[DialogueIndex].text.ToCharArray())
        {
            DialogueText.text += c;
            yield return new WaitForSeconds(_TextSpeed);
        }

        _isTyping = false;
    }

    void NextLine()
    {
        if (dialogueScript != null && DialogueIndex < dialogueScript.Length - 1)
        {
            DialogueIndex++;
            DialogueText.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            if (_SwipeHint != null) _SwipeHint.SetActive(false);
            LoadScene();
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(_MinigameName);
    }
}