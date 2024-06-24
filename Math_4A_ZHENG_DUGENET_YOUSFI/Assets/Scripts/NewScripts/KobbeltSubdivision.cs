using System.Collections.Generic;
using UnityEngine;

public class KobbeltSubdivision : MonoBehaviour
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
        // L'algorithme de subdivision de Kobbelt
        List<Vector3> newVertices = new List<Vector3>(mesh.vertices);
        List<int> newTriangles = new List<int>();

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int v0 = mesh.triangles[i];
            int v1 = mesh.triangles[i + 1];
            int v2 = mesh.triangles[i + 2];

            Vector3 p0 = mesh.vertices[v0];
            Vector3 p1 = mesh.vertices[v1];
            Vector3 p2 = mesh.vertices[v2];

            Vector3 m0 = (p0 + p1 + p2) / 3.0f;
            newVertices.Add(m0);
            int m0Index = newVertices.Count - 1;

            newTriangles.AddRange(new int[] { v0, v1, m0Index });
            newTriangles.AddRange(new int[] { v1, v2, m0Index });
            newTriangles.AddRange(new int[] { v2, v0, m0Index });
        }

        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
