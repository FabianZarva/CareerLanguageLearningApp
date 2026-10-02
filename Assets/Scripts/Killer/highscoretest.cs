using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class highscoretest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("highscore 1 = " + PlayerPrefs.GetInt("HighScore_Minigame1"));
    }
}
