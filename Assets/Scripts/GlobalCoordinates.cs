using UnityEngine;
using System.Collections;
using System;

public class GlobalCoordinates {

    public float globalCenter;

    private float[,] quadrant1;
    private float[,] quadrant2;
    private float[,] quadrant3;
    private float[,] quadrant4;

    public Area definedArea;
    public float globalMax;
    public float globalMin;

    public GlobalCoordinates(int quadrantSize)
    {
        definedArea = new Area(new Vertex(0, 0), new Vertex(0, 0));

        globalCenter = 666;

        globalMax = -666;
        globalMin = 666;

        quadrant1 = new float[quadrantSize, quadrantSize];
        quadrant2 = new float[quadrantSize, quadrantSize];
        quadrant3 = new float[quadrantSize, quadrantSize];
        quadrant4 = new float[quadrantSize, quadrantSize];

        InitialiseQuadrant(quadrant1);
        InitialiseQuadrant(quadrant2);
        InitialiseQuadrant(quadrant3);
        InitialiseQuadrant(quadrant4);
    }

    /// <summary>
    /// returns value on given coordinates (666 = undefined)
    /// heighmaps are stored in 4 quadrants
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>height value at given coordinates</returns>
    public float GetValue(int x, int z)
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
            try
            {
                return quadrant[Math.Abs(x), Math.Abs(z)];
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log(x + "," + z + " OUT");
                return 666;
            }
        }
    }

    public float GetValue(Vector3 point)
    {
        return GetValue((int)point.x, (int)point.z);
    }

    /// <summary>
    /// sets height to heightmap at given coordinates

    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="height"></param>
    /// <param name="overwrite">overwrite original value</param>
    public void SetValue(int x, int z, float height, bool overwrite)
    {
        while (!CheckIfQuadrantDefined(x, z))
        {
            //Debug.Log(x + "," + z + ": SMALL");
            DoubleSizeOfQuadrant(GetQuandrantNumber(x, z));
        }

        //do not overwite height if it is already set
        if (!overwrite && IsDefined(x, z))
            return;

        if (x == 0 && z == 0)
        {
            globalCenter = height;
        }
        else
        {
            GetQuandrant(x, z)[Math.Abs(x), Math.Abs(z)] = height;
        }

        if (!definedArea.Contains(new Vertex(x, z)))
        {
            UpdateDefinedArea(x, z);
        }

        if (height > globalMax)
            globalMax = height;
        else if (height < globalMin)
            globalMin = height;


    }

    public void SetValue(Vector3 point, float height, bool overwrite)
    {
        SetValue((int)point.x, (int)point.z, height, overwrite);
    }

    int counter = 0;
    public void SetValue(int x, int z, float height)
    {
        if(height > 666 && counter < 10)
        {
            Debug.Log(x + "," + z + ": " + height);
            counter++;
        }
        SetValue(x, z, height, true);
    }


    public void UpdateDefinedArea(int x, int z)
    {
        if (x < definedArea.botLeft.x)
        {
            definedArea.botLeft.x = x;
            definedArea.topLeft.x = x;
        }
        if (x > definedArea.topRight.x)
        {
            definedArea.topRight.x = x;
            definedArea.botRight.x = x;
        }

        if (z < definedArea.botLeft.z)
        {
            definedArea.botLeft.z = z;
            definedArea.topLeft.z = z;
        }
        if (z > definedArea.topRight.z)
        {
            definedArea.topRight.z = z;
            definedArea.botRight.z = z;
        }

    }

    public bool IsDefined(Vector3 point)
    {
        return IsDefined((int)point.x, (int)point.z);
    }

    public bool IsDefined(int x, int z)
    {
        return IsDefinedArea(x, z, 0);
        /*
        if (GetValue(x, z) == 666)
            return false;
        else
            return true;*/
    }

    /// <summary>
    /// checks if all points around [_x,_z] are defined
    /// </summary>
    public bool IsDefinedArea(int _x, int _z, int area)
    {
        for(int x = _x - area; x <= _x + area; x++)
        {
            for (int z = _z - area; z <= _z + area; z++)
            {
                if (GetValue(x, z) == 666)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// checks if all points around [_x,_z] are defined
    /// </summary>
    public bool IsDefinedArea(Vector3 point, int area)
    {
        return IsDefinedArea((int)point.x, (int)point.z, area);
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
            return !(Math.Abs(x) > Math.Sqrt(GetQuandrant(x, z).Length) - 1 || Math.Abs(z) > Math.Sqrt(GetQuandrant(x, z).Length) - 1);
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
                DoubleSizeOf(ref quadrant1);
                break;
            case 2:
                DoubleSizeOf(ref quadrant2);
                break;
            case 3:
                DoubleSizeOf(ref quadrant3);
                break;
            case 4:
                DoubleSizeOf(ref quadrant4);
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
    public void InitialiseQuadrant(float[,] quadrant)
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

    public void ResetQuadrants()
    {
        Debug.Log("reseting quadrants");
        int quadrantSize = 100; //TODO: assign value

        quadrant1 = new float[quadrantSize, quadrantSize];
        quadrant2 = new float[quadrantSize, quadrantSize];
        quadrant3 = new float[quadrantSize, quadrantSize];
        quadrant4 = new float[quadrantSize, quadrantSize];

        InitialiseQuadrant(quadrant1);
        InitialiseQuadrant(quadrant2);
        InitialiseQuadrant(quadrant3);
        InitialiseQuadrant(quadrant4);
    }
}
