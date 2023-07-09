using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Color = UnityEngine.Color;
using UnityEngine.EventSystems;

public class LineConstrutor : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject tracerMenu;
    public GameObject validerMenu;
    public GameObject fenêtragePanel;
    public GameObject remplissagePanel;
    public GameObject fermerForme;
    public GameObject bézierMenu;
    public GameObject lissageMenu;
    public GameObject extrusionsMenu;

    
    GameObject Sommet;
    [FormerlySerializedAs("Line")] public LineRenderer curLine;
    public LineRenderer Line2;
    public LineRenderer Line3;

    private int tempcount;
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

    public int hauteur = 5;
    public float scale = 1;
    public LineRenderer trajectoire;
    
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
        }

        


        if (tracé)
        {
            if (Isfenetre)
            {
                curLine.startWidth = 0.5f;
                curLine.endWidth = .5f;
                if (Input.GetMouseButtonDown(2)) // click milieu
                {
                    Vector3 point = new Vector3();

                    Vector2 mousePos = Input.mousePosition;

                    point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
                    if (nearClipPlaneWorldPoint == 0)
                        nearClipPlaneWorldPoint = point.z;
                    win.Sommets.Add(new Vector2(point.x, point.y));

                    curLine.positionCount += 1;
                    Sommet = new GameObject("Sommet");
                    Sommet.transform.position = point;
                    curLine.SetPosition(tempcount, Sommet.transform.position);
                    tempcount++;

                }

                //fermer le polygone
                if (Input.GetKeyDown(KeyCode.L))
                {
                    curLine.loop = true;
                    win.setNormal();
                    tracé = false;
                    //Affichage des normal

                    for (int i = 0; i < win.Normals.Count; i++)
                    {
                        LineRenderer newLine = Instantiate(Line3);
                        newLine.positionCount += 1;
                        Sommet = new GameObject("Sommet");
                        float x = (win.Sommets[i].x + win.Sommets[(i + 1) % win.Sommets.Count].x) / 2;
                        float y = (win.Sommets[i].y + win.Sommets[(i + 1) % win.Sommets.Count].y) / 2;

                        Sommet.transform.position = new Vector3(x, y, nearClipPlaneWorldPoint);
                        newLine.SetPosition(0, Sommet.transform.position);

                        newLine.positionCount += 1;
                        Sommet = new GameObject("Sommet");
                        Sommet.transform.position = new Vector3(x + win.Normals[i].x / 3, y + win.Normals[i].y / 3,
                            nearClipPlaneWorldPoint);
                        newLine.SetPosition(1, Sommet.transform.position);
                    }
                }
            }
            /*
            else if (isPoly)
            {
                if (Input.GetMouseButtonDown(2)) // click milieu
                {
                    Vector3 point = new Vector3();
                    //Event currentEvent = Event.current;

                    // Get the mouse position from Event.
                    // Note that the y position from Event is inverted.
                    //mousePos.x = currentEvent.mousePosition.x;
                    //mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
                    Vector2 mousePos = Input.mousePosition;

                    point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));

                    poly.Sommets.Add(new Vector2(point.x, point.y));

                    Line2.positionCount += 1;
                    Sommet = new GameObject("Sommet");
                    Sommet.transform.position = point;
                    Line2.SetPosition(tempcount2, Sommet.transform.position);
                    tempcount2++;
                }

                //tracer le polygone
                if (Input.GetKeyDown(KeyCode.M))
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
            if (Input.GetKeyDown(KeyCode.O))
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

            //clear polygone
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Isfenetre)
                {
                    curLine.positionCount = 0;
                    tempcount = 0;
                    curLine.loop = false;
                }
                else
                {
                    Line2.positionCount = 0;
                    tempcount2 = 0;
                    Line2.loop = false;
                }
            }*/
        }
        
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
        if (curSpline == null)
            return;

        if (Input.GetMouseButtonDown(0) && addingPoints) // click gauche
        {
           
            if(!EventSystem.current.IsPointerOverGameObject()) 
            {
               
                    Debug.Log("place point");
                    Vector3 point = new Vector3();
                    Vector2 mousePos = Input.mousePosition;

                    point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
                    if (nearClipPlaneWorldPoint == 0)
                        nearClipPlaneWorldPoint = point.z;
                    pointGO.transform.position = point;
                    GameObject newP = Instantiate(pointGO);
                    newP.tag = "Extru";
                    //newP.GetComponent<MeshRenderer>().sharedMaterial.color = curSpline.color;
                    curSpline.pgoList.AddLast(newP);
                
            }
           
        }
        else if (Input.GetMouseButton(0) && !addingPoints)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
               
                    Vector3 point = new Vector3();
                    Vector2 mousePos = Input.mousePosition;
                    point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
                    if (nearClipPlaneWorldPoint == 0)
                        nearClipPlaneWorldPoint = point.z;
                    foreach (GameObject pgo in curSpline.pgoList)
                    {
                        if (Vector3.Distance(point, pgo.transform.position) < 1)
                        {
                            Debug.Log("move point");
                            pgo.transform.SetPositionAndRotation(point, Quaternion.identity);
                            drawBezier();
                            break;
                        }
                    }
                

            }
                   
        }
        else if (Input.GetMouseButton(1) && !addingPoints)
        {

            if (!EventSystem.current.IsPointerOverGameObject())
            {

                Vector3 point = new Vector3();
                    Vector2 mousePos = Input.mousePosition;
                    point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
                    if (nearClipPlaneWorldPoint == 0)
                        nearClipPlaneWorldPoint = point.z;
                    foreach (GameObject pgo in curSpline.pgoList)
                    {
                        if (Vector3.Distance(point, pgo.transform.position) < 1)
                        {
                            Debug.Log("delete point");
                            curSpline.pgoList.Remove(pgo);
                            Destroy(pgo);

                            break;
                        }
                    }

                

            }
           
        }
        

        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.I))
        {
            
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.U))
        {
            
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (linkedSpline.Count > 1)
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
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            
            
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            
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
            curSpline.pointT.y += 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            curSpline.pointT.y -= 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            curSpline.pointT.x -= 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            curSpline.pointT.x += 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            curSpline.pointS.y += 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            curSpline.pointS.y -= 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            curSpline.pointS.x -= 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            curSpline.pointS.x += 0.1f;
            drawBezier();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            curSpline.pointSH.y += 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            curSpline.pointSH.y -= 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            curSpline.pointSH.x -= 0.1f;
            drawBezier();
        }

        if (Input.GetKeyDown(KeyCode.G))
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
                lr.SetPosition(lr.positionCount - 1, j);
            }

            lr.loop = true;
        }
    }

    List<Vector3> drawBezier()
    {
        curLine.positionCount = 0;
        List<Vector3> bezierPoints = curSpline.Casteljau();//curSpline.PascalMethod();
        foreach (var point in bezierPoints)
        {
            curLine.positionCount += 1;
            curLine.SetPosition(curLine.positionCount - 1, point);
        }
        return bezierPoints;
    }


