using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshNameHelper
{
    public static string GetRotatedMeshName(string meshName)
    {
        string result = "";

        foreach (string substr in meshName.Split('-'))
        {
            if (result != "")
                result += "-";
            result += substr[1];
            result += substr[2];
            result += substr[0];
        }

        return result;
    }

    public static string GetMirroredMeshName(string meshName)
    {
        string result = "";

        foreach (string substr in meshName.Split('-'))
        {
            if (result != "")
                result += "-";
            result += substr[1];
            result += substr[0];
            result += substr[2];
        }

        return result;
    }

    public static bool IsSymmetrical(string meshName)
    {
        return meshName == GetMirroredMeshName(meshName);
    }

    public static bool IsRotationallySymmetrical(string meshName)
    {
        foreach (string substr in meshName.Split('-'))
        {
            if (substr[0] != substr[1] || substr[1] != substr[2])
                return false;
        }

        return true;
    }
}
