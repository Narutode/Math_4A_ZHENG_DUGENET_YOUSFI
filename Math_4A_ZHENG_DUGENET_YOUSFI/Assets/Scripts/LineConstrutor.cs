using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class LineConstrutor : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject tracerMenu;
    public GameObject validerMenu;
    public GameObject fenêtragePanel;
    public GameObject remplissagePanel;
    public GameObject fermerForme;
    
    
    GameObject Sommet;
    [FormerlySerializedAs("Line")] public LineRenderer curLine;
    public LineRenderer Line2;
    public LineRenderer Line3;

    private int tempcount ;
    private int tempcount2;
    public Camera cam;
    public Window win;
    public Polygon poly;
    private bool Isfenetre = false;
    private bool isPoly = false;
    private bool tracé = false;
    private bool cyrusBeck = false;
    public Spline curSpline = null;
    public LinkedList<Spline> linkedSpline = new LinkedList<Spline>();
    public LinkedList<LineRenderer> linkedLine = new  LinkedList<LineRenderer>();
    public bool addingPoints = false;
    public GameObject pointGO;
    
    
    private float nearClipPlaneWorldPoint = 0;
    // Start is called before the first frame update
    void Start()
    {
        //win = new Window();
        //poly = new Polygon();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!addingPoints)
            {
                addingPoints = true;
                Debug.Log("Add spline " + linkedSpline.Count);
                linkedSpline.AddLast(new Spline());
                curSpline = linkedSpline.Last.Value;
                linkedLine.AddLast(new GameObject().AddComponent<LineRenderer>());
                curLine = linkedLine.Last.Value;
                curLine.startWidth = .5f;
                curLine.endWidth = .5f;
                curLine.positionCount = 0;
            }
            else
            {
                Debug.Log("Draw Bezier");
                drawBezier();
                addingPoints = false;
            }
        }

        if (curSpline == null)
            return;
        
        if (Input.GetMouseButtonDown(0) && addingPoints) // click gauche
        {
            Debug.Log("place point");
                Vector3 point = new Vector3();
                Vector2 mousePos = Input.mousePosition;

                point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
                if (nearClipPlaneWorldPoint == 0)
                    nearClipPlaneWorldPoint = point.z;
                curSpline.pList.Add(point);
                curSpline.pgoList.AddLast(Instantiate(pointGO));
                curSpline.pgoList.Last.Value.transform.SetPositionAndRotation(point, Quaternion.identity);
        }
        else if (Input.GetMouseButton(0)&& !addingPoints)
        {
            Vector3 point = new Vector3();
            Vector2 mousePos = Input.mousePosition;
            point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
            if (nearClipPlaneWorldPoint == 0)
                nearClipPlaneWorldPoint = point.z;
            int index = 0;
            foreach (GameObject pgo in curSpline.pgoList)
            {
                if (Vector3.Distance(point, pgo.transform.position) < 1)
                {
                    Debug.Log("move point");
                    pgo.transform.SetPositionAndRotation(point, Quaternion.identity);
<<<<<<< Updated upstream
                    if (Input.GetKeyDown(KeyCode.KeypadEnter))
=======
                    curSpline.pList[index] = point;
                    List<Vector3> bezierPoints = curSpline.PascalMethod();
                    curLine.positionCount = 0;
                    foreach (var p in bezierPoints)
>>>>>>> Stashed changes
                    {
                        curSpline.pList.Remove(curSpline.pList[index]);
                        curSpline.pgoList.Remove(pgo);
                        Destroy(pgo);
                    }
                    else
                    {
                        curSpline.pList[index] = point;
                        List<Vector3> bezierPoints = curSpline.Casteljau();
                        curLine.positionCount = 0;
                        foreach (var p in bezierPoints)
                        {
                            curLine.positionCount += 1;
                            curLine.SetPosition(curLine.positionCount - 1, p);
                        }
                    }

                    break;
                }
                index++;
            }
            
        }

        if(Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.I))
        {
            curSpline.step /= 2;
            Debug.Log("spline step : " + curSpline.step);
            drawBezier();
        }
        if(Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.U))
        {
            curSpline.step *= 2;
            Debug.Log("spline step : " + curSpline.step);
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.Log("Remove spline " + linkedSpline.Count);
            linkedSpline.Remove(curSpline);
            foreach (var point in curSpline.pgoList)
            {
                Destroy(point);
            }
            curSpline = linkedSpline.Last.Value;
            linkedLine.Remove(curLine);
            Destroy(curLine.gameObject);
            curLine = linkedLine.Last.Value;

        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            curSpline = linkedSpline.Find(curSpline).Previous.Value;
            curLine = linkedLine.Find(curLine).Previous.Value;
            Debug.Log("Previous spline");
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            curSpline = linkedSpline.Find(curSpline).Next.Value;
            curLine = linkedLine.Find(curLine).Next.Value;
            Debug.Log("Next spline");
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            curSpline.Angle -= 0.1f;
            Debug.Log(curSpline.Angle);
            drawBezier();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            curSpline.Angle += 0.1f;
            Debug.Log(curSpline.Angle);
            drawBezier();
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            curSpline.pointSH.y += 0.1f;
            drawBezier();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            curSpline.pointSH.y -= 0.1f;
            drawBezier();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            curSpline.pointSH.x -= 0.1f;
            drawBezier();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            curSpline.pointSH.x += 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            List<Vector3> jarvis = curSpline.Jarvis();
            LineRenderer lr = new GameObject().AddComponent<LineRenderer>();
            lr.startWidth = .5f;
            lr.endWidth = .5f;
            lr.positionCount = 0;
            foreach (var j in jarvis)
            {
                lr.positionCount += 1;
                lr.SetPosition(lr.positionCount-1,j);
            }
            lr.loop = true;
        }
    }

    void drawBezier()
    {
        curLine.positionCount = 0;
        List<Vector3> bezierPoints = curSpline.PascalMethod();//curSpline.Casteljau();
        foreach (var point in bezierPoints)
        {
            curLine.positionCount += 1;
            curLine.SetPosition(curLine.positionCount - 1, point);
        }
    }
    /*
    if (Input.GetMouseButtonDown(1))
    {
        tracé = false;
        fenêtragePanel.SetActive(false);
        remplissagePanel.SetActive(false);
        
        if (menuPanel.activeSelf == false)
        {
            menuPanel.SetActive(true);

        }
        else
        {
            menuPanel.SetActive(false);
        }
    }
*/
        /*
        if (tracé)
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

                //fermer le polygone
                if (Input.GetKeyDown(KeyCode.A))
                {
                    Line.loop = true;
                    win.setNormal();
                    tracé = false;
                    //Affichage des normal
                    /*
                    for (int i = 0; i < win.Normals.Count; i++)
                    {
                        LineRenderer newLine = Instantiate(Line3);
                        newLine.positionCount += 1;
                        Sommet = new GameObject("Sommet");
                        float x = (win.Sommets[i].x + win.Sommets[(i + 1) % win.Sommets.Count].x) / 2;
                        float y = (win.Sommets[i].y + win.Sommets[(i + 1) % win.Sommets.Count].y) / 2;

                        Sommet.transform.position = new Vector3(x,y,nearClipPlaneWorldPoint);
                        newLine.SetPosition(0, Sommet.transform.position);
                            
                        newLine.positionCount += 1;
                        Sommet = new GameObject("Sommet");
                        Sommet.transform.position = new Vector3(x+win.Normals[i].x/3,y+win.Normals[i].y/3,nearClipPlaneWorldPoint);
                        newLine.SetPosition(1, Sommet.transform.position);
                    }
 
                }
            }
            else if (isPoly)
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

                //tracer le polygone
                if (Input.GetKeyDown(KeyCode.A))
                {
                    
                    
                    Line2.loop = true;
                    if (cyrusBeck) //CyrusBeck
                    {
                       
                    }
                    else //Sutherland
                    {
                    }
                    tracé = false;
                }
            }
//changer de fenetre window
           /* if (Input.GetKeyDown(KeyCode.Z))
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
            */
        //clear polygone
          /*  if (Input.GetKeyDown(KeyCode.C))
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
            */
          


