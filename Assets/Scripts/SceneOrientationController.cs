using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOrientationController : MonoBehaviour
{
    public enum OrientationMode
    {
        Portrait,
        Landscape
    }

    [SerializeField] private OrientationMode orientationMode = OrientationMode.Portrait;

    private void Awake()
    {
        ApplyOrientation();
    }

    private void Start()
    {
        ApplyOrientation();
    }

    private void ApplyOrientation()
    {
        if (orientationMode == OrientationMode.Portrait)
        {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;

            Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }
}