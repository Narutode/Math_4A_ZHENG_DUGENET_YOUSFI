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
}

public class LineConstrutor : MonoBehaviour
{
    public Camera cam;
    GameObject _sommet;
    [FormerlySerializedAs("Line")] public LineRenderer curLine;

    public GameObject pointGO;
    public List<GameObject> listGameObjects;
    public List<Vector2> listPoints;
    public List<Segments> ListSegments = new List<Segments>();
    public List<Triangles> ListTriangles = new List<Triangles>();

    public List<LineRenderer> lines;
    private float _nearClipPlaneWorldPoint = 0;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click gauche
        {
            Debug.Log("place point");
            Vector3 point = new Vector3();
            Vector2 mousePos = Input.mousePosition;

            point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane + 1f));
            if (_nearClipPlaneWorldPoint == 0)
                _nearClipPlaneWorldPoint = point.z;
            pointGO.transform.position = point;
            GameObject newP = Instantiate(pointGO);
            /*
            if (listGameObjects.Count > 0)
            {
                GameObject newGO = new GameObject();
                newGO.name = "sides";
                LineRenderer newLine = newGO.AddComponent<LineRenderer>();
                newLine.positionCount = 2;
                newLine.SetPosition(0,listGameObjects.Last().transform.position);
                newLine.SetPosition(1,newP.transform.position);
                lines.Add(newLine);
            }
            */
            listGameObjects.Add(newP);
            listPoints.Add(newP.transform.position);
            
            if (listGameObjects.Count == 10)
            {
                /*
                //List<Vector2> jarvis = GetJarvis(listPoints);
                List<Vector2> jarvis = GetGrahamScan(listPoints);
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
                    List<Segments> ListSegmentsToAdd = new List<Segments>();
                    foreach (var curSeg in ListSegments)
                    {
                        if (isPointVisibleFromSegment(curP, curSeg))
                        {
                            Segments newSeg1 = new Segments {Point1 = curP, Point2 = curSeg.Point1};
                            Segments newSeg2 = new Segments {Point1 = curP, Point2 = curSeg.Point2};
                            ListSegmentsToAdd.Add(newSeg1);
                            ListSegmentsToAdd.Add(newSeg2);
                            ListTriangles.Add(new Triangles{Seg1 = curSeg, Seg2 = newSeg1, Seg3 = newSeg2});
                        }
                    }
                    ListSegments.AddRange(ListSegmentsToAdd);
                }
                
                int size = ListTriangles.Count;
                Debug.Log(size);
                for (int i = 0; i < size; i++)
                {
                    GameObject newGO = new GameObject();
                    newGO.name = "sides";
                    LineRenderer newLine = newGO.AddComponent<LineRenderer>();
                    newLine.positionCount = 6;
                    newLine.SetPosition(0,new Vector3(ListTriangles[i].Seg1.Point1.x,ListTriangles[i].Seg1.Point1.y, _nearClipPlaneWorldPoint));
                    newLine.SetPosition(1,new Vector3(ListTriangles[i].Seg1.Point2.x,ListTriangles[i].Seg1.Point2.y, _nearClipPlaneWorldPoint));
                    newLine.SetPosition(2,new Vector3(ListTriangles[i].Seg2.Point1.x,ListTriangles[i].Seg2.Point1.y, _nearClipPlaneWorldPoint));
                    newLine.SetPosition(3,new Vector3(ListTriangles[i].Seg2.Point2.x,ListTriangles[i].Seg2.Point2.y, _nearClipPlaneWorldPoint));
                    newLine.SetPosition(4,new Vector3(ListTriangles[i].Seg3.Point1.x,ListTriangles[i].Seg3.Point1.y, _nearClipPlaneWorldPoint));
                    newLine.SetPosition(5,new Vector3(ListTriangles[i].Seg3.Point2.x,ListTriangles[i].Seg3.Point2.y, _nearClipPlaneWorldPoint));
                    newLine.startWidth = 0.05f;
                    newLine.endWidth = 0.05f;
                    lines.Add(newLine);
                }
            }

            /*
                Vector3 centerCircle = fonctionMath.getCenterCircle(listPoints[0].transform.position, listPoints[1].transform.position, listPoints[2].transform.position);
                centerCircle.z = nearClipPlaneWorldPoint;
                pointGO.transform.position = centerCircle;
                GameObject center = Instantiate(pointGO);
                center.name = "center";
            */
        }
    }
    
    bool isPointVisibleFromSegment(Vector2 point, Segments seg)
    {
        Vector2 midPoint = (seg.Point1 + seg.Point2)/2f;
        Segments tmpSeg = new Segments{Point1 = midPoint, Point2 = point};
        foreach (var curSeg in ListSegments)
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

        return t1 is > 0.1 and < .9 && t2 is > 0.1 and < .9;
    }
}
