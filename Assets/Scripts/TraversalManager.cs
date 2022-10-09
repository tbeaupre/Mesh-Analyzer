using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversalManager
{
    public static void SetTraversalScores(List<FaceData> faceDatas)
    {
        foreach (FaceData fd in faceDatas)
            SetTraversalScore(fd);
    }

    static void SetTraversalScore(FaceData fd)
    {
        int traversalSum = 0;
        traversalSum += GetTraversalScoreForFace(fd.backFace);
        traversalSum += GetTraversalScoreForFace(fd.leftFace);
        traversalSum += GetTraversalScoreForFace(fd.rightFace);
        fd.traversalScore = traversalSum;
    }

    static int GetTraversalScoreForFace(Face face)
    {
        int traversalSum = 0;
        if (face.ContainsPoint(new Vertex(new Vector2(0, 0.1f))))
            ++traversalSum;
        if (face.ContainsPoint(new Vertex(new Vector2(0, 0.5f))))
            ++traversalSum;
        if (face.ContainsPoint(new Vertex(new Vector2(0.866025f, 0.1f))))
            ++traversalSum;
        if (face.ContainsPoint(new Vertex(new Vector2(0.866025f, 0.5f))))
            ++traversalSum;
        return traversalSum;
    }
}
