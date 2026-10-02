using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetHighScoreText : MonoBehaviour
{
    private TMP_Text _HighScoreText;

    void Start()
    {
        _HighScoreText = ScoreManager.Instance.HighScoreText;
        ScoreManager.Instance.UpdateHighScore();
    }
}
