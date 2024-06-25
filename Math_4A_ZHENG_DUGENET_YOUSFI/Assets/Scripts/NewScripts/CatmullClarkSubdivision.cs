using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullClarkSubdivision : MonoBehaviour
{
    public MeshFilter originalMeshFilter;
    public int subdivisions = 1;

    void Start()
    {
        Mesh originalMesh = originalMeshFilter.mesh;

        for (int i = 0; i < subdivisions; i++)
        {
            originalMesh = SubdivideMesh(originalMesh);
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = originalMesh;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.material = originalMeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
    }

    Mesh SubdivideMesh(Mesh mesh)
    {
        List<Vector3> vertices = new List<Vector3>(mesh.vertices);
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector3> facePoints = new List<Vector3>();
        Dictionary<Edge, Vector3> edgePoints = new Dictionary<Edge, Vector3>();
        Dictionary<int, List<int>> vertexNeighborMap = new Dictionary<int, List<int>>();

        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            Vector3 facePoint = (vertices[v0] + vertices[v1] + vertices[v2]) / 3f;
            facePoints.Add(facePoint);

            AddNeighbor(vertexNeighborMap, v0, v1);
            AddNeighbor(vertexNeighborMap, v0, v2);
            AddNeighbor(vertexNeighborMap, v1, v0);
            AddNeighbor(vertexNeighborMap, v1, v2);
            AddNeighbor(vertexNeighborMap, v2, v0);
            AddNeighbor(vertexNeighborMap, v2, v1);

            Edge edge1 = new Edge(v0, v1);
            Edge edge2 = new Edge(v1, v2);
            Edge edge3 = new Edge(v2, v0);

            AddEdgePoint(edgePoints, edge1, facePoint, vertices[v0], vertices[v1]);
            AddEdgePoint(edgePoints, edge2, facePoint, vertices[v1], vertices[v2]);
            AddEdgePoint(edgePoints, edge3, facePoint, vertices[v2], vertices[v0]);
        }

        // Mise à jour des sommets originaux
        for (int i = 0; i < vertices.Count; i++)
        {
            List<int> neighbors = vertexNeighborMap[i];
            Vector3 sumFacePoints = Vector3.zero;
            Vector3 sumEdgePoints = Vector3.zero;

            foreach (int neighbor in neighbors)
            {
                sumEdgePoints += (vertices[i] + vertices[neighbor]) / 2f;
            }

            foreach (Vector3 facePoint in facePoints)
            {
                sumFacePoints += facePoint;
            }

            sumEdgePoints /= neighbors.Count;
            sumFacePoints /= neighbors.Count;

            Vector3 newVertex = (sumFacePoints + 2f * sumEdgePoints + (neighbors.Count - 3f) * vertices[i]) / neighbors.Count;
            newVertices.Add(newVertex);
        }

        List<int> newTriangles = new List<int>();
        Dictionary<Edge, int> edgeMap = new Dictionary<Edge, int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            int f0 = newVertices.Count;
            newVertices.Add(facePoints[i / 3]);

            int e0 = GetEdgeVertex(v0, v1, vertices, edgeMap, newVertices, edgePoints);
            int e1 = GetEdgeVertex(v1, v2, vertices, edgeMap, newVertices, edgePoints);
            int e2 = GetEdgeVertex(v2, v0, vertices, edgeMap, newVertices, edgePoints);

            newTriangles.AddRange(new int[] { v0, e0, f0, e2 });
            newTriangles.AddRange(new int[] { v1, e1, f0, e0 });
            newTriangles.AddRange(new int[] { v2, e2, f0, e1 });
            newTriangles.AddRange(new int[] { e0, e1, f0, e2 });
        }

        Mesh subdividedMesh = new Mesh();
        subdividedMesh.vertices = newVertices.ToArray();
        subdividedMesh.triangles = newTriangles.ToArray();
        subdividedMesh.RecalculateNormals();

        return subdividedMesh;
    }

    void AddNeighbor(Dictionary<int, List<int>> vertexNeighborMap, int v0, int v1)
    {
        if (!vertexNeighborMap.ContainsKey(v0))
            vertexNeighborMap[v0] = new List<int>();
        vertexNeighborMap[v0].Add(v1);
    }

    void AddEdgePoint(Dictionary<Edge, Vector3> edgePoints, Edge edge, Vector3 facePoint, Vector3 vertex0, Vector3 vertex1)
    {
        if (!edgePoints.ContainsKey(edge))
        {
            edgePoints[edge] = (vertex0 + vertex1 + facePoint) / 3f;
        }
    }

    int GetEdgeVertex(int v0, int v1, List<Vector3> vertices, Dictionary<Edge, int> edgeMap, List<Vector3> newVertices, Dictionary<Edge, Vector3> edgePoints)
    {
        Edge edge = new Edge(v0, v1);
        if (edgeMap.TryGetValue(edge, out int index))
        {
            return index;
        }
        else
        {
            Vector3 newVertex = edgePoints[edge];
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