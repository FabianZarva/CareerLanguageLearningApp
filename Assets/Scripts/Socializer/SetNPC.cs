using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetNPC : MonoBehaviour
{
    [SerializeField]
    private Image _Image;
    [SerializeField]
    private Sprite _NPC;
    [SerializeField]
    private bool _IsHappy;
    [SerializeField]
    private bool _IsSad;
    
    void Start()
    {
        if (_IsHappy == true)
        {
            _NPC = ActiveCareerController.ActiveCareer.NPCHappy;
        }
        else if (_IsSad == true)
        {
            _NPC = ActiveCareerController.ActiveCareer.NPCSad;
        }
        else
        {
            _NPC = ActiveCareerController.ActiveCareer.NPC;
        }
        
        _Image = GetComponent<Image>();

        if (_NPC != null)
        {
            _Image.sprite = _NPC;
        }
    }
}
