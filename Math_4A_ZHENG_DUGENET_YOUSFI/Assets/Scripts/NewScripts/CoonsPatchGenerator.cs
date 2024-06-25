using System.Collections.Generic;
using UnityEngine;

public class CoonsPatchGenerator : MonoBehaviour
{
    public List<Vector3> CurveU0;
    public List<Vector3> CurveU1;
    public List<Vector3> CurveV0;
    public List<Vector3> CurveV1;
    public int ResolutionU;
    public int ResolutionV;
    public GameObject parent;
    public GameObject pointGO;
    private Vector3[,] points;

    void Start()
    {
        //GenerateCoonsPatch();
    }
    public void GenerateCoonsPatch()
    {
        points = new Vector3[CurveU0.Count,CurveV0.Count];
        ResolutionU = CurveU0.Count;
        ResolutionV = CurveV0.Count;
        for (int i = 0; i < ResolutionU; i++)
        {
            for (int j = 0; j < ResolutionV; j++)
            {
                float u = i / (float)(ResolutionU - 1);
                float v = j / (float)(ResolutionV - 1);
                Vector3 point = CoonsPatch(u, v);
                points[i, j] = point;
            }
        }

        for (int i = 0; i < ResolutionU; i++)
        {
            GameObject newGO = new GameObject();
            newGO.transform.SetParent(parent.transform);
            newGO.name = "coons " + i;
            var line = newGO.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.positionCount = ResolutionV;
            for (int j = 0; j < ResolutionV; j++)
            {
                line.SetPosition(j, points[i, j]);
            }
        }
        for (int j = 0; j < ResolutionV; j++)
        {
            GameObject newGO = new GameObject();
            newGO.transform.SetParent(parent.transform);
            newGO.name = "coons " + j;
            var line = newGO.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.positionCount = ResolutionU;
            for (int i = 0; i < ResolutionU; i++)
            {
                line.SetPosition(i, points[i, j]);
            }
        }
    }

    public Vector3 CoonsPatch(float u, float v)
    {
        Vector3 term1 = (1 - u) * CurveV0[(int)(v * (CurveV0.Count - 1))] + u * CurveV1[(int)(v * (CurveV1.Count - 1))];
        Vector3 term2 = (1 - v) * CurveU0[(int)(u * (CurveU0.Count - 1))] + v * CurveU1[(int)(u * (CurveU1.Count - 1))];
        Vector3 term3 = (1 - u) * (1 - v) * CurveU0[0] + u * (1 - v) * CurveU0[CurveU0.Count - 1] + (1 - u) * v * CurveU1[0] + u * v * CurveU1[CurveU1.Count - 1];
        return term1 + term2 - term3;
    }
}