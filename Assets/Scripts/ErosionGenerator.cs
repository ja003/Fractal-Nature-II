using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ErosionGenerator {

    public HydraulicErosion he;
    public LocalTerrain lt;

    public ErosionGenerator(LocalTerrain localTerrain)
    {
        lt = localTerrain;
        he = new HydraulicErosion(this);
    }

    public void AssignFunctions(FunctionTerrainManager functionTerrainManager)
    {
        he.AssignFunctions(functionTerrainManager);
    }

    public float GetErosionValue(int x, int z)
    {
        return he.GetSedimentValue(x, z);
    }

    public float GetTerrainValue(int x, int z)
    {
        List<Layer> layers = new List<Layer>();
        layers.Add(Layer.terrain);
        layers.Add(Layer.river);
        return lt.ft.GetValueFromLayers(x, z, layers);
    }

}
