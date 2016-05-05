using UnityEngine;
using System.Collections;
using System;

public class LocalTerrain {

    //public float[,] visibleTerrain; //practicaly not neccessary
    

    private int stepSize;
    public int terrainWidth;
    public int terrainHeight;
    
    public LocalCoordinates localTerrainC;
    public GlobalCoordinates globalTerrainC;

    public TerrainGenerator tg;
    public FilterGenerator fg;
    public RiverGenerator rg;
    public ErosionGenerator eg;

    public GlobalTerrain gt;
    public LayerManager lm;

    public LocalTerrain(int terrainWidth, int terrainHeight, int stepSize, GlobalTerrain globalTerrain, LayerManager layerManager)
    {
        gt = globalTerrain;
        gt.lt = this;
        lm = layerManager;

        localTerrainC = new LocalCoordinates(new Vector3(0, 0, 0), terrainWidth, terrainHeight);

        //visibleTerrain = new float[terrainHeight, terrainWidth];
        this.terrainHeight = terrainHeight;
        this.terrainWidth = terrainWidth;

        this.stepSize = stepSize;
    }

    public void AssignFunctions(GlobalCoordinates globalTerrainC, TerrainGenerator terrainGenerator, 
        FilterGenerator filterGenerator, RiverGenerator riverGenerator, ErosionGenerator erosionGenerator)
    {
        this.globalTerrainC = globalTerrainC;

        tg = terrainGenerator;
        fg = filterGenerator;
        rg = riverGenerator;
        eg = erosionGenerator;

        lm.AssignFunctions(tg, fg, rg, eg);
        
    }

    /// <summary>
    /// updates values to visible terrain based on camera position
    /// </summary>
    /// <param name="cameraPosition"></param>
    public void UpdateVisibleTerrain(Vector3 cameraPosition, bool defaultTerrain)
    {
        MoveVisibleTerrain(cameraPosition, false);

        if (!fg.ftm.IsDefinedTerrainArea(GetVisibleArea()))
        {
            //Debug.Log(GetVisibleArea() + " not defined!");
            tg.GenerateTerrainOn(localTerrainC.center, defaultTerrain); //localCoordinates.botLeft, localCoordinates.topRight);

            //connect river (if it has been generated)
            if (!defaultTerrain)
            {
                foreach (RiverInfo river in rg.rivers)
                {
                    //Debug.Log("connection river");
                    rg.GenerateConnectingRiver(river);
                }
            }
            /*
            if (rg.currentRiver != null && rg.currentRiver.riverPath.Count > 0)
            {

                Debug.Log("connection river");
                rg.GenerateConnectingRiver();
            }*/
        }

        //generate filter if selected
        if (tg.filterAverageLayer)
        {
            tg.filterGenerator.af.GenerateAverageFilterInRegion(GetVisibleArea());
        }

        if (tg.filterMedianLayer)
        {
            tg.filterGenerator.mdf.GenerateMedianFilterInRegion(GetVisibleArea());
        }

        if (tg.filterSpikeLayer)
        {
            tg.filterGenerator.sf.GenerateSpikeFilterInRegion(GetVisibleArea(), tg.filterGenerator.sf.lastEpsilon);
        }

        if (tg.filterGaussianLayer)
        {
            tg.filterGenerator.gf.ApplyGaussianBlurOnRegion(tg.filterGenerator.gf.lastBlurFactor, 
                tg.filterGenerator.gf.lastKernelSize, GetVisibleArea());
        }

        //fg.mf.PerserveMountainsInRegion(localTerrainC.botLeft, localTerrainC.topRight, 4, 60, 10);





        tg.build();

        //draw river (if it has been generated)
        foreach (RiverInfo river in rg.rivers)
        {
            river.DrawRiver();
        }
    }

    /// <summary>
    /// moves local center
    /// </summary>
    public void MoveVisibleTerrain(Vector3 newCenter, bool build)
    {
        UpdateLocalCoordinates(newCenter,
            new Vector3(newCenter.x - terrainWidth / 2, 0, newCenter.z - terrainHeight / 2),
            new Vector3(newCenter.x + terrainWidth / 2, 0, newCenter.z + terrainHeight / 2));
        
        if(build)
            tg.MoveVisibleTerrain(localTerrainC.center);//only for moving terrain (not generating new)
    }

    /// <summary>
    /// change visible area parameters to visibleArea x visibleArea
    /// </summary>
    public void UpdateVisibleArea(int visibleArea)
    {
        terrainWidth = visibleArea;
        terrainHeight = visibleArea;

        localTerrainC.UpdateSize(visibleArea, visibleArea);
        //localTerrainC.terrainHeight = visibleArea;
    }

