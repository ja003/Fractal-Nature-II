using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FilterGenerator : IFilterGenerator
{
    private LocalTerrain lt;
    private FunctionMathCalculator fmc;
    public LocalCoordinates localFilterC;
    public GlobalCoordinates globalFilterC;


    public FilterGenerator(int quadrantSize, LocalTerrain localTerrain)
    {
        globalFilterC = new GlobalCoordinates(100);
        lt = localTerrain;
        localFilterC = new LocalCoordinates(globalFilterC, new Vector3(0,0,0), lt.terrainWidth, lt.terrainHeight);
    }

    /// <summary>
    /// returns filter value on given coordiantes (0 if not derfined)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public float GetValue(int x, int z)
    {
        float value = localFilterC.GetGlobalValue(x, z);
        if (value != 666)
            return value;
        else
            return 0;
    }
    
    public void SetValue(int x, int z, float value, bool overwrite)
    {
        if (!overwrite && localFilterC.IsDefined(x, z))
        {
            //Debug.Log(x + "," + z + ": set");
            return;
        }
        localFilterC.SetGlobalValue(x, z, value, overwrite);
    }

    public void AssignFunctions(FunctionMathCalculator functionMathCalculator, LocalTerrain localTerrain)
    {
        fmc = functionMathCalculator;
        lt = localTerrain;
    }

    public void PerserveMountains(int count, int radius, int scaleFactor)
    {
        List<Vertex> peaks = new List<Vertex>();
        for (int i = 0; i < count; i++)
        {
            if (FindNextHighestPeak(radius, peaks) != null)
                peaks.Add(FindNextHighestPeak(radius, peaks));
        }
        int x_min = 0;
        int x_max = lt.terrainWidth;
        int z_min = 0;
        int z_max = lt.terrainHeight;

        

        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                Vertex vert = new Vertex(x, z, lt.GetGlobalHeight(x, z));
                double scale = 0;

                foreach (Vertex v in peaks)
                {
                    if (GetScale(vert, v, radius) > scale)
                    {
                        scale = GetScale(vert, v, radius);
                    }
                    
                }

                //vertices[x, z].y *= (float)Math.Pow(scale, scaleFactor) *((float)distance /(terrainSize/4));
                if (x == 50 && z == 50)
                {
                    //Debug.Log(GetValue(x, z));
                    //Debug.Log("setting: " + (lt.GetGlobalHeight(x, z) - lt.GetGlobalHeight(x, z) * (float)Math.Pow(scale, scaleFactor)));
                }
                SetValue(x, z, lt.GetGlobalHeight(x, z) - lt.GetGlobalHeight(x, z) * (float)Math.Pow(scale, scaleFactor), false);
                if (x == 50 && z == 50)
                {
                    //Debug.Log(GetValue(x, z));
                }

                //vertices[x, z].y *= (float)Math.Pow(scale, scaleFactor);
                if (x < 10 && z < 10)
                {
                    //Debug.Log(localFilterC.GetGlobalValue(x, z));

                    //Debug.Log(scale);
                }
            }
        }
        //Debug.Log(peaks.Count);


        //blur the peaks
        float blurringFactor = radius / 10;
        int kernelSize = radius / 10;

        for (int i = 0; i < peaks.Count; i++)
        {
            //rg.filtermanager.applyGaussianBlur(blurringFactor, kernelSize,
                //new Vector3(peaks[i].x - kernelSize, 0, peaks[i].z - kernelSize),
               // new Vector3(peaks[i].x + kernelSize, 0, peaks[i].z + kernelSize));

        }

        //lt.MoveVisibleTerrain(lt.center);
       // rg.terrain.build();
    }

    public Vertex FindNextHighestPeak(int radius, List<Vertex> foundPeaks)
    {
        int border = 20;
        Vertex highestPeak = new Vertex(0, 0, 0);
        for (int x = border; x < lt.terrainWidth - border; x++)
        {
            for (int z = border; z < lt.terrainHeight - border; z++)
            {
                if (lt.GetGlobalHeight(x, z) > highestPeak.height)
                {
                    bool isInRange = false;
                    foreach (Vertex v in foundPeaks)
                    {
                        if (fmc.IsInRange(new Vertex(x, z, lt.GetGlobalHeight(x, z)), v, radius * 2))
                        {
                            isInRange = true;
                        }
                    }
                    if (!isInRange)
                        highestPeak = new Vertex(x, z, lt.GetGlobalHeight(x, z));
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

    /// <summary>
    /// calculates scale function for mountain filter
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public double GetScale(Vertex v1, Vertex v2, int radius)
    {
        return Math.Log(lt.terrainWidth - fmc.GetDistance(v1, v2), lt.terrainWidth);
    }
}
