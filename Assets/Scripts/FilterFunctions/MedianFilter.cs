using UnityEngine;
using System.Collections;

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
    public void GenerateMedianFilterInRegion(Vector3 botLeft, Vector3 topRight)
    {   
        int x_min = (int)botLeft.x;
        int z_min = (int)botLeft.z;

        int x_max = (int)topRight.x;
        int z_max = (int)topRight.z;
        

        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                //fg.SetGlobalValue(x, z, lt.ft.GetValue(x, z) - ftm.GetGlobalMedian(x, z, 2), false, globalFilterMedianC);
                fg.SetGlobalValue(x, z, lt.gt.GetHeight(x, z) - ftm.GetGlobalMedian(x, z, 2), false, globalFilterMedianC);
                //fg.SetGlobalValue(x, z, lt.gt.GetHeight(x, z) - lt.gt.GetNeighbourAverage(x, z, 2), false, globalFilterMedianC);
            }
        }
    }

    public void ResetFilter()
    {
        globalFilterMedianC.ResetQuadrants();
    }
}
