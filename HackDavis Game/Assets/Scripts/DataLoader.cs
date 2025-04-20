using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TimeSeriesLoader : MonoBehaviour
{
    // Stores the loaded 20 most recent values per disease
    public Dictionary<string, List<float>> diseaseData = new Dictionary<string, List<float>>();

    // List of datasets to load (set from Inspector or hardcoded)
    public List<string> diseasesToLoad = new List<string> { "heart_disease", "cancer", "stroke", "suicide", "diabetes" };

    void Awake()
    {
        foreach (string disease in diseasesToLoad)
        {
            LoadDataset(disease);
        }
    }

    void LoadDataset(string diseaseName)
    {
        string path = Path.Combine(Application.dataPath, $"Data/{diseaseName}.csv");

        if (!File.Exists(path))
        {
            Debug.LogError($"CSV not found for {diseaseName} at {path}");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        List<float> allDeaths = new List<float>();

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string[] tokens = lines[i].Split(',');
            if (tokens.Length < 2) continue;

            if (float.TryParse(tokens[1], out float deathCount))
            {
                allDeaths.Add(deathCount);
            }
        }

        // Store last 20 values
        int start = Mathf.Max(0, allDeaths.Count - 20);
        diseaseData[diseaseName] = allDeaths.GetRange(start, allDeaths.Count - start);

        Debug.Log($"Loaded {diseaseData[diseaseName].Count} values for {diseaseName}");
    }

    public List<float> GetRecentDeaths(string diseaseName)
    {
        return diseaseData.ContainsKey(diseaseName) ? diseaseData[diseaseName] : null;
    }
}
