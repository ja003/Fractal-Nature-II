using UnityEngine;
using System.Collections;

public class RandomTerrain {

    private TerrainGenerator tg;

    int terrainWidth;
    int terrainHeight;


    public RandomTerrain(TerrainGenerator tg)
    {
        this.tg = tg;
    }

    /// <summary>
    /// generates random hights on given area
    /// </summary>
    /// <param name="botLeft">global coordinate</param>
    /// <param name="topRight">global coordinate</param>
    public void GenerateRandomTerrain(Vector3 botLeft, Vector3 topRight)
    {
        //Debug.Log(botLeft + " --- " + topRight);
        for (int x = (int)botLeft.x; x < (int)topRight.x; x++)
        {
            for (int z = (int)botLeft.z; z < (int)topRight.z; z++)  
            {
                float neighb_average = tg.globalTerrain.GetNeighbourHeight(x, z);
                if (neighb_average == 666)
                    neighb_average = 0;
                float rand_height = Random.Range(neighb_average-0.1f, neighb_average+0.1f);
                tg.globalTerrain.SetHeight(x, z, rand_height, false);
                tg.globalTerrain.SetHeight(x, z, 0, false);
                if (x < -98 && z < -98)
                {
                    //Debug.Log(x + "," + z + " => " + tg.globalTerrain.GetHeight(x,z));
                    
                }
            }
        }
    }


}
