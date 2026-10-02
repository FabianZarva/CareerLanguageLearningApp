using TMPro;
using UnityEngine;

public class MainMenuIntroController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dialogueText;
    public TMP_Text tapHintText;

    [Header("Finish Behaviour")]
    public MainMenuIntroFinish introFinish;

    [Header("Intro Lines")]
    [TextArea(2, 5)]
    public string[] introLines;

    [Header("Intro Gate (IntroMenu only)")]
    public IntroScreen introGate;


    private int currentLineIndex = 0;
    private bool introFinished = false;

    private void Start()
    {
        if (introGate != null)
            return;

        if (introLines != null && introLines.Length > 0)
        {
            ShowLine(0);
        }
        else
        {
            dialogueText.text = "Welcome!";
            if (tapHintText != null)
                tapHintText.text = "";
        }
    }

    public void RestartIntro()
    {
        currentLineIndex = 0;
        introFinished = false;

        if (introLines != null && introLines.Length > 0)
        {
            ShowLine(0);
        }
        else
        {
            dialogueText.text = "Welcome!";
            if (tapHintText != null)
                tapHintText.text = "";
        }
    }

    public void NextLine()
    {
        if (introFinished)
            return;

        if (introLines == null || introLines.Length == 0)
            return;

        // ── AUDIO ──────────────────────────────────────────────────────────
        AudioManager.Instance?.PlaySFX("DialogueSwipe");
        // ───────────────────────────────────────────────────────────────────

        if (currentLineIndex < introLines.Length - 1)
        {
            currentLineIndex++;
            ShowLine(currentLineIndex);
        }
        else
        {
            FinishIntroSequence();
        }
    }

    private void ShowLine(int index)
    {
        dialogueText.text = introLines[index];

        if (tapHintText != null)
        {
            tapHintText.text = "Tap to continue";
        }
    }

    private void FinishIntroSequence()
    {
        introFinished = true;

        if (introFinish != null)
            introFinish.FinishIntro();
    }
}