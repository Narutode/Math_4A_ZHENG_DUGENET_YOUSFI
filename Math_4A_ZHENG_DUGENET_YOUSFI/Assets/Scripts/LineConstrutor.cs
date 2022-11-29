using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LineConstrutor : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject tracerMenu;
    public GameObject validerMenu;
    public GameObject fenêtragePanel;
    public GameObject remplissagePanel;
    public GameObject fermerForme;
    
    
    GameObject Sommet;
    public LineRenderer Line;
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
                    */
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
                    else //Sutherland
                    {
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
        }
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

        }

   
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
            Line.loop = true;
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
        
            Line.positionCount = 0;
            tempcount = 0;
            Line.loop = false;
        
        
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
