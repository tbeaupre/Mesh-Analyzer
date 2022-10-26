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
        if (fd.meshName.Length == 0)
            return;
        string cliffStr = fd.meshName.Split('-')[1]; // Ex: FFF-CCE-R
        fd.traversalSet.back = IsTraversableOnSide(fd.backFace) &&
            (cliffStr[0] == 'E' || cliffStr[1] == 'E');
        fd.traversalSet.right = IsTraversableOnSide(fd.rightFace) &&
            (cliffStr[1] == 'E' || cliffStr[2] == 'E');
        fd.traversalSet.left = IsTraversableOnSide(fd.leftFace) &&
            (cliffStr[2] == 'E' || cliffStr[0] == 'E');
        fd.traversalSet.top = IsTraversableAbove(fd);
        fd.traversalSet.bottom = IsTraversableBelow(fd);
    }

    static bool IsTraversableAbove(FaceData fd)
    {
        bool back = fd.backFace.ContainsPoint(new Vertex(0, 1));
        bool right = fd.rightFace.ContainsPoint(new Vertex(0, 1));
        bool left = fd.leftFace.ContainsPoint(new Vertex(0, 1));

        return ((back && right) || (right && left) || (left && back));
    }

    static bool IsTraversableBelow(FaceData fd)
    {
        bool back = fd.backFace.ContainsPoint(new Vertex(0, 0));
        bool right = fd.rightFace.ContainsPoint(new Vertex(0, 0));
        bool left = fd.leftFace.ContainsPoint(new Vertex(0, 0));

        return ((back && right) || (right && left) || (left && back));
    }

    static bool IsTraversableOnSide(Face face)
    {
        return face.ContainsPoint(new Vertex(0, 0.5f)) ||
            face.ContainsPoint(new Vertex(0, 1)) ||
            (face.ContainsPoint(new Vertex(0, 0)) && // Covers Hills
                (face.ContainsPoint(new Vertex(0.433013f, 0.25f)) || // Covers Hilltops but not Ocean
                face.ContainsPoint(new Vertex(-0.433013f, 0.25f))));
    }
}
