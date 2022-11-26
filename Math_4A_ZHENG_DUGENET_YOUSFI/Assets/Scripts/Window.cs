using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window
{
    //fenètrage
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

    public bool CyrusBeck(ref float x1, ref float y1, ref float x2, ref float y2)
    {
        int nbSomWin = Sommets.Count;
        float t, tinf, tsup;
        float DX, DY, WN, DN;
        Vector2 C;
        int i;
        tinf = float.NegativeInfinity;
        tsup = float.PositiveInfinity;

        DX = x2 - x1;
        DY = y2 - y1;
        
        for (i = 0; i < nbSomWin; i++)
        {
            C = Sommets[i];
            DN = DX * Normals[i].x + DY * Normals[i].y;
            WN = (x1 - C.x) * Normals[i].x + (y1 - C.y) * Normals[i].y;
            if (DN == 0)
                return (WN >= 0);
            t = (-WN) / DN;
            if (DN > 0)
            {
                if (t > tinf)
                    tinf = t;
            }
            else
            {
                if (t < tsup)
                    tsup = t;
            }
        }
        if (tinf < tsup)
        {
            if (tinf < 0 && tsup > 1)
            {
                return true;
            }

            if (tinf > 1 || tsup < 0)
            {
                return false;
            }

            if (tinf < 0)
            {
                tinf = 0;
            }
            else if (tsup > 1)
            {
                tsup = 1;
            }

            x2 = x1 + DX * tsup;
            y2 = y1 + DY * tsup;
            x1 = x1 + DX * tinf;
            y1 = y1 + DY * tinf;
            return true;
        }
        return false;
    }
}
