using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FunctionTerrainManager {
    

    public FunctionMathCalculator fmc;
    public LocalTerrain lt;
    public RiverGenerator rg;
    public LayerManager lm;

    public FunctionTerrainManager()
    {

    }

    public void AssignFunctions(LocalTerrain localTerrain, FunctionMathCalculator functionMathCalculator, RiverGenerator riverGenerator, LayerManager layerManager)
    {
        lt = localTerrain;
        fmc = functionMathCalculator;
        rg = riverGenerator;
        lm = layerManager;
    }

    /// <summary>
    /// returns median of given area
    /// operates with global coordinates
    /// returns 666 if 1 of neighbours is not defined
    /// </summary>
    public float GetGlobalMedian(int _x, int _z, int regionSize)
    {
        //List<float> heights = new List<float>();
        float heightSum = 0;
        int count = 0;
        List<Layer> ignoreLayers = new List<Layer>();
        ignoreLayers.Add(Layer.filterMedian);

        for (int x = _x - regionSize; x < _x + regionSize; x++)
        {
            for (int z = _z - regionSize; z < _z + regionSize; z++)
            {
                //if(lm.GetCurrentHeight(x, z) != 666)
                if (lt.globalTerrainC.IsDefined(x,z))
                {
                    //heightSum += lm.GetTerrainValue(x, z);// lt.ft.GetValue(x, z);
                    heightSum += lm.GetCurrentHeight(x, z);// lt.lm.GetValue(x, z, ignoreLayers);
                    count++;
                }
                else
                {
                    return 666;
                }
            }
        }
        if (count == 0)
            return 666;//shouldnt happen
        return heightSum / count;
    }


    /// <summary>
    /// returns median of given area on visible terrain
    /// </summary>
    public float GetLocalMedian(int _x, int _z, int regionSize)
    {
        //List<float> heights = new List<float>();
        float heightSum = 0;
        int count = 0;
        for (int x = _x - regionSize; x < _x + regionSize; x++)
        {
            for (int z = _z - regionSize; z < _z + regionSize; z++)
            {
                if (CheckBounds(x, z))
                {
                    heightSum += lt.GetLocalHeight(x, z);
                    count++;
                }
            }
        }
        if (count == 0)
            return 0;
        return heightSum / count;
    }

    public float GetLocalMedian(int _x, int _z, int regionSize, float[,] depthMap)
    {
        //List<float> heights = new List<float>();
        float heightSum = 0;
        int count = 0;
        for (int x = _x - regionSize; x < _x + regionSize; x++)
        {
            for (int z = _z - regionSize; z < _z + regionSize; z++)
            {
                if (CheckBounds(x, z) && depthMap[x, z] != 666)
                {
                    heightSum += depthMap[x, z];
                    count++;
                }
            }
        }
        if (count == 0)
            return 0;
        return heightSum / count;
    }
    

    /// <summary>
    /// return 8 neighbourhood around center
    /// operates on local coordinates
    /// </summary>
    //public List<Vertex> GetGlobal8Neighbours(Vertex center, int step, int offset, float threshold)
    //{
    //    return GetGlobal8Neighbours(center, step, offset, threshold);//, 0, lt.terrainWidth, 0, lt.terrainHeight);
    //}

    /// <summary>
    /// return 8 neighbourhood around center from restricted area
    /// operates on global coordinates
    /// </summary>
    public List<Vertex> GetGlobal8Neighbours(Vertex center, int step, int offset, float threshold, 
        int x_min, int z_min, int x_max,  int z_max)
    {
        List<Vertex> neighbours = new List<Vertex>();
        int x = center.x;
        int z = center.z;
        
        //left
        if (CheckBounds(x - step,z,0,x_min, x_max, z_min, z_max) && lm.GetCurrentHeight(x - step, z) < threshold)
            { neighbours.Add(new Vertex(x - step, z, lm.GetCurrentHeight(x - step, z))); }
        //up
        if (CheckBounds(x, z + step, 0, x_min, x_max, z_min, z_max) && lm.GetCurrentHeight(x, z + step) < threshold)
            { neighbours.Add(new Vertex(x, z + step, lm.GetCurrentHeight(x, z + step))); }
        //righ
        if (CheckBounds(x + step, z, 0, x_min, x_max, z_min, z_max) && lm.GetCurrentHeight(x + step, z) < threshold)
            { neighbours.Add(new Vertex(x + step, z, lm.GetCurrentHeight(x + step, z))); }
        //down
        if (CheckBounds(x, z - step, 0, x_min, x_max, z_min, z_max) && lm.GetCurrentHeight(x, z - step) < threshold)
            { neighbours.Add(new Vertex(x, z - step, lm.GetCurrentHeight(x, z - step))); }

        //leftUp
        if (CheckBounds(x - step, z + step, 0, x_min, x_max, z_min, z_max) && lm.GetCurrentHeight(x - step, z + step)< threshold)
            { neighbours.Add(new Vertex(x - step, z + step, lm.GetCurrentHeight(x - step, z + step))); }
        //rightUp
        if (CheckBounds(x + step, z + step, 0, x_min, x_max, z_min, z_max) && lm.GetCurrentHeight(x + step, z + step) < threshold)
            { neighbours.Add(new Vertex(x + step, z + step, lm.GetCurrentHeight(x + step, z + step))); }
        //righDown
        if (CheckBounds(x + step, z - step, 0, x_min, x_max, z_min, z_max) && lm.GetCurrentHeight(x + step, z - step) < threshold)
            { neighbours.Add(new Vertex(x + step, z - step, lm.GetCurrentHeight(x + step, z - step))); }
        //leftDown
        if (CheckBounds(x - step, z - step, 0, x_min, x_max, z_min, z_max) && lm.GetCurrentHeight(x - step, z - step) < threshold)
            { neighbours.Add(new Vertex(x - step, z - step, lm.GetCurrentHeight(x - step, z - step))); }
        
        return neighbours;
    }

    /// <summary>
    /// returns 8 neighbours
    /// operates on global coordinates
    /// </summary>
    public List<Vertex> Get8Neighbours(Vertex center, int step)
    {
        List<Vertex> neighbours = new List<Vertex>();
        int x = center.x;
        int z = center.z;
        //left
        neighbours.Add(new Vertex(x - step, z, lm.GetCurrentHeight(x-step,z)));
        //up
        neighbours.Add(new Vertex(x, z + step, lm.GetCurrentHeight(x, z + step))); 
        //righ
        neighbours.Add(new Vertex(x + step, z, lm.GetCurrentHeight(x + step, z))); 
        //down
        neighbours.Add(new Vertex(x, z - step, lm.GetCurrentHeight(x, z - step))); 

        //leftUp
        neighbours.Add(new Vertex(x - step, z + step, lm.GetCurrentHeight(x - step, z + step))); 
        //rightUp
        neighbours.Add(new Vertex(x + step, z + step, lm.GetCurrentHeight(x + step, z + step))); 
        //righDown
        neighbours.Add(new Vertex(x + step, z - step, lm.GetCurrentHeight(x + step, z - step))); 
        //leftDown
        neighbours.Add(new Vertex(x - step, z - step, lm.GetCurrentHeight(x - step, z - step)));

        return neighbours;
    }

    /// <summary>
    /// returns 4 neighbours
    /// operates on global coordinates
    /// </summary>
    public List<Vertex> Get4Neighbours(Vertex center, int step)
    {
        List<Vertex> neighbours = new List<Vertex>();
        int x = center.x;
        int z = center.z;
        //left
        neighbours.Add(new Vertex(x - step, z, lm.GetCurrentHeight(x - step, z)));
        //up
        neighbours.Add(new Vertex(x, z + step, lm.GetCurrentHeight(x, z + step)));
        //righ
        neighbours.Add(new Vertex(x + step, z, lm.GetCurrentHeight(x + step, z)));
        //down
        neighbours.Add(new Vertex(x, z - step, lm.GetCurrentHeight(x, z - step)));
        
        return neighbours;
    }

    /// <summary>
    /// returns lowest neighbour of center
    /// neighbourCount 0-4: 4-neighbourhood
    /// neighbourCount 4-8: 8-neighbourhood
    /// </summary>
    public Vertex GetLowestNeighbour(Vertex center, int step, int neighbourCount)
    {
        List<Vertex> neighbours;
        if (neighbourCount <= 4)
            neighbours = Get4Neighbours(center, step);
        else
            neighbours = Get8Neighbours(center, step);

        float min = 666;
        Vertex minN = center;
        foreach(Vertex n in neighbours)
        {
            if(n.height < min)
            {
                min = n.height;
                minN = n;
            }
        }
        if(minN == center)
        {
            Debug.Log("neighbour not found");
        }
        return minN;
    }

    /// <summary>
    /// checks if linew between v1 and v2 contains higher values than threshold
    /// </summary>
    public bool IsOnContour(Vertex v1, Vertex v2, float threshold)
    {
        Vector3 dir = ((Vector3)v2 - v1).normalized;
        float dist = fmc.GetDistance(v1, v2);
        Vector3 currentV = v1;
        Vertex current = v1;
        for (int j = 0; j < dist; j++)
        {
            if (current.height > threshold)
                return false;
            currentV = currentV + dir;
            current = currentV;
            current.height = lt.globalTerrainC.GetValue(current.x, current.z);
        }
        return true;
    }
    

    /// <summary>
    /// used to find lowest region for river generation
    /// finds point on visible terrain with lowest neighbourhood where no river is defined
    /// returns global point
    /// </summary>
    public Vertex GetLowestRegionCenter(int radius, int offset)
    {
        double lowestSum = 666;
        Vertex lowestRegionCenter = lt.GetGlobalCoordinate(offset, offset);
        //new Vertex(offset, offset, 10);
        for (int x = offset; x < lt.terrainWidth - offset; x += radius/2)
        {
            for (int z = offset; z < lt.terrainHeight - offset; z += radius/2)
            {
                Vertex globalC = lt.GetGlobalCoordinate(x, z);
                if (!rg.IsRiverDefined(globalC.x, globalC.z))
                {
                    double sum = 0;
                    for (int i = globalC.x - radius; i < globalC.x + radius; i++)
                    {
                        for (int j = globalC.z - radius; j < globalC.z + radius; j++)
                        {
                            sum += lm.GetCurrentHeight(i, j);
                        }
                    }
                    if (sum < lowestSum)
                    {
                        lowestSum = sum;
                        lowestRegionCenter.Rewrite(globalC.x, globalC.z, lm.GetCurrentHeight(globalC.x, globalC.z));
                    }
                }

                

            }
        }
        return lowestRegionCenter;
    }
    
    

    /// <summary>
    /// returns highest point from given local region
    /// </summary>
    public Vertex GetHighestpoint(int x_min, int z_min, int x_max, int z_max)
    {


        Vertex highestPoint = new Vertex(x_min+10, z_min+10, lm.GetCurrentHeight(x_min+10,z_min + 10));
        for (int x = x_min; x < x_max - 1; x++)
        {
            for (int z = z_min; z < z_max - 1; z++)
            {
                float height = lm.GetCurrentHeight(x, z);
                if (height != 666 && height > highestPoint.height)
                    highestPoint = new Vertex(x, z, height);
            }
        }
        return highestPoint;
    }

    /// <summary>
    /// returns sum of defined region
    /// </summary>
    public float GetNeighbourhoodSum(Vertex point, int radius, int step)
    {
        float sum = 0;
        for(int x = point.x - radius;x<point.x + radius; x += step)
        {
            for (int z = point.z - radius; z < point.z + radius; z += step)
            {
                //float v = lt.globalTerrainC.GetValue(x, z);
                float v = lt.lm.GetCurrentHeight(x, z);
                if (v != 666)
                    sum += v;
            }
        }
        return sum;
    }

    /// <summary>
    /// checks if given point is on border of defined terrain
    /// (not only same coordinates as defined area)
    /// point = global
    /// </summary>
    public bool IsOnBorder(Vector3 point)
    {
        bool definedPoint = lt.globalTerrainC.IsDefined(point);

        bool definedTop = lt.globalTerrainC.IsDefined(point + new Vector3(0, 0, 1));
        bool definedRight = lt.globalTerrainC.IsDefined(point + new Vector3(1, 0, 1));
        bool definedBot = lt.globalTerrainC.IsDefined(point + new Vector3(0, 0, -1));
        bool definedLeft = lt.globalTerrainC.IsDefined(point + new Vector3(-1, 0, 0));

        return definedPoint && (!definedTop || !definedRight || !definedBot || !definedLeft);
    }

    /// <summary>
    /// check if given point is in visible terrain
    /// point = global coordinates
    /// </summary>
    public bool IsInVisibleterrain(Vector3 point)
    {
        return IsInRegion(point, lt.localTerrainC.botLeft, lt.localTerrainC.topRight);
    }

    /// <summary>
    /// check if given point is in defined terrain
    /// point = global coordinates
    /// </summary>
    public bool IsInDefinedTerrain(Vector3 point)
    {
        return lt.globalTerrainC.definedArea.Contains(point);
    }

    /// <summary>
    /// checks if corner points of given area are defined
    /// </summary>
    public bool IsDefinedTerrainArea(Area area)
    {
        bool botLeft = lt.globalTerrainC.IsDefined(area.botLeft);
        bool topLeft = lt.globalTerrainC.IsDefined(area.topLeft);
        bool topRight = lt.globalTerrainC.IsDefined(area.topRight);
        bool botRight = lt.globalTerrainC.IsDefined(area.botRight);

        return botLeft && topLeft && topRight && botRight;
    }

    /// <summary>
    /// determines wheter point lies in region defined by botLeft and topRight
    /// </summary>
    public bool IsInRegion(Vector3 point, Vector3 botLeft, Vector3 topRight)
    {
        return CheckBounds((int)point.x, (int)point.z, 0, (int)botLeft.x, (int)botLeft.z,(int)topRight.x,  (int)topRight.z);
    }
    /// <summary>
    /// operates on global space
    /// </summary>
    public bool CheckBounds(int x, int z, int offset)
    {
        int x_min = (int)lt.localTerrainC.botLeft.x;
        int z_min = (int)lt.localTerrainC.botLeft.z;
        int x_max = (int)lt.localTerrainC.topRight.x;
        int z_max = (int)lt.localTerrainC.topRight.z;
        return CheckBounds(x, z, offset, x_min, z_min, x_max, z_max);
    }

    /// <summary>
    /// operates on global space
    /// </summary>
    public bool CheckBounds(int x, int z, int offset, int x_min, int z_min, int x_max, int z_max)
    {
        //return x > x_min + offset && x < x_max - 1 - offset && z > z_min + offset && z < z_max - 1 - offset;
        return x >= x_min+offset && x < x_max - offset && z >= z_min+offset && z < z_max - offset;
    }
    /// <summary>
    /// operates on global space
    /// </summary>
    public bool CheckBounds(int x, int z)
    {
        return CheckBounds(x, z, 0);
    }
    /// <summary>
    /// operates on global space
    /// </summary>
    public bool CheckBounds(Vertex vertex)
    {
        return CheckBounds(vertex.x, vertex.z);
    }
    
    /// <summary>
    /// returns sum of neighbourhood
    /// [x,z] = local
    /// </summary>
    public float GetSumFromNeighbourhood(int x, int z, int offset)
    {
        float sum = 0;
        for (int _x = x - offset; _x <= x + offset; _x++)
        {
            for (int _z = z - offset; _z <= z + offset; _z++)
            {
                if (CheckBounds(_x,_z))
                {
                    sum += lt.GetLocalHeight(_x, _z);
                }
            }
        }
        return sum;
    }
}
