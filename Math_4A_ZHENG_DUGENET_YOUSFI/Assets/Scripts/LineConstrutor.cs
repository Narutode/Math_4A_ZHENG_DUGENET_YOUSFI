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

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click gauche
        {
            Line.positionCount += 1;
            Vector3 mousePos = Input.mousePosition;
            Sommet = new GameObject("Sommet");
            Sommet.transform.position = cam.ScreenToWorldPoint(mousePos);;
            Line.SetPosition(tempcount,Sommet.transform.position);
            tempcount++;
            
        }

    }
}
