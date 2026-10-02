using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text tipBody;

    [Header("Tips")]
    [TextArea]
    [SerializeField] private string[] tips;

    private void Awake()
    {
        SetProgress(0f);
        ShowRandomTip();
    }

    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);

        if (fillImage != null)
            fillImage.fillAmount = progress;

        if (loadingText != null)
            loadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100f)}%";
    }

    private void ShowRandomTip()
    {
        if (tipBody == null || tips == null || tips.Length == 0)
            return;

        int index = Random.Range(0, tips.Length);
        tipBody.text = tips[index];
    }
}