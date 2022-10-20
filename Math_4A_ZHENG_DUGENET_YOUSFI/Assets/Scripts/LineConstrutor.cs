using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConstrutor : MonoBehaviour
{
    GameObject Sommet;
    public LineRenderer Line;
    private int tempcount = 0;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        Line.startWidth = .05f;
        Line.endWidth = .05f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click gauche
        {
            Vector3 point = new Vector3();
            Event currentEvent = Event.current;

            // Get the mouse position from Event.
            // Note that the y position from Event is inverted.
            //mousePos.x = currentEvent.mousePosition.x;
            //mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
            Vector2 mousePos = Input.mousePosition;

            point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

            Line.positionCount += 1;
            Sommet = new GameObject("Sommet");
            Sommet.transform.position = point;
            Line.SetPosition(tempcount, Sommet.transform.position);
            tempcount++;
        }

    }
    /*
    void OnGUI()
    {
       
    }
    
    */
}
