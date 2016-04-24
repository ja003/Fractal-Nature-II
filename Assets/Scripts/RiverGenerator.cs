using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiverGenerator  {

    //public GlobalCoordinates globalRiverC;
    public LocalCoordinates localCoordinates;

    public LocalTerrain lt;

    public FunctionTerrainManager ftm;
    public FunctionRiverPlanner frp;
    public FunctionDebugger fd;
    public FunctionMathCalculator fmc;
    public FunctionRiverDigger frd;

    //public RiverInfo currentRiver;
    public List<RiverInfo> rivers;

    public GUIRiver riverGui;

    public RiverGenerator(LocalTerrain localTerrain)
    {
        lt = localTerrain;

        //globalRiverC = new GlobalCoordinates(100);
        localCoordinates = lt.localTerrainC;

        //currentRiver = new RiverInfo(this);

        rivers = new List<RiverInfo>();
        //localRiverC = new LocalCoordinates(globalRiverC, new Vector3(0, 0, 0), 200, 200);
    }

    public void AssignFunctions(FunctionTerrainManager functionTerrainManager, 
        FunctionRiverPlanner functionRiverPlanner, FunctionDebugger functionDebugger, 
        FunctionMathCalculator functionMathCalculator, FunctionRiverDigger functionRiverDigger,
        GUIRiver guiRiver)
    {
        ftm = functionTerrainManager;
        frp = functionRiverPlanner;
        fd = functionDebugger;
        fmc = functionMathCalculator;
        frd = functionRiverDigger;
        riverGui = guiRiver;
    }


    public void GenerateDefaultRiver()
    {

        int w = 20;
        
        List<Vertex> tempList = new List<Vertex>();
        tempList.Add(new Vertex(0, -lt.terrainHeight/2 + 5));
        tempList.Add(new Vertex(w, -w));
        tempList.Add(new Vertex(0, 0));
        tempList.Add(new Vertex(w, w));
        tempList.Add(new Vertex(0, lt.terrainHeight / 2 - 5));

        RiverInfo river = new RiverInfo(this);
        river.riverPath = tempList;
        

        frd.DistortPath(river.riverPath, river.gridStep / 3, river.gridStep);

        river.width = 15;
        river.areaEffect = 2;
        river.depth = 0.15f;

        rivers.Add(river);
        frd.DigRiver(rivers[rivers.Count - 1]);

        riverGui.riverFlags.Add(true);
    }

    public void DeleteRiverAt(int i)
    {
        rivers.RemoveAt(i);
        riverGui.riverFlags.RemoveAt(i);
    }

    public void GenerateNewRiver2()
    {

    }


    public void GenerateNewRiver(float width, float areaEffect, float depth, int gridStep)
    {

        Vertex start = ftm.GetLowestRegionCenter(20, 20);//LOCAL!
        Vertex globalStart = lt.GetGlobalCoordinate((int)start.x, (int)start.z);
        Debug.Log(globalStart);

        globalStart.height = start.height;

        RiverInfo river = frp.GetRiverFrom(globalStart, new List<Direction>(), gridStep);
        Debug.Log(river);
           
        globalStart.side = fmc.GetOppositeDirection(river.GetLastVertex().side);
        Area restrictedArea = fmc.CalculateRestrictedArea(globalStart);
        globalStart.side = Direction.none;

        List<Direction> reachedSides = new List<Direction>();
        reachedSides.Add(river.GetLastVertex().side);

        // 2)find second path
        //Debug.Log(restrictedArea);
        RiverInfo river2 = frp.GetRiverFrom(globalStart, reachedSides, restrictedArea, river, river.gridStep);
        Debug.Log(river2);

        // connect them
        river.ConnectWith(river2);
        river.DrawRiver();

        Debug.Log(river);

        frd.DistortPath(river.riverPath, river.gridStep/3, river.gridStep);

        river.width = width;
        river.areaEffect = areaEffect;
        river.depth = depth;

        rivers.Add(river);
        frd.DigRiver(rivers[rivers.Count - 1]);

        riverGui.riverFlags.Add(true);

        Debug.Log("distorted: " + river);
    }

    /// <summary>
    /// generates rivers starting from given river's starting and ending point
    /// operation is processed only if points are in visible terrain and not on border
    /// => meaning that terrain has been moved and river isn't complete
    /// </summary>
    public void GenerateConnectingRiver(RiverInfo river)
    {
        //Debug.Log("currentRiver:"+ river);
        //Debug.Log("definedArea:" + lt.globalTerrainC.definedArea);


        Vertex startPoint = river.riverPath[0];
        //if (ftm.IsInVisibleterrain(startPoint) && !ftm.IsOnBorder(startPoint))
        if (ftm.IsInDefinedTerrain(startPoint) && !ftm.IsOnBorder(startPoint))
        {
            //Debug.Log("connection river from start: " + startPoint);

            Area restrictArea = fmc.CalculateRestrictedArea(startPoint);
            //Debug.Log("ON: " + restrictArea);

            List<Direction> reachedSides = new List<Direction>();
            reachedSides.Add(fmc.GetOppositeDirection(startPoint.side));

            RiverInfo startRiver =
                frp.GetRiverFrom(startPoint, reachedSides, restrictArea, river, river.gridStep);
            frd.DistortPath(startRiver.riverPath, startRiver.gridStep/3, startRiver.gridStep);
            //Debug.Log("startRiver:" + startRiver);
            river.ConnectWith(startRiver);
            //Debug.Log(river);

            startPoint.side = Direction.none;
            
            //frd.DigRiver(startRiver.riverPath);
        }
        else
        {
            //Debug.Log(startPoint + " OUT [START]");
            //Debug.Log(lt.localTerrainC.botLeft + ", " + lt.localTerrainC.topRight);
        }

        Vertex endPoint = river.GetLastVertex();
        //direction of river might have been changed
        /*if(!ftm.IsInVisibleterrain(endPoint) && ftm.IsOnBorder(endPoint))
        {
            endPoint = river.riverPath[0];
        }*/

        //if (ftm.IsInVisibleterrain(endPoint) && !ftm.IsOnBorder(endPoint))
        if (ftm.IsInDefinedTerrain(endPoint) && !ftm.IsOnBorder(endPoint))
        {
            Area restrictArea = fmc.CalculateRestrictedArea(endPoint);
            //Debug.Log("connection river from end: " + endPoint);
            //Debug.Log("ON: " + restrictArea);

            List<Direction> reachedSides = new List<Direction>();
            reachedSides.Add(fmc.GetOppositeDirection(endPoint.side));

            RiverInfo endRiver =
                frp.GetRiverFrom(endPoint, reachedSides, restrictArea, river, river.gridStep);
            frd.DistortPath(endRiver.riverPath, endRiver.gridStep / 3, endRiver.gridStep);
            //Debug.Log("endRiver:" + endRiver);
            river.ConnectWith(endRiver);

            endPoint.side = Direction.none;
            
            //frd.DigRiver(endRiver.riverPath);
        }
        else
        {
            //Debug.Log(endPoint + " OUT [END]");
            //Debug.Log(lt.localTerrainC.botLeft + ", " + lt.localTerrainC.topRight);

        }

        //Debug.Log("connected river:" + river);

        frd.DigRiver(river);
    }

    /// <summary>
    /// returns river value on given local coordiantes (0 if not defined)
    /// </summary>
    public float GetLocalValue(RiverInfo river, int x, int z)
    {
        float value = localCoordinates.GetLocalValue(x, z, river.globalRiverC);
        if (value != 666)
            return value;
        else
            return 0;
    }

    /// <summary>
    /// returns river which is defined on given coordinates
    /// </summary>
    public RiverInfo GetRiverOn(int x, int z)
    {
        foreach (RiverInfo river in rivers)
        {
            if (river.globalRiverC.IsDefined(x, z))
            {
                return river;
            }
        }
        Debug.Log("no river on " + x + "," + z);
        return new RiverInfo(this);
    }

    /// <summary>
    /// returns if there is some defined value on one of generated rivers on given coordinates
    /// </summary>
    public bool IsRiverDefined(int x, int z)
    {
        foreach (RiverInfo river in rivers)
        {
            if (river.globalRiverC.IsDefined(x, z))
            {
                return true;
            }
        }
        return false;
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
        rivers = new List<RiverInfo>();
        riverGui.riverFlags = new List<bool>();
        /*for(int i = rivers.Count - 1; i >= 0; i--)
        {
            Debug.Log("remove: " + i);
            rivers.RemoveAt(i);
        }*/
        //globalRiverC.ResetQuadrants();
    }
}
