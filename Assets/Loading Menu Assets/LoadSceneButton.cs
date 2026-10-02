using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // ── AUDIO ──────────────────────────────────────────────────────
            AudioManager.Instance?.PlaySFX("UIClick");
            // ───────────────────────────────────────────────────────────────

            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is wrong or does not exist.");
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}