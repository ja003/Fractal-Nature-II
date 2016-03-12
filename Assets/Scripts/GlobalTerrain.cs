using UnityEngine;
using System.Collections;
using System;

public class GlobalTerrain : IGlobalTerrain
{
    public GlobalCoordinates globalTerrainC;

    

    public GlobalTerrain(int quadrantSize)
    {
        globalTerrainC = new GlobalCoordinates(quadrantSize);
    }

    /// <summary>
    /// heighmaps are stored in 4 quadrants
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>height value at given coordinates</returns>
    public float GetHeight(int x, int z)
    {
        return globalTerrainC.GetValue(x, z);
    }

    /// <summary>
    /// sets height to heightmap at given coordinates

    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="height"></param>
    /// <param name="overwrite">overwrite original value</param>
    public void SetHeight(int x, int z, float height, bool overwrite)
    {
        globalTerrainC.SetValue(x, z, height, overwrite);
    }


    public void SetHeight(int x, int z, float height)
    {
        SetHeight(x, z, height, true);
    }

    /// <summary>
    /// returns average of neighbour vertices heights in area = 1
    /// </summary>
    public float GetNeighbourAverage(int _x, int _z)
    {
        return GetNeighbourAverage(_x, _z, 1);
    }

    /// <summary>
    /// returns average of neighbour vertices heights in given area
    /// </summary>
    public float GetNeighbourAverage(int _x, int _z, int area)
    {
        float heightAverage = 0;
        int neighboursCount = 0;
        for (int x = _x - area; x <= _x + area; x++)
        {
            for (int z = _z - area; z <= _z + area; z++)
            {
                if (GetHeight(x, z) != 666)
                {
                    heightAverage += GetHeight(x, z);
                    neighboursCount++;
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

    /// <summary>
    /// returns highest neighbour of given point
    /// 0 if no neighbour is defined
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_z"></param>
    /// <returns></returns>
    public float GetHighestNeighbour(int _x,int _z)
    {
        float highest = -666;
        for (int x = _x - 1; x <= _x + 1; x++)
        {
            for (int z = _z - 1; z <= _z + 1; z++)
            {
                if (GetHeight(x, z) != 666 && GetHeight(x, z) > highest)
                {
                    highest = GetHeight(x, z);
                }
            }
        }
        if (highest == -666)
            highest = 0;

        return highest;
    }
    
}
