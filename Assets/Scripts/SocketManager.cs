using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketManager
{
    Dictionary<Face, string> socketMap = new Dictionary<Face, string>();
    private int socketCount = 0;

    public ModuleSet GetModuleSet(List<FaceData> faceDatas)
    {
        Face emptyFace = new Face();
        socketMap.Add(emptyFace, "0s");
        socketCount++;

        List<Module> modules = new List<Module>();

        foreach (FaceData faceData in faceDatas)
        {
            Module m = GetModule(faceData);
            modules.Add(m);

            // Rotationally symmetrical prototypes don't need their rotations to be added to the list
            if (m.sockets.top.EndsWith("s") &&
                m.sockets.bottom.EndsWith("s") &&
                m.sockets.back == m.sockets.left &&
                m.sockets.left == m.sockets.right)
                continue;

            Module m1 = Module.GetRotatedModule(m);
            modules.Add(m1);

            modules.Add(Module.GetRotatedModule(m1));
        }

        return new ModuleSet(modules.ToArray());
    }

    Module GetModule(FaceData faceData)
    {
        Module result = new Module();
        result.name = faceData.name;
        result.meshName = faceData.meshName;
        result.rotation = 0;
        result.isFacingUp = faceData.isFacingUp;
        result.traversalScore = faceData.traversalScore;
        result.sockets = new SocketSet();

        result.sockets.back = GetSocketForSideFace(faceData.backFace);
        result.sockets.right = GetSocketForSideFace(faceData.rightFace);
        result.sockets.left = GetSocketForSideFace(faceData.leftFace);
        result.sockets.top = GetSocketForVerticalFace(faceData.topFace);
        result.sockets.bottom = GetSocketForVerticalFace(faceData.bottomFace);
        return result;
    }

    string GetSocketForSideFace(Face face)
    {
        Face key = SocketMapContainsFace(face);
        if (!(key is null))
            return socketMap[key];

        string newSocket;
        if (face.IsFaceSymmetrical())
        {
            newSocket = $"{socketCount}s";
        }
        else
        {
            newSocket = socketCount.ToString();

            string flippedSocket = $"{socketCount}f";
            socketMap.Add(face.flippedFace, flippedSocket);
        }

        socketMap.Add(face, newSocket);
        ++socketCount;
        return newSocket;
    }

    string GetSocketForVerticalFace(Face face)
    {
        Face key = SocketMapContainsFace(face);
        if (!(key is null))
            return socketMap[key];

        string newSocket;
        if (face.IsFaceRotationallySymmetrical())
        {
            newSocket = $"v{socketCount}s";
        }
        else
        {
            newSocket = $"v{socketCount}_0";

            string r1Socket = $"v{socketCount}_1";
            socketMap.Add(face.rotatedFace, r1Socket);

            string r2Socket = $"v{socketCount}_2";
            Matrix4x4 rotMat = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 120));
            socketMap.Add(face.rotatedFace.GetRotatedFace(rotMat), r2Socket);
        }

        socketMap.Add(face, newSocket);
        ++socketCount;
        return newSocket;
    }

    Face SocketMapContainsFace(Face face)
    {
        foreach (Face faceKey in socketMap.Keys)
        {
            if (faceKey == face)
                return faceKey;
        }
        return null;
    }
}
