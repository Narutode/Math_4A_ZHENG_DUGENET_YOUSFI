using System.Collections.Generic;
using UnityEngine;

public class ButterflySubdivision : MonoBehaviour
{
    public MeshFilter meshFilter;

    void Start()
    {
        Mesh mesh = meshFilter.mesh;
        for (int i = 0; i < 3; i++)
        {
            mesh = Subdivide(mesh);
        }
        meshFilter.mesh = mesh;
    }

    public Mesh Subdivide(Mesh mesh)
    {
        // L'algorithme de subdivision Butterfly
        Dictionary<Edge, int> edgeMap = new Dictionary<Edge, int>();
        List<Vector3> newVertices = new List<Vector3>(mesh.vertices);
        List<int> newTriangles = new List<int>();

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

        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
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
            Vector3 newVertex = ButterflyEdgeVertex(v0, v1, vertices);
            newVertices.Add(newVertex);
            index = newVertices.Count - 1;
            edgeMap.Add(edge, index);
            return index;
        }
    }

    Vector3 ButterflyEdgeVertex(int v0, int v1, Vector3[] vertices)
    {
        // Calcul du point selon la méthode Butterfly
        // Ce n'est qu'un exemple, les poids doivent être calculés correctement pour l'algorithme Butterfly
        return (vertices[v0] + vertices[v1]) / 2;
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
