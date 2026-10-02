using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Eklektos/New GameEvents")]
public class GameEvents : ScriptableObject
{
    public delegate void UpdateQuestionUICallback(QuestionData question, int currentIndex, int totalQuestions);
    public UpdateQuestionUICallback UpdateQuestionUI = null;

    public delegate void UpdateQuestionAnswerCallback(AnswerData pickedAnswer);
    public UpdateQuestionAnswerCallback UpdateQuestionAnswer = null;

    public delegate void QuestionnaireCompletedCallback(GamerTypeResult result);
    public QuestionnaireCompletedCallback QuestionnaireCompleted = null;
    public System.Action RequestGoBack = null;
}