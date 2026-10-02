using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drop this component onto any Button GameObject.
/// It hooks into the Button's onClick at runtime and plays "UIClick" via AudioManager.
/// No wiring needed in the Inspector beyond having AudioManager in the scene.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    [Tooltip("Sound name registered in AudioManager. Default: UIClick")]
    [SerializeField] private string soundName = "UIClick";

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PlayClick);
        }
    }

    void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(soundName);
    }
}