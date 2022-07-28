using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketManager
{
    Dictionary<Face, string> socketMap = new Dictionary<Face, string>();
    private int socketCount = 0;

    public ModuleSet GetModuleSet(List<FaceData> faceDatas)
    {
        List<Module> modules = new List<Module>();

        foreach (FaceData faceData in faceDatas)
        {
            Module m = GetModule(faceData);
            modules.Add(m);

            // Rotationally symmetrical prototypes don't need their rotations to be added to the list
            if (m.sockets.top.EndsWith("s") && m.sockets.bottom.EndsWith("s"))
                continue;

            Module m1 = new Module();
            m1.name = m.name + "_1";
            m1.meshName = m.meshName;
            m1.rotation = 1;
            SocketSet sockets1 = new SocketSet();
            sockets1.back = m.sockets.left;
            sockets1.right = m.sockets.back;
            sockets1.left = m.sockets.right;
            sockets1.top = GetRotatedVerticalSocketString(m.sockets.top);
            sockets1.bottom = GetRotatedVerticalSocketString(m.sockets.bottom);
            m1.sockets = sockets1;
            modules.Add(m1);

            Module m2 = new Module();
            m2.name = m.name + "_2";
            m2.meshName = m.meshName;
            m2.rotation = 2;
            SocketSet sockets2 = new SocketSet();
            sockets2.back = m.sockets.right;
            sockets2.right = m.sockets.left;
            sockets2.left = m.sockets.back;
            sockets2.top = GetRotatedVerticalSocketString(m1.sockets.top);
            sockets2.bottom = GetRotatedVerticalSocketString(m1.sockets.bottom);
            m2.sockets = sockets2;
            modules.Add(m2);
        }

        return new ModuleSet(modules.ToArray());
    }

    Module GetModule(FaceData faceData)
    {
        Module result = new Module();
        result.name = faceData.meshName;
        result.meshName = faceData.meshName;
        result.rotation = 0;
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
            newSocket = socketCount + "s";
        }
        else
        {
            newSocket = socketCount.ToString();

            string flippedSocket = socketCount + "f";
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
    string GetRotatedVerticalSocketString(string socketName)
    {
        if (socketName.EndsWith("s"))
            return socketName;
        string[] subs = socketName.Split('_');
        int socketRotation = (int.Parse(subs[1]) + 1) % 3;
        return subs[0] + "_" + socketRotation;
    }
}
