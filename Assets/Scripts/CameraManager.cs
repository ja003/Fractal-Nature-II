using UnityEngine;
using System.Collections;
using System;

public class CameraManager : MonoBehaviour, ICameraManager
{
    private GlobalTerrain globalTerrain;
    private LocalTerrain localTerrain;
    private TerrainGenerator terrainGenerator;

    int terrainWidth;
    int terrainHeight;

    void Start () {
        
        terrainWidth = 200;
        terrainHeight = 200;

        globalTerrain = new GlobalTerrain(Math.Max(terrainWidth, terrainHeight));
        terrainGenerator = new TerrainGenerator();
        localTerrain = new LocalTerrain(terrainWidth, terrainHeight, 30);

        AssignFunctions();
        terrainGenerator.initialize();
        localTerrain.UpdateVisibleTerrain(new Vector3(0, 0, 0));
    }

    public void AssignFunctions()
    {
        localTerrain.globalTerrain = globalTerrain;
        localTerrain.terrainGenerator = terrainGenerator;

        terrainGenerator.globalTerrain = globalTerrain;
        terrainGenerator.localTerrain = localTerrain;
    }

    int lastActionFrame = 0;

    void Update () {

        if (Input.GetKey("8") && lastActionFrame < Time.frameCount - 30)
        {
            //Debug.Log("generating on: " + gameObject.transform.position);
            localTerrain.UpdateVisibleTerrain(gameObject.transform.position);
            lastActionFrame = Time.frameCount;
        }
    }
}
