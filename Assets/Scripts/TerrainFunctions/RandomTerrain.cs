using UnityEngine;
using System.Collections;

public class RandomTerrain {

    private TerrainGenerator tg;
    public FunctionMathCalculator fmc;

    int terrainWidth;
    int terrainHeight;


    public RandomTerrain(TerrainGenerator tg)
    {
        this.tg = tg;
    }

    public void AssignFunctions(FunctionMathCalculator functionMathCalculator)
    {
        fmc = functionMathCalculator;
    }

    /// <summary>
    /// generates random hights on given area
    /// </summary>
    /// <param name="botLeft">global coordinate</param>
    /// <param name="topRight">global coordinate</param>
    public void GenerateRandomTerrain(Vector3 botLeft, Vector3 topRight)
    {
        //determine which corner is defined and start from there
        Vector3 startCorner = botLeft;
        Vector3 endCorner = topRight;
        int stepX = 1;
        int stepZ = 1;

        if (tg.globalTerrain.globalTerrainC.IsDefined((int)botLeft.x, (int)botLeft.z)) //BOTLEFT
        {
            //Debug.Log("BOTLEFT");
            startCorner = botLeft;
            endCorner = topRight;
            stepX = 1;
            stepZ = 1;
        }
        else if (tg.globalTerrain.globalTerrainC.IsDefined((int)botLeft.x, (int)topRight.z)) //TOPLEFT
        {
            //Debug.Log("TOPLEFT");

            startCorner = new Vector3(botLeft.x, 0, topRight.z);
            endCorner = new Vector3(topRight.x, 0, botLeft.z);
            stepX = 1;
            stepZ = -1;
        }
        else if (tg.globalTerrain.globalTerrainC.IsDefined((int)topRight.x, (int)topRight.z)) //TOPRIGHT
        {
            //Debug.Log("TOPRIGHT");

            startCorner = topRight;
            endCorner = botLeft;
            stepX = -1;
            stepZ = -1;
        }
        else if (tg.globalTerrain.globalTerrainC.IsDefined((int)topRight.x, (int)botLeft.z)) //BOTRIGHT
        {
            //Debug.Log("BOTRIGHT");

            startCorner = new Vector3(topRight.x, 0, botLeft.z);
            endCorner = new Vector3(botLeft.x, 0, topRight.z);
            stepX = -1;
            stepZ = 1;
        }

        //Debug.Log("from: " + startCorner + " to: " + endCorner);

        //for (int x = (int)startCorner.x; x < (int)endCorner.x; x += stepX)
        //{
        //    for (int z = (int)startCorner.z; z < (int)endCorner.z; z += stepZ)  
        for (int x = (int)startCorner.x; fmc.LesserEqual(-stepX, x, (int)endCorner.x); x += stepX)
        {
            for (int z = (int)startCorner.z; fmc.LesserEqual(-stepZ, z, (int)endCorner.z); z += stepZ)
            {
                //do not overwrite already set values
                if (!tg.globalTerrain.globalTerrainC.IsDefined(x, z)){
                    float neighb_average = tg.globalTerrain.GetNeighbourAverage(x, z);
                    //float neighb_highest = tg.globalTerrain.GetHighestNeighbour(x, z);
                    if (neighb_average == 666)
                    {
                        Debug.Log(x + "," + z + ": unset");
                        neighb_average = 0;
                    }
                    float rand_height = Random.Range(neighb_average - 0.1f, neighb_average + 0.1f);
                    tg.globalTerrain.SetHeight(x, z, rand_height, false);
                    if (x < -98 && z < -98)
                    {
                        //Debug.Log(x + "," + z + " => " + tg.globalTerrain.GetHeight(x,z));

                    }
                }
            }
        }

        FixBorder(botLeft, topRight);
    }

    /// <summary>
    /// fucntion for setting unset values on borders of defined region
    /// </summary>
    /// <param name="botLeft"></param>
    /// <param name="topRight"></param>
    public void FixBorder(Vector3 botLeft, Vector3 topRight)
    {
        for (int x = (int)botLeft.x; x < (int)topRight.x; x++)
        {
            if (!tg.globalTerrain.globalTerrainC.IsDefined(x, (int)botLeft.z))
            {
                tg.globalTerrain.SetHeight(x, (int)botLeft.z, tg.globalTerrain.GetNeighbourAverage(x, (int)botLeft.z), false);
            }
            if (!tg.globalTerrain.globalTerrainC.IsDefined(x, (int)topRight.z))
            {
                tg.globalTerrain.SetHeight(x, (int)topRight.z, tg.globalTerrain.GetNeighbourAverage(x, (int)topRight.z), false);
            }
        }

        for (int z = (int)botLeft.z; z < (int)topRight.z; z++)
        {
            if (!tg.globalTerrain.globalTerrainC.IsDefined((int)botLeft.x, z))
            {
                tg.globalTerrain.SetHeight((int)botLeft.x, z, tg.globalTerrain.GetNeighbourAverage((int)botLeft.x, z), false);
            }
            if (!tg.globalTerrain.globalTerrainC.IsDefined((int)topRight.x, z))
            {
                tg.globalTerrain.SetHeight((int)topRight.x, z, tg.globalTerrain.GetNeighbourAverage((int)topRight.x, z), false);
            }
        }
    }

}
