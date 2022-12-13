using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private MeshAnalyzer meshAnalyzer = new MeshAnalyzer();
    private SocketManager socketManager = new SocketManager();

    // Start is called before the first frame update
    void Start()
    {
        meshAnalyzer.Init();
        List<Mesh> meshes = MeshMirrorer.GetMeshesWithMirrors();
        List<FaceData> faceDatas = meshAnalyzer.SetUpList(meshes);
        TraversalManager.SetTraversalScores(faceDatas);
        ModuleSet moduleSet = socketManager.GetModuleSet(faceDatas);
        NeighborManager.SetValidNeighbors(moduleSet);
        DataManager.SaveModules(moduleSet);
    }
}
