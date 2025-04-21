using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ForecastVisualizer : MonoBehaviour
{
    [Header("Graph UI Areas")]
    [SerializeField] private RectTransform heartGraphArea;
    [SerializeField] private RectTransform cancerGraphArea;
    [SerializeField] private RectTransform strokeGraphArea;
    [SerializeField] private RectTransform suicideGraphArea;
    [SerializeField] private RectTransform diabetesGraphArea;
    [SerializeField] private RectTransform totalGraphArea; 

    [Header("Line Renderers")]
    [SerializeField] private LineRenderer heartLine;
    [SerializeField] private LineRenderer cancerLine;
    [SerializeField] private LineRenderer strokeLine;
    [SerializeField] private LineRenderer suicideLine;
    [SerializeField] private LineRenderer diabetesLine;
    [SerializeField] private LineRenderer totalLine;

    [Header("Max Text Boxes")]
    [SerializeField] private TextMeshProUGUI heartMax;
    [SerializeField] private TextMeshProUGUI cancerMax;
    [SerializeField] private TextMeshProUGUI strokeMax;
    [SerializeField] private TextMeshProUGUI suicideMax;
    [SerializeField] private TextMeshProUGUI diabetesMax;
    [SerializeField] private TextMeshProUGUI totalMax;

    [Header("Min Text Boxes")]
    [SerializeField] private TextMeshProUGUI heartMin;
    [SerializeField] private TextMeshProUGUI cancerMin;
    [SerializeField] private TextMeshProUGUI strokeMin;
    [SerializeField] private TextMeshProUGUI suicideMin;
    [SerializeField] private TextMeshProUGUI diabetesMin;
    [SerializeField] private TextMeshProUGUI totalMin;

    public void UpdateLines(
        List<float> heartSeries,
        List<float> cancerSeries,
        List<float> strokeSeries,
        List<float> suicideSeries,
        List<float> diabetesSeries,
        List<float> totalSeries 
    )
    {
        if (heartSeries != null) DrawLine(heartGraphArea, heartLine, heartSeries, heartMax, heartMin);
        if (cancerSeries != null) DrawLine(cancerGraphArea, cancerLine, cancerSeries, cancerMax, cancerMin);
        if (strokeSeries != null) DrawLine(strokeGraphArea, strokeLine, strokeSeries, strokeMin, strokeMax);
        if (suicideSeries != null) DrawLine(suicideGraphArea, suicideLine, suicideSeries, suicideMax, suicideMin);
        if (diabetesSeries != null) DrawLine(diabetesGraphArea, diabetesLine, diabetesSeries, diabetesMax, diabetesMin);
        if (totalSeries != null) DrawLine(totalGraphArea, totalLine, totalSeries, totalMax, totalMin); 
    }

    void DrawLine(RectTransform graphArea, LineRenderer line, List<float> values, TextMeshProUGUI maxText, TextMeshProUGUI minText)
    {
        if (values == null || values.Count == 0 || graphArea == null) return;

        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        float xSpacing = width / Mathf.Max(values.Count - 1, 1);

        float minY = Mathf.Min(values.ToArray());
        float maxY = Mathf.Max(values.ToArray());
        float yRange = Mathf.Max(maxY - minY, 1e-5f); 

        Vector3[] positions = new Vector3[values.Count];

        for (int i = 0; i < values.Count; i++)
        {
            float x = i * xSpacing;
            float normalizedY = (values[i] - minY) / yRange;
            float y = normalizedY * height;
            positions[i] = new Vector3(x, y, 0f);
        }

        line.useWorldSpace = false;
        line.positionCount = positions.Length;
        line.SetPositions(positions);
        //maxText.text = "MAX: " + ShopManager.AddCommas(maxY);
        maxText.text = "MAX: " + maxY.ToString("N0");
        //minText.text = "MIN: " + ShopManager.AddCommas(minY);
        minText.text = "MIN: " + minY.ToString("N0");
    }
}
