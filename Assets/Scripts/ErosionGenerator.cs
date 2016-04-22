using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ErosionGenerator {

    public HydraulicErosion he;
    public ThermalErosion te;
    public LocalTerrain lt;

    public ErosionGenerator(LocalTerrain localTerrain)
    {
        lt = localTerrain;
        he = new HydraulicErosion(this);
        te = new ThermalErosion(this);
    }

    public void AssignFunctions(FunctionTerrainManager functionTerrainManager)
    {
        he.AssignFunctions(functionTerrainManager);
        te.AssignFunctions(functionTerrainManager);
    }
    

    public float GetTerrainValue(int x, int z)
    {
        List<Layer> layers = new List<Layer>();
        layers.Add(Layer.terrain);
        layers.Add(Layer.river);
        return lt.lm.GetValueFromLayers(x, z, layers);
    }

}
