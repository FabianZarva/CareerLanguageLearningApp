#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class DataCreator_Window : EditorWindow
{
    private Data data = new Data();
    private Vector2 scrollPos;

    [MenuItem("Eklektos/Questionnaire Editor")]
    public static void Open()
    {
        GetWindow<DataCreator_Window>("Questionnaire Editor");
    }

    private void OnEnable()
    {
        if (data == null)
            data = new Data();

        if (data.Questions == null)
            data.Questions = new QuestionData[0];
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Eklektos Introductory Questionnaire", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("New Questionnaire", GUILayout.Height(30)))
        {
            data = new Data();
            data.Questions = new QuestionData[0];
        }

        if (GUILayout.Button("Load XML", GUILayout.Height(30)))
        {
            string path = EditorUtility.OpenFilePanel("Load Questionnaire", "Assets", "xml");
            if (!string.IsNullOrEmpty(path))
            {
                data = Data.Fetch(path);
                if (data.Questions == null)
                    data.Questions = new QuestionData[0];
            }
        }

        if (GUILayout.Button("Save XML", GUILayout.Height(30)))
        {
            string path = EditorUtility.SaveFilePanel(
                "Save Questionnaire",
                "Assets",
                GameUtility.QuestionnaireFileName,
                "xml"
            );

            if (!string.IsNullOrEmpty(path))
            {
                Data.Write(data, path);
                AssetDatabase.Refresh();
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        int questionCount = data.Questions != null ? data.Questions.Length : 0;
        EditorGUILayout.LabelField($"Questions: {questionCount}");

        if (GUILayout.Button("Add Question"))
        {
            AddQuestion();
        }

        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (data.Questions != null)
        {
            for (int i = 0; i < data.Questions.Length; i++)
            {
                DrawQuestion(i);
                EditorGUILayout.Space(10);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void AddQuestion()
    {
        List<QuestionData> questionList = new List<QuestionData>(data.Questions ?? new QuestionData[0]);

        QuestionData newQuestion = new QuestionData
        {
            QuestionId = $"Q{questionList.Count + 1}",
            QuestionText = "New Question",
            Category = QuestionCategory.GamerType,
            Answers = new List<AnswerOption>()
        };

        questionList.Add(newQuestion);
        data.Questions = questionList.ToArray();
    }

    private void DrawQuestion(int index)
    {
        QuestionData question = data.Questions[index];

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField($"Question {index + 1}", EditorStyles.boldLabel);

        question.QuestionId = EditorGUILayout.TextField("Question ID", question.QuestionId);
        question.QuestionText = EditorGUILayout.TextField("Question Text", question.QuestionText);
        question.Category = (QuestionCategory)EditorGUILayout.EnumPopup("Category", question.Category);

        EditorGUILayout.Space();

        if (question.Answers == null)
            question.Answers = new List<AnswerOption>();

        EditorGUILayout.LabelField("Answers", EditorStyles.boldLabel);

        for (int j = 0; j < question.Answers.Count; j++)
        {
            DrawAnswer(question, j);
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Answer"))
        {
            question.Answers.Add(new AnswerOption
            {
                AnswerText = "New Answer"
            });
        }

        if (GUILayout.Button("Delete Question"))
        {
            DeleteQuestion(index);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawAnswer(QuestionData question, int answerIndex)
    {
        AnswerOption answer = question.Answers[answerIndex];

        EditorGUILayout.BeginVertical("helpbox");
        EditorGUILayout.LabelField($"Answer {answerIndex + 1}", EditorStyles.boldLabel);

        answer.AnswerText = EditorGUILayout.TextField("Answer Text", answer.AnswerText);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Gamer Type Weights", EditorStyles.miniBoldLabel);
        answer.KillerWeight = EditorGUILayout.FloatField("Killer", answer.KillerWeight);
        answer.SocializerWeight = EditorGUILayout.FloatField("Socializer", answer.SocializerWeight);
        answer.AchieverWeight = EditorGUILayout.FloatField("Achiever", answer.AchieverWeight);
        answer.ExplorerWeight = EditorGUILayout.FloatField("Explorer", answer.ExplorerWeight);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Job Weights", EditorStyles.miniBoldLabel);
        answer.LawyerWeight = EditorGUILayout.IntField("Lawyer", answer.LawyerWeight);
        answer.SurgeonWeight = EditorGUILayout.IntField("Surgeon", answer.SurgeonWeight);
        answer.CookWeight = EditorGUILayout.IntField("Cook", answer.CookWeight);

        if (GUILayout.Button("Delete Answer"))
        {
            question.Answers.RemoveAt(answerIndex);
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.EndVertical();
    }

    private void DeleteQuestion(int index)
    {
        List<QuestionData> questionList = new List<QuestionData>(data.Questions);
        questionList.RemoveAt(index);
        data.Questions = questionList.ToArray();
    }
}
#endif