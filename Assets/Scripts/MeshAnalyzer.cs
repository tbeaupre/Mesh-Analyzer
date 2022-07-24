using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshAnalyzer : MonoBehaviour
{
    public MeshFilter meshFilterPrefab;
    public static readonly float PRECISION_ERROR = 0.0001f;

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
            FaceData faceData = new FaceData(m.name);

            for (int i = 0; i < m.vertexCount; ++i)
            {
                Vector3 v = m.vertices[i];
                Debug.Log($"{m.name}: {v}");
                if (Mathf.Abs(v.z) < PRECISION_ERROR)
                    faceData.backFace.Add(new Vertex(v - sideOffset));
                if (Mathf.Abs(v.y - 1.0f) < PRECISION_ERROR)
                {
                    Vector3 centeredVertex = v - centroid; // Want the position relative to centroid so it can easily be checked for rotational symmetry
                    faceData.topFace.Add(new Vertex(new Vector2(centeredVertex.x, centeredVertex.z)));
                }
                if (Mathf.Abs(v.y) < PRECISION_ERROR)
                {
                    Vector3 centeredVertex = v - centroid; // Want the position relative to centroid so it can easily be checked for rotational symmetry
                    faceData.bottomFace.Add(new Vertex(new Vector2(centeredVertex.x, centeredVertex.z)));
                }
            }

            Vector3[] rotatedVertices = RotateVertices(m.vertices);
            for (int i = 0; i < rotatedVertices.Length; ++i)
            {
                Vector3 v = rotatedVertices[i];
                if (Mathf.Abs(v.z) < PRECISION_ERROR)
                    faceData.leftFace.Add(new Vertex(v - sideOffset));
            }

            rotatedVertices = RotateVertices(rotatedVertices);
            for (int i = 0; i < rotatedVertices.Length; ++i)
            {
                Vector3 v = rotatedVertices[i];
                if (Mathf.Abs(v.z) < PRECISION_ERROR)
                    faceData.rightFace.Add(new Vertex(v - sideOffset));
            }

            meshFaceDataMap[m.name] = faceData;
            Debug.Log($"{m.name}: {faceData.IsSymmetrical()}");
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
    public string meshName;
    public SortedSet<Vertex> backFace;
    public SortedSet<Vertex> rightFace;
    public SortedSet<Vertex> leftFace;
    public SortedSet<Vertex> topFace;
    public SortedSet<Vertex> bottomFace;

    public FaceData(string meshName)
    {
        this.meshName = meshName;
        backFace = new SortedSet<Vertex>();
        rightFace = new SortedSet<Vertex>();
        leftFace = new SortedSet<Vertex>();
        topFace = new SortedSet<Vertex>();
        bottomFace = new SortedSet<Vertex>();
    }

    public bool IsSymmetrical()
    {
        return AreFacesEqual(backFace, rightFace) && AreFacesEqual(rightFace, leftFace);
    }

    private bool AreFacesEqual(SortedSet<Vertex> face1, SortedSet<Vertex> face2)
    {
        if (face1.Equals(face2))
            return true;
        if (face1.Count != face2.Count)
            return false;

        // This is pretty gross, but the floating point precision errors were making .SetEquals() a non-option
        List<Vertex> list1 = new List<Vertex>(face1);
        List<Vertex> list2 = new List<Vertex>(face2);
        for (int i = 0; i < list1.Count; ++i)
        {
            if (list1[i] != list2[i])
                return false;
        }

        return true;
    }
}
