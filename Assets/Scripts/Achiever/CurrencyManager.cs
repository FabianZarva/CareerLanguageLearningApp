using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int CurrencyCount;
    public int CurrencyGained;
    public int CurrencyPrice;
    public TMP_Text CurrencyText;

    public TMP_Text CurrencyRewardText;

    void Awake()
    {
        if (GameManager.Instance.CurrencyEnabled)
        {
            Instance = this;
            CurrencyCount = PlayerPrefs.GetInt("CurrencyTotal", 0);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void AddCurrency()
    {
        CurrencyCount += CurrencyGained;
        PlayerPrefs.SetInt("CurrencyTotal", CurrencyCount);
    }

    public void PayCurrency()
    {
        if (CurrencyCount >= CurrencyPrice)
        {
            CurrencyCount -= CurrencyPrice;
            PlayerPrefs.SetInt("CurrencyTotal", CurrencyCount);
        }
        else
        {
            return;
        }
        
    }

    public void UpdateCurrencyText()
    {
        CurrencyText.text = "Opportunities: " + CurrencyCount;
    }

    public void UpdateRewardText()
    {
        CurrencyRewardText.text = "You got " + CurrencyGained.ToString() + " Opportunities";
    }
}
