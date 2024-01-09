using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Color = UnityEngine.Color;
//using UnityEngine.EventSystems;
using static FonctionMath;

public struct SegmentsOld
{
    public Vector2 Point1;
    public Vector2 Point2;
}

public struct TrianglesOld
{
    public SegmentsOld Seg1;
    public SegmentsOld Seg2;
    public SegmentsOld Seg3;
    public bool flip;
}

public struct Face
{
    public List<Arete> arreteAdj;
    public Sommet s1, s2, s3;
}

public struct Arete
{
    public Sommet somHaut;
    public Sommet somBas;
    public Face faceGauche;
    public Face faceDroite;
}

public struct Sommet
{
    public Vector3 coord;
    public List<Arete> arreteAdj;
}

public struct Cell
{
    public Vector2 Site;
    public List<Vector2> Vertices;
}

public class LineConstrutor : MonoBehaviour
{
    public Camera cam;
    GameObject _sommet;
    [FormerlySerializedAs("Line")] public LineRenderer curLine;

    public GameObject pointGO;
    public GameObject pointCenter;
    public List<GameObject> listGameObjects;
    public List<Vector2> listPoints;
    public List<SegmentsOld> ListSegments = new List<SegmentsOld>();
    public List<TrianglesOld> ListTriangles = new List<TrianglesOld>();

    public List<Sommet> listSommet = new List<Sommet>();
    public List<Arete> listArete = new List<Arete>();
    public List<Face> listFace = new List<Face>();

    public List<LineRenderer> lines;
    private float _nearClipPlaneWorldPoint = 0;

    [SerializeField]public static float Timer;

    public GameObject clickMenu;
    public GameObject parent;
    public Material mat;

    private void Start()
    {
        _nearClipPlaneWorldPoint = cam.nearClipPlane + 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click gauche
        {
            if (clickMenu.activeSelf == false)
            {        
                Debug.Log("place point");
                Vector3 point = new Vector3();
                Vector2 mousePos = Input.mousePosition;

                point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
                pointGO.transform.position = point;
                GameObject newP = Instantiate(pointGO);
                newP.transform.parent = parent.transform;
                listGameObjects.Add(newP);
                listPoints.Add(newP.transform.position);
                listSommet.Add(new Sommet() {coord = point, arreteAdj = new List<Arete>()});
            }

            if(listSommet.Count >= 3)
            {/*
                Face f = new Face() {s1 = listSommet[0], s2=listSommet[1], s3=listSommet[2], arreteAdj = new List<Arete>()};
                Arete a1 = new Arete();
                if (listSommet[0].coord.y < listSommet[1].coord.y)
                {
                    a1.somBas = listSommet[0];
                    a1.somHaut = listSommet[1];
                }
                else
                {
                    a1.somBas = listSommet[1];
                    a1.somHaut = listSommet[0];
                }

                if (Vector3.Cross(a1.somBas.coord - a1.somHaut.coord, a1.somBas.coord - listSommet[2].coord).z < 0)
                {
                    a1.faceDroite = f;
                }
                else
                {
                    a1.faceGauche = f;
                }
                listArete.Add(a1);
                
                
                Arete a2 = new Arete();
                if (listSommet[1].coord.y < listSommet[2].coord.y)
                {
                    a2.somBas = listSommet[1];
                    a2.somHaut = listSommet[2];
                }
                else
                {
                    a2.somBas = listSommet[2];
                    a2.somHaut = listSommet[1];
                }
                if (Vector3.Cross(a1.somBas.coord - a1.somHaut.coord, a1.somBas.coord - listSommet[0].coord).z < 0)
                {
                    a2.faceDroite = f;
                }
                else
                {
                    a2.faceGauche = f;
                }
                listArete.Add(a2);
                
                Arete a3 = new Arete();
                if (listSommet[2].coord.y < listSommet[0].coord.y)
                {
                    a3.somBas = listSommet[2];
                    a3.somHaut = listSommet[0];
                }
                else
                {
                    a3.somBas = listSommet[0];
                    a3.somHaut = listSommet[2];
                }
                if (Vector3.Cross(a1.somBas.coord - a1.somHaut.coord, a1.somBas.coord - listSommet[1].coord).z < 0)
                {
                    a3.faceDroite = f;
                }
                else
                {
                    a3.faceGauche = f;
                }
                listArete.Add(a1);
                f.arreteAdj.Add(a1);
                f.arreteAdj.Add(a2);
                f.arreteAdj.Add(a3);
                */
                
                //On supprime les segments précédents
                for (int i = 0; i < parent.transform.childCount; i++)
                {
                    var go = parent.transform.GetChild(i).gameObject;
                    if (go.TryGetComponent<LineRenderer>(out var t))
                        Destroy(go);
                }
                lines.Clear();
                ListSegments.Clear();
                ListTriangles.Clear();

                Triangulation();
                DelaunayRefinementAlgorithm();
                
            }
            /*
            else if (listSommet.Count > 3)
            {
                //On supprime les segments précédents
                for (int i = 0; i < parent.transform.childCount; i++)
                {
                    var go = parent.transform.GetChild(i).gameObject;
                    if (go.TryGetComponent<LineRenderer>(out var t))
                        Destroy(go);
                }
                lines.Clear();
                delaunayDirect(listSommet.Last().coord);
                foreach (Arete a in listArete)
                {
                    GameObject newGO = new GameObject();
                    newGO.transform.SetParent(parent.transform);
                    newGO.name = "sides";
                    LineRenderer newLine = newGO.AddComponent<LineRenderer>();
                    newLine.positionCount = 2;
                    newLine.SetPosition(0,
                        new Vector3(a.somBas.coord.x, a.somBas.coord.y, _nearClipPlaneWorldPoint));
                    newLine.SetPosition(1,
                        new Vector3(a.somHaut.coord.x, a.somHaut.coord.y, _nearClipPlaneWorldPoint));
                    newLine.startWidth = 0.05f;
                    newLine.endWidth = 0.05f;
                    lines.Add(newLine);
                }
            }*/
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (listGameObjects.Count >= 3)
            {
                Debug.Log(listGameObjects.Count);              
                Debug.Log("click");
                if (clickMenu.activeSelf == false)
                {
                    Debug.Log("clickmenu is false");
                    clickMenu.SetActive(true);
                }
                else
                {
                    clickMenu.SetActive(false);
                }
            }
            /*
            float start = Time.realtimeSinceStartup;
            //List<Vector2> jarvis = GetJarvis(listPoints);
            List<Vector2> jarvis = GetGrahamScan(listPoints);
            float end = Time.realtimeSinceStartup;
            Timer = end - start;
            Debug.Log(end-start);
            int size = jarvis.Count;
            for (int i = 0; i < size; i++)
            {
                GameObject newGO = new GameObject();
                newGO.name = "sides";
                LineRenderer newLine = newGO.AddComponent<LineRenderer>();
                newLine.positionCount = 2;
                newLine.SetPosition(0,new Vector3(jarvis[i].x,jarvis[i].y, _nearClipPlaneWorldPoint));
                newLine.SetPosition(1,new Vector3(jarvis[(i+1)%size].x,jarvis[(i+1)%size].y, _nearClipPlaneWorldPoint));
                lines.Add(newLine);
            }*/
        }
    }
    
