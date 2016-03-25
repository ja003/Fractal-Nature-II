using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    /// <summary>
    /// returns final height after all layers are applied of given global coordinates
    /// ignores selected layers
    /// </summary>
    public float GetValue(int x, int z, List<Layer> ignoreLayers)
    {
        float terrain = 0;
        if (!ignoreLayers.Contains(Layer.terrain))
        {
            terrain = tg.globalTerrain.GetHeight(x, z);
            if (terrain == 666)
                terrain = 0;
        }

        float filterMountain = 0;
        if (!ignoreLayers.Contains(Layer.filterMountain))
        {
            filterMountain = fg.globalFilterMountainC.GetValue(x, z);
            if (filterMountain == 666)
                filterMountain = 0;
        }

        float filterAverage = 0;
        if (!ignoreLayers.Contains(Layer.filterAverage))
        {
            filterAverage = fg.globalFilterAverageC.GetValue(x, z);
            if (filterAverage == 666)
                filterAverage = 0;
        }

        float filterMedian = 0;
        if (!ignoreLayers.Contains(Layer.filterMedian))
        {
            filterMedian = fg.globalFilterMedianC.GetValue(x, z);
            if (filterMedian == 666)
                filterMedian = 0;
        }

        float river = 0;
        if (!ignoreLayers.Contains(Layer.river))
        {            
            /*river = rg.globalRiverC.GetValue(x, z);
            if (river == 666)
                river = 0;*/
        }

        return terrain - filterMountain - filterMedian - filterAverage - river;
    }

}
