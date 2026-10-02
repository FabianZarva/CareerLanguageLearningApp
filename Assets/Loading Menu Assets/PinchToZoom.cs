using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Pinch-to-zoom for the Explorer world.
/// Works with the same RenderTexture + RawImage setup as MoveToTapInRenderTexture.
/// Attach this to the same GameObject as MoveToTapInRenderTexture (the player/Capsule),
/// or any active GameObject — it only needs the camera reference.
/// </summary>
public class PinchToZoom : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The GameplayCamera that renders to the RenderTexture.")]
    public Camera GameplayCamera;

    [Tooltip("The RawImage that displays the RenderTexture (same as in MoveToTapInRenderTexture).")]
    public RawImage RawImageUI;

    [Header("Zoom Limits")]
    [Tooltip("Minimum orthographic size (most zoomed IN).")]
    public float MinSize = 2f;

    [Tooltip("Maximum orthographic size (most zoomed OUT).")]
    public float MaxSize = 9f;

    [Header("Feel")]
    [Tooltip("How quickly the camera size lerps to the target. Higher = snappier.")]
    public float ZoomSpeed = 8f;

    [Tooltip("How many Unity units of ortho-size change per screen-pixel of pinch. " +
             "Tune this on device — 0.005 is a good starting point.")]
    public float Sensitivity = 0.005f;

    // ── state ────────────────────────────────────────────────────────────────
    private float _targetSize;
    private float _previousPinchDistance = -1f;

    // ─────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        if (GameplayCamera != null)
            _targetSize = GameplayCamera.orthographicSize;
    }

    private void Update()
    {
        if (GameplayCamera == null)
            return;

        // Ignore zoom if fingers are over UI
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);

                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject(t.fingerId))
                {
                    return;
                }
            }
        }

        // ── Two-finger pinch ─────────────────────────────────────────────────
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // Only start tracking once both fingers are down and moving
            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                // Record initial distance so there's no jump on frame 1
                _previousPinchDistance = Vector2.Distance(t0.position, t1.position);
                return;
            }

            float currentDistance = Vector2.Distance(t0.position, t1.position);

            if (_previousPinchDistance > 0f)
            {
                float delta = _previousPinchDistance - currentDistance; // positive = fingers closing = zoom in
                _targetSize += delta * Sensitivity;
                _targetSize = Mathf.Clamp(_targetSize, MinSize, MaxSize);
            }

            _previousPinchDistance = currentDistance;
        }
        else
        {
            _previousPinchDistance = -1f;
        }

        // ── Editor / mouse scroll fallback ───────────────────────────────────
#if UNITY_EDITOR
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            _targetSize -= scroll * 3f; // scroll up = zoom in
            _targetSize = Mathf.Clamp(_targetSize, MinSize, MaxSize);
        }
#endif

        // ── Smooth lerp to target ────────────────────────────────────────────
        GameplayCamera.orthographicSize = Mathf.Lerp(
            GameplayCamera.orthographicSize,
            _targetSize,
            Time.deltaTime * ZoomSpeed
        );
    }
}