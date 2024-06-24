using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ChaikinCurveGenerator : MonoBehaviour
{
    public List<Vector3> ControlPoints;
    public int Iterations = 4;
    [Range(0.0f, 1.0f)]
    public float QPercentage = 0.75f;
    [Range(0.0f, 1.0f)]
    public float RPercentage = 0.75f;

    private CoonsLineConstrutor lineConstructor;

    LineRenderer line;
    public Material mat;
    public GameObject pointGO;

    public bool drawChaikin = false;

    private void Start()
    {
        lineConstructor = gameObject.GetComponent<CoonsLineConstrutor>();
        
        GameObject newGO = new GameObject();
        newGO.transform.SetParent(lineConstructor.parent.transform);
        newGO.name = "sides";
        line = newGO.AddComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 0;
        line.material = mat;
    }

    public void Update()
    {
        if (drawChaikin)
            Subdivide();
    }

    public void Subdivide()
    {
        List<Vector3> points = new List<Vector3>(ControlPoints);
        for (int i = 0; i < Iterations; i++)
        {
            points = ChaikinSubdivision(points, QPercentage, RPercentage);
        }
        DrawCurve(points);
    }

    List<Vector3> ChaikinSubdivision(List<Vector3> points , float qPercent, float rPercent)
    {
        List<Vector3> newPoints = new List<Vector3>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 P0 = points[i];
            Vector3 P1 = points[i + 1];
            Vector3 Q = (qPercent * P0 + (1 - qPercent) * P1);
            Vector3 R = ((1 - rPercent) * P0 + rPercent * P1);
            newPoints.Add(Q);
            newPoints.Add(R);
        }
        return newPoints;
    }

    void DrawCurve(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if(i+1 > line.positionCount)
                line.positionCount++;
            line.SetPosition(i, points[i]);
        }
    }
}
    