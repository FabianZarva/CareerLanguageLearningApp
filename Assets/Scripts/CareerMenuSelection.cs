using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ─────────────────────────────────────────────────────────────────────────────
// CareerMenuItem  (data bag — no logic lives here)
// ─────────────────────────────────────────────────────────────────────────────
[System.Serializable]
public class CareerMenuItem
{
    public string debugName;
    public CareerSO career;

    [Tooltip("The Button whose EventTrigger we register PointerDown/Up on.")]
    public Button clickButton;

    [Tooltip("The RectTransform that scales up during the hold.")]
    public RectTransform imageTransform;

    [Tooltip("The main career illustration Image (alpha dimmed when another is chosen).")]
    public Image image;

    [Tooltip("The card / label background Image (tinted on confirm).")]
    public Image cardImage;

    public TMP_Text titleText;

    [Tooltip("Radial fill Image set to Image Type = Filled, Fill Method = Radial360. " +
             "Starts at fillAmount 0, fills to 1 during the hold. " +
             "Place it ON TOP of the button so the player can see it.")]
    public Image holdFillImage;

    // Set at runtime — do not touch in Inspector.
    [HideInInspector] public Vector3 baseScale;
}

// ─────────────────────────────────────────────────────────────────────────────
// CareerMenuSelection
// ─────────────────────────────────────────────────────────────────────────────
public class CareerMenuSelection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PickCareer pickCareer;

    [Header("Menu Items")]
    [SerializeField] private CareerMenuItem lawyer;
    [SerializeField] private CareerMenuItem chef;
    [SerializeField] private CareerMenuItem surgeon;

    // ── Hold settings ────────────────────────────────────────────────────────
    [Header("Hold-to-Confirm")]
    [Tooltip("Seconds the player must hold before the career loads.")]
    [SerializeField] private float holdDuration = 1.2f;

    [Tooltip("Maximum extra scale added to the image while holding (on top of selectedScale).")]
    [SerializeField] private float holdScaleBoost = 0.08f;

    [Tooltip("Final locked-in scale when the hold completes (before scene load).")]
    [SerializeField] private float selectedScale = 1.04f;

    // ── Visual feedback ──────────────────────────────────────────────────────
    [Header("Appearance")]
    [SerializeField] private float nonSelectedAlpha = 0.72f;

    [Header("Colors")]
    [SerializeField] private Color cardNormal    = Color.white;
    [SerializeField] private Color cardSelected  = new Color32(25, 146, 184, 255);
    [SerializeField] private Color titleNormal   = new Color32(25, 146, 184, 255);
    [SerializeField] private Color titleSelected = Color.white;

    [Tooltip("Color of the fill ring during the hold.")]
    [SerializeField] private Color fillColor     = new Color32(25, 146, 184, 200);

    // ── State ────────────────────────────────────────────────────────────────
    private bool            _busy         = false;
    private CareerMenuItem  _holding      = null;
    private Coroutine       _holdRoutine  = null;

    // ─────────────────────────────────────────────────────────────────────────
    // Init
    // ─────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        InitItem(lawyer,   OnPointerDownLawyer,   OnPointerUpLawyer);
        InitItem(chef,     OnPointerDownChef,     OnPointerUpChef);
        InitItem(surgeon,  OnPointerDownSurgeon,  OnPointerUpSurgeon);

        ResetAllVisuals();
    }

    private void InitItem(CareerMenuItem item,
                          UnityEngine.Events.UnityAction downAction,
                          UnityEngine.Events.UnityAction upAction)
    {
        if (item.imageTransform != null)
            item.baseScale = item.imageTransform.localScale;

        if (item.clickButton != null)
            item.clickButton.transition = Selectable.Transition.None;

        EventTrigger trigger = item.clickButton != null
            ? item.clickButton.gameObject.GetOrAddComponent<EventTrigger>()
            : null;

        if (trigger == null) return;

        AddTriggerEntry(trigger, EventTriggerType.PointerDown, _ => downAction());
        AddTriggerEntry(trigger, EventTriggerType.PointerUp,   _ => upAction());
        AddTriggerEntry(trigger, EventTriggerType.PointerExit, _ => upAction());

        if (item.holdFillImage != null)
        {
            item.holdFillImage.fillAmount = 0f;
            item.holdFillImage.color      = fillColor;
            item.holdFillImage.gameObject.SetActive(false);
        }
    }

    // ── EventTrigger helpers ─────────────────────────────────────────────────
    private static void AddTriggerEntry(EventTrigger trigger,
                                        EventTriggerType type,
                                        UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PointerDown / PointerUp callbacks (one pair per career)
    // ─────────────────────────────────────────────────────────────────────────
    private void OnPointerDownLawyer()  => BeginHold(lawyer,  chef,    surgeon);
    private void OnPointerUpLawyer()    => CancelHold(lawyer);

    private void OnPointerDownChef()    => BeginHold(chef,    lawyer,  surgeon);
    private void OnPointerUpChef()      => CancelHold(chef);

    private void OnPointerDownSurgeon() => BeginHold(surgeon, lawyer,  chef);
    private void OnPointerUpSurgeon()   => CancelHold(surgeon);

    // ─────────────────────────────────────────────────────────────────────────
    // Hold logic
    // ─────────────────────────────────────────────────────────────────────────
    private void BeginHold(CareerMenuItem target, CareerMenuItem otherA, CareerMenuItem otherB)
    {
        if (_busy) return;
        if (_holding == target) return;

        if (_holding != null)
            CancelHold(_holding);

        _holding = target;

        if (target.holdFillImage != null)
        {
            target.holdFillImage.fillAmount = 0f;
            target.holdFillImage.gameObject.SetActive(true);
        }

        DimItem(otherA);
        DimItem(otherB);

        _holdRoutine = StartCoroutine(HoldRoutine(target, otherA, otherB));
    }

    private void CancelHold(CareerMenuItem target)
    {
        if (_holding != target) return;

        if (_holdRoutine != null)
        {
            StopCoroutine(_holdRoutine);
            _holdRoutine = null;
        }

        _holding = null;

        if (!_busy)
            ResetAllVisuals();
    }

    private IEnumerator HoldRoutine(CareerMenuItem target,
                                    CareerMenuItem otherA,
                                    CareerMenuItem otherB)
    {
        float elapsed = 0f;

        while (elapsed < holdDuration)
        {
            elapsed += Time.deltaTime;
            float t  = Mathf.Clamp01(elapsed / holdDuration);

            if (target.holdFillImage != null)
                target.holdFillImage.fillAmount = t;

            if (target.imageTransform != null)
                target.imageTransform.localScale =
                    target.baseScale * Mathf.Lerp(1f, selectedScale + holdScaleBoost, t);

            yield return null;
        }

        // ── Hold complete ──────────────────────────────────────────────────
        _busy    = true;
        _holding = null;

        if (target.holdFillImage != null)
            target.holdFillImage.fillAmount = 1f;

        if (target.imageTransform != null)
            target.imageTransform.localScale = target.baseScale * selectedScale;

        ApplyConfirmed(target);

        // ── AUDIO: play confirm sound when hold completes ──
        AudioManager.Instance?.PlaySFX("CareerConfirm");

        yield return new WaitForSeconds(0.15f);

        if (target.holdFillImage != null)
            target.holdFillImage.gameObject.SetActive(false);

        pickCareer.LoadGame(target.career);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Visual helpers
    // ─────────────────────────────────────────────────────────────────────────
    private void ApplyConfirmed(CareerMenuItem item)
    {
        if (item.cardImage  != null) item.cardImage.color  = cardSelected;
        if (item.titleText  != null) item.titleText.color  = titleSelected;

        if (item.image != null)
        {
            Color c = item.image.color;
            c.a = 1f;
            item.image.color = c;
        }
    }

    private void DimItem(CareerMenuItem item)
    {
        if (item.imageTransform != null)
            item.imageTransform.localScale = item.baseScale;

        if (item.image != null)
        {
            Color c = item.image.color;
            c.a = nonSelectedAlpha;
            item.image.color = c;
        }

        if (item.cardImage  != null) item.cardImage.color  = cardNormal;
        if (item.titleText  != null) item.titleText.color  = titleNormal;
    }

    private void ResetAllVisuals()
    {
        ResetItem(lawyer);
        ResetItem(chef);
        ResetItem(surgeon);
    }

    private void ResetItem(CareerMenuItem item)
    {
        if (item.imageTransform != null)
            item.imageTransform.localScale = item.baseScale;

        if (item.image != null)
        {
            Color c = item.image.color;
            c.a = 1f;
            item.image.color = c;
        }

        if (item.cardImage  != null) item.cardImage.color  = cardNormal;
        if (item.titleText  != null) item.titleText.color  = titleNormal;

        if (item.holdFillImage != null)
        {
            item.holdFillImage.fillAmount = 0f;
            item.holdFillImage.gameObject.SetActive(false);
        }
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Small extension — keeps the init code clean without requiring a separate file
// ─────────────────────────────────────────────────────────────────────────────
public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T existing = go.GetComponent<T>();
        return existing != null ? existing : go.AddComponent<T>();
    }
}