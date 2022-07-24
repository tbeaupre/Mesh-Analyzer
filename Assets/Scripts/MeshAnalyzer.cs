using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnalyzer : MonoBehaviour
{
    public MeshFilter meshFilterPrefab;

    private const float PRECISION_ERROR = 0.0001f;

    private Dictionary<string, FaceData> meshFaceDataMap = new Dictionary<string, FaceData>();
    private Vector3 centroid = (new Vector3(0.86603f, 0, -1.50f) + new Vector3(1.7321f, 0, 0)) / 3;
    private Vector3 sideOffset = new Vector3(0.86603f, 0, 0); // Subtract this from a side face to get the face geometry in a form that is symmetrical across x=0;
    private Matrix4x4 transMat;
    private Matrix4x4 rotMat;
    private Matrix4x4 returnMat;

    // Start is called before the first frame update
    void Start()
    {
        transMat = Matrix4x4.Translate(-centroid);
        rotMat = Matrix4x4.Rotate(Quaternion.Euler(0, 120, 0));
        returnMat = Matrix4x4.Translate(centroid);

        SetUpMap();
    }

    private void SetUpMap()
    {
        List<Mesh> meshes = new List<Mesh>(Resources.LoadAll<Mesh>("Meshes"));
        Debug.Log(meshes.Count);
        foreach (Mesh m in meshes)
        {
            FaceData faceData = new FaceData();

            for (int i = 0; i < m.vertexCount; ++i)
            {
                Vector3 v = m.vertices[i];
                Debug.Log($"{m.name}: {v}");
                if (Mathf.Abs(v.z) < PRECISION_ERROR)
                    faceData.backFace.Add(v - sideOffset);
                if (Mathf.Abs(v.y - 1.0f) < PRECISION_ERROR)
                {
                    Vector3 centeredVertex = v - centroid;
                    faceData.topFace.Add(new Vector2(centeredVertex.x, centeredVertex.z)); // Want the position relative to centroid so it can easily be checked for rotational symmetry
                }
                if (Mathf.Abs(v.y) < PRECISION_ERROR)
                {
                    Vector3 centeredVertex = v - centroid;
                    faceData.bottomFace.Add(new Vector2(centeredVertex.x, centeredVertex.z));
                }
            }

            Vector3[] rotatedVertices = RotateVertices(m.vertices);
            for (int i = 0; i < rotatedVertices.Length; ++i)
            {
                Vector3 v = rotatedVertices[i];
                if (Mathf.Abs(v.z) < PRECISION_ERROR)
                    faceData.leftFace.Add(v - sideOffset);
            }

            rotatedVertices = RotateVertices(rotatedVertices);
            for (int i = 0; i < rotatedVertices.Length; ++i)
            {
                Vector3 v = rotatedVertices[i];
                if (Mathf.Abs(v.z) < PRECISION_ERROR)
                    faceData.rightFace.Add(v - sideOffset);
            }

            meshFaceDataMap[m.name] = faceData;
        }
    }

    private Vector3[] RotateVertices(Vector3[] vertices)
    {
        Vector3[] newVerts = new Vector3[vertices.Length];

        for (var i = 0; i < vertices.Length; ++i)
        {
            var pt = transMat.MultiplyPoint3x4(vertices[i]);
            pt = rotMat.MultiplyPoint3x4(pt);
            pt = returnMat.MultiplyPoint3x4(pt);
            newVerts[i] = pt;
        }

        return newVerts;
    }
}

public class FaceData
{
    public HashSet<Vector2> backFace;
    public HashSet<Vector2> rightFace;
    public HashSet<Vector2> leftFace;
    public HashSet<Vector2> topFace;
    public HashSet<Vector2> bottomFace;

    public FaceData()
    {
        backFace = new HashSet<Vector2>();
        rightFace = new HashSet<Vector2>();
        leftFace = new HashSet<Vector2>();
        topFace = new HashSet<Vector2>();
        bottomFace = new HashSet<Vector2>();
    }
}
