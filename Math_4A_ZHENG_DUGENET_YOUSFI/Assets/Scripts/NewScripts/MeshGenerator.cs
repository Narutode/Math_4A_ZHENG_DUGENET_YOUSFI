using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    /*public Transform[] points;
    public Material[] materials; // Array of materials to assign to each face

    void Start()
    {
        if (points.Length != 8)
        {
            Debug.LogError("Please assign exactly 8 points to create the cube.");
            return;
        }

        if (materials.Length < 6)
        {
            Debug.LogError("Please assign at least 6 materials.");
            return;
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            vertices[i] = points[i].position;
        }

        // Indices for each face
        int[][] faceIndices = new int[6][]
        {
            new int[] { 0, 2, 1, 0, 3, 2 }, // Front face
            new int[] { 2, 3, 6, 3, 7, 6 }, // Top face
            new int[] { 7, 5, 6, 7, 4, 5 }, // Back face
            new int[] { 1, 6, 5, 1, 2, 6 }, // Bottom face
            new int[] { 4, 0, 5, 0, 1, 5 }, // Left face
            new int[] { 3, 4, 7, 3, 0, 4 }  // Right face
        };

        mesh.vertices = vertices;
        mesh.subMeshCount = 6;

        for (int i = 0; i < 6; i++)
        {
            mesh.SetTriangles(faceIndices[i], i);
        }

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        // Assign random materials to each face
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material[] faceMaterials = new Material[6];
        for (int i = 0; i < 6; i++)
        {
            faceMaterials[i] = materials[Random.Range(0, materials.Length)];
        }
        meshRenderer.materials = faceMaterials;
    }*/
    public Transform[] points;
    public Material[] materials;

    void Start()
    {
        if (points.Length < 4)
        {
            Debug.LogError("Please assign at least 4 points to create a mesh.");
            return;
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i].position;
        }

        List<int> hullIndices = QuickHull(vertices);

        int[] triangles = CreateTriangles(hullIndices);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        int numFaces = triangles.Length / 3;
        Material[] faceMaterials = new Material[numFaces];
        for (int i = 0; i < numFaces; i++)
        {
            faceMaterials[i] = materials[Random.Range(0, materials.Length)];
        }
        meshRenderer.materials = faceMaterials;
    }

    List<int> QuickHull(Vector3[] points)
    {
        List<int> hull = new List<int>();
        // Find the leftmost and rightmost points
        int leftmost = 0, rightmost = 0;
        for (int i = 1; i < points.Length; i++)
        {
            if (points[i].x < points[leftmost].x) leftmost = i;
            if (points[i].x > points[rightmost].x) rightmost = i;
        }

        hull.Add(leftmost);
        hull.Add(rightmost);

        List<int> leftSet = new List<int>();
        List<int> rightSet = new List<int>();

        for (int i = 0; i < points.Length; i++)
        {
            if (i == leftmost || i == rightmost) continue;

            if (IsLeft(points[leftmost], points[rightmost], points[i]))
                leftSet.Add(i);
            else
                rightSet.Add(i);
        }

        FindHull(points, hull, leftmost, rightmost, leftSet);
        FindHull(points, hull, rightmost, leftmost, rightSet);

        return hull;
    }

    void FindHull(Vector3[] points, List<int> hull, int p1, int p2, List<int> set)
    {
        if (set.Count == 0) return;

        int furthest = set[0];
        float maxDistance = DistanceFromLine(points[p1], points[p2], points[set[0]]);

        for (int i = 1; i < set.Count; i++)
        {
            float distance = DistanceFromLine(points[p1], points[p2], points[set[i]]);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthest = set[i];
            }
        }

        hull.Insert(hull.IndexOf(p2), furthest);

        List<int> leftSet1 = new List<int>();
        List<int> leftSet2 = new List<int>();

        for (int i = 0; i < set.Count; i++)
        {
            if (set[i] == furthest) continue;

            if (IsLeft(points[p1], points[furthest], points[set[i]]))
                leftSet1.Add(set[i]);
            else if (IsLeft(points[furthest], points[p2], points[set[i]]))
                leftSet2.Add(set[i]);
        }

        FindHull(points, hull, p1, furthest, leftSet1);
        FindHull(points, hull, furthest, p2, leftSet2);
    }

    bool IsLeft(Vector3 p1, Vector3 p2, Vector3 p)
    {
        return ((p2.x - p1.x) * (p.z - p1.z) - (p2.z - p1.z) * (p.x - p1.x)) > 0;
    }

    float DistanceFromLine(Vector3 p1, Vector3 p2, Vector3 p)
    {
        return Mathf.Abs((p2.z - p1.z) * p.x - (p2.x - p1.x) * p.z + p2.x * p1.z - p2.z * p1.x) /
               Mathf.Sqrt((p2.z - p1.z) * (p2.z - p1.z) + (p2.x - p1.x) * (p2.x - p1.x));
    }

    int[] CreateTriangles(List<int> hull)
    {
        List<int> triangles = new List<int>();
        for (int i = 0; i < hull.Count - 2; i++)
        {
            triangles.Add(hull[0]);
            triangles.Add(hull[i + 1]);
            triangles.Add(hull[i + 2]);
        }
        return triangles.ToArray();
    }
}