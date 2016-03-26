using UnityEngine;
using System.Collections;

public class SpikeFilter {
    
    private LocalTerrain lt;
    private FunctionMathCalculator fmc;
    public GlobalCoordinates globalFilterSpikeC;
    public FilterGenerator fg;

    public float lastEpsilon;

    public SpikeFilter(FilterGenerator fg)
    {
        this.fg = fg;
    }

    public void AssignFunctions(LocalTerrain lt, FunctionMathCalculator fmc, GlobalCoordinates globalFilterSpikeC)
    {
        this.lt = lt;
        this.fmc = fmc;
        this.globalFilterSpikeC = globalFilterSpikeC;
    }

    public void GenerateSpikeFilterInRegion(Area region, float epsilon)
    {
        //Debug.Log("spikes on " + region + " with " + epsilon);

        int x_min = region.botLeft.x;
        int z_min = region.botLeft.z;

        int x_max = region.topRight.x;
        int z_max = region.topRight.z;


        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                if (!globalFilterSpikeC.IsDefined(x, z))
                {
                    float average = lt.gt.GetNeighbourAverage(x, z, 2);
                    float height = lt.gt.GetHeight(x, z);

                    if (height < average - epsilon)
                        fg.SetGlobalValue(x, z, height-(average - epsilon), false, globalFilterSpikeC);
                    else if (height > average + epsilon)
                        fg.SetGlobalValue(x, z, height-(average + epsilon), false, globalFilterSpikeC);
                }
            }
        }
    }

    public void ResetFilter()
    {
        globalFilterSpikeC.ResetQuadrants();
    }
}