    /// <summary>
    /// updates coordinates with same parameters
    /// </summary>
    public void UpdateLocalCoordinates()
    {
        UpdateLocalCoordinates(localTerrainC.center, localTerrainC.botLeft, localTerrainC.topRight);
    }


    /// <summary>
    /// updates center and botLeft+topRight of local coordinates
    /// params should always come in rounded values!
    /// </summary>
    public void UpdateLocalCoordinates(Vector3 center, Vector3 botLeft, Vector3 topRight)
    {
        localTerrainC.center = center;
        localTerrainC.botLeft = botLeft;
        localTerrainC.topRight = topRight;

        //tg.filterGenerator.UpdateLocalCoordinates(center, botLeft, topRight);

    }

    public Vertex GetGlobalCoordinate(int x, int z)
    {
        return new Vector3(x + (int)localTerrainC.center.x - terrainWidth / 2, 0, z + (int)localTerrainC.center.z - terrainHeight / 2);
    }

    /// <summary>
    /// returns height from global terrain
    /// </summary>
    public float GetGlobalHeight(int x, int z)
    {
        //Debug.Log("getting " + x + "," + z);
        //Debug.Log("= " + (x + (int)center.x - terrainWidth) + "," + (z + (int)center.z - terrainHeight / 2));
        return globalTerrainC.GetValue(x, z);
    }

    /// <summary>
    /// returns height from global terrain
    /// maps given local coordinates on global
    /// 666 if not defined
    /// </summary>
    public float GetLocalHeight(int x, int z)
    {
        return localTerrainC.GetLocalValue(x,z, globalTerrainC);
    }

    /// <summary>
    /// returns height from global terrain
    /// returns 'undefinedValue' if height is not defined
    /// maps given local coordinates on global
    /// </summary>
    public float GetLocalHeight(int x, int z, float undefinedValue)
    {
        float height = localTerrainC.GetLocalValue(x, z, globalTerrainC);
        if (height != 666)
            return height;
        else
            return undefinedValue;
    }

    /// <summary>
    /// sets height to global terrain
    /// maps given local coordinates on global
    /// </summary>
    public void SetLocalHeight(int x, int z, float height, bool overwrite)
    {
        localTerrainC.SetLocalValue(x ,z, height, overwrite, globalTerrainC);
    }

    /// <summary>
    /// returns average of neighbour vertices heights
    /// 666 if neighbouthood is not derfined
    /// </summary>
    public float GetNeighbourHeight(int _x, int _z)
    {
        float heightAverage = 0;
        int neighboursCount = 0;
        for (int x = _x - 1; x <= _x + 1; x++)
        {
            for (int z = _z - 1; z <= _z + 1; z++)
            {
                if (GetLocalHeight(x, z) != 666)
                {
                    heightAverage += GetLocalHeight(x, z);
                    neighboursCount++;
                    if (_x == 32 && _z == 32)
                    {
                        //Debug.Log(GetGlobalHeight(x, z));
                        //Debug.Log(heightAverage);
                        //Debug.Log(neighboursCount);
                    }
                }
            }
        }
        if (neighboursCount == 0)
        {
            return 666;
        }
        else
        {
            //Debug.Log(heightAverage / neighboursCount);
            return heightAverage / neighboursCount;
        }

    }

    public bool NeighbourhoodDefined(int _x,int _z, int neighbourhoodSize){
        for(int x = _x - neighbourhoodSize / 2; x < _x + neighbourhoodSize / 2; x++)
        {
            for (int z = _z - neighbourhoodSize / 2; z < _z + neighbourhoodSize / 2; z++)
            {
                if (GetLocalHeight(x, z) == 666)
                    return false;
            }
        }
        return true;
    }

    public void UpdateSize(int terrainWidth, int terrainHeight)
    {
        this.terrainWidth = terrainWidth;
        this.terrainHeight = terrainHeight;

        localTerrainC.UpdateSize(terrainWidth, terrainHeight);
    }

    public Vector3 GetBotLeft()
    {
        return localTerrainC.botLeft;
    }

    public Vector3 GetTopRight()
    {
        return localTerrainC.topRight;
    }

    public Area GetVisibleArea()
    {
        return new Area(GetBotLeft(), GetTopRight());
    }
    /*
    public void PrintValues(int from, int to)
    {
        for(int x = from; x < to; x++)
        {
            for (int z = from; z < to; z++)
            {
                Debug.Log(x + "," + z + ": " + visibleTerrain[x, z] + "/" + GetLocalHeight(x, z));

            }
        }
    }*/
    /*
    public void SetHeight(int x, int z, float height, bool overwrite)
    {
        if (!overwrite && GetLocalHeight(x, z) != 666)
            return;

        visibleTerrain[x, z] = height;
    }*/
}
