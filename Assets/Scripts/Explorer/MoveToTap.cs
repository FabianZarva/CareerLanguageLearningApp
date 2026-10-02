using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveToTapInRenderTexture : MonoBehaviour
{
    [Header("Movement")]
    public float Speed = 5f;
    public float StopDistance = 0.12f;

    [Header("References")]
    public Camera GameplayCamera;
    public RawImage RawImageUI;

    [Header("Optional")]
    public bool ClampToArea = false;
    public Vector2 MinBounds;
    public Vector2 MaxBounds;

    private Vector3 _target;
    private RectTransform _rawImageRect;
    private Rigidbody2D _rb;

    void Start()
    {
        _target = transform.position;
        _rawImageRect = RawImageUI.rectTransform;
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    if (Input.GetMouseButton(0))
        {
            // Ignore clicks on UI
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 world;

            if (TryGetWorldPointFromPointer(Input.mousePosition, out world))
            {
                if (ClampToArea)
                {
                    world.x = Mathf.Clamp(world.x, MinBounds.x, MaxBounds.x);
                    world.y = Mathf.Clamp(world.y, MinBounds.y, MaxBounds.y);
                }

                _target = world;
            }
        }
    else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Ignore touches on UI
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            Vector3 world;

            if (TryGetWorldPointFromPointer(touch.position, out world))
            {
                if (ClampToArea)
                {
                    world.x = Mathf.Clamp(world.x, MinBounds.x, MaxBounds.x);
                    world.y = Mathf.Clamp(world.y, MinBounds.y, MaxBounds.y);
                }

                _target = world;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 current = _rb.position;
        Vector2 target2D = new Vector2(_target.x, _target.y);

        if (Vector2.Distance(current, target2D) <= StopDistance)
            return;

        Vector2 newPosition = Vector2.MoveTowards(current, target2D, Speed * Time.fixedDeltaTime);
        _rb.MovePosition(newPosition);
    }

    private bool TryGetWorldPointFromPointer(Vector2 screenPoint, out Vector3 worldPoint)
    {
        worldPoint = transform.position;

        if (GameplayCamera == null || RawImageUI == null || RawImageUI.texture == null)
            return false;

        Vector2 localPoint;
        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rawImageRect,
            screenPoint,
            null,
            out localPoint
        );

        if (!inside || !_rawImageRect.rect.Contains(localPoint))
            return false;

        RenderTexture rt = RawImageUI.texture as RenderTexture;
        if (rt == null)
            return false;

        Vector2 uv = new Vector2(
            (localPoint.x - _rawImageRect.rect.xMin) / _rawImageRect.rect.width,
            (localPoint.y - _rawImageRect.rect.yMin) / _rawImageRect.rect.height
        );

        Vector3 rtPoint = new Vector3(uv.x * rt.width, uv.y * rt.height, 0f);
        worldPoint = GameplayCamera.ScreenToWorldPoint(rtPoint);
        worldPoint.z = transform.position.z;

        return true;
    }
}