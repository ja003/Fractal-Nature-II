using UnityEngine;
using System.Collections;

public class DiamondSquare2 {

    private TerrainGenerator tg;
    
    int patchWidth;
    int patchHeight;
    

    public DiamondSquare2(TerrainGenerator tg)
    {
        this.tg = tg;
    }

    public void SetupDiamondSquare(int patchWidth, int patchHeight)
    {
        this.patchWidth = patchWidth;
        this.patchHeight = patchHeight;
    }

    



    
    public void initDiamondSquare(Vector3 center, float scale)
    {
        //Size of step at iteration 0
        //int stepSize = patchSize;
        int stepX = patchWidth;
        int stepZ = patchHeight;

        //Random numbers limit (-value, +value)
        float rand_value1 = 0.50f;
        float rand_value2 = 0.40f;

        //make rand value dependent on distance from center of local terrain
        //rand_value1 = ((Mathf.Sqrt(tg.terrainWidth* tg.terrainWidth/4 + tg.terrainHeight* tg.terrainHeight/4)) -
                   // Vector3.Distance(tg.localTerrain.GetGlobalCoordinate((int)center.x, (int)center.z), tg.localTerrain.center)) / 100;
        
        //rand_value1 = 1;
        //rand_value2 = rand_value1 - 0.1f;

        float offset;

        int start_x = (int)center.x;
        int start_z = (int)center.z;


        //Start the main displacement loop
        //while (stepSize > 1)
        while (stepX > 1 && stepZ > 1)
        {
            //Debug.Log("step:" + stepSize);            //Halving the resolution each step


            //int half_step = stepSize / 2;
            int half_stepX = stepX / 2;
            int half_stepZ = stepZ / 2;
            /*
            for (int x = start_x + half_stepX; x < start_x + patchWidth + half_stepX; x = x + stepX)
            {
                for (int y = start_z + half_stepZ; y < start_z + patchHeight + half_stepZ; y = y + stepZ)
                {
                    stepSquare(x, y, rand_value1, scale, half_stepX, half_stepZ);
                }
            }

            //Diamond step
            for (int x = start_x + half_stepX; x < start_x + patchWidth + half_stepX; x = x + stepX)
            {
                for (int y = start_z + half_stepZ; y < start_z + patchHeight + half_stepZ; y = y + stepZ)
                {
                    stepDiamond(x, y, rand_value2, scale, half_stepX, half_stepZ);
                }
            }*/
            
            for (int x = (int)center.x - patchWidth/2 + half_stepX; x < (int)center.x + patchWidth/2 + half_stepX; x+= stepX)
            {
                for (int z = (int)center.z - patchWidth / 2 + half_stepZ; z < (int)center.z + patchWidth / 2 + half_stepZ; z += stepZ)
                {
                    stepSquare(x, z, rand_value1, scale, half_stepX, half_stepZ); 
                }
            }
            
            for (int x = (int)center.x - patchWidth / 2 + half_stepX; x < (int)center.x + patchWidth / 2 + half_stepX; x += stepX)
            {
                for (int z = (int)center.z - patchWidth / 2 + half_stepZ; z < (int)center.z + patchWidth / 2 + half_stepZ; z += stepZ)
                {
                    stepDiamond(x, z, rand_value2, scale, half_stepX, half_stepZ);
                }
            }

            //Halving the resolution and the roughness parameter
            stepX /= 2;
            stepZ /= 2;
            scale /= 2;

        }

        //Copy margin values to neighbouring vertices belonging to nearby pathes 
        //to avoid unwanted artifacts/seams between patches

        //west


        bool overwrite = true;
        //if (start_x != 0)
        for (int i = start_z - patchHeight / 2; i < start_z + patchHeight / 2; i++)
        {
            //Debug.Log(tg.GetVertexHeight(start_x + patchWidth / 2+1, i));
            tg.SetVertex(start_x + patchWidth / 2 + 1, i, tg.GetVertexHeight(start_x + patchWidth / 2, i), overwrite);
            //Debug.Log(tg.GetVertexHeight(start_x + patchWidth / 2 + 1, i));

        }
                //tg.SetVertex(start_x - 1, i, tg.SetVertex(start_x, i].y;
        //south
        //if (start_z != 0)
            for (int i = start_x- patchWidth/2; i < start_x + patchWidth/2; i++)
                tg.SetVertex(i, start_z - patchHeight/2-1, tg.GetVertexHeight(i, start_z - patchHeight / 2), overwrite);
                //tg.SetVertex(i, start_z - 1, tg.SetVertex(i, start_z].y;
        //east
        //if (start_x + patchSize != terrainSize - 1)
            for (int i = start_z- patchHeight/2; i < start_z + patchHeight/2; i++)
                tg.SetVertex(start_x - patchWidth/2 - 1, i, tg.GetVertexHeight(start_x - patchWidth/2, i), overwrite);
        //tg.SetVertex(start_x + patchSize + 1, i, tg.SetVertex(start_x + patchSize, i].y;
        //north
        //if (start_z + patchHeight != terrainSize - 1)
            for (int i = start_x- patchWidth/2; i < start_x + patchWidth/2; i++)
                tg.SetVertex(i, start_z + patchHeight/2 + 1, tg.GetVertexHeight(i, start_z + patchHeight/2), overwrite);
        //tg.SetVertex(i, start_z + patchSize + 1, tg.SetVertex(i, start_z + patchSize].y;
        
    }
    int ss = 0;
    private void stepSquare(int x, int z, float rand_value, float scale, int half_stepX, int half_stepZ)
    {
        //Debug.Log("stepSquare"); 
        ss++;
        //Get corner values
        float a = tg.GetVertexHeight(x - half_stepX, z - half_stepZ); //tg.SetVertex(x - half_step, y - half_step].y;
        float b = tg.GetVertexHeight(x + half_stepX, z - half_stepZ); //tg.SetVertex(x + half_step, y - half_step].y;
        float c = tg.GetVertexHeight(x - half_stepX, z + half_stepZ); //tg.SetVertex(x - half_step, y + half_step].y;
        float d = tg.GetVertexHeight(x + half_stepX, z + half_stepZ); //tg.SetVertex(x + half_step, y + half_step].y;
        
        tg.SetVertex(x, z, (a + b + c + d) / 4.0f + UnityEngine.Random.Range(-rand_value / 10, rand_value) * scale, false);//!!!!
    }
    int sd = 0;
    private void stepDiamond(int x, int z, float rand_value, float scale, int half_stepX, int half_stepZ) //, Vector3 start)???
    {
        sd++;
        //Get side points (diamond-shaped)
        float a = tg.GetVertexHeight(x - half_stepX, z - half_stepZ); //tg.SetVertex(x - half_step, y - half_step].y;
        float b = tg.GetVertexHeight(x + half_stepX, z - half_stepZ); //tg.SetVertex(x + half_step, y - half_step].y;
        float c = tg.GetVertexHeight(x - half_stepX, z + half_stepZ); //tg.SetVertex(x - half_step, y + half_step].y;
        float d = tg.GetVertexHeight(x + half_stepX, z + half_stepZ); //tg.SetVertex(x + half_step, y + half_step].y;

        float offset;
        float min_value = -rand_value / 10;
        float max_value = rand_value;
        bool overwrite = false;

        offset = (a + c) / 2.0f + UnityEngine.Random.Range(min_value, max_value) * scale;
        tg.SetVertex(x - half_stepX, z, offset, overwrite);
        offset = (c + d) / 2.0f + UnityEngine.Random.Range(min_value, max_value) * scale;
        tg.SetVertex(x, z + half_stepZ, offset, overwrite);
        offset = (b + d) / 2.0f + UnityEngine.Random.Range(min_value, max_value) * scale;
        tg.SetVertex(x + half_stepX, z, offset, overwrite);
        offset = (a + b) / 2.0f + UnityEngine.Random.Range(min_value, max_value) * scale;
        tg.SetVertex(x, z - half_stepZ, offset, overwrite);
    }


}
