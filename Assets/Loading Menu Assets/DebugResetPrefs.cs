using UnityEngine;

public class DebugResetPrefs : MonoBehaviour
{
    [ContextMenu("Delete ALL PlayerPrefs")]
    public void DeleteAllPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All PlayerPrefs deleted.");
    }
}