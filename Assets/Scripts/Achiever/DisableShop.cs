using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableShop : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance.CurrencyEnabled)
        {
            return;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
