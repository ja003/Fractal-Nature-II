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


        float median;
        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                if (lt.globalTerrainC.IsDefined(x, z) && !globalFilterMedianC.IsDefined(x, z))
                {
                    median = ftm.GetGlobalMedian(x, z, 2);
                    if (median != 666) {
                        fg.SetGlobalValue(x, z, lt.lm.GetCurrentHeight(x, z) - median, true, globalFilterMedianC);
                    }
                }
            }
        }
    }

    public void ResetFilter()
    {
        globalFilterMedianC.ResetQuadrants();
    }
}
