using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorMC.Gui.AudioPlayer;

public class QnAMapper
{
    string jsonFilePath;
    string jsonString;
    JsonDocument jsonDocument;

    public QnAMapper()
    {
        // Define the path to the JSON file
        string relativePath = @"..\..\..\..\ColorMC.Gui\Avatar\QuestionAndAnswer.json";
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

        try
        {
            // Read the JSON file into a string
            jsonString = File.ReadAllText(fullPath);

            // Parse the JSON string
            jsonDocument = JsonDocument.Parse(jsonString);

        } 
        catch (Exception e)
        {
            throw new Exception($"Error reading JSON file: {e.Message}");
        }
    }

    public int GetNumOfQuestions()
    {
        // Count the number of questions in the JSON file
        return jsonDocument.RootElement.EnumerateObject().Count();
    }

    public string[] GetQuestionAndAnswer(int qnum)
    {
        // Get the question and answer for the specified question number
        // Convert the question number to a string
        string questionNumber = qnum.ToString();

        // Get the question and answer for the specified question number
        if (jsonDocument.RootElement.TryGetProperty(questionNumber, out JsonElement questionElement))
        {
            string question = questionElement.GetProperty("question").GetString();
            string answer = questionElement.GetProperty("answer").GetString();
            return new string[] { question, answer };
        }
        else
        {
            throw new ArgumentException($"Question number {qnum} not found in the JSON file.");
        }

    }

    public string GetAudioFilePath(int qnum)
    {
        // Convert the question number to a string
        string questionNumber = qnum.ToString();

        // Get the audio file path for the specified question number
        if (jsonDocument.RootElement.TryGetProperty(questionNumber, out JsonElement questionElement))
        {
            string audioFilePath = questionElement.GetProperty("audioFilePath").GetString();
            return audioFilePath;
        }
        else
        {
            throw new ArgumentException($"Question number {qnum} not found in the JSON file.");
        }
    }
}
