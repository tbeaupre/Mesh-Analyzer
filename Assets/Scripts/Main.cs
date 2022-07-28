using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public MeshAnalyzer meshAnalyzer;
    public DataManager dataManager;
    private SocketManager socketManager = new SocketManager();

    // Start is called before the first frame update
    void Start()
    {
        meshAnalyzer.Init();
        List<FaceData> faceDatas = meshAnalyzer.SetUpList();
        ModuleSet moduleSet = socketManager.GetModuleSet(faceDatas);
        dataManager.SaveModules(moduleSet);
    }
}
