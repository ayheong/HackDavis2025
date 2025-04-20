using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ForecastManager : MonoBehaviour
{
    [Header("Dependencies")]
    public TimeSeriesLoader loader;

    [Header("API Settings")]
    [SerializeField] private string apiUrl = "https://HackDavis2025-project-api.onrender.com/forecast";
    [SerializeField] private ForecastVisualizer visualizer;


    private List<float> heartSeries;
    private List<float> cancerSeries;
    private List<float> strokeSeries;
    private List<float> suicideSeries;
    private List<float> diabetesSeries;

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

    IEnumerator ForecastLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FetchForecast("heart_disease", heartSeries));
            yield return StartCoroutine(FetchForecast("cancer", cancerSeries));
            yield return StartCoroutine(FetchForecast("stroke", strokeSeries));
            yield return StartCoroutine(FetchForecast("suicide", suicideSeries));
            yield return StartCoroutine(FetchForecast("diabetes", diabetesSeries));
            visualizer.UpdateLines(heartSeries, cancerSeries, strokeSeries, suicideSeries, diabetesSeries);
            yield return new WaitForSeconds(5f);
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
            order = new[] { 1, 1, 1 }
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

                series.Add(response.forecast);
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
