using UnityEngine;
using System.Collections;
using System;

public class LocalTerrain : ILocalTerrain {

    public float[,] visibleTerrain; //practicaly not neccessary
    

    private int stepSize;
    public int terrainWidth;
    public int terrainHeight;
    public TerrainGenerator terrainGenerator;
    public LocalCoordinates localTerrainC;

    public LocalTerrain(int terrainWidth, int terrainHeight, int stepSize)
    {
        visibleTerrain = new float[terrainHeight, terrainWidth];
        this.terrainHeight = terrainHeight;
        this.terrainWidth = terrainWidth;

        this.stepSize = stepSize;
        Debug.Log(":?");
    }

    public void AssignFunctions(GlobalCoordinates globalTerrain)
    {
        localTerrainC = new LocalCoordinates(globalTerrain, new Vector3(0, 0, 0), terrainWidth, terrainHeight);
    }

    /// <summary>
    /// updates values to visible terrain based on camera position
    /// </summary>
    /// <param name="cameraPosition"></param>
    public void UpdateVisibleTerrain(Vector3 cameraPosition)
    {
        MoveVisibleTerrain(cameraPosition);

        terrainGenerator.GenerateTerrainOn(visibleTerrain, localTerrainC.botLeft, localTerrainC.topRight);        
    }

    public void MoveVisibleTerrain(Vector3 cameraPosition)
    {


        UpdateLocalCoordinates(cameraPosition,
            new Vector3(cameraPosition.x - terrainWidth / 2, 0, cameraPosition.z - terrainHeight / 2),
            new Vector3(cameraPosition.x + terrainWidth / 2, 0, cameraPosition.z + terrainHeight / 2));
        
        terrainGenerator.MoveVisibleTerrain(localTerrainC.center);
    }

    /// <summary>
    /// updates all local centers based on Localterrain position
    /// params should always come in rounded values!
    /// </summary>
    public void UpdateLocalCoordinates(Vector3 center, Vector3 botLeft, Vector3 topRight)
    {
        localTerrainC.center = center;
        localTerrainC.botLeft = botLeft;
        localTerrainC.topRight = topRight;

        terrainGenerator.filterGenerator.localFilterC.center = center;
        terrainGenerator.filterGenerator.localFilterC.botLeft = botLeft;
        terrainGenerator.filterGenerator.localFilterC.topRight = topRight;

    }

    public Vector3 GetGlobalCoordinate(int x, int z)
    {
        return new Vector3(x + (int)localTerrainC.center.x - terrainWidth / 2, 0, z + (int)localTerrainC.center.z - terrainHeight / 2);
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
        return localTerrainC.GetGlobalValue(x,z);
    }
    /// <summary>
    /// sets height to global terrain
    /// maps given local coordinates on global
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void SetGlobalHeight(int x, int z, float height, bool overwrite)
    {
        localTerrainC.SetGlobalValue(x ,z, height, overwrite);
    }

    /// <summary>
    /// returns average of neighbour vertices heights
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_z"></param>
    /// <returns></returns>
    public float GetNeighbourHeight(int _x, int _z)
    {
        float heightAverage = 0;
        int neighboursCount = 0;
        for (int x = _x - 1; x <= _x + 1; x++)
        {
            for (int z = _z - 1; z <= _z + 1; z++)
            {
                if (GetGlobalHeight(x, z) != 666)
                {
                    heightAverage += GetGlobalHeight(x, z);
                    neighboursCount++;
                    if (_x == 32 && _z == 32)
                    {
                        //Debug.Log(GetGlobalHeight(x, z));
                        //Debug.Log(heightAverage);
                        //Debug.Log(neighboursCount);
                    }
                }
            }
        }
        if (neighboursCount == 0)
        {
            return 666;
        }
        else
        {
            //Debug.Log(heightAverage / neighboursCount);
            return heightAverage / neighboursCount;
        }

    }

    public bool NeighbourhoodDefined(int _x,int _z, int neighbourhoodSize){
        for(int x = _x - neighbourhoodSize / 2; x < _x + neighbourhoodSize / 2; x++)
        {
            for (int z = _z - neighbourhoodSize / 2; z < _z + neighbourhoodSize / 2; z++)
            {
                if (GetGlobalHeight(x, z) == 666)
                    return false;
            }
        }
        return true;
    }

    public void PrintValues(int from, int to)
    {
        for(int x = from; x < to; x++)
        {
            for (int z = from; z < to; z++)
            {
                Debug.Log(x + "," + z + ": " + visibleTerrain[x, z] + "/" + GetGlobalHeight(x, z));

            }
        }
    }

    public void SetHeight(int x, int z, float height, bool overwrite)
    {
        if (!overwrite && GetGlobalHeight(x, z) != 666)
            return;

        visibleTerrain[x, z] = height;
    }
}
