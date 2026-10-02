using UnityEngine;
using UnityEngine.Events;

public class MainMenuIntroFinish : MonoBehaviour
{
    [Header("Hide when Eki finishes")]
    public GameObject[] hideAfterLastLine;

    [Header("Show when Eki finishes")]
    public GameObject[] showAfterLastLine;

    [Header("Extra callback")]
    public UnityEvent onIntroFinished;

    private bool alreadyFinished = false;

    public void FinishIntro()
    {
        if (alreadyFinished)
            return;

        alreadyFinished = true;

        foreach (GameObject go in hideAfterLastLine)
        {
            if (go != null) go.SetActive(false);
        }

        foreach (GameObject go in showAfterLastLine)
        {
            if (go != null) go.SetActive(true);
        }

        onIntroFinished?.Invoke();
    }
}