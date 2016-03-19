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
    public List<RiverInfo> rivers;

    public GUIRiver riverGui;

    public RiverGenerator(LocalTerrain localTerrain)
    {
        lt = localTerrain;

        globalRiverC = new GlobalCoordinates(100);
        localCoordinates = lt.localTerrainC;

        currentRiver = new RiverInfo(this);

        rivers = new List<RiverInfo>();
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


    public void GenerateDefaultRiver()
    {

        int w = 20;

        List<Vertex> tempList = new List<Vertex>();
        tempList.Add(new Vertex(0, 0));
        tempList.Add(new Vertex(w, 0));
        tempList.Add(new Vertex(2*w, 0));
        tempList.Add(new Vertex(3*w, 0));

        RiverInfo defaultRiver = new RiverInfo(this);
        defaultRiver.riverPath = tempList;
        frd.DigRiver(defaultRiver);


        List<Vertex> tempList2 = new List<Vertex>();

        tempList2.Add(new Vertex(3*w, 0));
        tempList2.Add(new Vertex(4*w, 0));
        tempList2.Add(new Vertex(5*w, 0));

        RiverInfo defaultRiver2 = new RiverInfo(this);
        defaultRiver2.riverPath = tempList2;
        defaultRiver.ConnectWith(defaultRiver2);
        frd.DigRiver(defaultRiver);

        

        lt.tg.build();

        defaultRiver.DrawRiver();
    }

    public void DeleteRiverAt(int i)
    {
        rivers.RemoveAt(i);
        riverGui.riverFlags.RemoveAt(i);
    }

    public void GenerateNewRiver()
    {

        Vertex start = ftm.GetLowestRegionCenter(20, 50);//LOCAL!
        Vertex globalStart = lt.GetGlobalCoordinate((int)start.x, (int)start.z);

        globalStart.height = start.height;

        RiverInfo river = frp.GetRiverPathFrom(globalStart, new List<Direction>());
        //Debug.Log(river);
           
        globalStart.side = fmc.GetOppositeDirection(river.GetLastVertex().side);
        Area restrictedArea = fmc.CalculateRestrictedArea(globalStart);
        globalStart.side = Direction.none;

        List<Direction> reachedSides = new List<Direction>();
        reachedSides.Add(river.GetLastVertex().side);

        // 2)find second path
        //Debug.Log(restrictedArea);
        RiverInfo river2 = frp.GetRiverPathFrom(globalStart, reachedSides, restrictedArea);
            
        // connect them
        river.ConnectWith(river2);
        river.DrawRiver();

        //currentRiver = river;

        //Debug.Log(currentRiver);
        //frd.DistortPath(currentRiver.riverPath, 10);
        //frd.DigRiver(currentRiver);

        rivers.Add(river);
        frd.DigRiver(rivers[rivers.Count - 1]);

        riverGui.riverFlags.Add(true);

        Debug.Log(rivers.Count);
        Debug.Log(riverGui.riverFlags.Count);
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
        //if (ftm.IsInVisibleterrain(startPoint) && !ftm.IsOnBorder(startPoint))
        if (ftm.IsInDefinedTerrain(startPoint) && !ftm.IsOnBorder(startPoint))
        {
            //Debug.Log("connection river from start: " + startPoint);

            Area restrictArea = fmc.CalculateRestrictedArea(startPoint);

            List<Direction> reachedSides = new List<Direction>();
            reachedSides.Add(fmc.GetOppositeDirection(startPoint.side));

            RiverInfo startRiver =
                frp.GetRiverPathFrom(startPoint, reachedSides, restrictArea);
            frd.DistortPath(startRiver.riverPath, 10);
            currentRiver.ConnectWith(startRiver);

            startPoint.side = Direction.none;
            //Debug.Log("startRiver:" + startRiver);
            
            //frd.DigRiver(startRiver.riverPath);
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

        //if (ftm.IsInVisibleterrain(endPoint) && !ftm.IsOnBorder(endPoint))
        if (ftm.IsInDefinedTerrain(endPoint) && !ftm.IsOnBorder(endPoint))
        {
            //Debug.Log("connection river from end: " + endPoint);
            Area restrictArea = fmc.CalculateRestrictedArea(endPoint);

            List<Direction> reachedSides = new List<Direction>();
            reachedSides.Add(fmc.GetOppositeDirection(endPoint.side));

            RiverInfo endRiver =
                frp.GetRiverPathFrom(endPoint, reachedSides, restrictArea);
            frd.DistortPath(endRiver.riverPath, 10);
            currentRiver.ConnectWith(endRiver);

            endPoint.side = Direction.none;
            //Debug.Log("endRiver:" + endRiver);
            
            //frd.DigRiver(endRiver.riverPath);
        }
        else
        {
            //Debug.Log(endPoint + " OUT [END]");
            //Debug.Log(lt.localTerrainC.botLeft + ", " + lt.localTerrainC.topRight);

        }

        Debug.Log("connected river:" + currentRiver);

        frd.DigRiver(currentRiver);
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