    void delaunayDirect(Vector2 newP)
    {
        List<Arete> visibleArete = new List<Arete>();
        foreach(Arete ar in listArete)
        {
            if(isPointVisibleFromArete(newP, ar, listArete))
                visibleArete.Add(ar);
        }

        Arete curA = visibleArete.First();
        while (visibleArete.Count > 0)
        {
            curA = visibleArete.First();
            visibleArete.Remove(curA);
            List<Face> lTri = listFace.Where(t => t.arreteAdj.Contains(curA)).ToList();
            foreach (var tri in lTri)
            {
                Vector2 circumcenter = GETCenterCircle(tri.s1.coord, tri.s2.coord, tri.s3.coord);
                float circumradius = Vector2.Distance(circumcenter, tri.s1.coord);
                if ((circumcenter - newP).magnitude < circumradius)
                {
                    List<Arete> otherA = tri.arreteAdj.Where(t => !t.Equals(curA)).ToList();
                    visibleArete.Add(otherA[0]);
                    visibleArete.Add(otherA[1]);
                    listFace.Remove(tri);
                }
                else
                {
                    Sommet s = new Sommet() {coord = newP, arreteAdj = new List<Arete>()};
                    Face faceNew = new Face() {arreteAdj = new List<Arete>()};
                    faceNew.s1 = curA.somBas;
                    faceNew.s2 = curA.somHaut;
                    faceNew.s3 = s;
                    faceNew.arreteAdj.Add(curA);
                    Arete a1 = new Arete() {somBas = curA.somBas, somHaut = s};
                    Arete a2 = new Arete() {somBas = curA.somHaut, somHaut = s};
                    faceNew.arreteAdj.Add(a1);
                    faceNew.arreteAdj.Add(a2);
                    listFace.Add(faceNew);
                    listArete.Add(a1);
                    listArete.Add(a2);
                }
            }
        }
    }
    
