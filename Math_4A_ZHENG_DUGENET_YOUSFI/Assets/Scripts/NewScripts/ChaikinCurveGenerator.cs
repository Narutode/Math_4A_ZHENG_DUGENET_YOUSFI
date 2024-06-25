using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;

public class ChaikinCurveGenerator : MonoBehaviour
{
    public List<Vector3>[] ControlPoints;
    public int Iterations = 4;
    [Range(0.0f, 1.0f)]
    public float U = 0.25f;
    [Range(0.0f, 1.0f)]
    public float V = 0.75f;
    private CoonsLineConstrutor lineConstructor;

    LineRenderer[] lines = new LineRenderer[4];
    public Material mat;
    public GameObject pointGO;

    public bool drawChaikin = false;

    private void Start()
    {
        lineConstructor = gameObject.GetComponent<CoonsLineConstrutor>();
        for (int i = 0; i < 4; i++)
        {
            GameObject newGO = new GameObject();
            newGO.transform.SetParent(lineConstructor.parent.transform);
            newGO.name = "sides";
            var line = newGO.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.positionCount = 0;
            line.material = mat;
            lines[i] = line;
        }
    }

    public void Update()
    {
        if (drawChaikin)
            Subdivide();
    }

    public List<Vector3>[] Subdivide()
    {
        List<Vector3>[] sub = new List<Vector3>[4];
        for (int i = 0; i < 4; i++)
        {
            List<Vector3> points = new List<Vector3>(ControlPoints[i]);
            for (int k = 0; k < Iterations; k++)
            {
                points = ChaikinSubdivision(points, 1 - U, V);
            }
            DrawCurve(points, i);
            sub[i] = points;
        }
        return sub;
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

    void DrawCurve(List<Vector3> points, int index)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if(i+1 > lines[index].positionCount)
                lines[index].positionCount++;
            lines[index].SetPosition(i, points[i]);
        }
    }
}
    