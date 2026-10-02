using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetTextColor : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _Text;
    
    private TMP_ColorGradient _Color;
    
    void Start()
    {
        _Color = ActiveCareerController.ActiveCareer.TextColor;
        _Text = GetComponent<TMP_Text>();

        if (_Color != null)
        {
            _Text.colorGradientPreset = _Color;
        }
    }
}
