using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KobbeltSubdivision : MonoBehaviour
{
    public MeshFilter meshFilter;
    public int subdivisions = 1;
    private Mesh meshcopy;
    void Start()
    {
        meshcopy = CreateCube();

        for (int i = 0; i < subdivisions; i++)
        {
            meshcopy = Subdivide(meshcopy);
            //perturb(meshcopy);
        }

        MeshFilter meshFilterthis = GetComponent<MeshFilter>();
        meshFilterthis.mesh = meshcopy;
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = meshFilter.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public Mesh Subdivide(Mesh mesh)
    {
        List<Vector3> newVertices = new List<Vector3>(mesh.vertices);
        List<int> newTriangles = new List<int>();

        Dictionary<int, List<int>> adjDict = FindAdjacentTriangles(mesh);

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int v0 = mesh.triangles[i];
            int v1 = mesh.triangles[i + 1];
            int v2 = mesh.triangles[i + 2];

            Vector3 p0 = mesh.vertices[v0];
            Vector3 p1 = mesh.vertices[v1];
            Vector3 p2 = mesh.vertices[v2];

            Vector3 barycenter = (p0 + p1 + p2) / 3.0f;
            newVertices.Add(barycenter);
            int barycenterIndex = newVertices.Count - 1;

            // Create new triangles from the current triangle
            //newTriangles.AddRange(new int[] { v0, v1, barycenterIndex });
            //newTriangles.AddRange(new int[] { v1, v2, barycenterIndex });
            //newTriangles.AddRange(new int[] { v2, v0, barycenterIndex });

            // Find adjacent triangles and create new triangles between barycenters
            foreach (var adjacentIndex in adjDict[i / 3])
            {
                int av0 = mesh.triangles[adjacentIndex * 3];
                int av1 = mesh.triangles[adjacentIndex * 3 + 1];
                int av2 = mesh.triangles[adjacentIndex * 3 + 2];

                Vector3 ap0 = mesh.vertices[av0];
                Vector3 ap1 = mesh.vertices[av1];
                Vector3 ap2 = mesh.vertices[av2];

                var pointList = new List<int>() { av0, av1, av2 };

                Vector3 adjacentBarycenter = (ap0 + ap1 + ap2) / 3.0f;
                int adjacentBarycenterIndex = newVertices.IndexOf(adjacentBarycenter);
                if (adjacentBarycenterIndex == -1)
                {
                    newVertices.Add(adjacentBarycenter);
                    adjacentBarycenterIndex = newVertices.Count - 1;
                }

                if (pointList.Contains(v0))
                    newTriangles.AddRange(new int[] { barycenterIndex, adjacentBarycenterIndex, v0 });
                if (pointList.Contains(v1))
                    newTriangles.AddRange(new int[] { barycenterIndex, adjacentBarycenterIndex, v1 });
                if (pointList.Contains(v2))
                    newTriangles.AddRange(new int[] { barycenterIndex, adjacentBarycenterIndex, v2 });
            }
        }

        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    Dictionary<int, List<int>> FindAdjacentTriangles(Mesh mesh)
    {
        Dictionary<int, List<int>> adjacencyList = new Dictionary<int, List<int>>();

        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            AddAdjacentTriangle(adjacencyList, i / 3, v0, v1);
            AddAdjacentTriangle(adjacencyList, i / 3, v1, v2);
            AddAdjacentTriangle(adjacencyList, i / 3, v2, v0);
        }

        return adjacencyList;
    }

    void AddAdjacentTriangle(Dictionary<int, List<int>> adjacencyList, int triangleIndex, int v0, int v1)
    {
        foreach (var kvp in adjacencyList)
        {
            if (kvp.Value.Contains(v0) && kvp.Value.Contains(v1))
            {
                adjacencyList[kvp.Key].Add(triangleIndex);
                if (adjacencyList.ContainsKey(triangleIndex))
                    adjacencyList[triangleIndex].Add(kvp.Key);
                else
                {
                    adjacencyList[triangleIndex] = new List<int>();
                    adjacencyList[triangleIndex].Add(kvp.Key);
                }
                return;
            }
        }

        if (!adjacencyList.ContainsKey(triangleIndex))
        {
            adjacencyList[triangleIndex] = new List<int>();
        }
        adjacencyList[triangleIndex].Add(v0);
        adjacencyList[triangleIndex].Add(v1);
    }

    void perturb(Mesh mesh)
    {
        Vector3[] vertex = new Vector3[mesh.vertices.Length];
        var adjDict = FindAdjacentVertices(mesh);
        for(int i = 0; i < mesh.vertices.Length; i++)
        {
            var adj = adjDict[i];
            float n = adj.Count;
            var curP = mesh.vertices[i];
            Vector3 sum = Vector3.zero;
            foreach (var index in adj)
            {
                sum += mesh.vertices[index];
            }
            var alpha = (1.0f / 9.0f) * (4.0f - 2.0f * Mathf.Cos(2.0f * Mathf.PI / n));
            Vector3 vPrime = (1 - alpha) * curP + (alpha / n) * sum;
            vertex[i] = vPrime;
        }

        mesh.vertices = vertex;
        mesh.RecalculateNormals();
    }

    Dictionary<int, List<int>> FindAdjacentVertices(Mesh mesh)
    {
        Dictionary<int, List<int>> adjacencyList = new Dictionary<int, List<int>>();

        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            AddAdjacentVertex(adjacencyList, v0, v1);
            AddAdjacentVertex(adjacencyList, v0, v2);
            AddAdjacentVertex(adjacencyList, v1, v0);
            AddAdjacentVertex(adjacencyList, v1, v2);
            AddAdjacentVertex(adjacencyList, v2, v0);
            AddAdjacentVertex(adjacencyList, v2, v1);
        }

        return adjacencyList;
    }

    void AddAdjacentVertex(Dictionary<int, List<int>> adjacencyList, int vertex, int adjacentVertex)
    {
        if (!adjacencyList.ContainsKey(vertex))
        {
            adjacencyList[vertex] = new List<int>();
        }
        if (!adjacencyList[vertex].Contains(adjacentVertex))
        {
            adjacencyList[vertex].Add(adjacentVertex);
        }
    }

    Mesh CreateCube()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Define the 8 vertices of the cube
        Vector3[] vertices = {
            new Vector3(-0.5f, -0.5f, 0.5f),  // 0
            new Vector3(0.5f, -0.5f, 0.5f),   // 1
            new Vector3(0.5f, 0.5f, 0.5f),    // 2
            new Vector3(-0.5f, 0.5f, 0.5f),   // 3
            new Vector3(-0.5f, -0.5f, -0.5f), // 4
            new Vector3(0.5f, -0.5f, -0.5f),  // 5
            new Vector3(0.5f, 0.5f, -0.5f),   // 6
            new Vector3(-0.5f, 0.5f, -0.5f)   // 7
        };

        // Define the 12 triangles (2 per face)
        int[] triangles = {
            // Front face
            0, 1, 2,
            0, 2, 3,
            // Back face
            4, 6, 5,
            4, 7, 6,
            // Left face
            4, 5, 1,
            4, 1, 0,
            // Right face
            1, 5, 6,
            1, 6, 2,
            // Top face
            2, 6, 7,
            2, 7, 3,
            // Bottom face
            4, 0, 3,
            4, 3, 7
        };

        // Define the UVs (optional, for texturing)
        Vector2[] uvs = {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Recalculate normals for proper lighting
        mesh.RecalculateNormals();

        return mesh;
    }
}
