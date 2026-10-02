using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetName : MonoBehaviour
{
    [SerializeField]
    private string _Name;
    [SerializeField]
    private TMP_Text _NameText;

    void Start()
    {
        _Name = ActiveCareerController.ActiveCareer.NPCName;

        _NameText = GetComponent<TMP_Text>();
        
        _NameText.text = _Name;
    }
}
