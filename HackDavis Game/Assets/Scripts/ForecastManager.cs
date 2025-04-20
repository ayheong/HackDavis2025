using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using System.Linq;

public class ForecastManager : MonoBehaviour
{
    [Header("Dependencies")]
    public TimeSeriesLoader loader;

    [Header("API Settings")]
    [SerializeField] private string apiUrl = "https://HackDavis2025-project-api.onrender.com/forecast";
    [SerializeField] private ForecastVisualizer visualizer;

    [SerializeField] private TextMeshProUGUI yearText;
    private int year = 2022;

    private List<float> heartSeries;
    private List<float> cancerSeries;
    private List<float> strokeSeries;
    private List<float> suicideSeries;
    private List<float> diabetesSeries;
    private List<float> totalSeries = new List<float>();

    void Start()
    {
        heartSeries = loader.GetRecentDeaths("heart_disease");
        cancerSeries = loader.GetRecentDeaths("cancer");
        strokeSeries = loader.GetRecentDeaths("stroke");
        suicideSeries = loader.GetRecentDeaths("suicide");
        diabetesSeries = loader.GetRecentDeaths("diabetes");

        if (heartSeries == null || cancerSeries == null || strokeSeries == null || suicideSeries == null || diabetesSeries == null)
        {
            Debug.LogError("Failed to load time series data.");
            return;
        }

        StartCoroutine(ForecastLoop());
    }

    public void ReduceValues(float[] reduce_values)
    {
        heartSeries[heartSeries.Count - 1] = Mathf.Max(0.0f, heartSeries[heartSeries.Count - 1] - reduce_values[0]);
        cancerSeries[cancerSeries.Count - 1] = Mathf.Max(0.0f, cancerSeries[cancerSeries.Count - 1] - reduce_values[1]);
        strokeSeries[strokeSeries.Count - 1] = Mathf.Max(0.0f, strokeSeries[strokeSeries.Count - 1] - reduce_values[2]);
        suicideSeries[suicideSeries.Count - 1] = Mathf.Max(0.0f, suicideSeries[suicideSeries.Count - 1] - reduce_values[3]);
        diabetesSeries[diabetesSeries.Count - 1] = Mathf.Max(0.0f, diabetesSeries[diabetesSeries.Count - 1] - reduce_values[4]);
        // TODO MIGHT NEED TO UPDATE LINES FOR BETTER GAME FEEL
    }

    IEnumerator ForecastLoop()
    {
        UpdateTotalSeries();

        visualizer.UpdateLines(
                heartSeries,
                cancerSeries,
                strokeSeries,
                suicideSeries,
                diabetesSeries,
                totalSeries
            );
        yearText.text = "Year: " + year.ToString();

        while (true)
        {
            yield return StartCoroutine(FetchForecast("heart_disease", heartSeries));
            yield return StartCoroutine(FetchForecast("cancer", cancerSeries));
            yield return StartCoroutine(FetchForecast("stroke", strokeSeries));
            yield return StartCoroutine(FetchForecast("suicide", suicideSeries));
            yield return StartCoroutine(FetchForecast("diabetes", diabetesSeries));



            UpdateTotalSeries();

            visualizer.UpdateLines(
                heartSeries,
                cancerSeries,
                strokeSeries,
                suicideSeries,
                diabetesSeries,
                totalSeries
            );
            year += 1;
            yearText.text = "Year: " + year.ToString();

            yield return new WaitForSeconds(2f);
        }
    }

    void UpdateTotalSeries()
    {
        totalSeries.Clear();
        int count = heartSeries.Count;

        for (int i = 0; i < count; i++)
        {
            float total = 0f;
            if (i < heartSeries.Count) total += heartSeries[i];
            if (i < cancerSeries.Count) total += cancerSeries[i];
            if (i < strokeSeries.Count) total += strokeSeries[i];
            if (i < suicideSeries.Count) total += suicideSeries[i];
            if (i < diabetesSeries.Count) total += diabetesSeries[i];
            totalSeries.Add(total);
        }
    }

    IEnumerator FetchForecast(string diseaseName, List<float> series)
    {
        if (series == null || series.Count < 10)
        {
            Debug.LogWarning($"Not enough data for {diseaseName}, skipping forecast.");
            yield break;
        }

        var payload = new
        {
            data = series,
            model_name = diseaseName,
        };

        string json = JsonConvert.SerializeObject(payload);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"API call failed for {diseaseName}: {request.error}");
        }
        else
        {
            try
            {
                ForecastResponse response = JsonConvert.DeserializeObject<ForecastResponse>(request.downloadHandler.text);
                Debug.Log($"{diseaseName} forecast: {response.forecast}");

                // Compute stats
                float mean = 0f;
                foreach (float val in series) mean += val;
                mean /= series.Count;

                float std = 0f;
                foreach (float val in series) std += Mathf.Pow(val - mean, 2);
                std = Mathf.Sqrt(std / series.Count);

                float bias = Random.Range(-0.8f, 1.0f); // Mostly positive
                float noise = bias * std * 0.20f;
                float noisyForecast = Mathf.Max(0.0f, response.forecast + noise);

                series.Add(noisyForecast);


                if (series.Count > 20)
                    series.RemoveAt(0);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error parsing forecast for {diseaseName}: {ex.Message}");
            }
        }
    }

    [System.Serializable]
    public class ForecastResponse
    {
        public float forecast;
    }
}
