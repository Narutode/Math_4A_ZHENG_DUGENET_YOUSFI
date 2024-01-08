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
    public List<Segments> ListSegments = new List<Segments>();
    public List<Triangles> ListTriangles = new List<Triangles>();

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

        ListSegments.Add(new Segments { Point1 = p0, Point2 = p1 });
        ListSegments.Add(new Segments { Point1 = p1, Point2 = p2 });
        ListSegments.Add(new Segments { Point1 = p2, Point2 = p0 });

        ListTriangles.Add(new Triangles { Seg1 = ListSegments[0], Seg2 = ListSegments[1], Seg3 = ListSegments[2] });

        foreach (var curP in listPoints.Skip(3))
        {
            List<Segments> ListSegmentsToAdd = new List<Segments>(ListSegments);
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
    private Vector2 FindOppositePoint(Triangles triangle, Segments commonEdge)
    {
        if (!TriangleContainsEdge(triangle, commonEdge))
        {
            // L'arête commune n'appartient pas au triangle, retournez le premier point du triangle
            return triangle.Seg1.Point1;
        }

        List<Vector2> points = new List<Vector2>();
        points.Add(triangle.Seg1.Point1);
        if(!points.Contains(triangle.Seg1.Point2))
            points.Add(triangle.Seg1.Point2);
        if(!points.Contains(triangle.Seg2.Point1))
            points.Add(triangle.Seg2.Point1);
        if(!points.Contains(triangle.Seg2.Point2))
            points.Add(triangle.Seg2.Point2);
        if(!points.Contains(triangle.Seg3.Point1))
            points.Add(triangle.Seg3.Point1);
        if(!points.Contains(triangle.Seg3.Point2))
            points.Add(triangle.Seg3.Point2);
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
    private bool TriangleContainsEdge(Triangles triangle, Segments edge)
    {
        return AreEdgesEqual(triangle.Seg1, edge) || AreEdgesEqual(triangle.Seg2, edge) || AreEdgesEqual(triangle.Seg3, edge);
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
        /*
        for (int i = 0; i < ListTriangles.Count; i++)
        {
            for (int j = i+1; j < ListTriangles.Count; j++)
            {
                Segments commonEdge = FindCommonEdge(ListTriangles[i], ListTriangles[j]);

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
        }*/
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

        List<Segments> voronoiSegments = new List<Segments>();

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
            float circumradius = Vector2.Distance(circumcenter, triangle.Seg1.Point1);

            // Ajoutez les segments correspondants au cercle circonscrit
            voronoiSegments.Add(new Segments { Point1 = circumcenter, Point2 = (triangle.Seg1.Point1 + triangle.Seg1.Point2)/2 });
            voronoiSegments.Add(new Segments { Point1 = circumcenter, Point2 = (triangle.Seg2.Point1 + triangle.Seg2.Point2) / 2 });
            voronoiSegments.Add(new Segments { Point1 = circumcenter, Point2 = (triangle.Seg3.Point1 + triangle.Seg3.Point2) / 2 });
        }


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
        }
        //return voronoiCells;

    }
 }
