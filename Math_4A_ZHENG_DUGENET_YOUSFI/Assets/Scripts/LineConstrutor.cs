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

public struct Segments
{
    public Vector2 Point1;
    public Vector2 Point2;
}

public struct Triangles
{
    public Segments Seg1;
    public Segments Seg2;
    public Segments Seg3;
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
    public List<Vector2> listPoints = new List<Vector2>();
    public List<Segments> ListSegments = new List<Segments>();
    public List<Triangles> ListTriangles = new List<Triangles>();

    public List<Sommet> listSommet = new List<Sommet>();
    public List<Arete> listArete = new List<Arete>();
    public List<Face> listFace = new List<Face>();

    public List<LineRenderer> lines;
    private float _nearClipPlaneWorldPoint = 0;

    [SerializeField]public static float Timer;

    public GameObject clickMenu;
    public GameObject parent;
    public GameObject voronoiParent;
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

            if(listPoints.Count >= 3)
            {                
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

    List<Triangles> DelaunayIncremental(List<Triangles> listTri, Vector2 point)
    {
        List<Triangles> triangles = new List<Triangles>();

        triangles.AddRange(listTri);
        List<Segments> segVisible = new List<Segments>();
        List<Segments> segToCheck = new List<Segments>();
        List<Vector2> convexHull = getJarvis(listPoints);

        foreach (var triangle2 in triangles)
        {
            segToCheck.Add(triangle2.Seg1);
            segToCheck.Add(triangle2.Seg2);
            segToCheck.Add(triangle2.Seg3);
        }

        // Liste temporaire pour stocker les nouveaux triangles formés
        List<Triangles> newTriangles = new List<Triangles>();
        List<Triangles> visibleTriangles = new List<Triangles>();

        //On cherche les segments visibles
        foreach (var triangle in triangles)
        {
            if (IsPointInsideTriangle(point, triangle))
            {
                List<Vector2> points = getPointsFromTriangle(triangle);

                listTri.Remove(triangle);

                // Le point est à l'intérieur d'un triangle
                newTriangles.Add(new Triangles
                {
                    Seg1 = new Segments { Point1 = point, Point2 = points[0] },
                    Seg2 = new Segments { Point1 = point, Point2 = points[1] },
                    Seg3 = new Segments { Point1 = points[0], Point2 = points[1] }
                });
                segToCheck.Add(newTriangles.Last().Seg1);
                segToCheck.Add(newTriangles.Last().Seg2);
                segToCheck.Add(newTriangles.Last().Seg3);
                newTriangles.Add(new Triangles
                {
                    Seg1 = new Segments { Point1 = point, Point2 = points[1] },
                    Seg2 = new Segments { Point1 = point, Point2 = points[2] },
                    Seg3 = new Segments { Point1 = points[1], Point2 = points[2] }
                });
                segToCheck.Add(newTriangles.Last().Seg1);
                segToCheck.Add(newTriangles.Last().Seg2);
                segToCheck.Add(newTriangles.Last().Seg3);
                newTriangles.Add(new Triangles
                {
                    Seg1 = new Segments { Point1 = point, Point2 = points[2] },
                    Seg2 = new Segments { Point1 = point, Point2 = points[0] },
                    Seg3 = new Segments { Point1 = points[2], Point2 = points[0] }
                });
                segToCheck.Add(newTriangles.Last().Seg1);
                segToCheck.Add(newTriangles.Last().Seg2);
                segToCheck.Add(newTriangles.Last().Seg3);

                return listTri;
            }
            bool visible = false;
            if (isPointVisibleFromSegment(point, triangle.Seg1, segToCheck))
            {
                if(!segVisible.Any(t => (t.Point1.Equals(triangle.Seg1.Point1) || t.Point1.Equals(triangle.Seg1.Point2)) &&
                    (t.Point2.Equals(triangle.Seg1.Point1) || t.Point2.Equals(triangle.Seg1.Point2)))) {
                    segVisible.Add(triangle.Seg1);
                    visible = true;
                }
            }
            if (isPointVisibleFromSegment(point, triangle.Seg2, segToCheck))
            {
                if (!segVisible.Any(t => (t.Point1.Equals(triangle.Seg2.Point1) || t.Point1.Equals(triangle.Seg2.Point2)) &&
                    (t.Point2.Equals(triangle.Seg2.Point1) || t.Point2.Equals(triangle.Seg2.Point2))))
                {
                    segVisible.Add(triangle.Seg2);
                    visible = true;
                }
            }
            if (isPointVisibleFromSegment(point, triangle.Seg3, segToCheck))
            {
                if (!segVisible.Any(t => (t.Point1.Equals(triangle.Seg3.Point1) || t.Point1.Equals(triangle.Seg3.Point2)) &&
                     (t.Point2.Equals(triangle.Seg3.Point1) || t.Point2.Equals(triangle.Seg3.Point2))))
                {
                    segVisible.Add(triangle.Seg3);
                    visible = true;
                }
            }
            if(!visible)
            {
                newTriangles.Add(triangle);
            }
        }
        while(segVisible.Count > 0)
        {
            var segV = segVisible.First();
            segVisible.Remove(segV);
            bool matchTri = triangles.Any(t => t.Seg1.Equals(segV) || t.Seg2.Equals(segV) || t.Seg3.Equals(segV));
            Triangles triangle = new Triangles();
            if (matchTri)
                triangle = triangles.First(t => t.Seg1.Equals(segV) || t.Seg2.Equals(segV) || t.Seg3.Equals(segV));

            // Le point est à l'intérieur du cercle circonscrit on supprime le triangle et on ajoute les deux autres segments à la liste
            if (matchTri && IsPointInsideCircumcircle(point, triangle))
            {
                List<Vector2> points = getPointsFromTriangle(triangle);

                if (!triangle.Seg1.Equals(segV))
                    segVisible.Add(triangle.Seg1);
                if (!triangle.Seg2.Equals(segV))
                    segVisible.Add(triangle.Seg2);
                if (!triangle.Seg3.Equals(segV))
                    segVisible.Add(triangle.Seg3);

                triangles.Remove(triangle);
            }
            else
            {
                // Le point n'est pas à l'intérieur du cercle circonscrit
                if(matchTri)
                    newTriangles.Add(triangle);

                newTriangles.Add(new Triangles
                {
                    Seg1 = new Segments { Point1 = point, Point2 = segV.Point1 },
                    Seg2 = new Segments { Point1 = point, Point2 = segV.Point2 },
                    Seg3 = new Segments { Point1 = segV.Point1, Point2 = segV.Point2 },
                });
                segToCheck.Add(newTriangles.Last().Seg1);
                segToCheck.Add(newTriangles.Last().Seg2);
                segToCheck.Add(newTriangles.Last().Seg3);
            }
        }

        return newTriangles;
    }

    bool IsPointInsideTriangle(Vector2 point, Triangles triangle)
    {
        float sign1 = Sign(point, triangle.Seg1.Point1, triangle.Seg1.Point2);
        float sign2 = Sign(point, triangle.Seg2.Point1, triangle.Seg2.Point2);
        float sign3 = Sign(point, triangle.Seg3.Point1, triangle.Seg3.Point2);

        return !((sign1 >= 0 || sign2 >= 0 || sign3 >= 0) && (sign1 <= 0 || sign2 <= 0 || sign3 <= 0));
    }

    bool IsPointVisibleFromSegment(Vector2 point, Segments segment)
    {
        Vector2 AB = segment.Point2 - segment.Point1;
        Vector2 AP = point - segment.Point1;

        float crossProduct = Vector3.Cross(new Vector3(AB.x, AB.y, 0), new Vector3(AP.x, AP.y, 0)).z;

        // Vérifiez si le point est à gauche du segment (produit vectoriel positif)
        return crossProduct >= 0;
    }

    float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    bool IsPointVisibleFromTriangle(Vector2 point, Triangles triangle)
    {
        // Vérifiez si le point est à l'extérieur du triangle en utilisant la méthode du produit vectoriel
        Vector2 AB = triangle.Seg1.Point2 - triangle.Seg1.Point1;
        Vector2 BC = triangle.Seg2.Point2 - triangle.Seg2.Point1;
        Vector2 CA = triangle.Seg3.Point2 - triangle.Seg3.Point1;

        Vector2 AP = point - triangle.Seg1.Point1;
        Vector2 BP = point - triangle.Seg2.Point1;
        Vector2 CP = point - triangle.Seg3.Point1;

        float crossABP = Vector3.Cross(new Vector3(AB.x, AB.y, 0), new Vector3(AP.x, AP.y, 0)).z;
        float crossBCP = Vector3.Cross(new Vector3(BC.x, BC.y, 0), new Vector3(BP.x, BP.y, 0)).z;
        float crossCAP = Vector3.Cross(new Vector3(CA.x, CA.y, 0), new Vector3(CP.x, CP.y, 0)).z;

        return (crossABP >= 0 && crossBCP >= 0 && crossCAP >= 0) || (crossABP <= 0 && crossBCP <= 0 && crossCAP <= 0);
    }

    bool isPointVisibleFromSegment(Vector2 point, Segments seg, List<Segments> segToCheck)
    {
        Vector2 midPoint = (seg.Point1 + seg.Point2)/2f;
        Segments tmpSeg = new Segments{Point1 = midPoint, Point2 = point};
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
        Segments tmpSeg = new Segments{Point1 = midPoint, Point2 = point};
        foreach (var curAr in segToCheck)
        {
            Segments curSeg = new Segments() {Point1 = curAr.somBas.coord, Point2 = curAr.somHaut.coord};
            if (DoSegmentsIntersect(tmpSeg, curSeg))
                return false;
        }
        return true;
    }
    
    // Fonction pour vérifier si deux segments se croisent
    bool DoSegmentsIntersect(Segments segment1, Segments segment2)
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

        return t1 is > 0.001 and < .999 && t2 is > 0.001 and < .999;
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
        ListTriangles.Clear();
        // Sort the points (assuming ascending order)
        listPoints = listPoints.OrderBy(p => p.x).ThenBy(p => p.y).ToList();

        // Create the initial triangle
        Vector2 p0 = listPoints.First();
        Vector2 p1 = listPoints.Skip(1).First();
        Vector2 p2 = listPoints.Skip(2).First();

        ListSegments.Add(new Segments { Point1 = p0, Point2 = p1 });
        ListSegments.Add(new Segments { Point1 = p1, Point2 = p2 });
        ListSegments.Add(new Segments { Point1 = p2, Point2 = p0 });

        ListTriangles.Add(new Triangles { Seg1 = ListSegments[0], Seg2 = ListSegments[1], Seg3 = ListSegments[2] });
        List<Segments> ListSegmentsToAdd = new List<Segments>(ListSegments);

        foreach (var curP in listPoints.Skip(3))
        {
            foreach (var curSeg in ListSegments)
            {
                if (isPointVisibleFromSegment(curP, curSeg, ListSegmentsToAdd))
                {
                    Segments newSeg1 = new Segments { Point1 = curP, Point2 = curSeg.Point1 };
                    Segments newSeg2 = new Segments { Point1 = curP, Point2 = curSeg.Point2 };
                    ListSegmentsToAdd.Add(newSeg1);
                    ListSegmentsToAdd.Add(newSeg2);
                    ListTriangles.Add(new Triangles { Seg1 = curSeg, Seg2 = newSeg1, Seg3 = newSeg2 });
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
                    if (TryFlipEdge(ListTriangles[i], ListTriangles[j], out Triangles newTriangle1, out Triangles newTriangle2))
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
    private bool TryFlipEdge(Triangles triangle1, Triangles triangle2, out Triangles newTriangle1, out Triangles newTriangle2)
    {
        // Recherchez une arête commune entre les deux triangles
        Segments commonEdge = FindCommonEdge(triangle1, triangle2);

        if (!float.IsPositiveInfinity(commonEdge.Point1.x))
        {
            // Vérifiez si le flip d'arête est possible
            Vector2 oppositePoint1 = FindOppositePoint(triangle1, commonEdge);
            Vector2 oppositePoint2 = FindOppositePoint(triangle2, commonEdge);

            if (IsPointInsideCircumcircle(oppositePoint2, triangle1) || IsPointInsideCircumcircle(oppositePoint1, triangle2))
            {
                // Effectuez le flip d'arête
                newTriangle1 = new Triangles
                {
                    Seg1 = new Segments { Point1 = commonEdge.Point1, Point2 = oppositePoint2 },
                    Seg2 = new Segments { Point1 = oppositePoint1, Point2 = commonEdge.Point1 },
                    Seg3 = new Segments { Point1 = oppositePoint2, Point2 = oppositePoint1 }
                };

                newTriangle2 = new Triangles
                {
                    Seg1 = new Segments { Point1 = commonEdge.Point2, Point2 = oppositePoint1 },
                    Seg2 = new Segments { Point1 = oppositePoint2, Point2 = commonEdge.Point2 },
                    Seg3 = new Segments { Point1 = oppositePoint1, Point2 = oppositePoint2 }
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
    private Segments FindCommonEdge(Triangles triangle1, Triangles triangle2)
    {
        List<Segments> edges1 = new List<Segments> { triangle1.Seg1, triangle1.Seg2, triangle1.Seg3 };
        List<Segments> edges2 = new List<Segments> { triangle2.Seg1, triangle2.Seg2, triangle2.Seg3 };

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

        return new Segments() {Point1 = Vector2.positiveInfinity, Point2 = Vector2.positiveInfinity}; // Pas d'arête commune trouvée
    }
    
    // Fonction pour trouver le point opposé à une arête dans un triangle
    private Vector2 FindOppositePoint(Triangles triangleOld, Segments commonEdge)
    {
        if (!TriangleContainsEdge(triangleOld, commonEdge))
        {
            // L'arête commune n'appartient pas au triangle, retournez le premier point du triangle
            return triangleOld.Seg1.Point1;
        }

        List<Vector2> points = getPointsFromTriangle(triangleOld);
        foreach (var p in points)
        {
            if (commonEdge.Point1 != p && commonEdge.Point2 != p)
                return p;
        }

        return points[0];
    }

    // Fonction pour vérifier si deux arêtes sont égales
    private bool AreEdgesEqual(Segments edge1, Segments edge2)
    {
        return (edge1.Point1 == edge2.Point1 && edge1.Point2 == edge2.Point2) ||
               (edge1.Point1 == edge2.Point2 && edge1.Point2 == edge2.Point1);
    }
    
    // Fonction pour vérifier si un triangle contient une arête
    private bool TriangleContainsEdge(Triangles triangleOld, Segments edge)
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
        Vector3 center1, center2;
        foreach(var c in convexHull)
            Debug.Log(c);
        //pour chaque segment cheque si triangle adjacent (sinon check angle)
        List<Vector2> points1 = new List<Vector2>();
        List<Vector2> points2 = new List<Vector2>();

        for (int i = 0; i < ListTriangles.Count; i++)
        {
            points1 = getPointsFromTriangle(ListTriangles[i]);
            if (points1.Count < 3)
                continue;
            center1 = GETCenterCircle(points1[0], points1[1], points1[2]);

            //Cas des triangles adjacent
            for (int j = i+1; j < ListTriangles.Count; j++)
            {
                Segments commonEdge = FindCommonEdge(ListTriangles[i], ListTriangles[j]);

                if (!float.IsPositiveInfinity(commonEdge.Point1.x))
                {
                    points2 = getPointsFromTriangle(ListTriangles[j]);
                    if (points2.Count < 3)
                        continue;
                    center2 = GETCenterCircle(points2[0], points2[1], points2[2]);

                    GameObject newGO = new GameObject();
                    newGO.transform.SetParent(voronoiParent.transform);
                    newGO.name = "sides";
                    LineRenderer newLine = newGO.AddComponent<LineRenderer>();
                    newLine.positionCount = 2;
                    newLine.SetPosition(0,
                        new Vector3(center1.x, center1.y, _nearClipPlaneWorldPoint));
                    newLine.SetPosition(1,
                        new Vector3(center2.x, center2.y, _nearClipPlaneWorldPoint));
                    newLine.startWidth = 0.05f;
                    newLine.endWidth = 0.05f;
                    newLine.startColor = Color.yellow;
                    newLine.endColor = Color.yellow;
                    newLine.material = mat;
                    lines.Add(newLine);
                }
            }

            //Cas adjacent à l'enveloppe convexe
            if (convexHull.Contains(points1[0]) || convexHull.Contains(points1[1]) || convexHull.Contains(points1[2]))
            {
                Vector2 p1, p2, p3;
                int sizeMax = convexHull.Count;
                for (int c = 0; c < sizeMax; c++)
                {
                    if (points1.Any(t => t.Equals(convexHull[c])))
                    {
                        p1 = points1.First(t => t.Equals(convexHull[c]));
                        if(points1.Any(t => t.Equals(convexHull[(c + 1) % sizeMax])))
                        {
                            p2 = points1.First(t => t.Equals(convexHull[(c + 1) % sizeMax]));
                            p3 = points1.First(t => !t.Equals(p1) && !t.Equals(p2));
                           
                            Vector2 segm = p1 - p2;
                            Vector2 norm = new Vector2(-segm.y, segm.x) * 10;
                            GameObject outGO = new GameObject();
                            outGO.transform.SetParent(voronoiParent.transform);
                            outGO.name = "outside";
                            LineRenderer outLine = outGO.AddComponent<LineRenderer>();
                            outLine.positionCount = 2;
                            outLine.SetPosition(0,
                                new Vector3(center1.x, center1.y, _nearClipPlaneWorldPoint));
                            outLine.SetPosition(1,
                                new Vector3(center1.x + norm.x, center1.y + norm.y, _nearClipPlaneWorldPoint));
                            outLine.startWidth = 0.05f;
                            outLine.endWidth = 0.05f;
                            outLine.startColor = Color.yellow;
                            outLine.endColor = Color.yellow;
                            outLine.material = mat;
                            lines.Add(outLine);                         
                        }
                    }
                }
            }   
        }
    }

    List<Vector2> getPointsFromTriangle(Triangles tri)
    {
        var points2 = new List<Vector2>();
        points2.Add(tri.Seg1.Point1);
        if (!points2.Contains(tri.Seg1.Point2))
            points2.Add(tri.Seg1.Point2);
        if (!points2.Contains(tri.Seg2.Point1))
            points2.Add(tri.Seg2.Point1);
        if (!points2.Contains(tri.Seg2.Point2))
            points2.Add(tri.Seg2.Point2);
        if (!points2.Contains(tri.Seg3.Point1))
            points2.Add(tri.Seg3.Point1);
        if (!points2.Contains(tri.Seg3.Point2))
            points2.Add(tri.Seg3.Point2);
        return points2;
    }
 }
