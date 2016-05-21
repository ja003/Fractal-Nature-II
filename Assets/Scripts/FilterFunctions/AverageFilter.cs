using UnityEngine;
using System.Collections;

public class AverageFilter {

    private LocalTerrain lt;
    private FunctionMathCalculator fmc;
    public GlobalCoordinates globalFilterAverageC;
    public FilterGenerator fg;

    public AverageFilter(FilterGenerator fg)
    {
        this.fg = fg;
    }

    public void AssignFunctions(LocalTerrain lt, FunctionMathCalculator fmc, GlobalCoordinates globalFilterAverageC)
    {
        this.lt = lt;
        this.fmc = fmc;
        this.globalFilterAverageC = globalFilterAverageC;
    }

    /// <summary>
    /// generates average filter on given region
    /// </summary>
    public void GenerateAverageFilterInRegion(Area region)
    {
        int x_min = region.botLeft.x;
        int z_min = region.botLeft.z;

        int x_max = region.topRight.x;
        int z_max = region.topRight.z;


        float height;
        float neighbourAverage;
        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                
                if (lt.globalTerrainC.IsDefined(x, z) && !globalFilterAverageC.IsDefined(x, z))
                {
                    height = lt.lm.GetCurrentHeight(x, z);
                    neighbourAverage = lt.gt.GetNeighbourAverage(x, z, 2);
                    if (neighbourAverage != 666)
                    {
                        fg.SetGlobalValue(x, z, height -
                            neighbourAverage, false, globalFilterAverageC);
                    }
                }
            }
        }
    }

    public void ResetFilter()
    {
        globalFilterAverageC.ResetQuadrants();
    }

}
