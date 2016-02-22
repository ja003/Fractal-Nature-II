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

    public void GenerateRiver()
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

        Vertex start = ftm.GetLowestRegionCenter(20, 50);
        Debug.Log("starting from " + start);

        RiverInfo river = frp.GetRiverPathFrom(start, new List<Direction>(),
            0,lt.terrainWidth, 0, lt.terrainHeight);
        Debug.Log(river);

        //now river has reached 1 side. 
        //Find Path on other part of the map which reaches different side and connect them

        // 1)determine which part of map we want to seacrh on
        int x_min = 0;
        int x_max = lt.terrainWidth;
        int z_min = 0;
        int z_max = lt.terrainHeight;

        fmc.DetermineBoundaries(start,
            river.reachTop, river.reachRight, river.reachBot, river.reachLeft,
            ref x_min, ref x_max, ref z_min, ref z_max);
        river.UpdateReachedSides();

        // 2)find second path
        RiverInfo river2 = frp.GetRiverPathFrom(start, river.reachedSides,
            x_min, x_max, z_min, z_max);


        Debug.Log(river2);
        // connect them
        river.ConnectWith(river2);
        river.DrawRiver();

        Debug.Log(river);

        currentRiver = river;

        frd.DigRiver(currentRiver.riverPath);


    }
}
