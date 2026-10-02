using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetExplanation : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _ExplanationText;

    void Start()
    {
        _ExplanationText.text = Minigame1Manager.CurrentExplanation;
    }
}
