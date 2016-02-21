using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FilterGenerator// : IFilterGenerator
{
    private LocalTerrain lt;
    private FunctionMathCalculator fmc;

    public LocalCoordinates localFilterMountainC;
    public GlobalCoordinates globalFilterMountainC;

    public LocalCoordinates localFilterAverageC;
    public GlobalCoordinates globalFilterAverageC;

    public MountainFilter mf;
    public AverageFilter af;
    
    public FilterGenerator(int quadrantSize, LocalTerrain localTerrain)
    {
        globalFilterMountainC = new GlobalCoordinates(100);
        globalFilterAverageC = new GlobalCoordinates(100);
        lt = localTerrain;

        localFilterMountainC = new LocalCoordinates(globalFilterMountainC, new Vector3(0,0,0), lt.terrainWidth, lt.terrainHeight);
        localFilterAverageC = new LocalCoordinates(globalFilterAverageC, new Vector3(0,0,0), lt.terrainWidth, lt.terrainHeight);

        //peaks = new List<Vertex>();
        mf = new MountainFilter(this);
        af = new AverageFilter(this);
    }

    public void AssignFunctions(FunctionMathCalculator functionMathCalculator, LocalTerrain localTerrain)
    {
        fmc = functionMathCalculator;
        lt = localTerrain;

        mf.AssignFunctions(lt, fmc, globalFilterMountainC);
        af.AssignFunctions(lt, fmc, globalFilterAverageC);
    }
    
    /// <summary>
    /// returns filter value on given local coordiantes (0 if not defined)
    /// localFilter = filter type
    /// </summary>
    public float GetLocalValue(int x, int z, LocalCoordinates localFilter)
    {
        float value = localFilter.GetGlobalValue(x, z);
        if (value != 666)
            return value;
        else
            return 0;
    }

    /// <summary>
    /// returns filter value on given global coordiantes (0 if not defined)
    /// gc = global filter space
    /// </summary>
    public float GetGlobalValue(int x, int z, GlobalCoordinates gc)
    {
        float value = gc.GetValue(x, z);
        if (value != 666)
            return value;
        else
            return 0;
    }

    /// <summary>
    /// sets filter value
    /// all filters should operate on global space
    /// gc = global filter space
    /// </summary>
    public void SetGlobalValue(int x, int z, float value, bool overwrite, GlobalCoordinates gc)
    {
        gc.SetValue(x, z, value, overwrite);
    }
    
    /// <summary>
    /// deletes all filter values
    /// resets number of peaks
    /// </summary>
    public void ResetFilters()
    {
        mf.ResetFilter();
        af.ResetFilter();
    }

    public void UpdateLocalCoordinates(Vector3 center, Vector3 botLeft, Vector3 topRight)
    {
        localFilterMountainC.center = center;
        localFilterMountainC.botLeft = botLeft;
        localFilterMountainC.topRight = topRight;

        localFilterAverageC.center = center;
        localFilterAverageC.botLeft = botLeft;
        localFilterAverageC.topRight = topRight;
    }

}
