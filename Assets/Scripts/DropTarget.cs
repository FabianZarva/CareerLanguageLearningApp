using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class DropTarget : MonoBehaviour
{
    [HideInInspector] public bool IsCorrect;

    private RectTransform _rect;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public bool ContainsScreenPoint(Vector2 screenPoint, Camera cam)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(_rect, screenPoint, cam);
    }
}
