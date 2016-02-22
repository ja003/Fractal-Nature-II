using UnityEngine;
using System.Collections;

public class FilteredTerrain {

    public TerrainGenerator tg;
    public FilterGenerator fg;
    public RiverGenerator rg;


    public FilteredTerrain()
    {

    }

    public void AssignFunctions(TerrainGenerator terrainGenerator, FilterGenerator filterGenerator, RiverGenerator riverGenerator)
    {
        tg = terrainGenerator;
        fg = filterGenerator;
        rg = riverGenerator;
    }

    public float GetValue(int x, int z)
    {
        float terrain = tg.globalTerrain.GetHeight(x, z);
        if (terrain == 666)
            terrain = 0;

        float filterMountain = fg.globalFilterMountainC.GetValue(x, z);
        if (filterMountain == 666)
            filterMountain = 0;
        float filterAverage = fg.globalFilterAverageC.GetValue(x, z);
        if (filterAverage == 666)
            filterAverage = 0;
        float filterMedian = fg.globalFilterMedianC.GetValue(x, z);
        if (filterMedian == 666)
            filterMedian = 0;

        float river = rg.globalRiverC.GetValue(x, z);
        if (river == 666)
            river = 0;

        return terrain - filterMountain - filterMedian - filterAverage - river;
    }

}
