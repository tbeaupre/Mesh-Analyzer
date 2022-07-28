using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public void SaveModules(ModuleSet moduleSet)
    {
        Debug.Log($"Saving map data to {MapPath}...");
        string jsonData = JsonUtility.ToJson(moduleSet, true);
        File.WriteAllText(MapPath, jsonData);
    }

    private string MapPath => $"{Application.dataPath}/Resources/Modules.json";
}
