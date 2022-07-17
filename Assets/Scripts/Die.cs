using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Die : MonoBehaviour
{
    private Mesh mesh;
    private int[] triangles;
    private Vector3[] verts;
    private Rigidbody rb;
    private List<Vector3> allFaceNormals;
    private Color color;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        triangles = mesh.triangles;
        verts = mesh.vertices;
        rb = GetComponent<Rigidbody>();
        color = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);

        // Calculate normals
        allFaceNormals = new List<Vector3>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            allFaceNormals.Add(
                Vector3.Normalize(
                    Vector3.Cross(
                        verts[triangles[i + 1]] - verts[triangles[i]],
                        verts[triangles[i + 2]] - verts[triangles[i]]
                    )
                ) * 2.5f
            );
        }
        allFaceNormals = allFaceNormals.Distinct().ToList();
        Debug.Log(allFaceNormals.Count);
    }

    private void Update()
    {
        // Draw normals
        Vector3 center = transform.position + rb.centerOfMass;
        foreach (Vector3 v in allFaceNormals)
        {
            Debug.DrawRay(center, v, color);
        }
    }
}
