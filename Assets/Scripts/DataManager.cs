using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager
{
    public static void SaveModules(ModuleSet moduleSet)
    {
        Debug.Log($"Saving map data to {MapPath}...");
        string jsonData = JsonUtility.ToJson(moduleSet, true);
        File.WriteAllText(MapPath, jsonData);
    }

    private static string MapPath => $"{Application.dataPath}/Resources/Modules.json";
}
