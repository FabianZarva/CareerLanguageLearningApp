using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private Image _Image;
    [SerializeField]
    private Sprite _PlayerCharacter;
    [SerializeField]
    private bool _IsHappy;
    [SerializeField]
    private bool _IsSad;
    
    void Start()
    {
        if (_IsHappy == true)
        {
            _PlayerCharacter = ActiveCareerController.ActiveCareer.PlayerCharacterHappy;
        }
        else if (_IsSad == true)
        {
            _PlayerCharacter = ActiveCareerController.ActiveCareer.PlayerCharacterSad;
        }
        else
        {
            _PlayerCharacter = ActiveCareerController.ActiveCareer.PlayerCharacter;
        }
        
        _Image = GetComponent<Image>();

        if (_PlayerCharacter != null)
        {
            _Image.sprite = _PlayerCharacter;
        }
    }
}
