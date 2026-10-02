using UnityEngine;
using UnityEngine.UI;

public class IntroScreen : MonoBehaviour
{
    [Header("Splash (shown at scene start)")]
    public GameObject tapToStartPanel;

    [Header("Eki Dialogue (hidden at scene start, shown after first tap)")]
    public GameObject ekiDialoguePanel;

    [Header("Optional: Block Raycast during splash")]
    public Button tapAnywhereButton;

    private bool _gateOpen = false;

    private void Awake()
    {
        if (tapToStartPanel != null)
            tapToStartPanel.SetActive(true);

        if (ekiDialoguePanel != null)
            ekiDialoguePanel.SetActive(false);
    }

    private void Start()
    {
        if (tapAnywhereButton != null)
            tapAnywhereButton.onClick.AddListener(OpenGate);
    }

    private void Update()
    {
        if (_gateOpen)
            return;

        if (tapAnywhereButton == null)
        {
            if (Input.GetMouseButtonDown(0))
                OpenGate();
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                OpenGate();
        }
    }

    public void OpenGate()
    {
        if (_gateOpen)
            return;

        _gateOpen = true;

        // ── AUDIO ──────────────────────────────────────────────────────────
        AudioManager.Instance?.PlaySFX("UIClick");
        // ───────────────────────────────────────────────────────────────────

        if (tapToStartPanel != null)
            tapToStartPanel.SetActive(false);

        if (ekiDialoguePanel != null)
            ekiDialoguePanel.SetActive(true);

        MainMenuIntroController introController = ekiDialoguePanel
            .GetComponentInChildren<MainMenuIntroController>(includeInactive: true);

        if (introController != null)
            introController.RestartIntro();
    }
}