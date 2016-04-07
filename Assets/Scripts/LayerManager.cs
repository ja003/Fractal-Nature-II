using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerManager {

    public TerrainGenerator tg;
    public FilterGenerator fg;
    public RiverGenerator rg;
    public ErosionGenerator eg;
    public LocalTerrain lt;


    public LayerManager()
    {

    }

    public void AssignFunctions(TerrainGenerator terrainGenerator, FilterGenerator filterGenerator, 
        RiverGenerator riverGenerator, ErosionGenerator erosionGenerator)
    {
        tg = terrainGenerator;
        fg = filterGenerator;
        rg = riverGenerator;
        eg = erosionGenerator;
        lt = tg.localTerrain;
    }

    /// <summary>
    /// returns value from combination of give layers
    /// returns 0 from layer if not defined
    /// </summary>
    public float GetValueFromLayers(int x, int z, List<Layer> layers)
    {
        float value; 

        float terrain = 0;
        float river = 0;

        float filterAverage = 0;
        float filterMedian = 0;
        float filterSpike = 0;
        float filterGauss = 0;
        float filterMinThreshold = 0;
        float filterMaxThreshold = 0;

        float erosionHydraulic = 0;

        foreach (Layer l in layers)
        {
            switch (l)
            {
                case Layer.terrain:
                    terrain = tg.globalTerrain.GetHeight(x, z);
                    if (terrain == 666)
                        terrain = 0;
                    break;
                case Layer.river:
                    foreach(RiverInfo r in rg.rivers)
                    {
                        value = r.globalRiverC.GetValue(x, z);
                        if(value != 666)    
                            river += value;
                    }
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
                case Layer.filterSpike:
                    filterSpike = fg.globalFilterSpikeC.GetValue(x, z);
                    if (filterSpike == 666)
                        filterSpike = 0;
                    break;
                case Layer.filterGaussian:
                    filterGauss = fg.globalFilterGaussianC.GetValue(x, z);
                    if (filterGauss == 666)
                        filterGauss = 0;
                    break;
                case Layer.filterMinThreshold:
                    filterMinThreshold = fg.globalFilterMinThresholdC.GetValue(x, z);
                    if (filterMinThreshold == 666)
                        filterMinThreshold = 0;
                    break;
                case Layer.filterMaxThreshold:
                    filterMaxThreshold = fg.globalFilterMaxThresholdC.GetValue(x, z);
                    if (filterMaxThreshold == 666)
                        filterMaxThreshold = 0;
                    break;


                case Layer.erosionHydraulic:
                    erosionHydraulic = eg.GetErosionValue(x, z);
                    if (erosionHydraulic == 666)
                        erosionHydraulic = 0;
                    break;
            }
        }

        return terrain + river
            - (filterMedian + filterAverage + filterSpike + filterGauss + filterMinThreshold + filterMaxThreshold)
            - erosionHydraulic;
    }

    /// <summary>
    /// returns final height after all layers are applied of given global coordinates
    /// ignores selected layers
    /// </summary>
    public float GetValue(int x, int z, List<Layer> ignoreLayers)
    {
        float value;
        float terrain = 0;
        if (!ignoreLayers.Contains(Layer.terrain))
        {
            terrain = tg.globalTerrain.GetHeight(x, z);
            if (terrain == 666)
                terrain = 0;
        }
        float river = 0;
        if (!ignoreLayers.Contains(Layer.river))
        {
            foreach (RiverInfo r in rg.rivers)
            {
                value = r.globalRiverC.GetValue(x, z);
                if (value != 666)
                    river += value;
            }
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

        float filterSpike = 0;
        if (!ignoreLayers.Contains(Layer.filterSpike))
        {
            filterSpike = fg.globalFilterSpikeC.GetValue(x, z);
            if (filterSpike == 666)
                filterSpike = 0;
        }

        float filterGauss = 0;
        if (!ignoreLayers.Contains(Layer.filterGaussian))
        {
            filterGauss = fg.globalFilterGaussianC.GetValue(x, z);
            if (filterGauss == 666)
                filterGauss = 0;
        }
        float filterMinThreshold = 0;
        if (!ignoreLayers.Contains(Layer.filterMinThreshold))
        {
            filterMinThreshold = fg.globalFilterMinThresholdC.GetValue(x, z);
            if (filterMinThreshold == 666)
                filterMinThreshold = 0;
        }
        float filterMaxThreshold = 0;
        if (!ignoreLayers.Contains(Layer.filterMaxThreshold))
        {
            filterMaxThreshold = fg.globalFilterMaxThresholdC.GetValue(x, z);
            if (filterMaxThreshold == 666)
                filterMaxThreshold = 0;
        }



        float eroionHydraulic = 0;
        if (!ignoreLayers.Contains(Layer.erosionHydraulic))
        {
            eroionHydraulic = eg.GetErosionValue(x, z);
            if (eroionHydraulic == 666)
                eroionHydraulic = 0;
        }


        return terrain + river 
            - (filterMedian + filterAverage + filterSpike + filterGauss + filterMinThreshold + filterMaxThreshold) 
            - eroionHydraulic;
    }

}
