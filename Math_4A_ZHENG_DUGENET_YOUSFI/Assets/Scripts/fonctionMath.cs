using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public static class FonctionMath
{
    public static bool TestVisibilité(Vector2 normal, Vector2 line)
    {
        return Vector3.Dot(normal, line) > 0;
    }
    
    public static float Angle(Vector2 normal, Vector2 line)
    {
        return Mathf.Acos(Vector3.Dot(normal, line)/(normal.magnitude*line.magnitude));
    }

    public static bool isConvexe(Vector2 normal, Vector2 line)
    {
        return (Mathf.Acos(Vector3.Dot(normal, line)/(normal.magnitude*line.magnitude)) > Mathf.PI);
    }
    
    public static bool TestInteriorité(Vector2 normal1, Vector2 normal2, Vector2 normal3, Vector2 line)
    {
        if (TestVisibilité(normal1, line))
            return false;
        if (TestVisibilité(normal2, line))
            return false;
        if (TestVisibilité(normal3, line))
            return false;
        return true;
    }

    public static Vector3[] GETNormals(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        Vector3[] normals = new Vector3[3];
        normals[0] = new Vector3(point2.y - point1.y, -(point2.x - point1.x), 0).normalized;
        //normals[0].z = point1.z;
        normals[1] = new Vector3(point3.y - point2.y, -(point3.x - point2.x), 0).normalized;
        //normals[1].z = point1.z;
        normals[2] = new Vector3(point1.y - point3.y, -(point1.x - point3.x), 0).normalized;
        //normals[2].z = point1.z;
        return normals;
    }

    public static Vector3 GETCenterCircle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        float[][] m = new float[2][];
        for (int index = 0; index < 2; index++)
        {
            m[index] = new float[2];
        }

        float[] xy = new float[2];
        m[0][0] = point2.x - point1.x;
        m[1][0] = point3.x - point1.x;
        m[0][1] = point2.y - point1.y;
        m[1][1] = point3.y - point1.y;
        float detM = m[0][0] * m[1][1] - m[1][0] * m[0][1];
        if(Mathf.Abs(detM) < 0.0001f)
            return new Vector3();
        float[][] minv = new float[2][];
        for (int index = 0; index < 2; index++)
        {
            minv[index] = new float[2];
        }
        minv[0][0] = m[1][1] / detM;
        minv[1][0] = -m[1][0] / detM;
        minv[0][1] = -m[0][1] / detM;
        minv[1][1] = m[0][0] / detM;
        xy[0] = (point2.x * point2.x + point2.y * point2.y - point1.x * point1.x - point1.y * point1.y)/2;
        xy[1] = (point3.x * point3.x + point3.y * point3.y - point1.x * point1.x - point1.y * point1.y)/2;
        Vector3 res = new Vector3(xy[0]*minv[0][0] + xy[1]*minv[0][1], xy[0]*minv[1][0] + xy[1]*minv[1][1]);
        return res;
    }

    public static List<Vector2> GetJarvis(List<Vector2> pointList)
    {
        List<Vector2> jarvis = new List<Vector2>();
        
        //On trouve le point le plus à gauche et on le retire de la liste
        Vector2 startPoint = pointList.First();
        foreach (var point in pointList)
        {
            if (point.x < startPoint.x)
                startPoint = point;
        }
        jarvis.Add(startPoint);
        
        Vector2 curPoint = startPoint;

        do
        {
            Vector2 curNormal = new Vector2(curPoint.y, -curPoint.x);
            Vector2 nextPoint = pointList.First();
            float smallestAngle = Mathf.PI;
            foreach (var point in pointList)
            {
                if (point != curPoint)
                {
                    //On compare les angles et on stock le plus petit
                    float curAngle = Angle(curNormal, point - curPoint);
                    if (curAngle < smallestAngle)
                    {
                        smallestAngle = curAngle;
                        nextPoint = point;
                    }
                }
            }
            curPoint = nextPoint;
            pointList.Remove(curPoint);
            jarvis.Add(curPoint);
        } while (curPoint != startPoint);

        return jarvis;
    }

    public static List<Vector2> GetGrahamScan(List<Vector2> pointList)
    {
        List<Vector2> grahamScan = new List<Vector2>();

        // Trouver le point le plus bas (et le plus à gauche) comme point de départ
        Vector2 startPoint = pointList.OrderBy(p => p.y).ThenBy(p => p.x).First();

        // Trier les points par angle par rapport au point de départ
        List<Vector2> sortedPoints = pointList.OrderBy(p => Mathf.Atan2(p.y - startPoint.y, p.x - startPoint.x)).ToList();

        grahamScan.Add(startPoint);
        grahamScan.Add(sortedPoints[0]);

        for (int i = 1; i < sortedPoints.Count; i++)
        {
            while (grahamScan.Count > 1 && !IsConvex(grahamScan[^2], grahamScan.Last(), sortedPoints[i]))
            {
                grahamScan.RemoveAt(grahamScan.Count - 1);
            }
            grahamScan.Add(sortedPoints[i]);
        }

        return grahamScan;
    }
    
    private static bool IsConvex(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float crossProduct = (p1.x - p0.x) * (p2.y - p0.y) - (p2.x - p0.x) * (p1.y - p0.y);
        return crossProduct >= 0;
    }
}
