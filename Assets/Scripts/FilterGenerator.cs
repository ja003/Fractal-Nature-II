using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FilterGenerator// : IFilterGenerator
{
    private LocalTerrain lt;
    private FunctionMathCalculator fmc;
    public FunctionTerrainManager ftm;

    public LocalCoordinates localCoordinates;

    public GlobalCoordinates globalFilterMountainC;
    public GlobalCoordinates globalFilterAverageC;
    public GlobalCoordinates globalFilterMedianC;
    public GlobalCoordinates globalFilterSpikeC;
    public GlobalCoordinates globalFilterGaussianC;

    public MountainFilter mf;
    public AverageFilter af;
    public MedianFilter mdf;
    public SpikeFilter sf;
    public GaussianFilter gf;

    public FilterGenerator(int quadrantSize, LocalTerrain localTerrain)
    {
        globalFilterMountainC = new GlobalCoordinates(100);
        globalFilterAverageC = new GlobalCoordinates(100);
        globalFilterMedianC = new GlobalCoordinates(100);
        globalFilterSpikeC = new GlobalCoordinates(100);
        globalFilterGaussianC = new GlobalCoordinates(100);

        lt = localTerrain;
        localCoordinates = lt.localTerrainC;

        //localFilterMountainC = new LocalCoordinates(globalFilterMountainC, new Vector3(0,0,0), lt.terrainWidth, lt.terrainHeight);
        //localFilterAverageC = new LocalCoordinates(globalFilterAverageC, new Vector3(0,0,0), lt.terrainWidth, lt.terrainHeight);

        //peaks = new List<Vertex>();
        mf = new MountainFilter(this);
        af = new AverageFilter(this);
        mdf = new MedianFilter(this);
        sf = new SpikeFilter(this);
        gf = new GaussianFilter(this);
    }

    public void AssignFunctions(FunctionMathCalculator functionMathCalculator, 
        LocalTerrain localTerrain, FunctionTerrainManager functionTerrainManager)
    {
        fmc = functionMathCalculator;
        lt = localTerrain;
        ftm = functionTerrainManager;

        mf.AssignFunctions(lt, fmc, globalFilterMountainC);
        af.AssignFunctions(lt, fmc, globalFilterAverageC);
        mdf.AssignFunctions(lt, fmc, globalFilterMedianC, ftm);
        sf.AssignFunctions(lt, fmc, globalFilterSpikeC);
        gf.AssignFunctions(lt, fmc, globalFilterGaussianC);
    }
    
    /// <summary>
    /// returns filter value on given local coordiantes (0 if not defined)
    /// localFilter = filter type
    /// </summary>
    public float GetLocalValue(int x, int z, GlobalCoordinates gc)
    {
        float value = localCoordinates.GetLocalValue(x, z, gc);
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
        mdf.ResetFilter();
        sf.ResetFilter();
        gf.ResetFilter();
    }
    /*
    public void UpdateLocalCoordinates(Vector3 center, Vector3 botLeft, Vector3 topRight)
    {
        localFilterMountainC.center = center;
        localFilterMountainC.botLeft = botLeft;
        localFilterMountainC.topRight = topRight;

        localFilterAverageC.center = center;
        localFilterAverageC.botLeft = botLeft;
        localFilterAverageC.topRight = topRight;
    }*/

}
