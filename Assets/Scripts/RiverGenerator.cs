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
        localCoordinates = lt.localTerrainC;
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
        //Debug.Log("starting from " + start + " = " + globalStart);

        RiverInfo river = frp.GetRiverPathFrom(globalStart, new List<Direction>());
            //0,lt.terrainWidth, 0, lt.terrainHeight);
        //Debug.Log(river);

        //now river has reached 1 side. 
        //Find Path on other part of the map which reaches different side and connect them

        // 1)determine which part of map we want to seacrh on
        //int x_min = (int)lt.localTerrainC.botLeft.x;
        //int z_min = (int)lt.localTerrainC.botLeft.z;
        //int x_max = (int)lt.localTerrainC.topRight.x;
        //int z_max = (int)lt.localTerrainC.topRight.z;

        //fmc.DetermineBoundaries(globalStart,river,
        //    ref x_min, ref z_min, ref x_max, ref z_max);

        //river.UpdateReachedSides();

        globalStart.side = fmc.GetOppositeDirection(river.GetLastVertex().side);
        Area restrictedArea = fmc.CalculateRestrictedArea(globalStart);
        globalStart.side = Direction.none;

        List<Direction> reachedSides = new List<Direction>();
        reachedSides.Add(river.GetLastVertex().side);

        // 2)find second path
        RiverInfo river2 = frp.GetRiverPathFrom(globalStart, reachedSides, restrictedArea);
            //x_min, z_min, x_max, z_max);
        //Debug.Log(river2);

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
    /// => meaning that terrain has been moved and river isn't complete
    /// </summary>
    public void GenerateConnectingRiver()
    {
        //Debug.Log("currentRiver:"+currentRiver);
        Vertex startPoint = currentRiver.riverPath[0];
        if (ftm.IsInVisibleterrain(startPoint) && !ftm.IsOnBorder(startPoint))
        {
            //Debug.Log("connection river from start: " + startPoint);

            Area restrictArea = fmc.CalculateRestrictedArea(startPoint);

            List<Direction> reachedSides = new List<Direction>();
            reachedSides.Add(fmc.GetOppositeDirection(startPoint.side));

            RiverInfo startRiver =
                frp.GetRiverPathFrom(startPoint, reachedSides, restrictArea);
            currentRiver.ConnectWith(startRiver);

            startPoint.side = Direction.none;
            //Debug.Log("startRiver:" + startRiver);
        }
        else
        {
            //Debug.Log(startPoint + " OUT [START]");
            //Debug.Log(lt.localTerrainC.botLeft + ", " + lt.localTerrainC.topRight);
        }

        Vertex endPoint = currentRiver.GetLastVertex();
        //direction of river might have been changed
        if(!ftm.IsInVisibleterrain(endPoint) && ftm.IsOnBorder(endPoint))
        {
            endPoint = currentRiver.riverPath[0];
        }

        if (ftm.IsInVisibleterrain(endPoint) && !ftm.IsOnBorder(endPoint))
        {
            //Debug.Log("connection river from end: " + endPoint);
            Area restrictArea = fmc.CalculateRestrictedArea(endPoint);

            List<Direction> reachedSides = new List<Direction>();
            reachedSides.Add(fmc.GetOppositeDirection(endPoint.side));

            RiverInfo endRiver =
                frp.GetRiverPathFrom(endPoint, reachedSides, restrictArea);
            currentRiver.ConnectWith(endRiver);

            endPoint.side = Direction.none;
            //Debug.Log("endRiver:" + endRiver);
        }
        else
        {
            //Debug.Log(endPoint + " OUT [END]");
            //Debug.Log(lt.localTerrainC.botLeft + ", " + lt.localTerrainC.topRight);

        }

        Debug.Log("connected river:" + currentRiver);

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
        return lt.localTerrainC.botLeft;
    }
    public Vector3 GetTopRight()
    {
        return lt.localTerrainC.topRight;
    }


    public void ResetRivers()
    {
        globalRiverC.ResetQuadrants();
    }
}
