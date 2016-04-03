using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FilteredTerrain {

    public TerrainGenerator tg;
    public FilterGenerator fg;
    public RiverGenerator rg;
    public ErosionGenerator eg;


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
    /// returns value from combination of give layers
    /// returns 0 from layer if not defined
    /// </summary>
    public float GetValueFromLayers(int x, int z, List<Layer> layers)
    {
        float value; 

        float terrain = 0;
        float erosionHydraulic = 0;
        float filterAverage = 0;
        float filterMedian = 0;
        float river = 0;

        foreach (Layer l in layers)
        {
            switch (l)
            {
                case Layer.terrain:
                    terrain = tg.globalTerrain.GetHeight(x, z);
                    if (terrain == 666)
                        terrain = 0;
                    break;
                case Layer.erosionHydraulic:
                    erosionHydraulic = eg.GetErosionValue(x, z);
                    if (erosionHydraulic == 666)
                        erosionHydraulic = 0;
                    break;

                case Layer.filterAverage:
                    filterAverage = fg.globalFilterAverageC.GetValue(x, z);
                    if (filterAverage == 666)
                        filterAverage = 0;
                    break;
                case Layer.filterMedian:
                    filterMedian = fg.globalFilterMedianC.GetValue(x, z);
                    if (filterMedian == 666)
                        filterMedian = 0;
                    break;
                case Layer.river:
                    foreach(RiverInfo r in rg.rivers)
                    {
                        value = r.globalRiverC.GetValue(x, z);
                        if(value != 666)    
                            river += value;
                    }
                    break;
            }
        }
        
        return terrain - erosionHydraulic - filterMedian - filterAverage + river;
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
