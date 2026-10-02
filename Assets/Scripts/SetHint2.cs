using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetHint2 : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _HintText;

    void Start()
    {
        _HintText.text = Minigame2Manager.CurrentHint;
    }
}
