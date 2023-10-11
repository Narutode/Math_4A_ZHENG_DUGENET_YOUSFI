using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fonctionMath
{
    bool testVisibilité(Vector2 normal, Vector2 line)
    {
        return Vector3.Dot(normal, line) > 0;
    }
    
    float angle(Vector2 normal, Vector2 line)
    {
        return Mathf.Acos(Vector3.Dot(normal, line)/(normal.magnitude*line.magnitude));
    }

    bool testInteriorité(Vector2 normal1, Vector2 normal2, Vector2 normal3, Vector2 line)
    {
        if (testVisibilité(normal1, line))
            return false;
        if (testVisibilité(normal2, line))
            return false;
        if (testVisibilité(normal3, line))
            return false;
        return true;
    }

    Vector2[] getNormals(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        Vector2[] normals = new Vector2[3];
        normals[0] = new Vector2(point2.y - point1.y, -point2.x - point1.x);
        normals[1] = new Vector2(point3.y - point2.y, -point3.x - point2.x);
        normals[2] = new Vector2(point1.y - point3.y, -point1.x - point3.x);
        return normals;
    }

    Vector3 getCenterCircle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        return new Vector3();
    }
}
