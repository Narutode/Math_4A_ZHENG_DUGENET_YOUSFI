using System.Collections.Generic;
using UnityEngine;

public class LineConstructor3D : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject parent;
    public List<Vector3> points;
    public List<LineRenderer> lines;

    /*
    void Start()
    {
        GeneratePoints(20); // Vous pouvez ajuster le nombre de points selon vos besoins
        DrawConvexHull();
    }
    */

   public  void GeneratePoints()
    {
        points.Clear();

        for (int i = 0; i < 10; i++)
        {
            Vector3 point = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            InstantiatePoint(point);
            points.Add(point);
        }
    }

    void InstantiatePoint(Vector3 position)
    {
        GameObject newP = Instantiate(pointPrefab, position, Quaternion.identity);
        newP.transform.parent = parent.transform;
    }

    public void DrawConvexHull()
    {
        List<Tetrahedron> convexHull = GetConvexHull(points);

        foreach (var tetraedron in convexHull)
        {
            DrawTriangle(tetraedron.Vertex1, tetraedron.Vertex2, tetraedron.Vertex3);
            DrawTriangle(tetraedron.Vertex1, tetraedron.Vertex3, tetraedron.Vertex4);
            DrawTriangle(tetraedron.Vertex1, tetraedron.Vertex2, tetraedron.Vertex4);
            DrawTriangle(tetraedron.Vertex2, tetraedron.Vertex3, tetraedron.Vertex4);
        }
    }


    List<Tetrahedron> GetConvexHull(List<Vector3> pointCloud)
    {
        List<Tetrahedron> convexHull = new List<Tetrahedron>();

        // Trouver le point le plus bas comme point initial
        Vector3 lowestPoint = FindLowestPoint(pointCloud);
        InstantiatePoint(lowestPoint);

        // Trouver le deuxième point en utilisant le point le plus bas comme point initial
        Vector3 referencePoint = new Vector3(lowestPoint.x + 1, lowestPoint.y, lowestPoint.z);
        Vector3 secondPoint = FindNextPoint(pointCloud, lowestPoint, referencePoint, null);
        InstantiatePoint(secondPoint);

        // Ajouter ces deux points au convex hull
        convexHull.Add(new Tetrahedron(lowestPoint, referencePoint, secondPoint, secondPoint));

        // Trouver les autres points pour construire le convex hull
        while (true)
        {
            Vector3 nextPoint = FindNextPoint(pointCloud, lowestPoint, referencePoint, convexHull);

            if (nextPoint == Vector3.zero)
                break;

            InstantiatePoint(nextPoint);
            convexHull.Add(new Tetrahedron(lowestPoint, referencePoint, secondPoint, nextPoint));

            // Mettre à jour les points de référence pour le prochain tour
            referencePoint = secondPoint;
            secondPoint = nextPoint;
        }

        return convexHull;
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
    Vector3 FindNextPoint(List<Vector3> pointList, Vector3 currentPoint, Vector3 referencePoint, List<Tetrahedron> convexHull)
    {
        Vector3 nextPoint = Vector3.zero;
        float maxCosine = -1;

        foreach (Vector3 candidatePoint in pointList)
        {
            if (candidatePoint != currentPoint && candidatePoint != referencePoint &&
                !IsPointInsideConvexHull(candidatePoint, convexHull))
            {
                float cosine = CalculateCosine(currentPoint, referencePoint, candidatePoint);

                if (cosine > maxCosine)
                {
                    maxCosine = cosine;
                    nextPoint = candidatePoint;
                }
            }
        }

        return nextPoint;
    }


    // Fonction pour vérifier si un point est à l'intérieur d'un triangle en 3D
    private static bool IsPointInTriangle3D(Vector3 point, Tetrahedron triangle)
    {
        Vector3 normal = Vector3.Cross(triangle.Vertex2 - triangle.Vertex1, triangle.Vertex3 - triangle.Vertex1).normalized;
        float dot1 = Vector3.Dot(normal, Vector3.Cross(triangle.Vertex2 - triangle.Vertex1, point - triangle.Vertex1));
        float dot2 = Vector3.Dot(normal, Vector3.Cross(triangle.Vertex3 - triangle.Vertex2, point - triangle.Vertex2));
        float dot3 = Vector3.Dot(normal, Vector3.Cross(triangle.Vertex1 - triangle.Vertex3, point - triangle.Vertex3));


        return (dot1 >= 0 && dot2 >= 0 && dot3 >= 0) || (dot1 <= 0 && dot2 <= 0 && dot3 <= 0);

    }

    private static bool IsPointInsideConvexHull(Vector3 point, List<Tetrahedron> convexHullTriangles)
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



    float CalculateCosine(Vector3 currentPoint, Vector3 referencePoint, Vector3 candidatePoint)
    {
        Vector3 vec1 = referencePoint - currentPoint;
        Vector3 vec2 = candidatePoint - currentPoint;

        return Vector3.Dot(vec1.normalized, vec2.normalized);
    }



    public struct Tetrahedron
    {
        public Vector3 Vertex1;
        public Vector3 Vertex2;
        public Vector3 Vertex3;
        public Vector3 Vertex4;

        public Tetrahedron(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            Vertex1 = v1;
            Vertex2 = v2;
            Vertex3 = v3;
            Vertex4 = v4;
        }

    }
        void DrawTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            GameObject newGO = new GameObject();
            newGO.name = "sides";
            newGO.transform.parent = parent.transform;
            LineRenderer newLine = newGO.AddComponent<LineRenderer>();
            newLine.positionCount = 4;

            newLine.SetPosition(0, vertex1);
            newLine.SetPosition(1, vertex2);
            newLine.SetPosition(2, vertex3);
            newLine.SetPosition(3, vertex1); // Pour fermer le triangle

            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            lines.Add(newLine);
        }
    
}

