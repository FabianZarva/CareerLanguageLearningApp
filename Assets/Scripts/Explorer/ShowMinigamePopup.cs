using UnityEngine;

public class ShowMinigamePopup : MonoBehaviour
{
    public GameObject PopupObject;

    void Start()
    {
        PopupObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PopupObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PopupObject.SetActive(false);
        }
    }
}

