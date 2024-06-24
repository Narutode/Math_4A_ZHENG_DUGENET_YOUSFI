using System.Collections.Generic;
using UnityEngine;

public class CoonsSubdivisionMerger : MonoBehaviour
{
    public CoonsPatchGenerator coonsPatchGenerator;
    public LoopSubdivision loopSubdivision;
    public KobbeltSubdivision kobbeltSubdivision;
    public ButterflySubdivision butterflySubdivision;

    void Start()
    {
        // Récupérer les patchs de Coons
        Mesh coonsMesh = GenerateCoonsMesh();

        // Appliquer la subdivision de Loop sur les bords des patchs
        Mesh loopSubdividedMesh = loopSubdivision.Subdivide(coonsMesh);

        // Appliquer la subdivision de Kobbelt sur les bords des patchs
        Mesh kobbeltSubdividedMesh = kobbeltSubdivision.Subdivide(coonsMesh);

        // Appliquer la subdivision Butterfly sur les bords des patchs
        Mesh butterflySubdividedMesh = butterflySubdivision.Subdivide(coonsMesh);

        // Choisissez un des maillages subdivisés pour l'affichage
        coonsPatchGenerator.GetComponent<MeshFilter>().mesh = loopSubdividedMesh;
    }

    Mesh GenerateCoonsMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int resolution = coonsPatchGenerator.Resolution;

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float u = i / (float)(resolution - 1);
                float v = j / (float)(resolution - 1);
                Vector3 point = coonsPatchGenerator.CoonsPatch(u, v);
                vertices.Add(point);
                if (i < resolution - 1 && j < resolution - 1)
                {
                    int topLeft = i * resolution + j;
                    int topRight = topLeft + 1;
                    int bottomLeft = (i + 1) * resolution + j;
                    int bottomRight = bottomLeft + 1;

                    triangles.Add(topLeft);
                    triangles.Add(bottomLeft);
                    triangles.Add(topRight);

                    triangles.Add(topRight);
                    triangles.Add(bottomLeft);
                    triangles.Add(bottomRight);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
