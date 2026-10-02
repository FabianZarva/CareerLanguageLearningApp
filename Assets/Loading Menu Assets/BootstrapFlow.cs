using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class BootstrapFlow : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject logoPanel;
    [SerializeField] private GameObject videoPanel;

    [Header("Loading UI")]
    [SerializeField] private LoadingPanelUI loadingUI;
    [SerializeField] private float loadingDuration = 3f;
    [SerializeField] private float loadingFinishedPause = 1.5f;

    [Header("Logo")]
    [SerializeField] private float logoDuration = 1.5f;

    [Header("Video")]
    [SerializeField] private VideoPlayer introVideoPlayer;
    [SerializeField] private float afterVideoPause = 2f;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "Quiz";

    private bool videoFinished;

    private void Start()
    {
        StartCoroutine(RunStartupFlow());
    }

    private IEnumerator RunStartupFlow()
    {
        loadingPanel.SetActive(true);
        logoPanel.SetActive(false);
        videoPanel.SetActive(false);

        if (loadingUI != null)
            loadingUI.SetProgress(0f);

        float elapsed = 0f;

        while (elapsed < loadingDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / loadingDuration);

            if (loadingUI != null)
                loadingUI.SetProgress(progress);

            yield return null;
        }

        if (loadingUI != null)
            loadingUI.SetProgress(1f);

        yield return new WaitForSecondsRealtime(loadingFinishedPause);

        // logo
        loadingPanel.SetActive(false);
        logoPanel.SetActive(true);
        videoPanel.SetActive(false);

        yield return new WaitForSecondsRealtime(logoDuration);

        // video
        logoPanel.SetActive(false);
        videoPanel.SetActive(true);

        if (introVideoPlayer != null && introVideoPlayer.clip != null)
        {
            yield return StartCoroutine(PlayIntroVideo());
        }
        else
        {
            yield return new WaitForSecondsRealtime(3f);
        }

        // pause after video finishes
        yield return new WaitForSecondsRealtime(afterVideoPause);
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator PlayIntroVideo()
    {
        videoFinished = false;

        introVideoPlayer.Stop();
        introVideoPlayer.time = 0;
        introVideoPlayer.frame = 0;
        introVideoPlayer.isLooping = false;

        introVideoPlayer.loopPointReached -= OnVideoFinished;
        introVideoPlayer.loopPointReached += OnVideoFinished;

        introVideoPlayer.Prepare();

        while (!introVideoPlayer.isPrepared)
            yield return null;

        introVideoPlayer.Play();

        // wait until it actually starts playing
        while (!introVideoPlayer.isPlaying)
            yield return null;

        // now wait until the clip ends
        while (!videoFinished)
            yield return null;

        introVideoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true;
    }
}