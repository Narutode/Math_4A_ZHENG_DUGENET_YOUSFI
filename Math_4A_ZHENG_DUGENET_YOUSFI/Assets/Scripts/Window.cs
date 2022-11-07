using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window
{
    public List<Vector2> Sommets = new List<Vector2>();
    public List<Vector2> Normals = new List<Vector2>();

    public void setNormal() {
        for (int i = 0; i < Sommets.Count; i++)
        {
            Vector2 s1 = Sommets[i];
            Vector2 s2 = Sommets[(i+1)%Sommets.Count];

            Vector2 n = new Vector2(s2.y - s1.y, -(s2.x - s1.x));
            Normals.Add(n);
        }
    }

    public void checkPolygonInside(Polygon poly)
    {
        
    }
}