// BOUTONS MENU

    public void Beziers()
    {
        //menuPanel.SetActive(false);
        Clear();
        menuPanel.transform.GetChild(3).gameObject.SetActive(false);
        bézierMenu.transform.GetChild(0).gameObject.SetActive(true);
        bézierMenu.transform.GetChild(1).gameObject.SetActive(false);
        bézierMenu.transform.GetChild(2).gameObject.SetActive(false);
        bézierMenu.transform.GetChild(3).gameObject.SetActive(false);
        bézierMenu.SetActive(true);
    }


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
        GameObject[] extrus = GameObject.FindGameObjectsWithTag("Extru");
        foreach(GameObject obj in extrus)
        {
            Destroy(obj);
        }
        if(curSpline != null) curSpline.pgoList.Clear();

        linkedLine.Clear();
        linkedSpline.Clear();
        if (curLine != null)
        {
            curLine.positionCount = 0;
            tempcount = 0;
            curLine.loop = false;
        }
        
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
        List<Vector3> ListSpline = curSpline.Casteljau();
        for (int i = 0; i < ListSpline.Count; i++)
        {

            Vector3 ps1 = ListSpline[i];
            Vector3 ps2 = ListSpline[(i + 1)%ListSpline.Count];
            if (win.CyrusBeck(ref ps1.x, ref ps1.y, ref ps2.x, ref ps2.y))
            {
                LineRenderer newLine = Instantiate(Line3);
                newLine.positionCount += 1;
                Sommet = new GameObject("Sommet");
                Sommet.transform.position = new Vector3(ps1.x, ps1.y, ps1.z);
                newLine.SetPosition(0, Sommet.transform.position);

                newLine.positionCount += 1;
                Sommet = new GameObject("Sommet");
                Sommet.transform.position = new Vector3(ps2.x, ps2.y, ps2.z);
                newLine.SetPosition(1, Sommet.transform.position);

                newLine.startWidth = .5f;
                newLine.endWidth = .5f;
                
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
    

    public void CommencerBéziers()
    {
        bézierMenu.transform.GetChild(0).gameObject.SetActive(false);
        bézierMenu.transform.GetChild(1).gameObject.SetActive(true);
        bézierMenu.transform.GetChild(2).gameObject.SetActive(false);
        bézierMenu.transform.GetChild(3).gameObject.SetActive(false);

        addingPoints = true;

            Debug.Log("Add spline " + linkedSpline.Count);
            linkedSpline.AddLast(new Spline());
            if (linkedSpline.Last.Previous != null)
            {
                Spline s1 = linkedSpline.Last.Value;
                Spline s0 = linkedSpline.Last.Previous.Value;
                //Raccord C0
                s1.pgoList.AddLast(s0.pgoList.Last());

                //Raccord C1
                Vector3 P1 = s1.pgoList.First.Value.transform.position + s0.pgoList.Last.Value.transform.position - s0.pgoList.Last.Previous.Value.transform.position;
                pointGO.transform.position = P1;
                s1.pgoList.AddLast(Instantiate(pointGO));

                //Raccord C2
                Vector3 P2 = s0.pgoList.Last.Previous.Previous.Value.transform.position + 2 * (s1.pgoList.Last.Value.transform.position - s1.pgoList.First.Value.transform.position);
                pointGO.transform.position = P1;
                s1.pgoList.AddLast(Instantiate(pointGO));
            }

            curSpline = linkedSpline.Last.Value;
            GameObject lr = new GameObject();
            lr.tag = "Extru";
            linkedLine.AddLast(lr.AddComponent<LineRenderer>());
            curLine = linkedLine.Last.Value;
            curLine.startWidth = .5f;
            curLine.endWidth = .5f;
            curLine.positionCount = 0;
            //pointGO.GetComponent<MeshRenderer>().sharedMaterial.color = curSpline.color;
            //curLine.material = new Material(Shader.Find("Default-Line"));
            //curLine.material.color = curSpline.color; 
            //curLine.startColor = curSpline.color;
            //curLine.endColor = curSpline.color;
            //Material m = new Material(Shader.Find("Specular"));
            //m.color = curSpline.color;
            //curLine.material = m;
        
    }

    public void TerminerBéziers()
    {
        if (curSpline.pgoList.Count > 1)
        {
            bézierMenu.transform.GetChild(0).gameObject.SetActive(false);
            bézierMenu.transform.GetChild(1).gameObject.SetActive(false);
            bézierMenu.transform.GetChild(2).gameObject.SetActive(true);
            bézierMenu.transform.GetChild(3).gameObject.SetActive(true);
            Debug.Log("Draw Bezier");
            drawBezier();
            addingPoints = false;
        }
        else
        {
            Debug.Log("At least 2 points needed !");
        }
    }

    public void Lissage()
    {
        bézierMenu.SetActive(false);
        lissageMenu.SetActive(true);
    }

    public void Extrusions()
    {
        bézierMenu.SetActive(false);
        extrusionsMenu.SetActive(true);
    }

    public void RetourBéziers()
    {
        addingPoints = false;
        Clear();
        
        bézierMenu.transform.GetChild(0).gameObject.SetActive(true);
        bézierMenu.transform.GetChild(1).gameObject.SetActive(false);
        bézierMenu.transform.GetChild(2).gameObject.SetActive(false);
        bézierMenu.transform.GetChild(3).gameObject.SetActive(false);
        bézierMenu.SetActive(false);
        menuPanel.transform.GetChild(3).gameObject.SetActive(true);
        //menuPanel.SetActive(true);
        
    }

    public void RetourLissage()
    {
        lissageMenu.SetActive(false);
        bézierMenu.SetActive(true);
    }
    public void LissagePlus()
    {
        curSpline.step /= 2;
        Debug.Log("spline step : " + curSpline.step);
        drawBezier();
    }

    public void LissageMoins()
    {
        curSpline.step *= 2;
        Debug.Log("spline step : " + curSpline.step);
        drawBezier();
    }

    public void RetourExtrusions()
    {
        extrusionsMenu.SetActive(false);
        bézierMenu.SetActive(true);
    }
    public void OnClickExtru()
    {
        /*extrusionsMenu.transform.GetChild(0).gameObject.SetActive(false);
        extrusionsMenu.transform.GetChild(1).gameObject.SetActive(false);
        extrusionsMenu.transform.GetChild(2).gameObject.SetActive(false);
        extrusionsMenu.transform.GetChild(3).gameObject.SetActive(false);
        extrusionsMenu.transform.GetChild(4).gameObject.SetActive(true);
        */
       

        extrusionsMenu.SetActive(false);
        menuPanel.transform.GetChild(3).gameObject.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void ClearExtru()
    {
        extrusionsMenu.transform.GetChild(0).gameObject.SetActive(true);
        extrusionsMenu.transform.GetChild(1).gameObject.SetActive(true);
        extrusionsMenu.transform.GetChild(2).gameObject.SetActive(true);
        extrusionsMenu.transform.GetChild(3).gameObject.SetActive(true);
        extrusionsMenu.transform.GetChild(4).gameObject.SetActive(false);

        extrusionsMenu.SetActive(false);

        Clear();

        RetourBéziers();
    }

    public void RotExtru()
    {
        //Rotation

        if (curSpline != null)
        {
            List<Vector3> bezierPoints = drawBezier();
            List<Vector3> nextBezierPoints = new List<Vector3>();
            List<Vector3> vertices = new List<Vector3>(bezierPoints);
            List<int> tris = new List<int>();
            //List<Color32> colors = new List<Color32>();
            int sizeB = bezierPoints.Count;

            float step = (Mathf.PI * 2) / hauteur;
            float angle = step;
            //Axe de rotation
            Vector3 axis = (bezierPoints.Last() - bezierPoints.First()).normalized;
            Vector3 milieu = (bezierPoints.Last() + bezierPoints.First()) / 2f;

            for (int i = 0; i < hauteur + 1; i++)
            {
                //Matrice de rotation
                Vector3 rowX = new Vector3(Mathf.Cos(angle) + axis.x * axis.x * (1 - Mathf.Cos(angle)),
                    axis.x * axis.y * (1 - Mathf.Cos(angle)) - axis.z * Mathf.Sin(angle),
                    axis.x * axis.z * (1 - Mathf.Cos(angle)) + axis.y * Mathf.Sin(angle));

                Vector3 rowY = new Vector3(axis.x * axis.y * (1 - Mathf.Cos(angle)) + axis.z * Mathf.Sin(angle),
                    Mathf.Cos(angle) + axis.y * axis.y * (1 - Mathf.Cos(angle)),
                    axis.y * axis.z * (1 - Mathf.Cos(angle)) - axis.x * Mathf.Sin(angle));

                Vector3 rowZ = new Vector3(axis.x * axis.z * (1 - Mathf.Cos(angle)) - axis.y * Mathf.Sin(angle),
                    axis.z * axis.y * (1 - Mathf.Cos(angle)) + axis.x * Mathf.Sin(angle),
                    Mathf.Cos(angle) + axis.z * axis.z * (1 - Mathf.Cos(angle)));
                //creation de la prochaine ligne du mesh
                for (int n = 0; n < sizeB; n++)
                {
                    Vector3 p = bezierPoints[n] - milieu;
                    Vector3 newP = new Vector3(rowX.x * p.x + rowX.y * p.y + rowX.z * p.z,
                        rowY.x * p.x + rowY.y * p.y + rowY.z * p.z, rowZ.x * p.x + rowZ.y * p.y + rowZ.z * p.z);
                    nextBezierPoints.Add(newP + milieu);
                }

                angle += step;
                vertices.AddRange(nextBezierPoints);
                nextBezierPoints.Clear();
            }
            //Triangulation
            for (int i = 0; i < hauteur; i++)
            {
                for (int n = 0; n < sizeB; n++)
                {
                    //Premier triangle
                    tris.Add(i * sizeB + n);
                    tris.Add((i + 1) * sizeB + n);
                    tris.Add(i * sizeB + (n + 1) % sizeB);

                    //Second triangle
                    tris.Add((i + 1) * sizeB + n);
                    tris.Add((i + 1) * sizeB + (n + 1) % sizeB);
                    tris.Add(i * sizeB + (n + 1) % sizeB);
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = tris.ToArray();
            //mesh.colors32 = colors.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateUVDistributionMetric(0);

            // Set up game object with mesh;
            GameObject meshGameObject = new GameObject();
            meshGameObject.tag = "Extru";
            MeshRenderer mr = meshGameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter filter = meshGameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = mesh;
            mr.material = new Material(Shader.Find("Standard"));
        }
        OnClickExtru();
    }

    public void SimpleExtru()
    {
        //Extrusion simple (hauteur + scale)

        if (curSpline != null)
        {
            List<Vector3> bezierPoints = drawBezier();
            List<Vector3> nextBezierPoints = new List<Vector3>();
            List<Vector3> vertices = new List<Vector3>(bezierPoints);
            List<int> tris = new List<int>();
            //List<Color32> colors = new List<Color32>();
            int sizeB = bezierPoints.Count;

            for (int i = 0; i < hauteur + 1; i++)
            {
                //creation de la prochaine ligne du mesh
                for (int n = 0; n < sizeB; n++)
                {
                    //Debug.Log(n);
                    Vector3 p = bezierPoints[n];
                    nextBezierPoints.Add(new Vector3(p.x * scale, p.y * scale, p.z - 1));
                    //Vector3 np = nextBezierPoints.Last();
                    //colors.Add(Color.Lerp(Color.green, Color.red, np.z/5f));
                }
                vertices.AddRange(nextBezierPoints);
                bezierPoints.Clear();
                bezierPoints.AddRange(nextBezierPoints);
                nextBezierPoints.Clear();
            }
            //Triangulation
            //Face arrière
            for (int n = 0; n < sizeB - 1; n++)
            {
                tris.Add(0);
                tris.Add(n);
                tris.Add(n + 1);
            }
            //Face avant
            for (int n = 0; n < sizeB - 1; n++)
            {
                tris.Add(hauteur * sizeB + n + 1);
                tris.Add(hauteur * sizeB + n);
                tris.Add(hauteur * sizeB);
            }
            for (int i = 0; i < hauteur; i++)
            {
                for (int n = 0; n < sizeB; n++)
                {
                    //Premier triangle
                    tris.Add(i * sizeB + n);
                    tris.Add((i + 1) * sizeB + n);
                    tris.Add(i * sizeB + (n + 1) % sizeB);

                    //Second triangle
                    tris.Add((i + 1) * sizeB + n);
                    tris.Add((i + 1) * sizeB + (n + 1) % sizeB);
                    tris.Add(i * sizeB + (n + 1) % sizeB);
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = tris.ToArray();
            //mesh.colors32 = colors.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateUVDistributionMetric(0);

            // Set up game object with mesh;
            GameObject meshGameObject = new GameObject();
            meshGameObject.tag = "Extru";
            MeshRenderer mr = meshGameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter filter = meshGameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = mesh;
            mr.material = new Material(Shader.Find("Standard"));
        }
        OnClickExtru();
    }

    public void TrajExtru()
    {
        //Extrusion avec trajectoire

        if (curSpline != null)
        {
            List<Vector3> bezierPoints = drawBezier();
            List<Vector3> nextBezierPoints = new List<Vector3>();
            List<Vector3> vertices = new List<Vector3>(bezierPoints);
            List<int> tris = new List<int>();
            //List<Color32> colors = new List<Color32>();
            int sizeB = bezierPoints.Count;
            
            Vector3 normalBezier = new Vector3(0, 0, -1);
            Vector3 milieu = new Vector3(0, 0, 1.3f);

            hauteur = trajectoire.positionCount-1;
            
            for (int i = 1; i < hauteur+1; i++)
            {
                Vector3 prevTrajectoire = trajectoire.GetPosition(i-1);
                Vector3 posTrajectoire = trajectoire.GetPosition(i);

                //Vector3 axis = new Vector3(0,-1,0);
                Vector3 axis = Vector3.Cross(normalBezier, posTrajectoire-prevTrajectoire).normalized;
                float angle = Mathf.Deg2Rad*Vector3.Angle(posTrajectoire-prevTrajectoire,normalBezier);
                //float angle = 0;
                
                //Matrice de rotation
                Vector3 rowX = new Vector3(Mathf.Cos(angle) + axis.x * axis.x * (1 - Mathf.Cos(angle)),
                    axis.x * axis.y * (1 - Mathf.Cos(angle)) - axis.z * Mathf.Sin(angle),
                    axis.x * axis.z * (1 - Mathf.Cos(angle)) + axis.y * Mathf.Sin(angle));

                Vector3 rowY = new Vector3(axis.x * axis.y * (1 - Mathf.Cos(angle)) + axis.z * Mathf.Sin(angle),
                    Mathf.Cos(angle) + axis.y * axis.y * (1 - Mathf.Cos(angle)),
                    axis.y * axis.z * (1 - Mathf.Cos(angle)) - axis.x * Mathf.Sin(angle));

                Vector3 rowZ = new Vector3(axis.x * axis.z * (1 - Mathf.Cos(angle)) - axis.y * Mathf.Sin(angle),
                    axis.z * axis.y * (1 - Mathf.Cos(angle)) + axis.x * Mathf.Sin(angle),
                    Mathf.Cos(angle) + axis.z * axis.z * (1 - Mathf.Cos(angle)));
                
                //creation de la prochaine ligne du mesh
                for (int n = 0; n < sizeB; n++)
                {
                    //Debug.Log(n);
                    Vector3 p = bezierPoints[n] - milieu;
                    Vector3 newP = new Vector3(rowX.x * p.x + rowX.y * p.y + rowX.z * p.z,
                        rowY.x * p.x + rowY.y * p.y + rowY.z * p.z, rowZ.x * p.x + rowZ.y * p.y + rowZ.z * p.z);
                    nextBezierPoints.Add(newP + posTrajectoire + milieu);
                }
                vertices.AddRange(nextBezierPoints);
                nextBezierPoints.Clear();
            }
            //Triangulation
            //Face arrière
            for (int n = 0; n < sizeB - 1; n++)
            {
                tris.Add(0);
                tris.Add(n);
                tris.Add(n + 1);
            }
            //Face avant
            for (int n = 0; n < sizeB - 1; n++)
            {
                tris.Add(hauteur * sizeB + n + 1);
                tris.Add(hauteur * sizeB + n);
                tris.Add(hauteur * sizeB);
            }
            for (int i = 0; i < hauteur; i++)
            {
                for (int n = 0; n < sizeB; n++)
                {
                    //Premier triangle
                    tris.Add(i * sizeB + n);
                    tris.Add((i + 1) * sizeB + n);
                    tris.Add(i * sizeB + (n + 1) % sizeB);

                    //Second triangle
                    tris.Add((i + 1) * sizeB + n);
                    tris.Add((i + 1) * sizeB + (n + 1) % sizeB);
                    tris.Add(i * sizeB + (n + 1) % sizeB);
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = tris.ToArray();
            //mesh.colors32 = colors.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateUVDistributionMetric(0);

            // Set up game object with mesh;
            GameObject meshGameObject = new GameObject();
            meshGameObject.tag = "Extru";
            MeshRenderer mr = meshGameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter filter = meshGameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = mesh;
            mr.material = new Material(Shader.Find("Standard"));
        }
        OnClickExtru();
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
