using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullClarkSubdivision : MonoBehaviour
{
    public MeshFilter originalMeshFilter;
    public int subdivisions = 1;

    void Start()
    {
        Mesh originalMesh = CreateCube();

        for (int i = 0; i < subdivisions; i++)
        {
            originalMesh = SubdivideMesh(originalMesh);
        }
        originalMesh.RecalculateNormals();

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
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Step 1: Compute all face points
        Dictionary<int, Vector3> facePoints = new Dictionary<int, Vector3>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int[] faceVertices = new int[] { triangles[i], triangles[i + 1], triangles[i + 2] };
            Vector3 facePoint = ComputeFacePoint(mesh, faceVertices);
            facePoints[i / 3] = facePoint;
        }

        // Step 2: Compute all edge points and track adjacent faces
        Dictionary<(int, int), List<Vector3>> edgePoints = new Dictionary<(int, int), List<Vector3>>();
        Dictionary<(int, int), List<Vector3>> adjacentFacePoints = new Dictionary<(int, int), List<Vector3>>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int[] faceVertices = new int[] { triangles[i], triangles[i + 1], triangles[i + 2] };
            Vector3 facePoint = facePoints[i / 3];

            for (int j = 0; j < 3; j++)
            {
                int v0 = faceVertices[j];
                int v1 = faceVertices[(j + 1) % 3];

                // Ensure the key order is consistent
                var key = v0 < v1 ? (v0, v1) : (v1, v0);

                if (!adjacentFacePoints.ContainsKey(key))
                {
                    adjacentFacePoints[key] = new List<Vector3>();
                }

                if (!adjacentFacePoints[key].Contains(facePoint))
                {
                    adjacentFacePoints[key].Add(facePoint);
                }
            }
        }

        foreach (var edge in adjacentFacePoints.Keys)
        {
            List<Vector3> faces = adjacentFacePoints[edge];
            if (faces.Count == 2)
            {
                Vector3 edgePoint = ComputeEdgePoint(mesh, edge.Item1, edge.Item2, faces[0], faces[1]);
                Vector3 edgePoint2 = ComputeEdgePoint(mesh, edge.Item2, edge.Item1, faces[0], faces[1]);

                edgePoints[edge] = new List<Vector3>{ edgePoint, edgePoint2 };
            }
        }

        // Step 3: Compute all vertex points
        Dictionary<int, Vector3> vertexPoints = new Dictionary<int, Vector3>();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vertexIndex = i;
            List<Vector3> adjacentFaces = new List<Vector3>();
            List<Vector3> adjacentEdges = new List<Vector3>();

            foreach (var face in facePoints)
            {
                if (triangles[face.Key * 3] == vertexIndex || triangles[face.Key * 3 + 1] == vertexIndex || triangles[face.Key * 3 + 2] == vertexIndex)
                {
                    adjacentFaces.Add(face.Value);
                }
            }

            foreach (var edge in edgePoints)
            {
                if (edge.Key.Item1 == vertexIndex || edge.Key.Item2 == vertexIndex)
                {
                    if(!adjacentEdges.Contains(edge.Value[0]))
                        adjacentEdges.Add(edge.Value[0]);
                    if(!adjacentEdges.Contains(edge.Value[1]))
                        adjacentEdges.Add(edge.Value[1]);
                }
            }

            Vector3 vertexPoint = ComputeVertexPoint(mesh, vertexIndex, adjacentFaces, adjacentEdges);
            vertexPoints[vertexIndex] = vertexPoint;
        }

        // Step 4: Create new faces
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int[] faceVertices = new int[] { triangles[i], triangles[i + 1], triangles[i + 2] };
            Vector3 facePoint = facePoints[i / 3];
            SubdivideFace(mesh, faceVertices, facePoint, edgePoints, vertexPoints, newVertices, newTriangles);
        }

        // Update mesh with new vertices and triangles
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    Vector3 ComputeFacePoint(Mesh mesh, int[] faceVertices)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3 facePoint = Vector3.zero;

        foreach (int vertex in faceVertices)
        {
            facePoint += vertices[vertex];
        }

        facePoint /= faceVertices.Length;
        return facePoint;
    }

    Vector3 ComputeEdgePoint(Mesh mesh, int v0, int v1, Vector3 facePoint1, Vector3 facePoint2)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3 edgePoint = (vertices[v0] + vertices[v1] + facePoint1 + facePoint2) / 4.0f;
        return edgePoint;
    }

    Vector3 ComputeVertexPoint(Mesh mesh, int vertexIndex, List<Vector3> adjacentFaces, List<Vector3> adjacentEdges)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3 originalVertex = vertices[vertexIndex];

        Vector3 facePointsSum = Vector3.zero;
        foreach (Vector3 face in adjacentFaces)
        {
            facePointsSum += face;
        }
        facePointsSum /= adjacentFaces.Count;

        Vector3 edgePointsSum = Vector3.zero;
        foreach (Vector3 edge in adjacentEdges)
        {
            edgePointsSum += edge;
        }
        edgePointsSum /= adjacentEdges.Count;

        float n = adjacentFaces.Count;

        Vector3 newVertex = (facePointsSum + 2 * edgePointsSum + originalVertex * (n - 3)) / n;
        return newVertex;
    }


    void SubdivideFace(Mesh mesh, int[] faceVertices, Vector3 facePoint, Dictionary<(int, int), List<Vector3>> edgePoints, Dictionary<int, Vector3> vertexPoints, List<Vector3> newVertices, List<int> newTriangles)
    {
        int n = faceVertices.Length;

        // Add face point to newVertices and get its index
        int facePointIndex = AddVertexIfNotExists(facePoint, newVertices);

        for (int i = 0; i < n; i++)
        {
            int v0 = faceVertices[i];
            int v1 = faceVertices[(i + 1) % n];

            var key = v0 < v1 ? (v0, v1) : (v1, v0);

            // Ensure vertex points are added and retrieve their indices
            Vector3 v0New = vertexPoints[v0];
            Vector3 v1New = vertexPoints[v1];

            int v0NewIndex = AddVertexIfNotExists(v0New, newVertices);
            int v1NewIndex = AddVertexIfNotExists(v1New, newVertices);

            // Ensure edge points are added and retrieve their indices
            Vector3 e0 = edgePoints[key][0];
            Vector3 e1 = edgePoints[key][1];

            int e0Index = AddVertexIfNotExists(e0, newVertices);

            // Create new faces (triangles)
            // Triangle 1: (v0New, e0, facePoint)
            newTriangles.Add(v0NewIndex);
            newTriangles.Add(e0Index);
            newTriangles.Add(facePointIndex);

            // Triangle 2: (e0, v1New, facePoint)
            newTriangles.Add(e0Index);
            newTriangles.Add(v1NewIndex);
            newTriangles.Add(facePointIndex);
        }
    }

    int AddVertexIfNotExists(Vector3 vertex, List<Vector3> vertices)
    {
        int index = vertices.IndexOf(vertex);
        if (index == -1)
        {
            index = vertices.Count;
            vertices.Add(vertex);
        }
        return index;
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

    Vector3 ComputeVertexPoint(List<Vector3> vertices, List<Vector3> facePoints, Dictionary<(int, int), Vector3> edgePoints, Dictionary<int, List<int>> adjacencyList, int vertexIndex)
    {
        List<int> adjVertices = adjacencyList[vertexIndex];
        Vector3 vertex = vertices[vertexIndex];

        Vector3 facePointSum = Vector3.zero;
        Vector3 edgePointSum = Vector3.zero;
        foreach (int adjVertex in adjVertices)
        {
            Vector3 edgePoint = edgePoints[(Mathf.Min(vertexIndex, adjVertex), Mathf.Max(vertexIndex, adjVertex))];
            edgePointSum += edgePoint;
        }

        foreach (Vector3 facePoint in facePoints)
        {
            facePointSum += facePoint;
        }

        Vector3 newVertex = (facePointSum + 2 * edgePointSum + (adjVertices.Count - 3) * vertex) / adjVertices.Count;
        return newVertex;
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