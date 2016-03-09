using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MountainPeaksCoordinates {

    public MountainPeaks[,] quadrant1;
    public MountainPeaks[,] quadrant2;
    public MountainPeaks[,] quadrant3;
    public MountainPeaks[,] quadrant4;
    public MountainPeaks globalCenter;
    

    GridManager gm;

    public MountainPeaksCoordinates(int quadrantSize, GridManager gridManager)
    {
        gm = gridManager;

        globalCenter = new MountainPeaks(0,0, gm.GetPointArea(0,0));

        quadrant1 = new MountainPeaks[quadrantSize, quadrantSize];
        quadrant2 = new MountainPeaks[quadrantSize, quadrantSize];
        quadrant3 = new MountainPeaks[quadrantSize, quadrantSize];
        quadrant4 = new MountainPeaks[quadrantSize, quadrantSize];

        InitialiseQuadrant(quadrant1, 1);
        InitialiseQuadrant(quadrant2, 2);
        InitialiseQuadrant(quadrant3, 3);
        InitialiseQuadrant(quadrant4, 4);
    } 
    


    /// <summary>
    /// returns value on given coordinates (empty = undefined)
    /// </summary>
    public MountainPeaks GetValue(int x, int z)
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
            MountainPeaks[,] quadrant = GetQuandrant(x, z);
            try
            {
                return quadrant[Math.Abs(x), Math.Abs(z)];
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log(x + "," + z + " OUT");
                return new MountainPeaks(x,z,gm.GetPointArea(x,z));
            }
        }
    }


    public void UpdateValue(int x, int z, List<Vertex> peaks)
    {
        while (!CheckIfQuadrantDefined(x, z))
        {
            //Debug.Log(x + "," + z + ": SMALL");
            DoubleSizeOfQuadrant(GetQuandrantNumber(x, z));
        }

        if (x == 0 && z == 0)
        {
            globalCenter.AddPeaks(peaks);
        }
        else
        {
            GetQuandrant(x, z)[Math.Abs(x), Math.Abs(z)].AddPeaks(peaks);
        }
    }

    public void SetValue(int x, int z, MountainPeaks mountainPeaks, bool overwrite)
    {
        while (!CheckIfQuadrantDefined(x, z))
        {
            //Debug.Log(x + "," + z + ": SMALL");
            DoubleSizeOfQuadrant(GetQuandrantNumber(x, z));
        }

        //do not overwite if it is already set
        if (!overwrite && IsDefined(x, z))
            return;

        if (x == 0 && z == 0)
        {
            globalCenter = mountainPeaks;
        }
        else
        {
            GetQuandrant(x, z)[Math.Abs(x), Math.Abs(z)] = mountainPeaks;
        }
    }
    
    public bool IsDefined(int x, int z)
    {
        if (GetValue(x, z).gridCoordinates.x == 666)
            return false;
        return true;
    }


    public void DoubleSizeOfQuadrant(int quadrantNumber)
    {
        switch (quadrantNumber)
        {
            case 0:
                break;
            case 1:
                DoubleSizeOf(ref quadrant1, quadrantNumber);
                break;
            case 2:
                DoubleSizeOf(ref quadrant2, quadrantNumber);
                break;
            case 3:
                DoubleSizeOf(ref quadrant3, quadrantNumber);
                break;
            case 4:
                DoubleSizeOf(ref quadrant4, quadrantNumber);
                break;
        }
    }

    /// <summary>
    /// checks if quadrant is defined for given coordinates
    /// </summary>
    public bool CheckIfQuadrantDefined(int x, int z)
    {
        //Debug.Log("checking " + x + "," + z);
        if (x == 0 && z == 0)
        {
            return true;
        }
        else
        {
            return !(Math.Abs(x) > Math.Sqrt(GetQuandrant(x, z).Length) - 1 || Math.Abs(z) > Math.Sqrt(GetQuandrant(x, z).Length) - 1);
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

    /// <summary>
    /// returns quadrant of given coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public MountainPeaks[,] GetQuandrant(int x, int z)
    {
        if (x > 0 && z >= 0)
        {
            return quadrant1;
        }
        else if (x <= 0 && z > 0)
        {
            return quadrant2;
        }
        else if (x < 0 && z <= 0)
        {
            return quadrant3;
        }
        else if (x >= 0 && z < 0)
        {
            return quadrant4;
        }
        else
        {
            Debug.Log("incorrect coordinate calculation");
            return quadrant1;
        }
    }


    /// <summary>
    /// enlarge the size of quadrant
    /// copies original values
    /// </summary>
    /// <param name="quadrant"></param>
    public void DoubleSizeOf(ref MountainPeaks[,] quadrant, int quadrantNumber)
    {
        int newQuadrantSize = (int)Math.Sqrt(quadrant.Length) * 2;
        MountainPeaks[,] newQuadrant = new MountainPeaks[newQuadrantSize, newQuadrantSize];
        for (int x = 0; x < newQuadrantSize; x++)
        {
            for (int z = 0; z < newQuadrantSize; z++)
            {
                if (x < newQuadrantSize / 2 && z < newQuadrantSize / 2)
                {
                    newQuadrant[x, z] = quadrant[x, z];
                }
                else
                {
                    if (quadrantNumber == 1)
                        newQuadrant[x, z] = new MountainPeaks(x, z, gm.GetPointArea(x, z));
                    else if (quadrantNumber == 2)
                        newQuadrant[x, z] = new MountainPeaks(-x, z, gm.GetPointArea(-x, z));
                    else if (quadrantNumber == 3)
                        newQuadrant[x, z] = new MountainPeaks(-x, -z, gm.GetPointArea(-x, -z));
                    else if (quadrantNumber == 4)
                        newQuadrant[x, z] = new MountainPeaks(x, -z, gm.GetPointArea(x, -z));
                }
            }
        }
        quadrant = newQuadrant;
    }

    public void InitialiseQuadrant(MountainPeaks[,] quadrant, int quadrantNumber)
    {
        //can't acces array size with quadrant[0].Length
        for (int x = 0; x < Math.Sqrt(quadrant.Length); x++)
        {
            for (int z = 0; z < Math.Sqrt(quadrant.Length); z++)
            {
                if(quadrantNumber == 1)
                    quadrant[x, z] = new MountainPeaks(x,z, gm.GetPointArea(x,z));
                else if (quadrantNumber == 2)
                    quadrant[x, z] = new MountainPeaks(-x, z, gm.GetPointArea(-x, z));
                else if (quadrantNumber == 3)
                    quadrant[x, z] = new MountainPeaks(-x, -z, gm.GetPointArea(-x, -z));
                else if (quadrantNumber == 4)
                    quadrant[x, z] = new MountainPeaks(x, -z, gm.GetPointArea(x, -z));
            }
        }
    }

    public void ResetQuadrants()
    {
        Debug.Log("reseting quadrants");
        int quadrantSize = 100; //TODO: assign value

        quadrant1 = new MountainPeaks[quadrantSize, quadrantSize];
        quadrant2 = new MountainPeaks[quadrantSize, quadrantSize];
        quadrant3 = new MountainPeaks[quadrantSize, quadrantSize];
        quadrant4 = new MountainPeaks[quadrantSize, quadrantSize];

        InitialiseQuadrant(quadrant1, 1);
        InitialiseQuadrant(quadrant2, 2);
        InitialiseQuadrant(quadrant3, 3);
        InitialiseQuadrant(quadrant4, 4);
    }
}
