using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System;

public static class GameUtility
{
    public const string QuestionnaireFileName = "IntroQuestionnaire.xml";

    // Used by Resources.Load<TextAsset>()
    public const string QuestionnaireResourcePath = "Questionnaire/IntroQuestionnaire";
}

[System.Serializable]
public class Data
{
    public QuestionData[] Questions = new QuestionData[0];

    public static void Write(Data data, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Data));
        using (Stream stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, data);
        }
    }

    public static Data Fetch(string filePath)
    {
        return Fetch(out bool result, filePath);
    }

    public static Data Fetch(out bool result, string filePath)
    {
        if (!File.Exists(filePath))
        {
            result = false;
            return new Data();
        }

        XmlSerializer deserializer = new XmlSerializer(typeof(Data));
        using (Stream stream = new FileStream(filePath, FileMode.Open))
        {
            var data = (Data)deserializer.Deserialize(stream);
            result = true;
            return data;
        }
    }

    // NEW: load from xml text instead of file path
    public static Data FetchFromXmlString(string xml)
    {
        if (string.IsNullOrEmpty(xml))
            return new Data();

        XmlSerializer deserializer = new XmlSerializer(typeof(Data));
        using (StringReader reader = new StringReader(xml))
        {
            return (Data)deserializer.Deserialize(reader);
        }
    }
}