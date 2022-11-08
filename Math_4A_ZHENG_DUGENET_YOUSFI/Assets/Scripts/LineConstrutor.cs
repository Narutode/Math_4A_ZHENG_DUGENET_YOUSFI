using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class LineConstrutor : MonoBehaviour
{
    GameObject Sommet;
    public LineRenderer Line;
    public LineRenderer Line2;
    public LineRenderer Line3;

    private int tempcount ;
    private int tempcount2;
    public Camera cam;
    public Window win;
    public Polygon poly;
    private bool Isfenetre = true;

    private float nearClipPlaneWorldPoint = 0;
    // Start is called before the first frame update
    void Start()
    {
        win = new Window();
        poly = new Polygon();
    }

    // Update is called once per frame
    void Update()
    {
        if (Isfenetre)
        {
            if (Input.GetMouseButtonDown(0)) // click gauche
            {
                Vector3 point = new Vector3();

                Vector2 mousePos = Input.mousePosition;

                point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
                if (nearClipPlaneWorldPoint == 0)
                    nearClipPlaneWorldPoint = point.z;
                win.Sommets.Add(new Vector2(point.x, point.y));
                
                Line.positionCount += 1;
                Sommet = new GameObject("Sommet");
                Sommet.transform.position = point;
                Line.SetPosition(tempcount, Sommet.transform.position);
                tempcount++;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                Line.loop=true;
                win.setNormal();
            }  
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) // click gauche
            {
                Vector3 point = new Vector3();
                //Event currentEvent = Event.current;

                // Get the mouse position from Event.
                // Note that the y position from Event is inverted.
                //mousePos.x = currentEvent.mousePosition.x;
                //mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
                Vector2 mousePos = Input.mousePosition;

                point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

                poly.Sommets.Add(new Vector2(point.x, point.y));
                
                Line2.positionCount += 1;
                Sommet = new GameObject("Sommet");
                Sommet.transform.position = point;
                Line2.SetPosition(tempcount2, Sommet.transform.position);
                tempcount2++;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                Line2.loop=true;
                for (int i = 0; i < poly.Sommets.Count; i++)
                {
                    Vector2 ps1 = poly.Sommets[i];
                    Vector2 ps2 = poly.Sommets[(i + 1) % poly.Sommets.Count];

                    if (win.CyrusBeck(ref ps1.x,ref ps1.y, ref ps2.x, ref ps2.y))
                    {
                        LineRenderer newLine = Instantiate(Line3);
                        newLine.positionCount += 1;
                        Sommet = new GameObject("Sommet");
                        Sommet.transform.position = new Vector3(ps1.x,ps1.y,nearClipPlaneWorldPoint);
                        newLine.SetPosition(0, Sommet.transform.position);
                        
                        newLine.positionCount += 1;
                        Sommet = new GameObject("Sommet");
                        Sommet.transform.position = new Vector3(ps2.x,ps2.y,nearClipPlaneWorldPoint);
                        newLine.SetPosition(1, Sommet.transform.position);
                    }
                }
            }  
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (Isfenetre)
            {
                Isfenetre = false;
            }
            else
            {
                Isfenetre = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Isfenetre)
            {
                Line.positionCount = 0;
                tempcount = 0;
                Line.loop = false;
            }
            else
            {
                Line2.positionCount = 0;
                tempcount2 = 0;
                Line2.loop = false;
            }
        }

    }
    
    /*
    void OnGUI()
    {
       
    }
    
    */
}
