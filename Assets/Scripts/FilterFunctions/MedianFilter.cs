using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MedianFilter  {

    private LocalTerrain lt;
    private FunctionMathCalculator fmc;
    public GlobalCoordinates globalFilterMedianC;
    public FilterGenerator fg;
    public FunctionTerrainManager ftm;

    public MedianFilter(FilterGenerator fg)
    {
        this.fg = fg;
    }

    public void AssignFunctions(LocalTerrain lt, FunctionMathCalculator fmc, 
        GlobalCoordinates globalFilterMedianC, FunctionTerrainManager functionTerrainManager)
    {
        this.lt = lt;
        this.fmc = fmc;
        this.globalFilterMedianC = globalFilterMedianC;
        ftm = functionTerrainManager;
    }

    /// <summary>
    /// generates average filter on given region
    /// </summary>
    public void GenerateMedianFilterInRegion(Area region)
    {
        Debug.Log("median on " + region);

        int x_min = region.botLeft.x;
        int z_min = region.botLeft.z;

        int x_max = region.topRight.x;
        int z_max = region.topRight.z;

        List<Layer> ignoreLayers = new List<Layer>();
        ignoreLayers.Add(Layer.filterMedian);

        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                //if(fg.GetGlobalValue(x,z,globalFilterMedianC)
                //if(!globalFilterMedianC.IsDefined(x, z)){ //TODO: mountain filter sometimes changes provious values, therefore median has to also overwrite

                fg.SetGlobalValue(x, z, lt.ft.GetValue(x, z, ignoreLayers) - ftm.GetGlobalMedian(x, z, 2), true, globalFilterMedianC);
                //}
                //fg.SetGlobalValue(x, z, lt.gt.GetHeight(x, z) - ftm.GetGlobalMedian(x, z, 2), false, globalFilterMedianC);
                //fg.SetGlobalValue(x, z, lt.gt.GetHeight(x, z) - lt.gt.GetNeighbourAverage(x, z, 2), false, globalFilterMedianC);
            }
        }
    }

    public void ResetFilter()
    {
        globalFilterMedianC.ResetQuadrants();
    }
}
