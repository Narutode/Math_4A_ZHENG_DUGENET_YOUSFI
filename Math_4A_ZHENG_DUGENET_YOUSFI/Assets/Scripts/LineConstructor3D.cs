using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FonctionMath;

public struct Triangle
{
    public Vector3 Vertex1;
    public Vector3 Vertex2;
    public Vector3 Vertex3;

    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Vertex1 = v1;
        Vertex2 = v2;
        Vertex3 = v3;
    }
}
public class LineConstructor3D : MonoBehaviour
{
    public GameObject pointGO;
    public GameObject pointCenter;
    public List<GameObject> listGameObjects;
    public List<Vector3> listPoints;
    public List<Segments> ListSegments = new List<Segments>();
    public List<Triangles> ListTriangles = new List<Triangles>();

    public List<LineRenderer> lines;
    public int nbPoints;

    public GameObject clickMenu;
    public GameObject parent;




    public static List<Vector3> GetConvexHull2(List<Vector3> pointCloud)
    {
        if (pointCloud == null || pointCloud.Count < 4)
        {
            // Pas possible de former une enveloppe convexe avec moins de 4 points en 3D
            return null;
        }

        List<Vector3> convexHull = new List<Vector3>();

        // Trouver les points extrêmes pour former une face initiale
        int minIndex, maxIndex;
        FindExtremes(pointCloud, out minIndex, out maxIndex);

        convexHull.Add(pointCloud[minIndex]);
        convexHull.Add(pointCloud[maxIndex]);

        // Diviser pour régner
        List<Vector3> upperHull = BuildConvexHull(pointCloud, pointCloud[minIndex], pointCloud[maxIndex]);
        List<Vector3> lowerHull = BuildConvexHull(pointCloud, pointCloud[maxIndex], pointCloud[minIndex]);

        // Assembler les deux parties pour former l'enveloppe convexe complète
        convexHull.AddRange(upperHull);
        convexHull.AddRange(lowerHull);

        return convexHull;
    }

    private static void FindExtremes(List<Vector3> points, out int minIndex, out int maxIndex)
    {
        minIndex = maxIndex = 0;

        for (int i = 1; i < points.Count; i++)
        {
            if (points[i].x < points[minIndex].x)
                minIndex = i;
            if (points[i].x > points[maxIndex].x)
                maxIndex = i;
        }
    }

    private static List<Vector3> BuildConvexHull(List<Vector3> points, Vector3 p1, Vector3 p2)
    {
        List<Vector3> convexHull = new List<Vector3>();

        Vector3 furthestPoint = FindFurthestPoint(points, p1, p2);

        if (furthestPoint != Vector3.zero)
        {
            convexHull.AddRange(BuildConvexHullHelper(points, p1, furthestPoint, p2));
            convexHull.AddRange(BuildConvexHullHelper(points, furthestPoint, p2, p1));
        }

        return convexHull;
    }

    private static List<Vector3> BuildConvexHullHelper(List<Vector3> points, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        List<Vector3> convexHull = new List<Vector3>();

        List<Vector3> outsidePoints = new List<Vector3>();

        foreach (Vector3 point in points)
        {
            if (IsOutside(point, p1, p2, p3))
                outsidePoints.Add(point);
        }

        if (outsidePoints.Count > 0)
        {
            Vector3 furthestPoint = FindFurthestPoint(outsidePoints, p1, p2, p3);

            if (furthestPoint != Vector3.zero)
            {
                convexHull.AddRange(BuildConvexHullHelper(outsidePoints, p1, furthestPoint, p2));
                convexHull.Add(furthestPoint);
                convexHull.AddRange(BuildConvexHullHelper(outsidePoints, furthestPoint, p2, p3));
            }
        }

        return convexHull;
    }

    private static Vector3 FindFurthestPoint(List<Vector3> points, Vector3 p1, Vector3 p2, Vector3 p3 = default(Vector3))
    {
        float maxDistance = -1f;
        Vector3 furthestPoint = Vector3.zero;

        foreach (Vector3 point in points)
        {
            float distance;
            if (p3 == default(Vector3))
            {
                // Cas où p3 n'est pas spécifié, utilisez les deux premiers points
                distance = Mathf.Abs(Vector3.Dot(Vector3.Cross(p2 - p1, point - p1).normalized, point - p1));
            }
            else
            {
                // Cas où p3 est spécifié
                distance = Mathf.Abs(Vector3.Dot(Vector3.Cross(p2 - p1, p3 - p1).normalized, point - p1));
            }

            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestPoint = point;
            }
        }

