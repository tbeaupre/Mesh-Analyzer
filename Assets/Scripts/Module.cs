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
    public SocketSet sockets;
    public ValidNeighbors validNeighbors;
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