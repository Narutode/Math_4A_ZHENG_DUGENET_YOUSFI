using System.Collections.Generic;
using UnityEngine;

public class LoopSubdivision : MonoBehaviour
{
    public MeshFilter meshFilter;
    public int subdivisionSteps = 3;

    void Start()
    {
        // Récupérer le MeshFilter du GameObject auquel ce script est attaché
        meshFilter = GetComponent<MeshFilter>();

        // Vérifier si le MeshFilter est valide
        if (meshFilter == null || meshFilter.mesh == null)
        {
            Debug.LogError("MeshFilter or mesh not assigned properly.");
            return;
        }

        // Subdiviser le maillage du cube avec Loop subdivision steps fois
        Mesh mesh = meshFilter.mesh;
        for (int i = 0; i < subdivisionSteps; i++)
        {
            mesh = Subdivide(mesh);
        }

        // Assigner le maillage subdivisé au MeshFilter et recalculer les normales
        meshFilter.mesh = mesh;
        meshFilter.mesh.RecalculateNormals();
    }

    public Mesh Subdivide(Mesh mesh)
    {
        Dictionary<Edge, int> edgeMap = new Dictionary<Edge, int>();
        List<Vector3> newVertices = new List<Vector3>(mesh.vertices);
        List<int> newTriangles = new List<int>();

        // Créer des points de bord pour chaque arête
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int v0 = mesh.triangles[i];
            int v1 = mesh.triangles[i + 1];
            int v2 = mesh.triangles[i + 2];

            int m0 = GetEdgeVertex(v0, v1, mesh.vertices, edgeMap, newVertices);
            int m1 = GetEdgeVertex(v1, v2, mesh.vertices, edgeMap, newVertices);
            int m2 = GetEdgeVertex(v2, v0, mesh.vertices, edgeMap, newVertices);

            newTriangles.AddRange(new int[] { v0, m0, m2 });
            newTriangles.AddRange(new int[] { v1, m1, m0 });
            newTriangles.AddRange(new int[] { v2, m2, m1 });
            newTriangles.AddRange(new int[] { m0, m1, m2 });
        }
    /*
        // Réajuster les positions des sommets existants
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
    */
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

    int GetEdgeVertex(int v0, int v1, Vector3[] vertices, Dictionary<Edge, int> edgeMap, List<Vector3> newVertices)
    {
        Edge edge = new Edge(v0, v1);
        if (edgeMap.TryGetValue(edge, out int index))
        {
            return index;
        }
        else
        {
            Vector3 newVertex = (vertices[v0] + vertices[v1]) * 0.5f;
            newVertices.Add(newVertex);
            index = newVertices.Count - 1;
            edgeMap.Add(edge, index);
            return index;
        }
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
}
