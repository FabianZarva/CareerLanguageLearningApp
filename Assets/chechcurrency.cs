using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chechcurrency : MonoBehaviour
{
    [SerializeField]
    private int money;
    private int price;

    void Start()
    {
        money = CurrencyManager.Instance.CurrencyCount;
        price = CurrencyManager.Instance.CurrencyPrice;
    }
    public void GetCurrency()
    {
        Debug.Log("currency is now " + money);
        Debug.Log("price is " + price);
    }
}
