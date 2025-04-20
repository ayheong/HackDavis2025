using System.Collections.Generic;
using UnityEngine;

public class ForecastVisualizer : MonoBehaviour
{
    [SerializeField] private LineRenderer heartLine;
    [SerializeField] private LineRenderer cancerLine;
    [SerializeField] private float xSpacing = 1f;
    [SerializeField] private float yScale = 0.01f;

    public void UpdateLines(List<float> heartSeries, List<float> cancerSeries)
    {
        DrawLine(heartLine, heartSeries);
        DrawLine(cancerLine, cancerSeries);
    }

    void DrawLine(LineRenderer line, List<float> values)
    {
        line.positionCount = values.Count;

        for (int i = 0; i < values.Count; i++)
        {
            line.SetPosition(i, new Vector3(i * xSpacing, values[i] * yScale, 0));
        }
    }
}
