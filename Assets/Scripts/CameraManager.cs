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

        Debug.Log("!");

        terrainWidth = 200;
        terrainHeight = 200;

        globalTerrain = new GlobalTerrain(Math.Max(terrainWidth, terrainHeight));
        Debug.Log("!");
        localTerrain = new LocalTerrain(terrainWidth, terrainHeight, 30, globalTerrain);
        Debug.Log("!");
        terrainGenerator = new TerrainGenerator(globalTerrain, localTerrain);
        Debug.Log("!");
        terrainGenerator.GenerateTerrainOn(localTerrain.visibleTerrain);
        Debug.Log("!");
    }
	
	void Update () {
	
	}
}
