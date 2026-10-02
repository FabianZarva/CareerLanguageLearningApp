using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetCurrencyText : MonoBehaviour
{
    private TMP_Text _CurrencyText;

    void Start()
    {
        _CurrencyText = CurrencyManager.Instance.CurrencyText;
        CurrencyManager.Instance.UpdateCurrencyText();
    }
}
