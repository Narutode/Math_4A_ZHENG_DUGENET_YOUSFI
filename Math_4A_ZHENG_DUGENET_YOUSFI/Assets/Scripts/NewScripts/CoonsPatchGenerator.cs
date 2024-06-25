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

    void Start()
    {
        GenerateCoonsPatch();
    }

    public void GenerateCoonsPatch()
    {
        ResolutionU = CurveU0.Count;
        ResolutionV = CurveV0.Count;
        for (int i = 0; i < ResolutionU; i++)
        {
            GameObject newGO = new GameObject();
            newGO.transform.SetParent(parent.transform);
            newGO.name = "coons " + i;
            var line = newGO.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.positionCount = ResolutionV;
            //line.material = mat;
            for (int j = 0; j < ResolutionV; j++)
            {
                float u = i / (float)(ResolutionU - 1);
                float v = j / (float)(ResolutionV - 1);
                Vector3 point = CoonsPatch(u, v);
                line.SetPosition(j, point);
                //Debug.DrawLine(point, point + Vector3.up * 0.1f, Color.blue, 100f);
            }
        }
    }

    public Vector3 CoonsPatch(float u, float v)
    {
        Vector3 term1 = (1 - u) * CurveV0[(int)(v * (CurveV0.Count - 1))] + u * CurveV1[(int)(v * (CurveV1.Count - 1))];
        Vector3 term2 = (1 - v) * CurveU0[(int)(u * (CurveU0.Count - 1))] + v * CurveU1[(int)(u * (CurveU1.Count - 1))];
        Vector3 term3 = (1 - u) * (1 - v) * CurveV0[0] + u * (1 - v) * CurveV1[0] + (1 - u) * v * CurveU0[CurveU0.Count - 1] + u * v * CurveU1[CurveU1.Count - 1];
        Vector3 result = term1 + term2 - term3;
        return result;
    }
}
