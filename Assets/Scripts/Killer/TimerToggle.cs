using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class FeatureToggleController : MonoBehaviour
{
    public Toggle featureToggle;

    public string FieldName;

    private void Start()
    {
        var FieldInfo = GameManager.Instance.GetType().GetField(FieldName);
        
        if (FieldInfo != null)
        {
            bool isFeatureEnabled = (bool)FieldInfo.GetValue(GameManager.Instance);
            featureToggle.isOn = isFeatureEnabled;
        }
        else
        {
            Debug.LogError($"Field {FieldName} not found on GameManager.");
        }

        // Add the listener to handle toggle changes
        featureToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        // Use reflection to set the Field in GameManager
        var FieldInfo = GameManager.Instance.GetType().GetField(FieldName);
        
        if (FieldInfo != null)
        {
            FieldInfo.SetValue(GameManager.Instance, isOn);
        }
        else
        {
            Debug.LogError($"Field {FieldName} not found on GameManager.");
        }
    }
}


