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
    public void GenerateAverageFilterInRegion(Vector3 botLeft, Vector3 topRight)
    {
        int x_min = (int)botLeft.x;
        int z_min = (int)botLeft.z;

        int x_max = (int)topRight.x;
        int z_max = (int)topRight.z;


        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                fg.SetGlobalValue(x, z, lt.gt.GetHeight(x, z) - lt.gt.GetNeighbourAverage(x, z, 2), false, globalFilterAverageC);
            }
        }
    }

    public void ResetFilter()
    {
        globalFilterAverageC.ResetQuadrants();
    }

}
