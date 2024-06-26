using UnityEngine;
using System.Collections.Generic;

public class KobbeltMeshSubdiv : MonoBehaviour
{
    // Reference to the mesh filter of the object
    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        // Ensure there's a mesh filter attached
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not assigned!");
            return;
        }

        // Get the mesh and its data
        Mesh originalMesh = meshFilter.mesh;
        Vector3[] originalVertices = originalMesh.vertices;
        int[] originalTriangles = originalMesh.triangles;

        // Prepare lists for new vertices and triangles
        List<Vector3> newVertices = new List<Vector3>(originalVertices);
        List<int> newTriangles = new List<int>(originalTriangles);

        // Dictionary to store edge midpoints (to avoid duplicates)
        Dictionary<Edge, int> edgeMidpoints = new Dictionary<Edge, int>();

        // Iterate through each triangle and subdivide
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            // Original triangle vertices
            int v1 = originalTriangles[i];
            int v2 = originalTriangles[i + 1];
            int v3 = originalTriangles[i + 2];

            // Compute center of the triangle
            Vector3 center = (originalVertices[v1] + originalVertices[v2] + originalVertices[v3]) / 3f;

            // Subdivide the triangle into three new triangles
            int centerIndex = newVertices.Count;
            newVertices.Add(center);

            // Edge midpoint indices
            int v12, v23, v31;

            // Edge between v1 and v2
            if (!edgeMidpoints.TryGetValue(new Edge(v1, v2), out v12))
            {
                v12 = newVertices.Count;
                newVertices.Add((originalVertices[v1] + originalVertices[v2]) / 2f);
                edgeMidpoints[new Edge(v1, v2)] = v12;
            }

            // Edge between v2 and v3
            if (!edgeMidpoints.TryGetValue(new Edge(v2, v3), out v23))
            {
                v23 = newVertices.Count;
                newVertices.Add((originalVertices[v2] + originalVertices[v3]) / 2f);
                edgeMidpoints[new Edge(v2, v3)] = v23;
            }

            // Edge between v3 and v1
            if (!edgeMidpoints.TryGetValue(new Edge(v3, v1), out v31))
            {
                v31 = newVertices.Count;
                newVertices.Add((originalVertices[v3] + originalVertices[v1]) / 2f);
                edgeMidpoints[new Edge(v3, v1)] = v31;
            }

            // Create new triangles
            newTriangles.Add(v1); newTriangles.Add(v12); newTriangles.Add(centerIndex);
            newTriangles.Add(v12); newTriangles.Add(v2); newTriangles.Add(centerIndex);
            newTriangles.Add(v2); newTriangles.Add(v23); newTriangles.Add(centerIndex);
            newTriangles.Add(v23); newTriangles.Add(v3); newTriangles.Add(centerIndex);
            newTriangles.Add(v3); newTriangles.Add(v31); newTriangles.Add(centerIndex);
            newTriangles.Add(v31); newTriangles.Add(v1); newTriangles.Add(centerIndex);
        }

        // Apply the new vertices and triangles to the mesh
        Mesh subdividedMesh = new Mesh();
        subdividedMesh.vertices = newVertices.ToArray();
        subdividedMesh.triangles = newTriangles.ToArray();
        subdividedMesh.RecalculateNormals(); // Recalculate normals for smooth shading

        // Assign the new mesh to the MeshFilter
        meshFilter.mesh = subdividedMesh;
    }

    // Helper class to represent an edge between two vertices
    private struct Edge
    {
        public int v1;
        public int v2;

        public Edge(int v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public override int GetHashCode()
        {
            // Combine hash codes of vertices
            return v1.GetHashCode() ^ v2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge))
                return false;

            Edge other = (Edge)obj;
            return (v1 == other.v1 && v2 == other.v2) || (v1 == other.v2 && v2 == other.v1);
        }
    }
}
