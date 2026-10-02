using UnityEngine;

public static class CareerProgress
{
    private const int MaxStage = 3;
    private const int DefaultUnlockedStage = 1;

    private static string GetKey(CareerSO career)
    {
        if (career == null)
        {
            Debug.LogError("CareerProgress: career is null.");
            return "CareerProgress_NULL";
        }

        return $"CareerProgress_{career.name}";
    }

    public static int GetUnlockedStage(CareerSO career)
    {
        return PlayerPrefs.GetInt(GetKey(career), DefaultUnlockedStage);
    }

    public static bool IsUnlocked(CareerSO career, int stage)
    {
        return stage <= GetUnlockedStage(career);
    }

    public static void CompleteStage(CareerSO career, int completedStage)
    {
        if (career == null) return;

        int current = GetUnlockedStage(career);

        // unlock next stage
        int newUnlocked = Mathf.Clamp(Mathf.Max(current, completedStage + 1), 1, MaxStage);

        if (newUnlocked != current)
        {
            PlayerPrefs.SetInt(GetKey(career), newUnlocked);
            PlayerPrefs.Save();
            Debug.Log($"CareerProgress: {career.name} unlocked stage {newUnlocked}");
        }
    }

    public static void ResetCareer(CareerSO career)
    {
        if (career == null) return;

        PlayerPrefs.SetInt(GetKey(career), DefaultUnlockedStage);
        PlayerPrefs.Save();
    }
}