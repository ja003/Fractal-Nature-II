using UnityEngine;
using System.Collections;
using System;

public class GlobalTerrain : IGlobalTerrain
{
    private float globalCenter;

    private float[,] quadrant1heightmap;
    private float[,] quadrant2heightmap;
    private float[,] quadrant3heightmap;
    private float[,] quadrant4heightmap;

    private float[,] quadrant1rivermap;
    private float[,] quadrant2rivermap;
    private float[,] quadrant3rivermap;
    private float[,] quadrant4rivermap;

    public GlobalTerrain(int quadrantSize)
    {
        Debug.Log(quadrantSize);

        quadrant1heightmap = new float[quadrantSize, quadrantSize];
        quadrant2heightmap = new float[quadrantSize, quadrantSize];
        quadrant3heightmap = new float[quadrantSize, quadrantSize];
        quadrant4heightmap = new float[quadrantSize, quadrantSize];

        InitialiseQuadrant(quadrant1heightmap);
        InitialiseQuadrant(quadrant2heightmap);
        InitialiseQuadrant(quadrant3heightmap);
        InitialiseQuadrant(quadrant4heightmap);

        quadrant1rivermap = new float[quadrantSize, quadrantSize];
        quadrant2rivermap = new float[quadrantSize, quadrantSize];
        quadrant3rivermap = new float[quadrantSize, quadrantSize];
        quadrant4rivermap = new float[quadrantSize, quadrantSize];

        InitialiseQuadrant(quadrant1rivermap);
        InitialiseQuadrant(quadrant2rivermap);
        InitialiseQuadrant(quadrant3rivermap);
        InitialiseQuadrant(quadrant4rivermap);

        
    }

    /// <summary>
    /// heighmaps are stored in 4 quadrants
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>height value at given coordinates</returns>
    public float GetHeight(int x, int z)
    {
        while (!CheckIfQuadrantDefined(x, z))
        {
            //Debug.Log(x + "," + z + ": SMALL");
            DoubleSizeOfQuadrant(GetQuandrantNumber(x, z));
        }

        if (x == 0 && z == 0)
        {
            return globalCenter;
        }
        else
        {
            float[,] quadrant = GetQuandrant(x, z);
            try {
                return quadrant[Math.Abs(x), Math.Abs(z)];
            }catch(IndexOutOfRangeException e)
            {
                Debug.Log(x + "," + z + " OUT");
                return 666;
            }
        }
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
        while (!CheckIfQuadrantDefined(x, z))
        {
            Debug.Log(x + "," + z + ": SMALL");
            DoubleSizeOfQuadrant(GetQuandrantNumber(x, z));
        }

        //do not overwite height if it is already set
        if (!overwrite && GetHeight(x, z) != 666)
            return;

        if (x == 0 && z == 0)
        {
            globalCenter = height;
        }
        else
        {
            GetQuandrant(x, z)[Math.Abs(x), Math.Abs(z)] = height;
        }
    }

    public void SetHeight(int x, int z, float height)
    {
        SetHeight(x, z, height, true);
    }

    /// <summary>
    /// checks if quadrant is defined for given coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool CheckIfQuadrantDefined(int x, int z)
    {
        //Debug.Log("checking " + x + "," + z);
        if (x == 0 && z == 0)
        {
            return true;
        }
        else
        {
            return !(Math.Abs(x) > Math.Sqrt(GetQuandrant(x, z).Length)-1 || Math.Abs(z) > Math.Sqrt(GetQuandrant(x, z).Length)-1);
        }
    }

    /// <summary>
    /// returns quadrant of given coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public float[,] GetQuandrant(int x, int z)
    {
        if (x > 0 && z >= 0)
        {
            return quadrant1heightmap;
        }
        else if (x <= 0 && z > 0)
        {
            return quadrant2heightmap;
        }
        else if (x < 0 && z <= 0)
        {
            return quadrant3heightmap;
        }
        else if (x >= 0 && z < 0)
        {
            return quadrant4heightmap;
        }
        else
        {
            Debug.Log("incorrect coordinate calculation");
            return quadrant1heightmap;
        }
    }

    public int GetQuandrantNumber(int x, int z)
    {
        if (x > 0 && z >= 0)
        {
            return 1;
        }
        else if (x <= 0 && z > 0)
        {
            return 2;
        }
        else if (x < 0 && z <= 0)
        {
            return 3;
        }
        else if (x >= 0 && z < 0)
        {
            return 4;
        }
        else
        {
            return 0;
        }
    }

    public void DoubleSizeOfQuadrant(int quadrantNumber)
    {
        switch (quadrantNumber)
        {
            case 0:
                break;
            case 1:
                DoubleSizeOf(ref quadrant1heightmap);
                break;
            case 2:
                DoubleSizeOf(ref quadrant2heightmap);
                break;
            case 3:
                DoubleSizeOf(ref quadrant3heightmap);
                break;
            case 4:
                DoubleSizeOf(ref quadrant4heightmap);
                break;
        }
    }

    /// <summary>
    /// enlarge the size of quadrant
    /// copies original values
    /// </summary>
    /// <param name="quadrant"></param>
    public void DoubleSizeOf(ref float[,] quadrant)
    {
        int newQuadrantSize = (int)Math.Sqrt(quadrant.Length) * 2;
        float[,] newQuadrant = new float[newQuadrantSize, newQuadrantSize];
        for(int x = 0; x < newQuadrantSize; x++)
        {
            for(int z = 0; z < newQuadrantSize; z++)
            {
                if(x < newQuadrantSize/2 && z < newQuadrantSize / 2)
                {
                    newQuadrant[x, z] = quadrant[x, z];
                }
                else
                {
                    newQuadrant[x, z] = 666;
                }
            }
        }
        quadrant = newQuadrant;
    }

    /// <summary>
    /// sets values to 666
    /// 666 will be reffered as undefined value
    /// </summary>
    /// <param name="quadrant"></param>
    public void InitialiseQuadrant(float [,] quadrant)
    {
        //can't acces array size with quadrant[0].Length
        for (int x = 0; x < Math.Sqrt(quadrant.Length); x++)
        {
            for (int z = 0; z < Math.Sqrt(quadrant.Length); z++)
            {
                quadrant[x, z] = 666;
            }
        }
    }

}
