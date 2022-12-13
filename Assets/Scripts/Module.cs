using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModuleSet
{
    public Module[] modules;

    public ModuleSet(Module[] modules)
    {
        this.modules = modules;
    }
}

[System.Serializable]
public class Module
{
    public string name;
    public string meshName;
    public int rotation;
    public bool isFacingUp;
    public TraversalSet traversalSet;
    public SocketSet sockets;
    public ValidNeighbors validNeighbors;

    public static Module GetRotatedModule(Module m)
    {
        Module rotatedModule = new Module();
        int newRotation = (m.rotation + 1) % 3;

        string[] subs = m.name.Split('_');
        rotatedModule.name = $"{subs[0]}_{newRotation}";

        rotatedModule.meshName = m.meshName;
        rotatedModule.rotation = newRotation;
        rotatedModule.isFacingUp = m.isFacingUp;

        TraversalSet traversalSet = new TraversalSet();
        traversalSet.back = m.traversalSet.left;
        traversalSet.right = m.traversalSet.back;
        traversalSet.left = m.traversalSet.right;
        traversalSet.top = m.traversalSet.top;
        traversalSet.bottom = m.traversalSet.bottom;
        rotatedModule.traversalSet = traversalSet;

        SocketSet sockets = new SocketSet();
        sockets.back = m.sockets.left;
        sockets.right = m.sockets.back;
        sockets.left = m.sockets.right;
        sockets.top = GetRotatedVerticalSocketString(m.sockets.top);
        sockets.bottom = GetRotatedVerticalSocketString(m.sockets.bottom);
        rotatedModule.sockets = sockets;

        return rotatedModule;
    }

    static string GetRotatedVerticalSocketString(string socketName)
    {
        if (socketName.EndsWith("s"))
            return socketName;
        string[] subs = socketName.Split('_');
        int socketRotation = (int.Parse(subs[1]) + 1) % 3;
        return $"{subs[0]}_{socketRotation}";
    }
}

[System.Serializable]
public class TraversalSet
{
    public bool back;
    public bool right;
    public bool left;
    public bool top;
    public bool bottom;

    public bool GetIsTraversableInDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Back:
                return back;
            case Direction.Right:
                return right;
            case Direction.Left:
                return left;
            case Direction.Top:
                return top;
            case Direction.Bottom:
                return bottom;
            default:
                return top;
        }
    }
}

[System.Serializable]
public class SocketSet
{
    public string back;
    public string right;
    public string left;
    public string top;
    public string bottom;

    public string GetSocketInDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Back:
                return back;
            case Direction.Right:
                return right;
            case Direction.Left:
                return left;
            case Direction.Top:
                return top;
            case Direction.Bottom:
                return bottom;
            default:
                return top;
        }
    }
}

[System.Serializable]
public class ValidNeighbors
{
    public List<string> back;
    public List<string> right;
    public List<string> left;
    public List<string> top;
    public List<string> bottom;

    public ValidNeighbors()
    {
        back = new List<string>();
        right = new List<string>();
        left = new List<string>();
        top = new List<string>();
        bottom = new List<string>();
    }
}
