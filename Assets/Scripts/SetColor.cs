using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetColor : MonoBehaviour
{
    [SerializeField]
    private Image _Image;
    [SerializeField]
    private Color _Color;
    
    void Start()
    {
        _Color = ActiveCareerController.ActiveCareer.BackgroundColor;
        _Image = GetComponent<Image>();

        if (_Color != null)
        {
            _Image.color = _Color;
        }
    }
}
