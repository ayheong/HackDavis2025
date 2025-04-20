using System.Collections.Generic;
using UnityEngine;

public class ForecastVisualizer : MonoBehaviour
{
    [Header("Graph UI Area")]
    [SerializeField] private RectTransform graphArea;

    [Header("Line Renderers")]
    [SerializeField] private LineRenderer heartLine;
    [SerializeField] private LineRenderer cancerLine;

    public void UpdateLines(List<float> heartSeries, List<float> cancerSeries)
    {
        if (heartSeries != null) DrawLine(heartLine, heartSeries);
        if (cancerSeries != null) DrawLine(cancerLine, cancerSeries);
    }

    void DrawLine(LineRenderer line, List<float> values)
    {
        if (values == null || values.Count == 0 || graphArea == null) return;

        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        float xSpacing = width / Mathf.Max(values.Count - 1, 1);

        float minY = Mathf.Min(values.ToArray());
        float maxY = Mathf.Max(values.ToArray());
        float yRange = Mathf.Max(maxY - minY, 1e-5f); // Prevent div by 0

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
    }
}
