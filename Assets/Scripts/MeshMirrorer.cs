using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshMirrorer
{
    HashSet<string> meshNames = new HashSet<string>();
    List<Mesh> mirroredMeshes = new List<Mesh>();

    public MeshMirrorer()
    {
        List<Mesh> meshes = new List<Mesh>(Resources.LoadAll<Mesh>("Meshes"));

        foreach(Mesh mesh in meshes)
        {
            string rotatedMeshName = MeshNameHelper.GetRotatedMeshName(mesh.name);
            meshNames.Add(mesh.name);
            meshNames.Add(rotatedMeshName);
            meshNames.Add(MeshNameHelper.GetRotatedMeshName(rotatedMeshName));
        }

        MirrorMeshes(meshes);

        foreach (Mesh mesh in mirroredMeshes)
        {
            AssetDatabase.CreateAsset(mesh, $"Assets/Resources/Mirrors/{mesh.name}.asset");
        }
        AssetDatabase.SaveAssets();
    }

    public void MirrorMeshes(List<Mesh> meshes)
    {
        foreach (Mesh mesh in meshes)
        {
            if (MeshNameHelper.IsSymmetrical(mesh.name))
                continue;

            Mesh mirroredMesh = MirrorMesh(mesh);
            if (mirroredMesh is not null)
            {
                mirroredMeshes.Add(mirroredMesh);
                meshNames.Add(mirroredMesh.name);
            }
        }
    }

    private Mesh MirrorMesh(Mesh mesh)
    {
        string mirroredMeshName = MeshNameHelper.GetMirroredMeshName(mesh.name);
        if (meshNames.Contains(mirroredMeshName))
            return null;

        Mesh mirroredMesh = new Mesh();
        Matrix4x4 mirrorTranslateMat = Matrix4x4.TRS(new Vector3(1.732052f, 0, 0), Quaternion.identity, new Vector3(-1, 1, 1));
        Matrix4x4 mirrorMat = Matrix4x4.Scale(new Vector3(-1, 1, 1));

        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector3[] newVerts = new Vector3[verts.Length];
        Vector3[] newNormals = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; ++i)
        {
            newVerts[i] = mirrorTranslateMat.MultiplyPoint3x4(verts[i]);
            newNormals[i] = mirrorMat.MultiplyPoint3x4(normals[i]);
        }

        mirroredMesh.vertices = newVerts;
        mirroredMesh.normals = newNormals;

        List<int> reversedTriangles = new List<int>(mesh.triangles);
        reversedTriangles.Reverse();
        mirroredMesh.triangles = reversedTriangles.ToArray();

        mirroredMesh.name = mirroredMeshName;

        return mirroredMesh;
    }
}
