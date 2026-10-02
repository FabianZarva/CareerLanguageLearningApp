using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPreview : MonoBehaviour
{
    [SerializeField]
    private Image PreviewImage;
    [SerializeField]
    private Sprite PreviewSprite;
    // Start is called before the first frame update
    void Start()
    {
        PreviewSprite = ActiveCareerController.ActiveCareer.Shopitem1.Preview;
        PreviewImage.sprite = PreviewSprite;
    }
}