    bool isPointVisibleFromSegment(Vector2 point, SegmentsOld seg, List<SegmentsOld> segToCheck)
    {
        Vector2 midPoint = (seg.Point1 + seg.Point2)/2f;
        SegmentsOld tmpSeg = new SegmentsOld{Point1 = midPoint, Point2 = point};
        foreach (var curSeg in segToCheck)
        {
            if (DoSegmentsIntersect(tmpSeg, curSeg))
                return false;
        }
        return true;
    }
    
    bool isPointVisibleFromArete(Vector2 point, Arete seg, List<Arete> segToCheck)
    {
        Vector2 midPoint = (seg.somBas.coord + seg.somHaut.coord)/2f;
        SegmentsOld tmpSeg = new SegmentsOld{Point1 = midPoint, Point2 = point};
        foreach (var curAr in segToCheck)
        {
            SegmentsOld curSeg = new SegmentsOld() {Point1 = curAr.somBas.coord, Point2 = curAr.somHaut.coord};
            if (DoSegmentsIntersect(tmpSeg, curSeg))
                return false;
        }
        return true;
    }
    
    // Fonction pour vérifier si deux segments se croisent
    bool DoSegmentsIntersect(SegmentsOld segment1, SegmentsOld segment2)
    {
        double dx1 = segment1.Point2.x - segment1.Point1.x;
        double dy1 = segment1.Point2.y - segment1.Point1.y;
        double dx2 = segment2.Point2.x - segment2.Point1.x;
        double dy2 = segment2.Point2.y - segment2.Point1.y;

        double delta = dx1 * dy2 - dx2 * dy1;

        if (delta == 0)
        {
            // Les segments sont parallèles, ils ne se croisent pas (ou ils se superposent)
            return false;
        }

        double t1 = ((segment2.Point1.x - segment1.Point1.x) * dy2 - (segment2.Point1.y - segment1.Point1.y) * dx2) / delta;
        double t2 = ((segment2.Point1.x - segment1.Point1.x) * dy1 - (segment2.Point1.y - segment1.Point1.y) * dx1) / delta;

        return t1 is > 0.01 and < .99 && t2 is > 0.01 and < .99;
    }



    public void JarvisMarche()
    {
        float start = Time.realtimeSinceStartup;
        List<Vector2> jarvis = getJarvis(listPoints);
        
        float end = Time.realtimeSinceStartup;
        Timer = end - start;
        Timer = Timer * 1000;
        Debug.Log(end - start);
        int size = jarvis.Count;
        for (int i = 0; i < size; i++)
        {
            GameObject newGO = new GameObject();
            newGO.transform.parent = parent.transform;
            newGO.name = "sides";
            LineRenderer newLine = newGO.AddComponent<LineRenderer>();
            newLine.positionCount = 2;
            newLine.SetPosition(0, new Vector3(jarvis[i].x, jarvis[i].y, _nearClipPlaneWorldPoint));
            newLine.SetPosition(1, new Vector3(jarvis[(i + 1) % size].x, jarvis[(i + 1) % size].y, _nearClipPlaneWorldPoint));
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            lines.Add(newLine);
        }
    }

    public void GrahamScan()
    {
        float start = Time.realtimeSinceStartup;
       
        List<Vector2> graham = GetGrahamScan(listPoints);
        float end = Time.realtimeSinceStartup;
        Timer = end - start;
        Timer = Timer * 1000;
        Debug.Log(end - start);
        int size = graham.Count;
        for (int i = 0; i < size; i++)
        {
            GameObject newGO = new GameObject();
            newGO.name = "sides";
            newGO.transform.parent = parent.transform;
            LineRenderer newLine = newGO.AddComponent<LineRenderer>();
            newLine.positionCount = 2;
            newLine.SetPosition(0, new Vector3(graham[i].x, graham[i].y, _nearClipPlaneWorldPoint));
            newLine.SetPosition(1, new Vector3(graham[(i + 1) % size].x, graham[(i + 1) % size].y, _nearClipPlaneWorldPoint));
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            lines.Add(newLine);
        }
    }

