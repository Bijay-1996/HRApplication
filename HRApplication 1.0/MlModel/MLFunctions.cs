using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms.Text;

namespace HRApplication_1._0.MlModel
{
    public class MLFunctions
    {
       public static void InputMlModel(List<DataModelClass.Profile> profiles)
        {
            try
            {
                foreach (var person in profiles)
                {
                    //string jobDescriptionText = File.ReadAllText(jobDescriptionPath);
                    string jobDescriptionText = person.Description;
                    List<string> cvTexts = new List<string>();
                    string cvText = ExtractTextFromCV(person.CV);
                    cvTexts.Add(cvText);
                    List<DataModelClass.MatchingResult> matchingResults = GetMatchingResults(jobDescriptionText, cvTexts);
                    Console.WriteLine("Job Description:");
                    Console.WriteLine(jobDescriptionText);
                    Console.WriteLine("Matching CVs:");
                    foreach (var result in matchingResults)
                    {
                        Console.WriteLine($"- CV: {result.CVIndex}, Similarity: {result.Similarity:F2}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.ReadLine();
        }

        private static string ExtractTextFromCV(string cvFilePath)
        {
            return File.ReadAllText(cvFilePath);
        }

        private static List<DataModelClass.MatchingResult> GetMatchingResults(string jobDescription, List<string> cvTexts)
        {
            var mlContext = new MLContext();
            var data = new List<DataModelClass.TextData>
        {
            new DataModelClass.TextData { Text = jobDescription, Label = "JobDescription" }
        };
            for (int i = 0; i < cvTexts.Count; i++)
            {
                data.Add(new DataModelClass.TextData { Text = cvTexts[i], Label = $"CV{i + 1}" });
            }
            var dataView = mlContext.Data.LoadFromEnumerable(data);
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(DataModelClass.TextData.Text))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"));
            var model = pipeline.Fit(dataView);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<DataModelClass.TextData, DataModelClass.TextFeatures>(model);
            List<DataModelClass.MatchingResult> matchingResults = new List<DataModelClass.MatchingResult>();
            for (int i = 1; i < data.Count; i++)
            {
                var jobDescriptionFeatures = predictionEngine.Predict(data[0]);
                var cvFeatures = predictionEngine.Predict(data[i]);
                double similarity = CalculateCosineSimilarity(jobDescriptionFeatures.Features, cvFeatures.Features);

                matchingResults.Add(new DataModelClass.MatchingResult { CVIndex = i, Similarity = similarity });
            }
            matchingResults = matchingResults.OrderByDescending(r => r.Similarity).ToList();

            return matchingResults;
        }

        private static double CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            double dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
            double magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
            double magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));

            if (magnitudeA == 0 || magnitudeB == 0)
            {
                return 0;
            }

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }

   

}
