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
    public Vector2 pointS = new Vector2(0, 0);
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
    
    public List<Vector3> PascalMethod() {
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
            newP.Set(newP.x + pointS.x * newP.x, newP.y + pointS.y * newP.y, newP.z);
            //Shearing
            newP.Set(newP.x + pointSH.x * newP.y, newP.y + pointSH.y * newP.x, newP.z);

            pCopy.Add(newP);
        }
        int n = pList.Count()-1;
        for (float t = 0; t < 1; t += step)
        {
            Vector3 res = new Vector3(0,0,  pList.First().z);
            for (int j = 0; j < pList.Count(); j++)
            {
                Vector3 v = pCopy[j] * (chose(n,j)*powI(1-t,n-j)*powI(t,j)); 
                res.Set(res.x + v.x, res.y + v.y, res.z);
            }
            bezierPoints.Add(res);
        }
        bezierPoints.Add(pCopy.Last());
        return bezierPoints;
    }

    float powI(float a, float b)
    {
        if (Math.Abs(b) < 0.00001)
            return 1;
        if (Math.Abs(a) < 0.00001)
            return 0;
        return Mathf.Pow(a, b);
    }
    
    int chose(int n, int k)
    {
        int res = 1;
        for (int i = 1; i <= k; i++)
        {
            res *= n;
            n--;
            res /= i;
        }
        return res;
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
