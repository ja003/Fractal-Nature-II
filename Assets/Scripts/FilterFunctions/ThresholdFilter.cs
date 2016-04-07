using UnityEngine;
using System.Collections;

public class ThresholdFilter {
    private LocalTerrain lt;
    private FunctionMathCalculator fmc;
    public GlobalCoordinates globalMinThresholdC;
    public GlobalCoordinates globalMaxThresholdC;
    public FilterGenerator fg;

    public float lastMinThreshold;
    public float lastMaxThreshold;

    public ThresholdFilter(FilterGenerator fg)
    {
        this.fg = fg;
    }

    public void AssignFunctions(LocalTerrain lt, FunctionMathCalculator fmc, GlobalCoordinates globalMinThresholdC, GlobalCoordinates globalMaxThresholdC)
    {
        this.lt = lt;
        this.fmc = fmc;
        this.globalMinThresholdC = globalMinThresholdC;
        this.globalMaxThresholdC = globalMaxThresholdC;
    }

    public void GenerateMinThresholdInRegion(Area region, float minThreshold)
    {
        lastMinThreshold = minThreshold;

        int x_min = region.botLeft.x;
        int z_min = region.botLeft.z;

        int x_max = region.topRight.x;
        int z_max = region.topRight.z;


        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                if (!globalMinThresholdC.IsDefined(x, z))
                {
                    float height = lt.gt.GetHeight(x, z);

                    if (height < minThreshold)
                    {
                        fg.SetGlobalValue(x, z, -Mathf.Log(Mathf.Abs(height - minThreshold)+1), false, globalMinThresholdC);
                        //fg.SetGlobalValue(x, z, -Mathf.Sqrt(Mathf.Abs(height - minThreshold)), false, globalMinThresholdC);
                    }
                }
            }
        }
    }
    
    public void GenerateMaxThresholdInRegion(Area region, float maxThreshold)
    {
        lastMaxThreshold = maxThreshold;

        int x_min = region.botLeft.x;
        int z_min = region.botLeft.z;

        int x_max = region.topRight.x;
        int z_max = region.topRight.z;


        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                if (!globalMaxThresholdC.IsDefined(x, z))
                {
                    float height = lt.gt.GetHeight(x, z);

                    if (height > maxThreshold)
                    {
                        fg.SetGlobalValue(x, z, Mathf.Log(height - maxThreshold+1), false, globalMaxThresholdC);
                        /*if(height - maxThreshold > 1)
                            fg.SetGlobalValue(x, z, Mathf.Sqrt(Mathf.Abs(height - maxThreshold)), false, globalMaxThresholdC);
                        else
                            fg.SetGlobalValue(x, z, height - maxThreshold, false, globalMaxThresholdC);*/
                    }
                }
            }
        }
    }

    public void ResetMinFilter()
    {
        globalMinThresholdC.ResetQuadrants();
    }
    public void ResetMaxFilter()
    {
        globalMaxThresholdC.ResetQuadrants();
    }

    public void ResetFilters()
    {
        ResetMinFilter();
        ResetMaxFilter();
    }
}
