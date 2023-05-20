using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spline
{
    public float step = 0.1f;
    public List<Vector3> pList;

    public Spline()
    {
        pList = new List<Vector3>();
    }

    public List<Vector3> Casteljau()
    {
        List<Vector3> bezierPoints = new List<Vector3>();
        List<Vector3> pCopy = new List<Vector3>(pList);
        for (float t = 0; t <= 1; t += step)
        {
            for (int j = 0; j < pList.Count; j++)
            {
                for (int i = 0; i < pList.Count - j - 1; i++)
                {
                    pCopy[i] = (1 - t) * pCopy[i] + t * pCopy[i + 1];
                }
            }
            bezierPoints.Add(pCopy[0]);
        }
        return bezierPoints;
    }
}
