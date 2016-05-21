using UnityEngine;
using System.Collections;

public class ThermalErosion {

    ErosionGenerator eg;
    FunctionTerrainManager ftm;

    public GlobalCoordinates thermalErosionMap;

    public ThermalErosion(ErosionGenerator erosionGenerator)
    {
        eg = erosionGenerator;
        thermalErosionMap = new GlobalCoordinates(100);
    }

    public void AssignFunctions(FunctionTerrainManager functionTerrainManager)
    {
        ftm = functionTerrainManager;
    }

    int counter = 0;

    /// <summary>
    /// applies given number of iteration on given area
    /// slopeMin = min difference between neighbours for erosion to take effect
    /// </summary>
    public void ThermalErosionStep(Area area, int iterations, float minDif, float strength)
    {

        //Thermal erosion main algorithm

        //Start iterating
        for (int iter = 0; iter < iterations; iter++)
        {

            //Pick random position and start the main algorithm
            int x = Random.Range(area.botLeft.x, area.topRight.x);
            int z = Random.Range(area.botLeft.z, area.topRight.z);

            //Call the sediment transportation recursive step
            ThermalRecursion(x, z, minDif, strength, 0);
        }
    }
    private void ThermalRecursion(int x, int z, float minDif, float strength, int recursionLvl)
    {
        if (recursionLvl > 20)
            return;
        recursionLvl++;

        Vertex current = new Vertex(x, z, GetErodedTerrainValue(x,z));
        //Recursive sediment transportation with slope checking

        //Find lowest neighbour coordinates
        Vertex lowestN = ftm.GetLowestNeighbour(current, 1, 8);
        //lowestN.height = GetErodedTerrainValue(lowestN.x, lowestN.z);//SHIT

        //Calculate distance/slope
        float dif = current.height - lowestN.height;
            //vertices[x, z].y - vertices[(int)lowestNeighbour.x, (int)lowestNeighbour.y].y;

        //Check bounds
        if (dif > minDif)
        {
            //Move sediment 
            float sedAmount = strength * (dif - minDif);

            //vertices[x, z].y -= sedAmount;
            thermalErosionMap.SetValue(x, z, thermalErosionMap.GetValue(x, z, 0) - sedAmount);
            //vertices[(int)lowestN.x, (int)lowestN.y].y += sedAmount;
            thermalErosionMap.SetValue(lowestN.x, lowestN.z, 
                thermalErosionMap.GetValue(lowestN.x, lowestN.z, 0) + sedAmount);

            //Recall
            ThermalRecursion(lowestN.x, lowestN.z, minDif, strength, recursionLvl);
        }
    }

    /// <summary>
    /// returns terrain height + thermal erosion value
    /// </summary>
    private float GetErodedTerrainValue(int x, int z)
    {
        return eg.GetTerrainValue(x, z) + thermalErosionMap.GetValue(x, z, 0);
    }

    public void ResetErosion()
    {
        thermalErosionMap.ResetQuadrants();
    }
}
