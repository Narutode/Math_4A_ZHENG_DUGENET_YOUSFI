using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spline
{
    public float step = 0.1f;
    //public LinkedList<Vector3> pList;
    public LinkedList<GameObject> pgoList;
    public float Angle = 0;
    public Vector2 pointT = new Vector2(0, 0);
    public Vector2 pointS = new Vector2(0, 0);
    public Vector2 pointSH = new Vector2(0, 0);

    public Color color;
    
    public Spline()
    {
        //pList = new LinkedList<Vector3>();
        pgoList = new LinkedList<GameObject>();
        color = Random.ColorHSV();
    }

    public List<Vector3> Casteljau()
    {
        List<Vector3> bezierPoints = new List<Vector3>();
        List<Vector3> pCopy = new List<Vector3>();
        
        foreach (GameObject p in pgoList)
        {
            Vector3 point = p.transform.position;
            Vector3 newP = new Vector3(point.x, point.y, point.z);
            /*
            //Translation
            newP.Set(newP.x + pointT.x, newP.y + pointT.y, newP.z);
            //Rotation
            newP.Set(newP.x*Mathf.Cos(Angle)-newP.y*Mathf.Sin(Angle),
                newP.x*Mathf.Sin(Angle)+newP.x*Mathf.Cos(Angle),newP.z);
            //Scaling
            newP.Set(newP.x + pointS.x * newP.x, newP.y + pointS.y * newP.y, newP.z);
            //Shearing
            newP.Set(newP.x + pointSH.x * newP.y, newP.y + pointSH.y * newP.x, newP.z);
            */
            pCopy.Add(newP);
            //pCopy.Add(new Vector3(p.x,p.y));
        }
        
        for (float t = 0; t <= 1; t += step)
        {
            for (int j = 0; j < pgoList.Count; j++)
            {
                for (int i = 0; i < pgoList.Count - j - 1; i++)
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
        foreach (var p in pgoList)
        {
            if (p != null)
            {
                Vector3 point = p.transform.position;
                Vector3 newP = new Vector3(point.x, point.y, point.z);
                //Translation
                newP.Set(newP.x + pointT.x, newP.y + pointT.y, newP.z);
                //Rotation
                newP.Set(newP.x * Mathf.Cos(Angle) - newP.y * Mathf.Sin(Angle),
                    newP.x * Mathf.Sin(Angle) + newP.y * Mathf.Cos(Angle), newP.z);
                //Scaling
                newP.Set(newP.x + pointS.x * newP.x, newP.y + pointS.y * newP.y, newP.z);
                //Shearing
                newP.Set(newP.x + pointSH.x * newP.y, newP.y + pointSH.y * newP.x, newP.z);

                pCopy.Add(newP);
            }
        }
        int n = pgoList.Count()-1;
        for (float t = 0; t < 1; t += step)
        {
            Vector3 res = new Vector3(0,0,  pgoList.First().transform.position.z);
            for (int j = 0; j < pgoList.Count(); j++)
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
        Vector3 curPoint = pgoList.First().transform.position;
        foreach (var p in pgoList)
        {
            if (p.transform.position.x < curPoint.x)
                curPoint = p.transform.position;
        }

        do
        {
            list.Add(curPoint);
            Vector3 bestP = pgoList.First().transform.position;
            foreach (var p in pgoList)
            {
                if (bestP == p.transform.position || Vector2.Dot(new Vector2(curPoint.x - bestP.x, curPoint.y - bestP.y),
                    new Vector2(curPoint.x - p.transform.position.x, curPoint.y - p.transform.position.y)) < 0)
                {
                    bestP = p.transform.position;
                }
            }
            curPoint = bestP;
        } while (list.Last() != list.First());
        return list;
    }
}
