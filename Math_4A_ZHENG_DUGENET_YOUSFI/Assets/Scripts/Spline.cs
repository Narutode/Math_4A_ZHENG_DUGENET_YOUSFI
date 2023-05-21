using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Spline
{
    public float step = 0.1f;
    public List<Vector3> pList;
    public LinkedList<GameObject> pgoList;
    public float Angle = 0;
    public Vector2 pointT = new Vector2(0, 0);
    public Vector2 pointS = new Vector2(1, 1);
    public Vector2 pointSH = new Vector2(0, 0);

    public Spline()
    {
        pList = new List<Vector3>();
        pgoList = new LinkedList<GameObject>();
    }

    public List<Vector3> Casteljau()
    {
        List<Vector3> bezierPoints = new List<Vector3>();
        List<Vector3> pCopy = new List<Vector3>();
        foreach (var p in pList)
        {
            Vector3 newP = new Vector3(p.x, p.y, p.z);
            //Translation
            newP.Set(newP.x + pointT.x, newP.y + pointT.y, newP.z);
            //Rotation
            newP.Set(newP.x*Mathf.Cos(Angle)-p.y*Mathf.Sin(Angle),
                newP.x*Mathf.Sin(Angle)+p.y*Mathf.Cos(Angle),p.z);
            //Scaling
            newP.Set(newP.x + pointSH.x * newP.y, newP.y + pointSH.y * newP.x, newP.z);
            //Shearing
            
            pCopy.Add(newP);
            //pCopy.Add(new Vector3(p.x,p.y));

        }
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

    public List<Vector3> Jarvis()
    {
        List<Vector3> list = new List<Vector3>();
        Vector3 curPoint = pList.First();
        foreach (var p in pList)
        {
            if (p.x < curPoint.x)
                curPoint = p;
        }

        do
        {
            list.Add(curPoint);
            Vector3 bestP = pList.First();
            foreach (var p in pList)
            {
                if (bestP == p || Vector2.Dot(new Vector2(curPoint.x - bestP.x, curPoint.y - bestP.y),
                    new Vector2(curPoint.x - p.x, curPoint.y - p.y)) < 0)
                {
                    bestP = p;
                }
            }
            curPoint = bestP;
        } while (list.Last() != list.First());
        return list;
    }
}
