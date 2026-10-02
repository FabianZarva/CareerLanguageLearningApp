using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TermItemUI : MonoBehaviour
{
    public GameObject descriptionPanel;
    private LayoutElement layoutElement;
    private CanvasGroup canvasGroup;
    private bool isExpanded = false;

    void Awake()
    {
        layoutElement = descriptionPanel.GetComponent<LayoutElement>();
        if (layoutElement == null)
            layoutElement = descriptionPanel.gameObject.AddComponent<LayoutElement>();

        canvasGroup = descriptionPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = descriptionPanel.AddComponent<CanvasGroup>();

        // Start hidden
        layoutElement.ignoreLayout = true;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        descriptionPanel.SetActive(true); // Keep active for layout
    }

    public void ToggleDescription()
    {
        // ── AUDIO ──
        AudioManager.Instance?.PlaySFX("UIClick");

        isExpanded = !isExpanded;
        layoutElement.ignoreLayout = !isExpanded;
        canvasGroup.alpha = isExpanded ? 1f : 0f;
        canvasGroup.blocksRaycasts = isExpanded;
        canvasGroup.interactable = isExpanded;

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}