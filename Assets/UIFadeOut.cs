using System.Collections;
using UnityEngine;

public class UIFadeInOut : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float stayTime = 3f;
    public float fadeTime = 1f;
    public bool playOnStart = true;

    private bool hasStarted = false;

    void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        if (playOnStart)
            StartFadeSequence();
    }

    public void StartFadeSequence()
    {
        if (hasStarted)
            return;

        hasStarted = true;
        StartCoroutine(ShowAndFadeOut());
    }

    IEnumerator ShowAndFadeOut()
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(stayTime);

        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsedTime / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}