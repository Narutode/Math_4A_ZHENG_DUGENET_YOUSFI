using System;
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

    public List<Vector2> CyrusBeck(Polygon poly)
    {
        bool modified = false;
        Vector2 ps1 = poly.Sommets[0];
        Vector2 ps2;
        List<Vector2> SommetInterieurs = new List<Vector2>();
        
        int nbSomWin = Sommets.Count;
        for (int k = 0; k < poly.Sommets.Count; k++)
        {
            if (!modified)
            {
                ps1 = poly.Sommets[k];
            }
            else
            {
                modified = false;
            }

            ps2 = poly.Sommets[(k+1)%poly.Sommets.Count];

            float t, tinf, tsup;
            float DX, DY, WN, DN;
            Vector2 C;
            int i, Nbseg;

            tinf = float.MinValue;
            tsup = float.MaxValue;

            DX = ps2.x - ps1.x;
            DY = ps2.y - ps1.y;

            Nbseg = nbSomWin - 1;

            for (i = 0; i < Nbseg; i++)
            {
                C = Sommets[i];
                DN = DX * Normals[i].x + DY * Normals[i].y;
                WN = (ps1.x - C.x) * Normals[i].x + (ps1.y - C.y) * Normals[i].y;
                if (DN == 0)
                    continue;
                t = -(WN / DN);
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
                    SommetInterieurs.Add(ps1);
                    //SommetInterieurs.Add(ps2);
                }
                else
                {
                    if (tinf < 1 && tsup > 0)
                    {
                        if (tinf < 0)
                        {
                            tinf = 0;
                        }

                        if (tsup > 1)
                        {
                            tsup = 1;
                        }

                        Vector2 newPs1 = new Vector2(ps1.x + DX * tinf,ps1.y + DY * tinf);
                        Vector2 newPs2 = new Vector2(ps1.x + DX * tsup,ps1.y + DY * tsup);
                        ps1 = newPs2;
                        modified = true;
                        SommetInterieurs.Add(newPs1);
                        //SommetInterieurs.Add(newPs2);
                    }
                }
            }
        }
        return SommetInterieurs;
    }
}
