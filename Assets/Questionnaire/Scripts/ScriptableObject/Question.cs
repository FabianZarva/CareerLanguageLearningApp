using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Questionnaire
{
    public List<QuestionData> Questions = new List<QuestionData>();
}

[Serializable]
public class QuestionData
{
    public string QuestionId;

    [TextArea(2, 5)]
    public string QuestionText;

    public QuestionCategory Category;
    public List<AnswerOption> Answers = new List<AnswerOption>();
}

[Serializable]
public class AnswerOption
{
    public string AnswerText;

    public float KillerWeight;
    public float SocializerWeight;
    public float AchieverWeight;
    public float ExplorerWeight;

    public int LawyerWeight;
    public int SurgeonWeight;
    public int CookWeight;
}

public enum QuestionCategory
{
    GamerType,
    JobPreference
}