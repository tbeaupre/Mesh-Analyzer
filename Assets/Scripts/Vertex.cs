using System;
using UnityEngine;

public struct Vertex : IComparable<Vertex>
{
    public Vector2 position;

    public Vertex(Vector2 position)
    {
        this.position = position;
    }

    public static bool operator ==(Vertex v1, Vertex v2)
    {
        return Mathf.Abs(v1.position.x - v2.position.x) < MeshAnalyzer.PRECISION_ERROR &&
            Mathf.Abs(v1.position.y - v2.position.y) < MeshAnalyzer.PRECISION_ERROR;
    }

    public static bool operator !=(Vertex v1, Vertex v2)
    {
        return Mathf.Abs(v1.position.x - v2.position.x) >= MeshAnalyzer.PRECISION_ERROR ||
            Mathf.Abs(v1.position.y - v2.position.y) >= MeshAnalyzer.PRECISION_ERROR;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Vertex v = (Vertex)obj;
        return this == v;
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }

    public int CompareTo(Vertex other)
    {
        if (position.x.CompareTo(other.position.x) != 0)
            return position.x.CompareTo(other.position.x);
        else
            return position.y.CompareTo(other.position.y);
    }
}
