using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI answerText = null;
    [SerializeField] private Image toggleImage = null;
    [SerializeField] private Sprite defaultSprite = null;
    [SerializeField] private Sprite selectedSprite = null;
    [SerializeField] private GameEvents events = null;

    private bool checkedState = false;
    private int answerIndex = -1;
    private RectTransform rect = null;

    public int AnswerIndex => answerIndex;
    public RectTransform Rect => rect;
    public bool IsChecked => checkedState;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        Reset();
    }

    public void UpdateData(string newAnswerText, int newAnswerIndex)
    {
        answerText.text = newAnswerText;
        answerIndex = newAnswerIndex;
        Reset();
    }

    public void Reset()
    {
        SetSelectedVisual(false);
    }

    public void SetSelectedVisual(bool isSelected)
    {
        checkedState = isSelected;

        if (toggleImage != null)
        {
            toggleImage.sprite = isSelected ? selectedSprite : defaultSprite;
        }
    }

    public void SwitchState()
    {
        checkedState = true;

        if (toggleImage != null && selectedSprite != null)
        {
            toggleImage.sprite = selectedSprite;
        }

        if (events != null)
        {
            events.UpdateQuestionAnswer?.Invoke(this);
        }
    }
}