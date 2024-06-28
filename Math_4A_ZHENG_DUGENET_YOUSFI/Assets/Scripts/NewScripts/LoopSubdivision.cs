using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoopSubdivision : MonoBehaviour
{
    public MeshFilter meshFilter;
    public int subdivisionSteps = 3;

    void Start()
    {
        // R�cup�rer le MeshFilter du GameObject auquel ce script est attach�
        meshFilter = GetComponent<MeshFilter>();

        // V�rifier si le MeshFilter est valide
        if (meshFilter == null || meshFilter.mesh == null)
        {
            Debug.LogError("MeshFilter or mesh not assigned properly.");
            return;
        }

        // Subdiviser le maillage du cube avec Loop subdivision steps fois
        Mesh mesh = CreateTetrahedron();
        for (int i = 0; i < subdivisionSteps; i++)
        {
            mesh = Subdivide(mesh);
        }

        // Assigner le maillage subdivis� au MeshFilter et recalculer les normales
        meshFilter.mesh = mesh;
        meshFilter.mesh.RecalculateNormals();
    }

    public Mesh Subdivide(Mesh mesh)
    {
        Dictionary<Edge, int> edgeMap = new Dictionary<Edge, int>();
        List<Vector3> newVertices = new List<Vector3>(mesh.vertices);
        List<int> newTriangles = new List<int>();

        // Cr�er des points de bord pour chaque ar�te
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int v0 = mesh.triangles[i];
            int v1 = mesh.triangles[i + 1];
            int v2 = mesh.triangles[i + 2];

            int m0 = GetEdgeVertex(v0, v1, mesh, edgeMap, newVertices);
            int m1 = GetEdgeVertex(v1, v2, mesh, edgeMap, newVertices);
            int m2 = GetEdgeVertex(v2, v0, mesh, edgeMap, newVertices);

            newTriangles.AddRange(new int[] { v0, m0, m2 });
            newTriangles.AddRange(new int[] { v1, m1, m0 });
            newTriangles.AddRange(new int[] { v2, m2, m1 });
            newTriangles.AddRange(new int[] { m0, m1, m2 });
        }
    
        // R�ajuster les positions des sommets existants
        Vector3[] originalVertices = mesh.vertices;
        Vector3[] adjustedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            adjustedVertices[i] = AdjustVertex(i, originalVertices, edgeMap);
        }

        for (int i = 0; i < adjustedVertices.Length; i++)
        {
            newVertices[i] = adjustedVertices[i];
        }
    
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    Vector3 AdjustVertex(int index, Vector3[] vertices, Dictionary<Edge, int> edgeMap)
    {
        List<int> neighborIndices = new List<int>();

        foreach (var edge in edgeMap.Keys)
        {
            if (edge.v0 == index)
            {
                neighborIndices.Add(edge.v1);
            }
            else if (edge.v1 == index)
            {
                neighborIndices.Add(edge.v0);
            }
        }

        int n = neighborIndices.Count;
        float beta = (n == 3) ? 3f / 16f : 3f / (8f * n);
        Vector3 newPos = (1 - n * beta) * vertices[index];

        foreach (int neighborIndex in neighborIndices)
        {
            newPos += beta * vertices[neighborIndex];
        }

        return newPos;
    }

    int GetEdgeVertex(int v0, int v1, Mesh mesh, Dictionary<Edge, int> edgeMap, List<Vector3> newVertices)
    {
        Vector3[] vertices = mesh.vertices;
        Edge edge = new Edge(v0, v1);
        var leftright = FindOtherVerticesOfAdjacentTriangles(mesh, v0, v1);
        
        if (edgeMap.TryGetValue(edge, out int index))
        {
            return index;
        }
        else
        {
            Vector3 newVertex = (vertices[v0] + vertices[v1]) * 3f/8f + (leftright[0] + leftright[1]) * 1f/8f;
            newVertices.Add(newVertex);
            index = newVertices.Count - 1;
            edgeMap.Add(edge, index);
            return index;
        }
    }

    List<Vector3> FindOtherVerticesOfAdjacentTriangles(Mesh mesh, int v0, int v1)
    {
        List<Vector3> otherVertices = new List<Vector3>();
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t0 = triangles[i];
            int t1 = triangles[i + 1];
            int t2 = triangles[i + 2];

            if ((t0 == v0 && t1 == v1) || (t0 == v1 && t1 == v0) ||
                (t0 == v0 && t2 == v1) || (t0 == v1 && t2 == v0) ||
                (t1 == v0 && t2 == v1) || (t1 == v1 && t2 == v0))
            {
                int otherVertex = t0 != v0 && t0 != v1 ? t0 : t1 != v0 && t1 != v1 ? t1 : t2;
                otherVertices.Add(mesh.vertices[otherVertex]);
            }
        }

        return otherVertices;
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

    struct Edge
    {
        public int v0, v1;

        public Edge(int v0, int v1)
        {
            this.v0 = Mathf.Min(v0, v1);
            this.v1 = Mathf.Max(v0, v1);
        }

        public override int GetHashCode()
        {
            return v0.GetHashCode() ^ v1.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge))
            {
                return false;
            }

            Edge other = (Edge)obj;
            return v0 == other.v0 && v1 == other.v1;
        }
    }
    Mesh CreateTetrahedron()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Define the 4 vertices of the tetrahedron
        Vector3[] vertices =
        {
            new Vector3(1, 1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, 1, -1),
            new Vector3(1, -1, -1)
        };

        // Define the 4 triangles (each with 3 vertices) in counter-clockwise order
        int[] triangles =
        {
            // Triangle 1
            0, 2, 1,
            // Triangle 2
            0, 1, 3,
            // Triangle 3
            0, 3, 2,
            // Triangle 4
            1, 2, 3
        };

        // Define the UVs (optional, for texturing)
        Vector2[] uvs =
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0.5f, 1),
            new Vector2(0.5f, 0.5f)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Recalculate normals for proper lighting
        mesh.RecalculateNormals();

        return mesh;
    }
}
