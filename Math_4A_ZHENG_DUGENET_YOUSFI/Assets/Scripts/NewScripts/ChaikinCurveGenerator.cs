using System.Collections.Generic;
using UnityEngine;

public class ChaikinCurveGenerator : MonoBehaviour
{
    public List<Vector3> ControlPoints;
    public int Iterations = 4;

    void Start()
    {
        List<Vector3> points = new List<Vector3>(ControlPoints);
        for (int i = 0; i < Iterations; i++)
        {
            points = ChaikinSubdivision(points);
        }
        DrawCurve(points);
    }

    List<Vector3> ChaikinSubdivision(List<Vector3> points)
    {
        List<Vector3> newPoints = new List<Vector3>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 P0 = points[i];
            Vector3 P1 = points[i + 1];
            Vector3 Q = (3 * P0 + P1) / 4;
            Vector3 R = (P0 + 3 * P1) / 4;
            newPoints.Add(Q);
            newPoints.Add(R);
        }
        return newPoints;
    }

    void DrawCurve(List<Vector3> points)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Debug.DrawLine(points[i], points[i + 1], Color.red, 100f);
        }
    }
}
    