        return furthestPoint;
    }

    private static bool IsOutside(Vector3 point, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float dot1 = Vector3.Dot(Vector3.Cross(p2 - p1, p3 - p1).normalized, point - p1);
        float dot2 = Vector3.Dot(Vector3.Cross(p2 - p1, p3 - p1).normalized, p3 - p1);

        return dot1 * dot2 < 0;
    }

    public static List<Vector3> GetGrahamScan3D(List<Vector3> pointList)
    {
        List<Vector3> grahamScan = new List<Vector3>();

        // Trouver le point le plus bas (et le plus à gauche) comme point de départ
        Vector3 startPoint = pointList.OrderBy(p => p.y).ThenBy(p => p.x).ThenBy(p => p.z).First();

        // Trier les points par angle par rapport au point de départ
        List<Vector3> sortedPoints = pointList.OrderBy(p => GetAngle(startPoint, p)).ToList();

        grahamScan.Add(startPoint);
        grahamScan.Add(sortedPoints[0]);

        for (int i = 1; i < sortedPoints.Count; i++)
        {
            while (grahamScan.Count > 1 && !IsConvex3D(grahamScan[grahamScan.Count - 2], grahamScan.Last(), sortedPoints[i]))
            {
                grahamScan.RemoveAt(grahamScan.Count - 1);
            }
            grahamScan.Add(sortedPoints[i]);
        }

        return grahamScan;
    }

    // Fonction pour calculer l'angle entre deux points par rapport à l'axe x
    private static float GetAngle(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        Quaternion rotation = Quaternion.LookRotation(direction);
        return rotation.eulerAngles.y;
    }

    // Fonction pour vérifier si un point est à gauche ou à droite d'un vecteur dans l'espace 3D
    private static bool IsConvex3D(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 crossProduct = Vector3.Cross(pointB - pointA, pointC - pointA);
        return crossProduct.z >= 0f; // Vérifier si la composante z du produit vectoriel est négative
    }

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

    public static List<Vector3> GetConvexHull(List<Vector3> points)
    {
        if (points == null || points.Count < 4)
        {
            // Pas possible de former une enveloppe convexe avec moins de 4 points en 3D
            return null;
        }

        // Tri initial des points par coordonnée x
        points.Sort((a, b) => a.x.CompareTo(b.x));

        // Diviser et conquérir pour obtenir l'enveloppe convexe
        return DivideAndConquerHelper(points);
    }

    private static List<Vector3> DivideAndConquerHelper(List<Vector3> points)
    {
        int n = points.Count;

        if (n <= 3)
        {
            // Cas de base : retourner les points tels quels
            return points;
        }

        // Diviser la liste de points en deux moitiés
        int mid = n / 2;
        List<Vector3> leftPoints = points.GetRange(0, mid);
        List<Vector3> rightPoints = points.GetRange(mid, n - mid);

        // Résoudre récursivement les deux moitiés
        List<Vector3> leftHull = DivideAndConquerHelper(leftPoints);
        List<Vector3> rightHull = DivideAndConquerHelper(rightPoints);

        // Fusionner les enveloppes convexes des deux moitiés
        return MergeHulls(leftHull, rightHull);
    }

    private static List<Vector3> MergeHulls(List<Vector3> leftHull, List<Vector3> rightHull)
    {
        // Trouver les points extrêmes des deux enveloppes convexes
        Vector3 leftMost = leftHull[leftHull.Count - 1];
        Vector3 rightMost = rightHull[0];

        // Trouver les points de jonction entre les enveloppes convexes
        while (Vector3.Dot(leftMost - rightMost, rightHull[1] - rightMost) >= 0 && rightHull.Count > 1)
        {
            rightHull.RemoveAt(0);
        }

        while (Vector3.Dot(rightMost - leftMost, leftHull[leftHull.Count - 2] - leftMost) >= 0 && leftHull.Count > 1)
        {
            leftHull.RemoveAt(leftHull.Count - 1);
        }

        // Fusionner les deux enveloppes convexes
        leftHull.AddRange(rightHull);

        return leftHull;
    }

    public void DrawHull()
    {
        float start = Time.realtimeSinceStartup;

        List<Vector3> graham = GetConvexHull(listPoints);
        Debug.Log("graham : " + graham.Count);
        float end = Time.realtimeSinceStartup;
        Debug.Log(end - start);
        int size = graham.Count;
        for (int i = 0; i < size; i++)
        {
            GameObject newGO = new GameObject();
            newGO.name = "sides";
            newGO.transform.parent = parent.transform;
            LineRenderer newLine = newGO.AddComponent<LineRenderer>();
            newLine.positionCount = 2;
            newLine.SetPosition(0, graham[i]);
            newLine.SetPosition(1, graham[(i+1)%size]);
            newLine.loop = true;
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            lines.Add(newLine);
        }
    }
}
