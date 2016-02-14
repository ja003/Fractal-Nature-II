using UnityEngine;
using System.Collections;
using System;

public class LocalTerrain : ILocalTerrain {

    public float[,] visibleTerrain;
    private Vector3 center;
    private int stepSize;
    public int terrainWidth;
    public int terrainHeight;
    private GlobalTerrain globalTerrain;

    public LocalTerrain( int terrainWidth, int terrainHeight, int stepSize, GlobalTerrain globalTerrain)
    {
        this.globalTerrain = globalTerrain;
        visibleTerrain = new float[terrainHeight, terrainWidth];
        center = new Vector3(0, 0, 0);
        this.terrainHeight = terrainHeight;
        this.terrainWidth = terrainWidth;
        this.stepSize = stepSize;
        UpdateVisibleTerrain(center);

        Debug.Log(":?");
    }

    /// <summary>
    /// updates values to visible terrain based on camera position
    /// </summary>
    /// <param name="cameraPosition"></param>
    public void UpdateVisibleTerrain(Vector3 cameraPosition)
    {
        for(int x = 0; x < terrainWidth; x++)
        {
            for (int z = 0 ; z < terrainHeight; z++)
            {
                visibleTerrain[x, z] = 666f;/*
                    globalTerrain.GetHeight(
                        x + (int)center.x - terrainWidth / 2,
                        z + (int)center.z - terrainHeight / 2);*/
            }
        }
    }
    
    /// <summary>
    /// returns height from global terrain
    /// maps given local coordinates on global
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public float GetHeight(int x, int z)
    {
        //Debug.Log("getting " + x + "," + z);
        //Debug.Log("= " + (x + (int)center.x - terrainWidth) + "," + (z + (int)center.z - terrainHeight / 2));
        return globalTerrain.GetHeight(x + (int)center.x - terrainWidth / 2, z + (int)center.z - terrainHeight / 2);
    }
}
