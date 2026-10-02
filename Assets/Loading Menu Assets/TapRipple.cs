using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Spawns a ripple ring at the world position the player tapped.
/// Uses the same RenderTexture → world coordinate conversion as MoveToTapInRenderTexture.
///
/// Setup:
///  1. Create a child GameObject on the player called "TapRipple".
///     Give it a SpriteRenderer with a white circle/ring sprite (or just a plain circle).
///     Set its Order in Layer to something high (e.g. 10) so it renders on top.
///  2. Assign that child's SpriteRenderer as RippleRenderer.
///  3. Attach this script to the same GameObject as MoveToTapInRenderTexture.
///
/// The ripple expands from 0 → MaxScale and fades out over Duration seconds,
/// then resets and waits for the next tap. It stays at the tap position, not
/// following the player.
/// </summary>
public class TapRipple : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Same camera as in MoveToTapInRenderTexture.")]
    public Camera GameplayCamera;

    [Tooltip("Same RawImage as in MoveToTapInRenderTexture.")]
    public RawImage RawImageUI;

    [Tooltip("A SpriteRenderer on a separate child/prefab that draws the ring. " +
             "Use a circle sprite — solid or outline — white works best.")]
    public SpriteRenderer RippleRenderer;

    [Header("Feel")]
    [Tooltip("How long one ripple pulse lasts in seconds.")]
    public float Duration = 0.45f;

    [Tooltip("World-space scale the ring grows to at peak.")]
    public float MaxScale = 1.4f;

    [Tooltip("Color of the ring at the start of the pulse (alpha = fully visible).")]
    public Color StartColor = new Color(1f, 1f, 1f, 0.7f);

    [Tooltip("Allow a new ripple while one is already playing (looks nice on rapid taps).")]
    public bool AllowOverlap = true;

    // ── state ────────────────────────────────────────────────────────────────
    private RectTransform _rawImageRect;
    private bool _isPlaying = false;

    // ─────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        if (RawImageUI != null)
            _rawImageRect = RawImageUI.rectTransform;

        // Hide the ripple at start
        if (RippleRenderer != null)
        {
            RippleRenderer.transform.localScale = Vector3.zero;
            Color c = StartColor;
            c.a = 0f;
            RippleRenderer.color = c;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Ignore clicks on UI
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            HandleTap(Input.mousePosition);
        }
    else
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                // Ignore touches on UI
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject(t.fingerId))
                    return;

                HandleTap(t.position);
            }
        }
    }

    private void HandleTap(Vector2 screenPos)
    {
        if (!AllowOverlap && _isPlaying)
            return;

        Vector3 worldPos;
        if (!TryGetWorldPoint(screenPos, out worldPos))
            return;

        // Move the ripple renderer to the tap world position
        if (RippleRenderer != null)
        {
            RippleRenderer.transform.position = new Vector3(worldPos.x, worldPos.y, RippleRenderer.transform.position.z);
            StartCoroutine(PlayRipple());
        }
    }

    private IEnumerator PlayRipple()
    {
        _isPlaying = true;

        float elapsed = 0f;

        while (elapsed < Duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / Duration;

            // Scale: 0 → MaxScale, eased out (fast start, slow finish)
            float scale = Mathf.Lerp(0f, MaxScale, EaseOut(t));
            RippleRenderer.transform.localScale = new Vector3(scale, scale, 1f);

            // Alpha: 1 → 0
            Color c = StartColor;
            c.a = Mathf.Lerp(StartColor.a, 0f, t);
            RippleRenderer.color = c;

            yield return null;
        }

        // Reset
        RippleRenderer.transform.localScale = Vector3.zero;
        Color reset = StartColor;
        reset.a = 0f;
        RippleRenderer.color = reset;

        _isPlaying = false;
    }

    /// <summary>Quadratic ease-out: fast expand that slows at the edge.</summary>
    private float EaseOut(float t) => 1f - (1f - t) * (1f - t);

    // ── RenderTexture → world coord (same logic as MoveToTapInRenderTexture) ─
    private bool TryGetWorldPoint(Vector2 screenPos, out Vector3 worldPoint)
    {
        worldPoint = transform.position;

        if (GameplayCamera == null || RawImageUI == null || RawImageUI.texture == null)
            return false;

        Vector2 localPoint;
        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rawImageRect, screenPos, null, out localPoint);

        if (!inside || !_rawImageRect.rect.Contains(localPoint))
            return false;

        RenderTexture rt = RawImageUI.texture as RenderTexture;
        if (rt == null) return false;

        Vector2 uv = new Vector2(
            (localPoint.x - _rawImageRect.rect.xMin) / _rawImageRect.rect.width,
            (localPoint.y - _rawImageRect.rect.yMin) / _rawImageRect.rect.height
        );

        Vector3 rtPoint = new Vector3(uv.x * rt.width, uv.y * rt.height, 0f);
        worldPoint = GameplayCamera.ScreenToWorldPoint(rtPoint);
        worldPoint.z = 0f;

        return true;
    }
}