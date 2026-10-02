using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetRewardText : MonoBehaviour
{
    private TMP_Text _RewardText;

    void Start()
    {
        CurrencyManager.Instance.AddCurrency();
        
        _RewardText = CurrencyManager.Instance.CurrencyRewardText;
        CurrencyManager.Instance.UpdateRewardText();
    }
}
