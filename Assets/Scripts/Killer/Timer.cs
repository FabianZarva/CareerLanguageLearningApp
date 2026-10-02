using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _Text;
    public float CountdownValue; 

    private float currentCountdownValue;

    public float CurrentTime
    {
        get { return currentCountdownValue; }
    }
    public string MinigameName;

    private bool _Active;

    void Start()
    {
        if (GameManager.Instance.ScoreEnabled)
        {
            _Active = true;
            StartCoroutine(StartCountdown());
        }
        else
        {
            gameObject.SetActive(false);  
        }
    }

    private void Update()
    {
        if (!_Active) return;

        _Text.text = currentCountdownValue.ToString("F1");  

        if (currentCountdownValue <= 0)
        {
            ScoreManager.Instance.RemoveScore();
            
            SceneManager.LoadScene(MinigameName + " Failure");
        }
    }

    public IEnumerator StartCountdown()
    {
        currentCountdownValue = CountdownValue;

        while (currentCountdownValue > 0)
        {
            yield return new WaitForSeconds(0.1f);  
            currentCountdownValue -= 0.1f; 
        }

        currentCountdownValue = 0f;
    }
}

