using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    public SortedSet<Vertex> vertices = new SortedSet<Vertex>();
    public Face flippedFace;
    public Face rotatedFace;

    public Face()
    {
        this.vertices = new SortedSet<Vertex>();
    }

    public Face(SortedSet<Vertex> vertices)
    {
        this.vertices = vertices;
    }

    public void Add(Vertex v)
    {
        vertices.Add(v);
    }

    public bool ContainsPoint(Vertex target)
    {
        foreach (Vertex v in vertices)
        {
            if (v == target)
                return true;
        }
        return false;
    }

    public Face GetRotatedFace(Matrix4x4 mat)
    {
        SortedSet<Vertex> rotatedVerts = new SortedSet<Vertex>();

        foreach (Vertex v in vertices)
        {
            var pt = mat.MultiplyPoint3x4(v.position);
            rotatedVerts.Add(new Vertex(pt));
        }

        return new Face(rotatedVerts);
    }

    public bool IsFaceRotationallySymmetrical()
    {
        Matrix4x4 rotMat = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -120));
        rotatedFace = GetRotatedFace(rotMat);
        return this == rotatedFace;
    }

    public bool IsFaceSymmetrical()
    {
        Matrix4x4 flipMat = Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0));
        flippedFace = GetRotatedFace(flipMat);
        return this == flippedFace;
    }

    public static bool operator ==(Face f1, Face f2)
    {
        if (f1.Equals(f2))
            return true;
        if (f1.vertices.Count != f2.vertices.Count)
            return false;

        // This is pretty gross, but the floating point precision errors were making .SetEquals() a non-option
        List<Vertex> list1 = new List<Vertex>(f1.vertices);
        List<Vertex> list2 = new List<Vertex>(f2.vertices);
        for (int i = 0; i < list1.Count; ++i)
        {
            if (list1[i] != list2[i])
                return false;
        }

        return true;
    }

    public static bool operator !=(Face f1, Face f2)
    {
        if (f1.Equals(f2))
            return false;
        if (f1.vertices.Count != f2.vertices.Count)
            return true;

        // This is pretty gross, but the floating point precision errors were making .SetEquals() a non-option
        List<Vertex> list1 = new List<Vertex>(f1.vertices);
        List<Vertex> list2 = new List<Vertex>(f2.vertices);
        for (int i = 0; i < list1.Count; ++i)
        {
            if (list1[i] != list2[i])
                return true;
        }

        return false;
    }
}