using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField]
    private TMP_Text _ScoreText;
    public TMP_Text HighScoreText;

    public static int Score;

    public static int HighScore;

    [SerializeField]
    private string gameID;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance.ScoreEnabled)
        {
            UpdateScoreText();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void AddScore(float timeLeft)
    {
        int bonusPoints = Mathf.FloorToInt(timeLeft * 10) * 10;
        Score += 100 + bonusPoints;

        Debug.Log("added " + (100 + bonusPoints) + " points, total score is: " + Score);

        // Ensure that the UI is updated after adding points
        UpdateScoreText();
    }


    public void RemoveScore()
    {
        Score -= 250;

        if (Score < 0)
        {
            Score = 0;
        }

        UpdateScoreText();
        Debug.Log("removing score");
    }

    public void UpdateHighScore()
    {
        if (Score > HighScore)
        {
            PlayerPrefs.SetInt(GetHighScoreKey(), Score);
            HighScore = Score;
        }
        else
        {
            HighScore = PlayerPrefs.GetInt(GetHighScoreKey(), 0);
        }

        HighScoreText.text = "Your Personal Best: " + HighScore.ToString();
    }

    public void UpdateScoreText()
    {
        _ScoreText.text = "Score: " + Score.ToString();
    }

    public void ResetScore()
    {
        Score = 0;
    }

    // Generate a unique key for each mini-game's high score
    private string GetHighScoreKey()
    {
        return "HighScore_" + gameID;
    }
}

