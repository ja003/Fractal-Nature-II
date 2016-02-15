using UnityEngine;
using System.Collections;
using System;

public class LocalTerrain : ILocalTerrain {

    public float[,] visibleTerrain;
    public Vector3 center;
    private int stepSize;
    public int terrainWidth;
    public int terrainHeight;
    public GlobalTerrain globalTerrain;
    public TerrainGenerator terrainGenerator;

    public LocalTerrain( int terrainWidth, int terrainHeight, int stepSize)
    {
        visibleTerrain = new float[terrainHeight, terrainWidth];
        center = new Vector3(0, 0, 0);
        this.terrainHeight = terrainHeight;
        this.terrainWidth = terrainWidth;
        this.stepSize = stepSize;
        Debug.Log(":?");
    }
    

    /// <summary>
    /// updates values to visible terrain based on camera position
    /// </summary>
    /// <param name="cameraPosition"></param>
    public void UpdateVisibleTerrain(Vector3 cameraPosition)
    {
        center = cameraPosition;

        for(int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0 ; z < terrainHeight; z++)
            {
                //visibleTerrain[x, z] = 666f;
                visibleTerrain[x, z] = GetGlobalHeight(x, z);
            }
        }

        terrainGenerator.GenerateTerrainOn(visibleTerrain);

    }
    
    /// <summary>
    /// returns height from global terrain
    /// maps given local coordinates on global
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public float GetGlobalHeight(int x, int z)
    {
        //Debug.Log("getting " + x + "," + z);
        //Debug.Log("= " + (x + (int)center.x - terrainWidth) + "," + (z + (int)center.z - terrainHeight / 2));
        return globalTerrain.GetHeight(x + (int)center.x - terrainWidth / 2, z + (int)center.z - terrainHeight / 2);
    }
    /// <summary>
    /// sets height to global terrain
    /// maps given local coordinates on global
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void SetGlobalHeight(int x, int z, float height, bool overwrite)
    {
        globalTerrain.SetHeight(x + (int)center.x - terrainWidth / 2, z + (int)center.z - terrainHeight / 2, height, overwrite);
    }



    public void SetHeight(int x, int z, float height, bool overwrite)
    {
        if (!overwrite && GetGlobalHeight(x, z) != 666)
            return;

        visibleTerrain[x, z] = height;
    }
}
