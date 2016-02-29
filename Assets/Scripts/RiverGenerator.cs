using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiverGenerator  {

    public GlobalCoordinates globalRiverC;
    public LocalCoordinates localCoordinates;

    public LocalTerrain lt;

    public FunctionTerrainManager ftm;
    public FunctionRiverPlanner frp;
    public FunctionDebugger fd;
    public FunctionMathCalculator fmc;
    public FunctionRiverDigger frd;

    public RiverInfo currentRiver;

    public RiverGenerator(LocalTerrain localTerrain)
    {
        lt = localTerrain;

        globalRiverC = new GlobalCoordinates(100);
        localCoordinates = lt.localCoordinates;
        //localRiverC = new LocalCoordinates(globalRiverC, new Vector3(0, 0, 0), 200, 200);
    }

    public void AssignFunctions(FunctionTerrainManager functionTerrainManager, 
        FunctionRiverPlanner functionRiverPlanner, FunctionDebugger functionDebugger, 
        FunctionMathCalculator functionMathCalculator, FunctionRiverDigger functionRiverDigger)
    {
        ftm = functionTerrainManager;
        frp = functionRiverPlanner;
        fd = functionDebugger;
        fmc = functionMathCalculator;
        frd = functionRiverDigger;
    }

    public void GenerateNewRiver()
    {


        //List<Vertex> tempList = new List<Vertex>();
        //tempList.Add(new Vertex(30, 30));
        //tempList.Add(new Vertex(60, 30));
        //tempList.Add(new Vertex(20, 70));
        //tempList.Add(new Vertex(30, 80));
        //tempList.Add(new Vertex(40, 90));
        //tempList.Add(new Vertex(20, 150));
        //tempList.Add(new Vertex(35, 140));
        //tempList.Add(new Vertex(40, 135));
        //tempList.Add(new Vertex(55, 120));

        //ftm.ClearTerrain();

        //frd.DigRiver(tempList, 5, 0.4f);

        //ACTUAL
        //frp.FloodFromLowestPoint();

        Vertex start = ftm.GetLowestRegionCenter(20, 50);//LOCAL!
        Vertex globalStart = lt.GetGlobalCoordinate((int)start.x, (int)start.z);
        globalStart.height = start.height;
        Debug.Log("starting from " + start + " = " + globalStart);

        RiverInfo river = frp.GetRiverPathFrom(globalStart, new List<Direction>());
            //0,lt.terrainWidth, 0, lt.terrainHeight);
        Debug.Log(river);

        //now river has reached 1 side. 
        //Find Path on other part of the map which reaches different side and connect them

        // 1)determine which part of map we want to seacrh on
        int x_min = (int)lt.localCoordinates.botLeft.x;
        int z_min = (int)lt.localCoordinates.botLeft.z;
        int x_max = (int)lt.localCoordinates.topRight.x;
        int z_max = (int)lt.localCoordinates.topRight.z;

        fmc.DetermineBoundaries(globalStart,river,
            ref x_min, ref z_min, ref x_max, ref z_max);
        river.UpdateReachedSides();

        // 2)find second path
        RiverInfo river2 = frp.GetRiverPathFrom(globalStart, river.reachedSides,
            x_min, z_min, x_max, z_max);
        Debug.Log(river2);

        // connect them
        river.ConnectWith(river2);
        river.DrawRiver();

        currentRiver = river;

        Debug.Log(currentRiver);
        
        //frd.DigRiver(currentRiver.riverPath);


    }

    /// <summary>
    /// generates rivers starting from curent river's starting and ending point
    /// operation is processed only if points are in visible terrain and not on border
    /// => meaning that terrain ahs been moved and river isn't complete
    /// </summary>
    public void GenerateConnectingRiver()
    {
        Debug.Log("currentRiver:"+currentRiver);
        Vector3 startPoint = currentRiver.riverPath[0];
        if (ftm.IsInVisibleterrain(startPoint) && !ftm.IsOnBorder(startPoint))
        {
            Debug.Log("connection river from start: " + startPoint);

            RiverInfo startRiver =
                frp.GetRiverPathFrom(startPoint, new List<Direction>());
            currentRiver.ConnectWith(startRiver);

            Debug.Log("startRiver:" + startRiver);
        }
        else
        {
            Debug.Log(startPoint + " OUT");
            Debug.Log(lt.localCoordinates.botLeft + ", " + lt.localCoordinates.topRight);
        }

        Vector3 endPoint = currentRiver.riverPath[currentRiver.riverPath.Count-1];
        if (ftm.IsInVisibleterrain(endPoint) && !ftm.IsOnBorder(endPoint))
        {
            Debug.Log("connection river from end: " + endPoint);

            RiverInfo endRiver =
                frp.GetRiverPathFrom(endPoint, new List<Direction>());
            currentRiver.ConnectWith(endRiver);

            Debug.Log("endRiver:" + endRiver);
        }
        else
        {
            Debug.Log(endPoint + " OUT");
            Debug.Log(lt.localCoordinates.botLeft + ", " + lt.localCoordinates.topRight);

        }

    }

    /// <summary>
    /// returns river value on given local coordiantes (0 if not defined)
    /// </summary>
    public float GetLocalValue(int x, int z)
    {
        float value = localCoordinates.GetLocalValue(x, z, globalRiverC);
        if (value != 666)
            return value;
        else
            return 0;
    }

    public Vector3 GetBotLeft()
    {
        return lt.localCoordinates.botLeft;
    }
    public Vector3 GetTopRight()
    {
        return lt.localCoordinates.topRight;
    }


    public void ResetRivers()
    {
        globalRiverC.ResetQuadrants();
    }
}
