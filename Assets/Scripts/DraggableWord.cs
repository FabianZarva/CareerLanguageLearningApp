using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWord : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Minigame3Manager Manager { get; set; }

    private RectTransform _rect;
    private Canvas        _canvas;
    private Vector2       _originalAnchoredPos;
    private bool          _dragging;

    void Awake()
    {
        _rect   = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData e)
    {
        _originalAnchoredPos = _rect.anchoredPosition;
        _dragging = true;
    }

    public void OnDrag(PointerEventData e)
    {
        if (!_dragging) return;
        _rect.anchoredPosition += e.delta / _canvas.scaleFactor;
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (!_dragging) return;
        _dragging = false;
        Manager?.OnWordDropped(e.position);
        _rect.anchoredPosition = _originalAnchoredPos;
    }
}
