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

        int x_min = region.botLeft.x;
        int z_min = region.botLeft.z;

        int x_max = region.topRight.x;
        int z_max = region.topRight.z;


        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                if (lt.globalTerrainC.IsDefined(x, z) && !globalFilterSpikeC.IsDefined(x, z))
                {
                    float average = lt.gt.GetNeighbourAverage(x, z, 2);
                    if (average != 666)
                    {
                        float height = lt.lm.GetCurrentHeight(x, z);

                        if (height < average - epsilon)
                            fg.SetGlobalValue(x, z, height - (average - epsilon), false, globalFilterSpikeC);
                        else if (height > average + epsilon)
                            fg.SetGlobalValue(x, z, height - (average + epsilon), false, globalFilterSpikeC);
                    }
                }
            }
        }
    }

    /// <summary>
    /// apply filter to given heightmap
    /// </summary>
    public void FilterMapInRegion(GlobalCoordinates map, Area region, float epsilon)
    {
        int x_min = region.botLeft.x;
        int z_min = region.botLeft.z;

        int x_max = region.topRight.x;
        int z_max = region.topRight.z;

        int area = 1;
        GlobalCoordinates mapTmp = new GlobalCoordinates(100);

        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                if (map.IsDefined(x, z))
                {
                    float average = 0;
                    int neighboursCount = 0;
                    for (int _x = x - area; _x <= x + area; _x++)
                    {
                        for (int _z = z - area; _z <= z + area; _z++)
                        {
                            if (map.IsDefined(_x, _z))
                            {
                                average += map.GetValue(_x, _z, 0);
                                neighboursCount++;
                            }
                        }
                    }
                    if (neighboursCount != 0)
                        average /= neighboursCount;
                    else
                        average = 666;

                    if (average != 666)
                    {
                        float height = map.GetValue(x, z);

                        if (height < average - epsilon)
                            fg.SetGlobalValue(x, z, (average + epsilon), true, mapTmp);
                        else if (height > average + epsilon)
                            fg.SetGlobalValue(x, z, (average - epsilon), true, mapTmp);

                        
                    }
                }
            }
        }
        for (int x = x_min; x < x_max; x++)
        {
            for (int z = z_min; z < z_max; z++)
            {
                if(mapTmp.IsDefined(x, z))
                    map.SetValue(x, z, mapTmp.GetValue(x, z));
            }
        }
    }

    public void ResetFilter()
    {
        globalFilterSpikeC.ResetQuadrants();
    }
}