// BOUTONS MENU
    public void Couleurs()
    {
        menuPanel.SetActive(false);
    }

    

    public void Tracer()
    {   
        menuPanel.SetActive(false);
        tracerMenu.SetActive(true);
        
        
        menuPanel.SetActive(false);
        
    }

    public void Fenêtre()
    {
        tracerMenu.SetActive(false);
        fermerForme.SetActive(true);
        
        tracé = true;
        isPoly = false;
        Isfenetre = true;
    }
    
    public void Polygone()
    {
        tracerMenu.SetActive(false);
        fermerForme.SetActive(true);
        tracé = true;
        Isfenetre = false;
        isPoly = true;
        
    }

    public void FermerForme()
    {
        if (Isfenetre)
        {
            curLine.loop = true;
            win.setNormal();
            tracé = false;
        }
        else if (isPoly)
        {
            Line2.loop = true;
            tracé = false;
        }
        
        fermerForme.SetActive(false);
        
        
    }

    public void Valider()
    {
        menuPanel.SetActive(false);
        validerMenu.SetActive(true);
    }

    public void Fenêtrage()
    {
        validerMenu.SetActive(false);
        menuPanel.SetActive(false);
        fenêtragePanel.SetActive(true);
    }

    public void Remplissage()
    {
        validerMenu.SetActive(false);
        menuPanel.SetActive(false);
        remplissagePanel.SetActive(true);
    }

    public void Clear()
    {
        
            curLine.positionCount = 0;
            tempcount = 0;
            curLine.loop = false;
        
        
            Line2.positionCount = 0;
            tempcount2 = 0;
            Line2.loop = false;
        
        
    }

    
    public void RetourTrace()
    {
        tracerMenu.SetActive(false);
        menuPanel.SetActive(true);
    }
    public void RetourValider()
    {
        validerMenu.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void RetourFR()
    {
        fenêtragePanel.SetActive(false);
        remplissagePanel.SetActive(false);
        validerMenu.SetActive(true);
    }

    public void CyrusBeckMenu()
    {
        fenêtragePanel.SetActive(false);
        for (int i = 0; i < poly.Sommets.Count; i++)
        {
            Vector2 ps1 = poly.Sommets[i];
            Vector2 ps2 = poly.Sommets[(i + 1) % poly.Sommets.Count];

            if (win.CyrusBeck(ref ps1.x, ref ps1.y, ref ps2.x, ref ps2.y))
            {
                LineRenderer newLine = Instantiate(Line3);
                newLine.positionCount += 1;
                Sommet = new GameObject("Sommet");
                Sommet.transform.position = new Vector3(ps1.x, ps1.y, nearClipPlaneWorldPoint);
                newLine.SetPosition(0, Sommet.transform.position);

                newLine.positionCount += 1;
                Sommet = new GameObject("Sommet");
                Sommet.transform.position = new Vector3(ps2.x, ps2.y, nearClipPlaneWorldPoint);
                newLine.SetPosition(1, Sommet.transform.position);

                newLine.startWidth = .01f;
                newLine.endWidth = .01f;
            }
        }
    }

    public void SutherlandMenu()
    {
        fenêtragePanel.SetActive(false);
        List<Vector2> newPoly = win.SutherlandHodgman(poly.Sommets);
        LineRenderer newLine = Instantiate(Line3);

        for (int i = 0; i < newPoly.Count; i++)
        {
            Vector2 ps1 = newPoly[i];

            newLine.positionCount += 1;
            Sommet = new GameObject("Sommet New poly");
            Sommet.transform.position = new Vector3(ps1.x, ps1.y, nearClipPlaneWorldPoint);
            newLine.SetPosition(i, Sommet.transform.position);
        }
        newLine.startWidth = .01f;
        newLine.endWidth = .01f;
        newLine.loop = true;
    }
    

    public void DécoupageQuelconque()
    {
        
    }

    public void GermesRécursive()
    {
        
    }

    public void GermesPile()
    {
        
    }

    public void LigneBalayage()
    {
        
    }

    public void LCA()
    {
        
    }
    
    /*
    void OnGUI()
    {
       
    }
    
    */
}
