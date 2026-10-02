using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetHint : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _HintText;

    void Start()
    {
        _HintText.text = Minigame3Manager.CurrentHint;
    }
}
