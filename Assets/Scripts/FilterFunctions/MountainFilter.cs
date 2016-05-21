using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MountainFilter {

    private LocalTerrain lt;
    private FunctionMathCalculator fmc;
    public GlobalCoordinates globalFilterMountainC;
    public FilterGenerator fg;

    private List<Vertex> peaks;

    /// <summary>
    /// currently not used
    /// </summary>
    public MountainFilter(FilterGenerator fg)
    {
        this.fg = fg;
        peaks = new List<Vertex>();
    }

    public void AssignFunctions(LocalTerrain lt,FunctionMathCalculator fmc,GlobalCoordinates globalFilterMountainC)
    {
        this.lt = lt;
        this.fmc = fmc;
        this.globalFilterMountainC = globalFilterMountainC;
    }

    public void ResetFilter()
    {
        peaks = new List<Vertex>();
        globalFilterMountainC.ResetQuadrants();
    }

    /// <summary>
    /// finds 'count' highest peaks in given area which are distatnt enough from already found peaks
    /// sets filter value to given region 
    /// !!! values are being overwriten
    /// </summary>
    public void PerserveMountainsInRegion(Vector3 botLeft, Vector3 topRight, int count, int radius, int scaleFactor)
    {

        //List<Vertex> peaks = new List<Vertex>();
        for (int i = 0; i < count; i++)
        {
            if (FindNextHighestPeakInRegion(radius, botLeft, topRight) != null)
                peaks.Add(FindNextHighestPeakInRegion(radius, botLeft, topRight));
        }
        int x_min = (int)botLeft.x;
        int z_min = (int)botLeft.z;

        int x_max = (int)topRight.x;
        int z_max = (int)topRight.z;


        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                Vertex vert = new Vertex(x, z, lt.GetLocalHeight(x, z));
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
                fg.SetGlobalValue(x, z, lt.GetGlobalHeight(x, z) - lt.GetGlobalHeight(x, z) * (float)Math.Pow(scale, scaleFactor), true, globalFilterMountainC);
                if (x == 50 && z == 50)
                {
                    //Debug.Log(GetValue(x, z));
                }

                //vertices[x, z].y *= (float)Math.Pow(scale, scaleFactor);
                if (x > 90 && z > 90)
                {
                    //Debug.Log(GetGlobalValue(x, z));

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
    public Vertex FindNextHighestPeakInRegion(int radius, Vector3 botLeft, Vector3 topRight)
    {
        int border = 20;
        int x_min = (int)botLeft.x + border;
        int z_min = (int)botLeft.z + border;

        int x_max = (int)topRight.x - border;
        int z_max = (int)topRight.z - border;

        Vertex highestPeak = new Vertex(0, 0, -666);
        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                if (fg.GetGlobalValue(x, z, globalFilterMountainC) > highestPeak.height)
                {
                    bool isInRange = false;
                    foreach (Vertex v in peaks)
                    {
                        //dont add peak if it is too close to one of already found
                        if (fmc.IsInRange(new Vertex(x, z), v, radius * 2))
                        {
                            isInRange = true;
                        }
                    }
                    if (!isInRange)
                        highestPeak = new Vertex(x, z, fg.GetGlobalValue(x, z, globalFilterMountainC));
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
