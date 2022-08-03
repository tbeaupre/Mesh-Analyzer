using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshAnalyzer
{
    public MeshFilter meshFilterPrefab;
    public static readonly float PRECISION_ERROR = 0.0001f;

    private List<FaceData> faceDatas = new List<FaceData>();
    private Vector3 centroid = (new Vector3(0.86603f, 0, -1.50f) + new Vector3(1.7321f, 0, 0)) / 3;
    private Vector3 sideOffset = new Vector3(0.86603f, 0, 0); // Subtract this from a side face to get the face geometry in a form that is symmetrical across x=0;
    private Matrix4x4 transMat;
    private Matrix4x4 rotMat;
    private Matrix4x4 returnMat;

    public void Init()
    {
        transMat = Matrix4x4.Translate(-centroid);
        rotMat = Matrix4x4.Rotate(Quaternion.Euler(0, 120, 0));
        returnMat = Matrix4x4.Translate(centroid);
    }

    public List<FaceData> SetUpList()
    {
        List<Mesh> meshes = new List<Mesh>(Resources.LoadAll<Mesh>("Meshes"));
        Debug.Log(meshes.Count);
        foreach (Mesh m in meshes)
        {
            FaceData faceData = new FaceData(m.name);
            Vector3 sumOfNormals = Vector3.zero;

            for (int i = 0; i < m.vertexCount; ++i)
            {
                sumOfNormals += m.normals[i];
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

            // Check if the mesh is facing mostly up or mostly down.
            faceData.isFacingUp = sumOfNormals.y > PRECISION_ERROR;

            faceDatas.Add(faceData);
        }
        FaceData externalFaceData = new FaceData("", FaceData.EXTERNAL_NAME);
        externalFaceData.isFacingUp = true;
        FaceData internalFaceData = new FaceData("", FaceData.INTERNAL_NAME);
        internalFaceData.isFacingUp = true;

        faceDatas.Add(externalFaceData);
        faceDatas.Add(internalFaceData);
        return faceDatas;
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
    public static readonly string EXTERNAL_NAME = "EEE";
    public static readonly string INTERNAL_NAME = "III";

    public string name;
    public string meshName;
    public bool isFacingUp;
    public Face backFace;
    public Face rightFace;
    public Face leftFace;
    public Face topFace;
    public Face bottomFace;

    public FaceData(string meshName, string name = "")
    {
        this.name = name != "" ? name : meshName;
        this.meshName = meshName;
        isFacingUp = false;
        backFace = new Face();
        rightFace = new Face();
        leftFace = new Face();
        topFace = new Face();
        bottomFace = new Face();
    }
}