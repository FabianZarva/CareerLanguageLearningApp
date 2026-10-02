using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRegistry : MonoBehaviour
{
    public static BackgroundRegistry Instance;

    // Dictionary to store background items per career
    private static Dictionary<string, Dictionary<string, BackgroundItem>> careerBackgroundDict = new Dictionary<string, Dictionary<string, BackgroundItem>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure it persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Get background by career and background ID
    public static BackgroundItem GetBackgroundById(string careerId, string backgroundId)
    {
        if (careerBackgroundDict.ContainsKey(careerId) && careerBackgroundDict[careerId].ContainsKey(backgroundId))
        {
            return careerBackgroundDict[careerId][backgroundId];
        }
        return null; // Return null if not found
    }

    // Add or update a background for a specific career
    public static void AddOrUpdateBackground(string careerId, BackgroundItem item)
    {
        if (!careerBackgroundDict.ContainsKey(careerId))
        {
            careerBackgroundDict[careerId] = new Dictionary<string, BackgroundItem>();
        }

        if (careerBackgroundDict[careerId].ContainsKey(item.Id))
        {
            careerBackgroundDict[careerId][item.Id] = item; // Update the existing background
        }
        else
        {
            careerBackgroundDict[careerId].Add(item.Id, item); // Add a new background
        }
    }

    // Optionally, remove a background for a specific career
    public static void RemoveBackground(string careerId, string id)
    {
        if (careerBackgroundDict.ContainsKey(careerId) && careerBackgroundDict[careerId].ContainsKey(id))
        {
            careerBackgroundDict[careerId].Remove(id);
        }
    }
}



