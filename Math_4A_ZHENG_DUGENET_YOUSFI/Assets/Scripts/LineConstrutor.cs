using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConstrutor : MonoBehaviour
{
    GameObject Sommet;
    public LineRenderer Line;
    public LineRenderer Line2;
    private int tempcount ;
    private int tempcount2;
    public Camera cam;
    public Window win;
    public Polygon poly;
    private bool Isfenetre = true;
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
