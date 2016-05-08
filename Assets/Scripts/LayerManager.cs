using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerManager {

    public TerrainGenerator tg;
    public FilterGenerator fg;
    public RiverGenerator rg;
    public ErosionGenerator eg;
    public LocalTerrain lt;

    public List<Layer> activeLayers;

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

        UpdateLayers();
    }

    int counter = 0;

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

        float erosionHydraulicWater = 0;
        float erosionHydraulic = 0; //eroded terrain

        float erosionThermal = 0; //eroded terrain


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
                        //Debug.Log(r);
                        //Debug.Log(rg.rivers[0]);
                        //Debug.Log(rg.riverGui.riverFlags[0]);

                        if (rg.riverGui.riverFlags[rg.rivers.IndexOf(r)])
                        {
                            value = r.globalRiverC.GetValue(x, z);
                            if (value != 666)
                                river += value;
                        }
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


                //case Layer.water:
                //    erosionHydraulicWater = eg.he.GetWaterValue(x, z);
                //    if (erosionHydraulicWater == 666)
                //        erosionHydraulicWater = 0;
                //    break;
                case Layer.erosionHydraulic:
                    erosionHydraulic = eg.he.GetHydraulicErosionValue(x, z);
                    if (erosionHydraulic == 666)
                        erosionHydraulic = 0;
                    break;

                case Layer.erosionThermal:                    
                    erosionThermal = eg.te.thermalErosionMap.GetValue(x, z, 0);
                    break;
            }
        }
        if (counter < 10)
        {
            if (BadValue(terrain))
                Debug.Log(x + "," + z + ":" + "terrain = " + terrain);
            if (BadValue(filterAverage))
                Debug.Log(x + "," + z + ":" + "filterAverage = " + filterAverage);
            if (BadValue(filterSpike))
                Debug.Log(x + "," + z + ":" + "filterSpike = " + filterSpike);
            if (BadValue(filterGauss))
                Debug.Log(x + "," + z + ":" + "filterGauss = " + filterGauss);
            if (BadValue(filterMinThreshold))
                Debug.Log(x + "," + z + ":" + "filterMinThreshold = " + filterMinThreshold);
            if (BadValue(filterMaxThreshold))
                Debug.Log(x + "," + z + ":" + "filterMaxThreshold = " + filterMaxThreshold);
            if (BadValue(erosionHydraulicWater))
                Debug.Log(x + "," + z + ":" + "erosionHydraulicWater = " + erosionHydraulicWater);
            if (BadValue(erosionHydraulic))
                Debug.Log(x + "," + z + ":" + "erosionHydraulic = " + erosionHydraulic);
            if (BadValue(erosionThermal))
                Debug.Log(x + "," + z + ":" + "erosionThermal = " + erosionThermal);
        }

        return terrain + river
            - (filterMedian + filterAverage + filterSpike + filterGauss + filterMinThreshold + filterMaxThreshold)
            - erosionHydraulicWater + erosionHydraulic + erosionThermal;
    }

    private bool BadValue(float value)
    {
        return value > 500 || value < -500;
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
        

        //float erosionHydraulicWater = 0;
        //if (!ignoreLayers.Contains(Layer.water))
        //{
        //    erosionHydraulicWater = eg.he.GetWaterValue(x, z);
        //    if (erosionHydraulicWater == 666)
        //        erosionHydraulicWater = 0;
        //}
        float erosionHydraulicSediment = 0;
        if (!ignoreLayers.Contains(Layer.erosionHydraulic))
        {
            erosionHydraulicSediment = eg.he.GetSedimentValue(x, z);
            if (erosionHydraulicSediment == 666)
                erosionHydraulicSediment = 0;
        }

        float erosionThermal = 0;
        if (!ignoreLayers.Contains(Layer.erosionThermal))
        {
            erosionThermal = eg.te.thermalErosionMap.GetValue(x, z);
        }

        return terrain + river 
            - (filterMedian + filterAverage + filterSpike + filterGauss + filterMinThreshold + filterMaxThreshold) 
            //- erosionHydraulicWater 
            + erosionHydraulicSediment + erosionThermal;
    }
    
    /// <summary>
    /// returns value from active layers
    /// </summary>
    public float GetCurrentHeight(int x, int z)
    {
        return GetValueFromLayers(x, z, activeLayers);
    }


    public void UpdateLayers()
    {
        activeLayers = new List<Layer>();
        if (tg.terrainLayer)
            activeLayers.Add(Layer.terrain);
        if (tg.riverLayer)
            activeLayers.Add(Layer.river);

        if (tg.filterAverageLayer)
            activeLayers.Add(Layer.filterAverage);
        if (tg.filterMedianLayer)
            activeLayers.Add(Layer.filterMedian);
        if (tg.filterSpikeLayer)
            activeLayers.Add(Layer.filterSpike);
        if (tg.filterGaussianLayer)
            activeLayers.Add(Layer.filterGaussian);
        if (tg.filterMinThresholdLayer)
            activeLayers.Add(Layer.filterMinThreshold);
        if (tg.filterMaxThresholdLayer)
            activeLayers.Add(Layer.filterMaxThreshold);

        //if (tg.waterLayer)
        //    activeLayers.Add(Layer.water);
        if (tg.erosionHydraulicLayer)
            activeLayers.Add(Layer.erosionHydraulic);
        if (tg.erosionThermalLayer)
            activeLayers.Add(Layer.erosionThermal);

        //Debug.Log("update: " + this);
    }

    public override string ToString()
    {
        string s = "active layers: ";
        foreach(Layer layer in activeLayers)
        {
            s += layer + "\n";
        }

        return s;
    }
}
