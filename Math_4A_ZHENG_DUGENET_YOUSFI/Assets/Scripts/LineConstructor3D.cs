using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FonctionMath;

public struct Tetraedre
{
    public Vector3 Vertex1;
    public Vector3 Vertex2;
    public Vector3 Vertex3;
    public Vector3 Vertex4;

    public Tetraedre(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        Vertex1 = v1;
        Vertex2 = v2;
        Vertex3 = v3;
        Vertex4 = v4;
    }
}
public class LineConstructor3D : MonoBehaviour
{
    public GameObject pointGO;
    public GameObject pointCenter;
    public List<GameObject> listGameObjects;
    public List<Vector3> listPoints;
    public List<SegmentsOld> ListSegments = new List<SegmentsOld>();
    public List<TrianglesOld> ListTriangles = new List<TrianglesOld>();
    public List<Tetraedre> ListTetraedre = new List<Tetraedre>();

    public List<LineRenderer> lines;
    public int nbPoints;

    public GameObject clickMenu;
    public GameObject parent;

    public void placePoint()
    {
        for(int i = 0; i < nbPoints; i++)
        {
            Vector3 point = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            pointGO.transform.position = point;
            GameObject newP = Instantiate(pointGO);
            newP.transform.parent = parent.transform;
            listGameObjects.Add(newP);
            listPoints.Add(newP.transform.position);
        }
    }

    public void DrawHull()
    {
        float start = Time.realtimeSinceStartup;

        /*
        //List<Triangle3D> graham = GetConvexHullTriangles(listPoints);
        //Debug.Log("graham : " + graham.Count);
        float end = Time.realtimeSinceStartup;
        Debug.Log(end - start);
        //int size = graham.Count;
        for (int i = 0; i < size; i++)
        {
            GameObject newGO = new GameObject();
            newGO.name = "sides";
            newGO.transform.parent = parent.transform;
            LineRenderer newLine = newGO.AddComponent<LineRenderer>();
            newLine.positionCount = 3;

            newLine.SetPosition(0, graham[i].Vertex1);
            newLine.SetPosition(1, graham[i].Vertex2);
            newLine.SetPosition(2, graham[i].Vertex3);
            
            newLine.loop = true;
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            lines.Add(newLine);
        }*/
    }
    

    // Fonction pour trouver le point le plus bas
    private static Vector3 FindLowestPoint(List<Vector3> pointList)
    {
        Vector3 lowestPoint = pointList[0];

        foreach (Vector3 point in pointList)
        {
            if (point.y < lowestPoint.y || (point.y == lowestPoint.y && point.x < lowestPoint.x))
            {
                lowestPoint = point;
            }
        }

        return lowestPoint;
    }

    // Fonction pour trouver le prochain point le plus éloigné dans la direction actuelle
    private static Vector3 FindNextPoint(List<Vector3> pointList, Vector3 currentPoint, List<Tetraedre> convexHullTriangles)
    {
        Vector3 nextPoint = pointList[0];

        foreach (Vector3 candidatePoint in pointList)
        {
            if (candidatePoint != currentPoint && !IsPointInsideConvexHull(candidatePoint, convexHullTriangles))
            {
                Vector3 directionToCandidate = candidatePoint - currentPoint;
                Vector3 directionToNextPoint = nextPoint - currentPoint;

                if (Vector3.Cross(directionToCandidate, directionToNextPoint).sqrMagnitude < Mathf.Epsilon)
                {
                    float distanceToCandidate = Vector3.Distance(currentPoint, candidatePoint);
                    float distanceToNextPoint = Vector3.Distance(currentPoint, nextPoint);

                    if (distanceToCandidate > distanceToNextPoint)
                    {
                        nextPoint = candidatePoint;
                    }
                }
                else if (Vector3.Cross(directionToCandidate, directionToNextPoint).y > 0)
                {
                    nextPoint = candidatePoint;
                }
            }
        }

        return nextPoint;
    }

    // Fonction pour vérifier si un point est à l'intérieur de l'enveloppe convexe
    private static bool IsPointInsideConvexHull(Vector3 point, List<Tetraedre> convexHullTriangles)
    {
        foreach (var triangle in convexHullTriangles)
        {
            if (IsPointInTriangle3D(point, triangle))
            {
                return true;
            }
        }
        return false;
    }

    // Fonction pour vérifier si un point est à l'intérieur d'un triangle en 3D
    private static bool IsPointInTriangle3D(Vector3 point, Tetraedre triangle)
    {
        Vector3 normal = Vector3.Cross(triangle.Vertex2 - triangle.Vertex1, triangle.Vertex3 - triangle.Vertex1).normalized;
        float dot1 = Vector3.Dot(normal, Vector3.Cross(triangle.Vertex2 - triangle.Vertex1, point - triangle.Vertex1));
        float dot2 = Vector3.Dot(normal, Vector3.Cross(triangle.Vertex3 - triangle.Vertex2, point - triangle.Vertex2));
        float dot3 = Vector3.Dot(normal, Vector3.Cross(triangle.Vertex1 - triangle.Vertex3, point - triangle.Vertex3));

        return (dot1 >= 0 && dot2 >= 0 && dot3 >= 0) || (dot1 <= 0 && dot2 <= 0 && dot3 <= 0);
    }
}
