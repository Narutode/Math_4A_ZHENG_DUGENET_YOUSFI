using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Window
{
    //fenètrage
    public List<Vector2> Sommets = new List<Vector2>();
    public List<Vector2> Normals = new List<Vector2>();

    public void setNormal() {
        //Calcul des normals
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

    public Boolean coupe(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4)
    {
        float a = P2.x - P1.x;
        float b = P3.x - P4.x;
        float c = P2.y - P1.y;
        float d = P3.y - P4.y;
        float det = a * d - b * c;
        return det != 0;
    }
    
    public (Vector2,Boolean) intersect(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4)
    {
        float a = P2.x - P1.x;
        float b = P3.x - P4.x;
        float c = P2.y - P1.y;
        float d = P3.y - P4.y;

        float e = P3.x - P1.x;
        float f = P3.y - P1.y;
        
        float det = a * d - b * c;

        if(det == 0)
            return (Vector2.zero,false);
        
        //float invA = d / det;
        //float invB = -b / det;
        float invC = -c / det;
        float invD = a / det;

        float s = invC * e + invD * f;
        
        return (P3 + (P4 - P3) * s,true);
    }

    public Boolean visible(Vector2 P1, Vector2 P2, Vector2 n)
    {
        Vector2 dir = new Vector2(P1.x - P2.x, P1.y - P2.y);
        double prodScalaire = Vector2.Dot(n, dir);
        return prodScalaire <= 0;
    }
    
    int mod(int x, int m) {
        int r = x%m;
        return r<0 ? r+m : r;
    }

    public List<Vector2> SutherlandHodgman(List<Vector2> poly)
    {
        //List<Vector2> allStep = new List<Vector2>();

        (Vector2, Boolean) tmpI;
        Vector2 F0,I,C0,C1,F1;
        List<Vector2> newPoly = new List<Vector2>();
        newPoly.AddRange(poly);
        for (int i = 0; i < Sommets.Count+1; i++)
        {
            C0 = Sommets[i% Sommets.Count];
            C1 = Sommets[(i + 1) % Sommets.Count];
            poly.Clear();
            poly.AddRange(newPoly);
            newPoly.Clear();
            for (int j = 0; j < poly.Count; j++)
            {
                F1 = poly[j];
                F0 = poly[mod(j-1,poly.Count)];
                tmpI = intersect(F0, F1, C0, C1);
                if (visible(C0, F1, Normals[i% Sommets.Count]))
                {
                    if (!visible(C0, F0, Normals[i% Sommets.Count]))
                    {
                        if(tmpI.Item2)
                            newPoly.Add(tmpI.Item1);
                    }
                    newPoly.Add(F1);
                }
                else if (visible(C0, F0, Normals[i% Sommets.Count]))
                {
                    if(tmpI.Item2)
                        newPoly.Add(tmpI.Item1);
                }
            }
            //allStep.AddRange(newPoly);
        }
        return poly;
    }
}