    public void Triangulation()
    {
        // Sort the points (assuming ascending order)
        listPoints = listPoints.OrderBy(p => p.x).ThenBy(p => p.y).ToList();

        // Create the initial triangle
        Vector2 p0 = listPoints.First();
        Vector2 p1 = listPoints.Skip(1).First();
        Vector2 p2 = listPoints.Skip(2).First();

        ListSegments.Add(new SegmentsOld { Point1 = p0, Point2 = p1 });
        ListSegments.Add(new SegmentsOld { Point1 = p1, Point2 = p2 });
        ListSegments.Add(new SegmentsOld { Point1 = p2, Point2 = p0 });

        ListTriangles.Add(new TrianglesOld { Seg1 = ListSegments[0], Seg2 = ListSegments[1], Seg3 = ListSegments[2] });

        foreach (var curP in listPoints.Skip(3))
        {
            List<SegmentsOld> ListSegmentsToAdd = new List<SegmentsOld>(ListSegments);
            foreach (var curSeg in ListSegments)
            {
                if (isPointVisibleFromSegment(curP, curSeg, ListSegmentsToAdd))
                {
                    SegmentsOld newSeg1 = new SegmentsOld { Point1 = curP, Point2 = curSeg.Point1 };
                    SegmentsOld newSeg2 = new SegmentsOld { Point1 = curP, Point2 = curSeg.Point2 };
                    ListSegmentsToAdd.Add(newSeg1);
                    ListSegmentsToAdd.Add(newSeg2);
                    ListTriangles.Add(new TrianglesOld { Seg1 = curSeg, Seg2 = newSeg1, Seg3 = newSeg2 });
                }
            }
            ListSegments.Clear();
            ListSegments.AddRange(ListSegmentsToAdd);
        }

        int size = ListTriangles.Count;
        Debug.Log(size);

        for (int i = 0; i < size; i++)
        {
            GameObject newGO = new GameObject();
            newGO.transform.SetParent(parent.transform);
            newGO.name = "sides";
            LineRenderer newLine = newGO.AddComponent<LineRenderer>();
            newLine.positionCount = 6;
            newLine.SetPosition(0,
                new Vector3(ListTriangles[i].Seg1.Point1.x, ListTriangles[i].Seg1.Point1.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(1,
                new Vector3(ListTriangles[i].Seg1.Point2.x, ListTriangles[i].Seg1.Point2.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(2,
                new Vector3(ListTriangles[i].Seg2.Point1.x, ListTriangles[i].Seg2.Point1.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(3,
                new Vector3(ListTriangles[i].Seg2.Point2.x, ListTriangles[i].Seg2.Point2.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(4,
                new Vector3(ListTriangles[i].Seg3.Point1.x, ListTriangles[i].Seg3.Point1.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(5,
                new Vector3(ListTriangles[i].Seg3.Point2.x, ListTriangles[i].Seg3.Point2.y,
                    _nearClipPlaneWorldPoint));
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            lines.Add(newLine);
        }
    }
    
    public void DelaunayRefinementAlgorithm()
    {
        //On supprime les segments précédents
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            var go = parent.transform.GetChild(i).gameObject;
            if(go.TryGetComponent<LineRenderer>(out var t))
                Destroy(go);
        }
        lines.Clear();
        
        // Appliquez l'algorithme du flipping d'arêtes pour améliorer la triangulation
        bool flipped;
        int limit = 1000;
        do
        {
            flipped = false;
            limit--;
            for (int i = 0; i < ListTriangles.Count; i++)
            {
                for (int j = i+1; j < ListTriangles.Count; j++)
                {
                    if (TryFlipEdge(ListTriangles[i], ListTriangles[j], out TrianglesOld newTriangle1, out TrianglesOld newTriangle2))
                    {
                        // Le flip d'arête est possible, remplacez les triangles existants par les nouveaux
                        newTriangle1.flip = true;
                        newTriangle2.flip = true;
                        ListTriangles[i] = newTriangle1;
                        ListTriangles[j] = newTriangle2;
                        flipped = true;
                    }
                }
            }
        } while (flipped && limit > 0);

        int size = ListTriangles.Count;
        
        for (int i = 0; i < size; i++)
        {
            GameObject newGO = new GameObject();
            newGO.transform.SetParent(parent.transform);
            newGO.name = "sides";
            LineRenderer newLine = newGO.AddComponent<LineRenderer>();
            newLine.positionCount = 6;
            newLine.SetPosition(0,
                new Vector3(ListTriangles[i].Seg1.Point1.x, ListTriangles[i].Seg1.Point1.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(1,
                new Vector3(ListTriangles[i].Seg1.Point2.x, ListTriangles[i].Seg1.Point2.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(2,
                new Vector3(ListTriangles[i].Seg2.Point1.x, ListTriangles[i].Seg2.Point1.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(3,
                new Vector3(ListTriangles[i].Seg2.Point2.x, ListTriangles[i].Seg2.Point2.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(4,
                new Vector3(ListTriangles[i].Seg3.Point1.x, ListTriangles[i].Seg3.Point1.y,
                    _nearClipPlaneWorldPoint));
            newLine.SetPosition(5,
                new Vector3(ListTriangles[i].Seg3.Point2.x, ListTriangles[i].Seg3.Point2.y,
                    _nearClipPlaneWorldPoint));
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            lines.Add(newLine);
        }
    }
    
    // Fonction pour essayer de faire un flip d'arête entre deux triangles
    private bool TryFlipEdge(TrianglesOld triangle1, TrianglesOld triangle2, out TrianglesOld newTriangle1, out TrianglesOld newTriangle2)
    {
        // Recherchez une arête commune entre les deux triangles
        SegmentsOld commonEdge = FindCommonEdge(triangle1, triangle2);

        if (!float.IsPositiveInfinity(commonEdge.Point1.x))
        {
            // Vérifiez si le flip d'arête est possible
            Vector2 oppositePoint1 = FindOppositePoint(triangle1, commonEdge);
            Vector2 oppositePoint2 = FindOppositePoint(triangle2, commonEdge);

            if (IsPointInsideCircumcircle(oppositePoint2, triangle1) || IsPointInsideCircumcircle(oppositePoint1, triangle2))
            {
                // Effectuez le flip d'arête
                newTriangle1 = new TrianglesOld
                {
                    Seg1 = new SegmentsOld { Point1 = commonEdge.Point1, Point2 = oppositePoint2 },
                    Seg2 = new SegmentsOld { Point1 = oppositePoint1, Point2 = commonEdge.Point1 },
                    Seg3 = new SegmentsOld { Point1 = oppositePoint2, Point2 = oppositePoint1 }
                };

                newTriangle2 = new TrianglesOld
                {
                    Seg1 = new SegmentsOld { Point1 = commonEdge.Point2, Point2 = oppositePoint1 },
                    Seg2 = new SegmentsOld { Point1 = oppositePoint2, Point2 = commonEdge.Point2 },
                    Seg3 = new SegmentsOld { Point1 = oppositePoint1, Point2 = oppositePoint2 }
                };

                return true;
            }
        }

        // Aucun flip d'arête possible
        newTriangle1 = triangle1;
        newTriangle2 = triangle2;
        return false;
    }

    // Fonction pour trouver une arête commune entre deux triangles
    private SegmentsOld FindCommonEdge(TrianglesOld triangle1, TrianglesOld triangle2)
    {
        List<SegmentsOld> edges1 = new List<SegmentsOld> { triangle1.Seg1, triangle1.Seg2, triangle1.Seg3 };
        List<SegmentsOld> edges2 = new List<SegmentsOld> { triangle2.Seg1, triangle2.Seg2, triangle2.Seg3 };

        foreach (var edge1 in edges1)
        {
            foreach (var edge2 in edges2)
            {
                if (AreEdgesEqual(edge1, edge2))
                {
                    return edge1;
                }
            }
        }

        return new SegmentsOld() {Point1 = Vector2.positiveInfinity, Point2 = Vector2.positiveInfinity}; // Pas d'arête commune trouvée
    }
    
    // Fonction pour trouver le point opposé à une arête dans un triangle
    private Vector2 FindOppositePoint(TrianglesOld triangleOld, SegmentsOld commonEdge)
    {
        if (!TriangleContainsEdge(triangleOld, commonEdge))
        {
            // L'arête commune n'appartient pas au triangle, retournez le premier point du triangle
            return triangleOld.Seg1.Point1;
        }

        List<Vector2> points = new List<Vector2>();
        points.Add(triangleOld.Seg1.Point1);
        if(!points.Contains(triangleOld.Seg1.Point2))
            points.Add(triangleOld.Seg1.Point2);
        if(!points.Contains(triangleOld.Seg2.Point1))
            points.Add(triangleOld.Seg2.Point1);
        if(!points.Contains(triangleOld.Seg2.Point2))
            points.Add(triangleOld.Seg2.Point2);
        if(!points.Contains(triangleOld.Seg3.Point1))
            points.Add(triangleOld.Seg3.Point1);
        if(!points.Contains(triangleOld.Seg3.Point2))
            points.Add(triangleOld.Seg3.Point2);
        foreach (var p in points)
        {
            if (commonEdge.Point1 != p && commonEdge.Point2 != p)
                return p;
        }

        return points[0];
    }

    // Fonction pour vérifier si deux arêtes sont égales
    private bool AreEdgesEqual(SegmentsOld edge1, SegmentsOld edge2)
    {
        return (edge1.Point1 == edge2.Point1 && edge1.Point2 == edge2.Point2) ||
               (edge1.Point1 == edge2.Point2 && edge1.Point2 == edge2.Point1);
    }
    
    // Fonction pour vérifier si un triangle contient une arête
    private bool TriangleContainsEdge(TrianglesOld triangleOld, SegmentsOld edge)
    {
        return AreEdgesEqual(triangleOld.Seg1, edge) || AreEdgesEqual(triangleOld.Seg2, edge) || AreEdgesEqual(triangleOld.Seg3, edge);
    }

    public void DelaunayCore()
    {
        GameObject newGO = new GameObject();
        newGO.transform.SetParent(parent.transform); 
        newGO.name = "sides";
        LineRenderer newLine = newGO.AddComponent<LineRenderer>();
        newLine.positionCount = 4;
        newLine.SetPosition(0, new Vector3(listPoints[0].x, listPoints[0].y, _nearClipPlaneWorldPoint));
        newLine.SetPosition(1, new Vector3(listPoints[1].x, listPoints[1].y, _nearClipPlaneWorldPoint));
        newLine.SetPosition(2, new Vector3(listPoints[2].x, listPoints[2].y, _nearClipPlaneWorldPoint));
        newLine.SetPosition(3, new Vector3(listPoints[0].x, listPoints[0].y, _nearClipPlaneWorldPoint));
        newLine.startWidth = 0.05f;
        newLine.endWidth = 0.05f;
        lines.Add(newLine);
        Vector3 centerCircle = FonctionMath.GETCenterCircle(listPoints[0], listPoints[1], listPoints[2]);
        centerCircle.z = _nearClipPlaneWorldPoint;
        pointCenter.transform.position = centerCircle;
        GameObject center = Instantiate(pointCenter);
        center.transform.parent = parent.transform;
        center.name = "center";
    }

    public void Clear()
    {
        Timer = 0;
        lines.Clear();
        listPoints.Clear();
        listGameObjects.Clear();
        ListSegments.Clear();
        ListTriangles.Clear();
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
        clickMenu.SetActive(false);
    }

    public void voronoi()
    {
        Color color = Color.green;
        List<Vector2> convexHull = getJarvis(listPoints);
        Debug.Log(convexHull.Count);
        for (int i = 0; i < ListTriangles.Count; i++)
        {
            for (int j = i+1; j < ListTriangles.Count; j++)
            {
                SegmentsOld commonEdge = FindCommonEdge(ListTriangles[i], ListTriangles[j]);

                if (!float.IsPositiveInfinity(commonEdge.Point1.x))
                {
                    List<Vector2> points1 = new List<Vector2>();
                    points1.Add(ListTriangles[i].Seg1.Point1);
                    if(!points1.Contains(ListTriangles[i].Seg1.Point2))
                        points1.Add(ListTriangles[i].Seg1.Point2);
                    if(!points1.Contains(ListTriangles[i].Seg2.Point1))
                        points1.Add(ListTriangles[i].Seg2.Point1);
                    if(!points1.Contains(ListTriangles[i].Seg2.Point2))
                        points1.Add(ListTriangles[i].Seg2.Point2);
                    if(!points1.Contains(ListTriangles[i].Seg3.Point1))
                        points1.Add(ListTriangles[i].Seg3.Point1);
                    if(!points1.Contains(ListTriangles[i].Seg3.Point2))
                        points1.Add(ListTriangles[i].Seg3.Point2);
                    var center1 = GETCenterCircle(points1[0],points1[1],points1[2]);
                    
                    points1.Clear();
                    points1.Add(ListTriangles[j].Seg1.Point1);
                    if(!points1.Contains(ListTriangles[j].Seg1.Point2))
                        points1.Add(ListTriangles[j].Seg1.Point2);
                    if(!points1.Contains(ListTriangles[j].Seg2.Point1))
                        points1.Add(ListTriangles[j].Seg2.Point1);
                    if(!points1.Contains(ListTriangles[j].Seg2.Point2))
                        points1.Add(ListTriangles[j].Seg2.Point2);
                    if(!points1.Contains(ListTriangles[j].Seg3.Point1))
                        points1.Add(ListTriangles[j].Seg3.Point1);
                    if(!points1.Contains(ListTriangles[j].Seg3.Point2))
                        points1.Add(ListTriangles[j].Seg3.Point2);
                    var center2 = GETCenterCircle(points1[0],points1[1],points1[2]);

                    if (convexHull.Contains(points1[0]) && convexHull.Contains(points1[1]))
                    {
                        Debug.Log("test");
                        if (Mathf.Abs(Angle(points1[2] - points1[1], points1[2] - points1[0])) > Mathf.PI/2)
                        {
                            Vector2 segm = points1[0] - points1[1];
                            Vector2 norm = new Vector2(-segm.y, segm.x) * -10;
                            GameObject outGO = new GameObject();
                            outGO.transform.SetParent(parent.transform);
                            outGO.name = "outside";
                            LineRenderer outLine = outGO.AddComponent<LineRenderer>();
                            outLine.positionCount = 2;
                            outLine.SetPosition(0,
                                new Vector3(center2.x, center2.y, _nearClipPlaneWorldPoint));
                            outLine.SetPosition(1,
                                new Vector3(center2.x+norm.x, center2.y+norm.y, _nearClipPlaneWorldPoint));
                            outLine.startWidth = 0.05f;
                            outLine.endWidth = 0.05f;
                            outLine.startColor = color;
                            outLine.endColor = color;
                            outLine.material = mat;
                            lines.Add(outLine);
                        }
                    } 
                    if (convexHull.Contains(points1[1]) && convexHull.Contains(points1[2]))
                    {
                        Debug.Log("test");
                        if (Mathf.Abs(Angle(points1[0] - points1[1], points1[0] - points1[2])) > Mathf.PI/2)
                        {
                            Vector2 segm = points1[1] - points1[2];
                            Vector2 norm = new Vector2(-segm.y, segm.x) * -10;
                            GameObject outGO = new GameObject();
                            outGO.transform.SetParent(parent.transform);
                            outGO.name = "outside";
                            LineRenderer outLine = outGO.AddComponent<LineRenderer>();
                            outLine.positionCount = 2;
                            outLine.SetPosition(0,
                                new Vector3(center2.x+norm.x, center2.y+norm.y, _nearClipPlaneWorldPoint));
                            outLine.SetPosition(1,
                                new Vector3(norm.x, norm.y, _nearClipPlaneWorldPoint));
                            outLine.startWidth = 0.05f;
                            outLine.endWidth = 0.05f;
                            outLine.startColor = color;
                            outLine.endColor = color;
                            outLine.material = mat;
                            lines.Add(outLine);
                        }
                    }
                    if (convexHull.Contains(points1[0]) && convexHull.Contains(points1[2]))
                    {
                        Debug.Log("test");
                        if (Mathf.Abs(Angle(points1[1] - points1[0], points1[1] - points1[2])) > Mathf.PI/2)
                        {
                            Vector2 segm = points1[0] - points1[2];
                            Vector2 norm = new Vector2(-segm.y, segm.x) * -10;
                            GameObject outGO = new GameObject();
                            outGO.transform.SetParent(parent.transform);
                            outGO.name = "outside";
                            LineRenderer outLine = outGO.AddComponent<LineRenderer>();
                            outLine.positionCount = 2;
                            outLine.SetPosition(0,
                                new Vector3(center2.x+norm.x, center2.y+norm.y, _nearClipPlaneWorldPoint));
                            outLine.SetPosition(1,
                                new Vector3(norm.x, norm.y, _nearClipPlaneWorldPoint));
                            outLine.startWidth = 0.05f;
                            outLine.endWidth = 0.05f;
                            outLine.startColor = color;
                            outLine.endColor = color;
                            outLine.material = mat;
                            lines.Add(outLine);
                        }
                    }
                    
                    GameObject newGO = new GameObject();
                    newGO.transform.SetParent(parent.transform);
                    newGO.name = "sides";
                    LineRenderer newLine = newGO.AddComponent<LineRenderer>();
                    newLine.positionCount = 2;
                    newLine.SetPosition(0,
                        new Vector3(center1.x, center1.y, _nearClipPlaneWorldPoint));
                    newLine.SetPosition(1,
                        new Vector3(center2.x, center2.y, _nearClipPlaneWorldPoint));
                    newLine.startWidth = 0.05f;
                    newLine.endWidth = 0.05f;
                    newLine.startColor = color;
                    newLine.endColor = color;
                    newLine.material = mat;
                    lines.Add(newLine);
                }
            }
        }
        /*
        // Fonction pour générer le diagramme de Voronoi à partir de la triangulation de Delaunay
        List<Vector2> points = listPoints;
        // Initialisez les cellules du diagramme de Voronoi
        List<Cell> voronoiCells = new List<Cell>();
        for (int i = 0; i < points.Count; i++)
        {
            voronoiCells.Add(new Cell { Site = points[i], Vertices = new List<Vector2>() });
        }

        // Parcourez chaque triangle de la triangulation de Delaunay
        foreach (var triangle in ListTriangles)
        {
            // Trouvez le cercle circonscrit du triangle (centre et rayon)
            List<Vector2> points1 = new List<Vector2>();
            points1.Add(triangle.Seg1.Point1);
            if(!points1.Contains(triangle.Seg1.Point2))
                points1.Add(triangle.Seg1.Point2);
            if(!points1.Contains(triangle.Seg2.Point1))
                points1.Add(triangle.Seg2.Point1);
            if(!points1.Contains(triangle.Seg2.Point2))
                points1.Add(triangle.Seg2.Point2);
            if(!points1.Contains(triangle.Seg3.Point1))
                points1.Add(triangle.Seg3.Point1);
            if(!points1.Contains(triangle.Seg3.Point2))
                points1.Add(triangle.Seg3.Point2);
            Vector2 circumcenter = GETCenterCircle(points1[0],points1[1],points1[2]);
            float circumradius = Vector2.Distance(circumcenter, triangle.Seg1.Point1);

            // Trouvez l'index du site correspondant au cercle circonscrit
            int siteIndex = -1;
            for (int i = 0; i < points.Count; i++)
            {
                if (Vector2.Distance(points[i], circumcenter) < circumradius * 1.01f) // Tolerance pour éviter les erreurs d'arrondi
                {
                    siteIndex = i;
                    break;
                }
            }

            // Si le cercle circonscrit correspond à un site, ajoutez les vertices de la cellule
            if (siteIndex != -1)
            {
                voronoiCells[siteIndex].Vertices.Add(triangle.Seg1.Point1);
                voronoiCells[siteIndex].Vertices.Add(triangle.Seg2.Point1);
                voronoiCells[siteIndex].Vertices.Add(triangle.Seg3.Point1);
            }
        }
        */
        /*
        List<SegmentsOld> voronoiSegments = new List<SegmentsOld>();
        List<Vector2> convexHull = new List<Vector2>();

        // Parcourez chaque triangle de la triangulation de Delaunay
        foreach (var triangle in ListTriangles)
        {
            // Trouvez le cercle circonscrit du triangle (centre et rayon)
            List<Vector2> points1 = new List<Vector2>();
            points1.Add(triangle.Seg1.Point1);
            if (!points1.Contains(triangle.Seg1.Point2))
                points1.Add(triangle.Seg1.Point2);
            if (!points1.Contains(triangle.Seg2.Point1))
                points1.Add(triangle.Seg2.Point1);
            if (!points1.Contains(triangle.Seg2.Point2))
                points1.Add(triangle.Seg2.Point2);
            if (!points1.Contains(triangle.Seg3.Point1))
                points1.Add(triangle.Seg3.Point1);
            if (!points1.Contains(triangle.Seg3.Point2))
                points1.Add(triangle.Seg3.Point2);
            Vector2 circumcenter = GETCenterCircle(points1[0], points1[1], points1[2]);
            //float circumradius = Vector2.Distance(circumcenter, triangle.Seg1.Point1);

            if ((convexHull.Contains(triangle.Seg1.Point1) && convexHull.Contains(triangle.Seg1.Point2))
                || (convexHull.Contains(triangle.Seg2.Point1) && convexHull.Contains(triangle.Seg2.Point2)) || 
                    (convexHull.Contains(triangle.Seg3.Point1) && convexHull.Contains(triangle.Seg3.Point2)))
            {
                for
            }
            
            // Ajoutez les segments correspondants au cercle circonscrit
            voronoiSegments.Add(new SegmentsOld { Point1 = circumcenter, Point2 = (triangle.Seg1.Point1 + triangle.Seg1.Point2) / 2 });
            voronoiSegments.Add(new SegmentsOld { Point1 = circumcenter, Point2 = (triangle.Seg2.Point1 + triangle.Seg2.Point2) / 2 });
            voronoiSegments.Add(new SegmentsOld { Point1 = circumcenter, Point2 = (triangle.Seg3.Point1 + triangle.Seg3.Point2) / 2 });
        }
*/
        /*
        foreach (var c in voronoiSegments)
        {
            GameObject newGO = new GameObject();
            newGO.transform.SetParent(parent.transform);
            newGO.name = "sides";
            LineRenderer newLine = newGO.AddComponent<LineRenderer>();
            newLine.positionCount = 2;
            newLine.SetPosition(0,
                new Vector3(c.Point1.x, c.Point1.y, _nearClipPlaneWorldPoint));
            newLine.SetPosition(1,
                new Vector3(c.Point2.x, c.Point2.y, _nearClipPlaneWorldPoint));
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            lines.Add(newLine);
        }*/
        //return voronoiCells;

    }
 }
