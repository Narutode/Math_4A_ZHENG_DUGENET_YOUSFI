using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class fonctionMath
{
    public static bool testVisibilité(Vector2 normal, Vector2 line)
    {
        return Vector3.Dot(normal, line) > 0;
    }
    
    public static float angle(Vector2 normal, Vector2 line)
    {
        return Mathf.Acos(Vector3.Dot(normal, line)/(normal.magnitude*line.magnitude));
    }

    public static bool testInteriorité(Vector2 normal1, Vector2 normal2, Vector2 normal3, Vector2 line)
    {
        if (testVisibilité(normal1, line))
            return false;
        if (testVisibilité(normal2, line))
            return false;
        if (testVisibilité(normal3, line))
            return false;
        return true;
    }

    public static Vector3[] getNormals(Vector3 point1, Vector3 point2, Vector3 point3)
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

    public static Vector3 getCenterCircle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        float[,] M = new float[2, 2];
        float[] XY = new float[2];
        M[0, 0] = point2.x - point1.x;
        M[1, 0] = point3.x - point2.x;
        M[0, 1] = point2.y - point1.y;
        M[1, 1] = point3.y - point2.y;
        float detM = M[0, 0] * M[1, 1] - M[1, 0] * M[0, 1];
        if(Mathf.Abs(detM) < 0.0001f)
            return new Vector3();
        float[,] Minv = new float[2, 2];
        Minv[0, 0] = M[1, 1] / detM;
        Minv[1, 0] = -M[1, 0] / detM;
        Minv[0, 1] = -M[0, 1] / detM;
        Minv[1, 1] = M[0, 0] / detM;
        XY[0] = point2.x * point2.x + point2.y * point2.y - point1.x * point1.x - point1.y * point1.y;
        XY[1] = point3.x * point3.x + point3.y * point3.y - point2.x * point2.x - point2.y * point2.y;
        Vector3 res = new Vector3(XY[0]*Minv[0, 0] + XY[1]*Minv[0, 1], XY[0]*Minv[1, 0] + XY[1]*Minv[1, 1]);
        return res;
    }
}
