using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FunctionTerrainManager {
    

    public FunctionMathCalculator fmc;
    public LocalTerrain lt;

    public FunctionTerrainManager()
    {

    }

    public void AssignFunctions(LocalTerrain localTerrain, FunctionMathCalculator functionMathCalculator)
    {
        lt = localTerrain;
        fmc = functionMathCalculator;
    }

    /// <summary>
    /// returns median of given area
    /// operates with global coordinates
    /// </summary>
    public float GetGlobalMedian(int _x, int _z, int regionSize)
    {
        //List<float> heights = new List<float>();
        float heightSum = 0;
        int count = 0;
        for (int x = _x - regionSize; x < _x + regionSize; x++)
        {
            for (int z = _z - regionSize; z < _z + regionSize; z++)
            {
                if(lt.GetGlobalHeight(x, z) != 666)
                //if (lt.ft.GetValue(x, z) != 666)
                {
                    heightSum += lt.GetGlobalHeight(x, z);// lt.ft.GetValue(x, z);
                    count++;
                }
            }
        }
        if (count == 0)
            return 0;
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
    public List<Vertex> Get8Neighbours(Vertex center, int step, int offset, float threshold)
    {
        return Get8Neighbours(center, step, offset, threshold, 0, lt.terrainWidth, 0, lt.terrainHeight);
    }

    /// <summary>
    /// return 8 neighbourhood around center
    /// operates on local coordinates
    /// </summary>
    public List<Vertex> Get8Neighbours(Vertex center, int step, int offset, float threshold, 
        int x_min, int x_max, int z_min, int z_max)
    {
        List<Vertex> neighbours = new List<Vertex>();
        int x = center.x;
        int z = center.z;

        if(fmc.GetDistanceFromCorner(x,z, x_min, x_max, z_min, z_max) < 2 * step) //dont process points too close to corners
        {
            //Debug.Log(center + " is too close to corner!");
            return neighbours;
        }

        //left
        if (CheckBounds(x - step, z, offset, x_min,x_max,z_min,z_max) && lt.GetLocalHeight(x - step, z) < threshold)
            { neighbours.Add(new Vertex(x - step, z, lt.GetLocalHeight(x - step, z))); }
        //up
        if (CheckBounds(x, z + step, offset, x_min, x_max, z_min, z_max) && lt.GetLocalHeight(x, z + step) < threshold)
            { neighbours.Add(new Vertex(x, z + step, lt.GetLocalHeight(x, z + step))); }
        //righ
        if (CheckBounds(x + step, z, offset, x_min, x_max, z_min, z_max) && lt.GetLocalHeight(x + step, z) < threshold)
            { neighbours.Add(new Vertex(x + step, z, lt.GetLocalHeight(x + step, z))); }
        //down
        if (CheckBounds(x, z - step, offset, x_min, x_max, z_min, z_max) && lt.GetLocalHeight(x, z - step) < threshold)
            { neighbours.Add(new Vertex(x, z - step, lt.GetLocalHeight(x, z - step))); }

        //leftUp
        if (CheckBounds(x - step, z + step, offset, x_min, x_max, z_min, z_max) && lt.GetLocalHeight(x - step, z + step)< threshold)
            { neighbours.Add(new Vertex(x - step, z + step, lt.GetLocalHeight(x - step, z + step))); }
        //rightUp
        if (CheckBounds(x + step, z + step, offset, x_min, x_max, z_min, z_max) && lt.GetLocalHeight(x + step, z + step) < threshold)
            { neighbours.Add(new Vertex(x + step, z + step, lt.GetLocalHeight(x + step, z + step))); }
        //righDown
        if (CheckBounds(x + step, z - step, offset, x_min, x_max, z_min, z_max) && lt.GetLocalHeight(x + step, z - step) < threshold)
            { neighbours.Add(new Vertex(x + step, z - step, lt.GetLocalHeight(x + step, z - step))); }
        //leftDown
        if (CheckBounds(x - step, z - step, offset, x_min, x_max, z_min, z_max) && lt.GetLocalHeight(x - step, z - step) < threshold)
            { neighbours.Add(new Vertex(x - step, z - step, lt.GetLocalHeight(x - step, z - step))); }
        
        return neighbours;
    }

    /*
    //obsolete!!!
    public List<Vertex> Get4Neighbours(Vertex center, int step, int offset)
    {
        List<Vertex> neighbours = new List<Vertex>();
        int x = center.x;
        int z = center.z;
        //left
        if (CheckBounds(x - step, z, offset)) { neighbours.Add(new Vertex(x - step, z, vertices[x - step, z].y)); }
        //up
        if (CheckBounds(x, z + step, offset)) { neighbours.Add(new Vertex(x, z + step, vertices[x, z + step].y)); }
        //righ
        if (CheckBounds(x + step, z, offset)) { neighbours.Add(new Vertex(x + step, z, vertices[x + step, z].y)); }
        //down
        if (CheckBounds(x, z - step, offset)) { neighbours.Add(new Vertex(x, z - step, vertices[x, z - step].y)); }

        return neighbours;
    }
    */

    /// <summary>
    /// finds point on visible terrain with lowest neighbourhood 
    /// </summary>
    public Vertex GetLowestRegionCenter(int radius, int offset)
    {
        double lowestSum = 100;
        Vertex lowestRegionCenter = new Vertex(offset, offset, 10);
        for (int x = offset; x < lt.terrainWidth - offset; x += radius/2)
        {
            for (int z = offset; z < lt.terrainHeight - offset; z += radius/2)
            {
                double sum = 0;
                for (int i = x - radius; i < x + radius; i++)
                {
                    for (int j = z - radius; j < z + radius; j++)
                    {
                        sum += lt.GetLocalHeight(x, z);

                        //if (CheckBounds(i, j))
                        //    sum += vertices[i, j].y;
                        //else
                        //    sum += 1;
                    }
                }
                if (sum < lowestSum)
                {
                    lowestSum = sum;
                    lowestRegionCenter.Rewrite(x, z, lt.GetLocalHeight(x, z));
                }

            }
        }
        return lowestRegionCenter;
    }

    /*
    /// <summary>
    /// calls PerserveMountains on whole map
    /// </summary>
    /// <param name="count"></param>
    /// <param name="radius"></param>
    public void PerserveMountains(int count, int radius, int scaleFactor)
    {
        PerserveMountains(count, radius, scaleFactor, 0, terrainSize, 0, terrainSize);
    }

    /// <summary>
    /// modifies current terrain
    /// finds 'x' highest peaks which are at least 'radius' away from each other
    /// then all vertices hight is lowered using GetScale function based on logarithm 
    /// 
    /// applied only on restricted area
    /// </summary>
    /// <param name="count"></param>
    /// <param name="radius"></param>
    public void PerserveMountains(int count, int radius, int scaleFactor,int x_min, int x_max, int z_min, int z_max)
    {
        List<Vertex> peaks = new List<Vertex>();
        for (int i = 0; i < count; i++)
        {
            if (FindNextHighestPeak(radius, peaks) != null)
                peaks.Add(FindNextHighestPeak(radius, peaks));
        }

        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                Vertex vert = new Vertex(x, z, vertices[x, z].y);
                double scale = 0;
                foreach (Vertex v in peaks)
                {
                    if (fmc.GetScale(vert, v, radius) > scale)
                    {
                        scale = fmc.GetScale(vert, v, radius);
                    }
                }
                
                //int distance = DistanceFromLine(x, z, x_min, x_max,z_min,z_max);
                //if (x < 10 && z < 10)
                //    Debug.Log((float)Math.Pow(scale, scaleFactor) *((float)distance / terrainSize));

                //vertices[x, z].y *= (float)Math.Pow(scale, scaleFactor) *((float)distance /(terrainSize/4));
                vertices[x, z].y *= (float)Math.Pow(scale, scaleFactor);

            }
        }



        //blur the peaks
        float blurringFactor = radius / 10;
        int kernelSize = radius / 10;

        for (int i = 0; i < peaks.Count; i++)
        {
            rg.filtermanager.applyGaussianBlur(blurringFactor, kernelSize,
                new Vector3(peaks[i].x - kernelSize, 0, peaks[i].z - kernelSize),
                new Vector3(peaks[i].x + kernelSize, 0, peaks[i].z + kernelSize));

        }

        rg.terrain.build();
    }
    

    public int DistanceFromLine(int x, int z, int x_min, int x_max, int z_min, int z_max)
    {
        return Math.Min(
            Math.Min(
                Math.Min(x, z),
                Math.Min(x_max - x, z_max - z)),
            Math.Min(
                Math.Abs(x_min - x), 
                Math.Abs(z_min - z)));
    }

    public void MedianBlur(Vertex downLeft, Vertex upRight)
    {
        float[,] depthField = new float[vertices.Length, vertices.Length];
        for(int x = 0; x < vertices.Length; x++)
        {
            for(int z = 0; z < vertices.Length; z++)
            {
                depthField[x, z] = vertices[x, z].y;
            }
        }

        for(int x = downLeft.x; x < upRight.x; x++)
        {
            for(int z = downLeft.z; z < upRight.z; z++)
            {
                vertices[x, z].y = GetMedian(x, z, 10, depthField);
            }
        }
    }

        

    public void MirrorEdge(int patchSize, int width, Direction direction)
    {
        int line;
        if (direction == Direction.top || direction == Direction.right)
            line = terrainSize - patchSize;
        else
            line = patchSize;

        if (direction == Direction.top || direction == Direction.bot)
        {
            for (int x = 0; x < terrainSize; x++)
            {
                for (int w = 0; w < width; w++)
                {
                    if (direction == Direction.top)
                    {
                        int z_orig = line - w - 1;
                        int z_new = line + w;

                        vertices[x, z_new].y =
                            ((float)(width - w) / width) * vertices[x, z_orig].y +
                            ((float)w / width) * vertices[x, z_new].y;
                    }
                    else
                    {
                        int z_orig = line + w;
                        int z_new = line - w - 1;

                        vertices[x, z_new].y =
                            ((float)(width - w) / width) * vertices[x, z_orig].y +
                            ((float)w / width) * vertices[x, z_new].y;

                    }                    
                }
            }
        }
        else
        {
            for (int z = 0; z < terrainSize; z++)
            {
                for (int w = 0; w < width; w++)
                {
                    if (direction == Direction.right)
                    {
                        int x_orig = line - w - 1;
                        int x_new = line + w;

                        vertices[x_new,z].y =
                            ((float)(width - w) / width) * vertices[x_orig,z].y +
                            ((float)w / width) * vertices[x_new,z].y;
                    }
                    else
                    {
                        int x_orig = line + w;
                        int x_new = line - w - 1;

                        vertices[x_new, z].y =
                            ((float)(width - w) / width) * vertices[x_orig,z].y +
                            ((float)w / width) * vertices[x_new,z].y;

                    }
                }
            }

        }

        rg.terrain.build();
    }

    /// <summary>
    /// DOESNT WORK WELL
    /// </summary>
    /// <param name="width"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="direction"></param>
    public void SmoothTerrainTransition(int width, Vertex start, Vertex end, Direction direction)
    {
        //Debug.Log("!");
        //Debug.Log(start);
        //Debug.Log(end);
        //Debug.Log(direction);
        int lineStart;
        int lineEnd;

        int transStart;

        int sgnDirection;

        switch (direction)
        {
            case Direction.right:
                lineStart = start.z;
                lineEnd = end.z;
                transStart = start.x; 
                sgnDirection = 1;
                break;
            case Direction.left:
                lineStart = start.z;
                lineEnd = end.z;
                transStart = start.x;
                sgnDirection = -1;
                break;
            case Direction.top:
                lineStart = start.x;
                lineEnd = end.x;
                transStart = start.z;
                sgnDirection = 1;
                break;
            case Direction.bot:
                lineStart = start.x;
                lineEnd = end.x;
                transStart = start.x;
                sgnDirection = -1;
                break;
            default:
                lineStart = start.x;
                lineEnd = end.x;
                transStart = start.z;
                sgnDirection = 1;
                break;
        }
        //Debug.Log(lineStart);
        //Debug.Log(lineEnd);
        //Debug.Log(transStart);
        //Debug.Log(sgnDirection);
        for (int line = lineStart; line < lineEnd-1; line++)
        {
            float step;
            switch (direction)
            {
                case Direction.right:
                    step = (vertices[transStart, line].y + vertices[transStart + sgnDirection * width, line].y) / width;
                    break;
                case Direction.left:
                    step = (vertices[transStart, line].y + vertices[transStart + sgnDirection * width, line].y) / width;
                    break;
                case Direction.top:
                    if(line > 100 && line < 110)
                    {
                        Debug.Log(vertices[line, transStart].y);
                        Debug.Log(vertices[line, transStart + sgnDirection * width].y);
                        Debug.Log((vertices[line, transStart].y + vertices[line, transStart + sgnDirection * width].y) / width);
                    }
                    step = (vertices[line, transStart].y - vertices[line, transStart + sgnDirection * width].y) / width;
                    break;
                case Direction.bot:
                    step = (vertices[line, transStart].y + vertices[line, transStart + sgnDirection * width].y) / width;
                    break;
                default:
                    step = (vertices[transStart, line].y + vertices[transStart + sgnDirection * width, line].y) / width;
                    break;
            }
            int stepCount = -1;
            for(int trans = transStart+2; trans != transStart + sgnDirection * width; trans+= sgnDirection)
            {
                stepCount++;
                int x;
                int z;

                switch (direction)
                {
                    case Direction.right:
                        x = trans;
                        z = line;
                        break;
                    case Direction.left:
                        x = trans;
                        z = line;
                        break;
                    case Direction.top:
                        x = line;
                        z = trans;
                        break;
                    case Direction.bot:
                        x = line;
                        z = trans;
                        break;
                    default:
                        x = trans;
                        z = line;
                        break;
                }
                if (CheckBounds(x, z))
                {
                    vertices[x, z].y = (width - stepCount) * step;
                    //vertices[x, z].y = vertices[line, transStart + 1].y - stepCount * step;
                }
                else
                {
                    Debug.Log(x + "," + z);
                }
            }
        }
    }
    */

    /// <summary>
    /// returns highest point from visible terrain
    /// </summary>
    public Vertex GetHighestpoint()
    {
        return GetHighestpoint(0, lt.terrainWidth, 0, lt.terrainHeight);
    }

    /// <summary>
    /// returns highest point from given region
    /// </summary>
    public Vertex GetHighestpoint(int x_min, int x_max, int z_min, int z_max)
    {


        Vertex highestPoint = new Vertex(x_min+10, z_min+10, lt.GetLocalHeight(x_min+10,z_min + 10));
        for (int x = x_min; x < x_max - 1; x++)
        {
            for (int z = z_min; z < z_max - 1; z++)
            {
                float height = lt.GetLocalHeight(x, z);
                if (height != 666 && height > highestPoint.height)
                    highestPoint = new Vertex(x, z, height);
            }
        }
        return highestPoint;
    }
    /*
    public Vertex FindNextHighestPeak(int radius, List<Vertex> foundPeaks)
    {
        int border = 20;
        Vertex highestPeak = new Vertex(0, 0, 0);
        for (int x = border; x < terrainSize - border; x++)
        {
            for (int z = border; z < terrainSize - border; z++)
            {
                if (vertices[x, z].y > highestPeak.height)
                {
                    bool isInRange = false;
                    foreach (Vertex v in foundPeaks)
                    {
                        if (fmc.IsInRange(new Vertex(x, z, vertices[x, z].y), v, radius * 2))
                        {
                            isInRange = true;
                        }
                    }
                    if (!isInRange)
                        highestPeak = new Vertex(x, z, vertices[x, z].y);
                }
            }
        }
        if (highestPeak.x == 0 && highestPeak.z == 0)
        {
            //Debug.Log("no place for more mountains");
            return null;
        }

        return highestPeak;
    }

    public Vertex GetLowestPointInArea(int _x, int _z, int area)
    {
        Vertex lowestVert = new Vertex(_x, _z, vertices[_x, _z].y);
        for (int x = _x - area; x <= _x + area; x++)
        {
            for (int z = _z - area; z <= _z + area; z++)
            {
                if (CheckBounds(x, z) && vertices[x, z].y < lowestVert.height)
                {
                    lowestVert.Rewrite(x, z, vertices[x, z].y);
                }
            }
        }
        return lowestVert;
    }
    */
    public bool CheckBounds(int x, int z, int offset)
    {
        return CheckBounds(x, z, offset, 0, lt.terrainWidth, 0, lt.terrainHeight);
    }

    public bool CheckBounds(int x, int z, int offset, int x_min, int x_max, int z_min, int z_max)
    {
        //return x > x_min + offset && x < x_max - 1 - offset && z > z_min + offset && z < z_max - 1 - offset;
        return x >= x_min+offset && x < x_max - offset && z >= z_min+offset && z < z_max - offset;
    }

    public bool CheckBounds(int x, int z)
    {
        return CheckBounds(x, z, 0);
    }

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
    /*
    public bool IsOnBorder(Vertex vert)
    {
        bool value = false;
        value =
            vert.x == 0 ||
            vert.x == terrainSize - 1 ||
            vert.z == 0 ||
            vert.z == terrainSize-1;

        return value;
    }
*/
}
