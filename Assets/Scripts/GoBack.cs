using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBack : MonoBehaviour
{
    public void Loadintro(string minigameName)
    {
        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("UIClick");

        if (GameManager.Instance.DialogueEnabled)
        {
            SceneManager.LoadScene(minigameName + " Dialogue");
        }
        else
        {
            SceneManager.LoadScene(minigameName + " Intro");
        }
    }

    public void LoadScene(string sceneName)
    {
        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("UIClick");

        SceneManager.LoadScene(sceneName);
    }

    public void LoadGame1()
    {
        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("UIClick");

        if (Minigame1Manager.UnansweredQuestions.Count == 0)
        {
            SceneManager.LoadScene("Minigame 1 End");
            return;
        }
        else
        {
            SceneManager.LoadScene("Minigame 1");
        }
    }

    public void LoadGame2()
    {
        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("UIClick");

        if (Minigame2Manager.UnansweredQuestions.Count == 0)
        {
            SceneManager.LoadScene("Minigame 2 End");
            return;
        }
        else
        {
            SceneManager.LoadScene("Minigame 2");
        }
    }

    public void BeginGame3()
    {
        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("UIClick");

        var activeCareer = ActiveCareerController.ActiveCareer;
        SceneManager.LoadScene(activeCareer.Minigame3Scene);
    }

    public void LoadGame3()
    {
        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("UIClick");

        var activeCareer = ActiveCareerController.ActiveCareer;

        if (Minigame3Manager.UnansweredQuestions.Count == 0)
        {
            SceneManager.LoadScene("Minigame 3 End");
            return;
        }
        else
        {
            SceneManager.LoadScene(activeCareer.Minigame3Scene);
        }
    }

    public void LoadMenu()
    {
        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("UIClick");

        var activeCareer = ActiveCareerController.ActiveCareer;

        if (GameManager.Instance.ScoreEnabled)
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.ResetScore();
            }
        }

        if (GameManager.Instance.WorldEnabled)
        {
            SceneManager.LoadScene(activeCareer.ExplorerWorld);
        }
        else
        {
            SceneManager.LoadScene("Testing");
        }
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}