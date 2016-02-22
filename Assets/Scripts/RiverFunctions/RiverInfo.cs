﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiverInfo  {

    public List<Vertex> riverPath;
    public bool reachTop;
    public bool reachRight;
    public bool reachBot;
    public bool reachLeft;

    public Vertex topVertex;
    public Vertex rightVertex;
    public Vertex botVertex;
    public Vertex leftVertex;
    
    public FunctionRiverPlanner frp;
    public FunctionDebugger fd;
    public FunctionTerrainManager ftm;
    public FunctionMathCalculator fmc;

    public List<Direction> reachedSides;

    public RiverInfo(RiverGenerator rg)
    {
        riverPath = new List<Vertex>();
        reachTop = false;
        reachRight = false;
        reachBot = false;
        reachLeft = false;

        fd = rg.fd;
        frp = rg.frp;
        ftm = rg.ftm;
        fmc = rg.fmc;
        reachedSides = new List<Direction>();
    }

    public void UpdateReachedSides()
    {
        if (reachTop && !reachedSides.Contains(Direction.top))
        {
            reachedSides.Add(Direction.top);
        }
        if (reachRight && !reachedSides.Contains(Direction.right))
        {
            reachedSides.Add(Direction.right);
        }
        if (reachBot && !reachedSides.Contains(Direction.bot))
        {
            reachedSides.Add(Direction.bot);
        }
        if (reachLeft && !reachedSides.Contains(Direction.left))
        {
            reachedSides.Add(Direction.left);
        }
    }
    
    /*
    public void UpdatePosition(Direction direction)
    {
        switch (direction)
        {
            case Direction.top:
                UpdateRiverValues(0, -terrain.patchSize);                
                break;
            case Direction.right:
                UpdateRiverValues(-terrain.patchSize, 0);
                break;
            case Direction.bot :
                UpdateRiverValues(0, terrain.patchSize);
                break;
            case Direction.left:
                UpdateRiverValues(terrain.patchSize, 0);
                break;
        }
        SetNotReachedSide(direction);
        CutoffRiver();
    }
    */
    /*
    /// <summary>
    /// cuts all points of path which are out of boundaries
    /// it counts that path starts with points that are out of bounds
    /// at the end it calculates and add the boundary point
    /// 
    /// then it determines if new sides has been reached or lost (by cutting)
    /// </summary>
    public void CutoffRiver()
    {
        int maxIndex = -1;
        for (int i = 0; i < riverPath.Count; i++)
        {
            if (!ftm.CheckBounds(riverPath[i]))
            {
                maxIndex = i;
            }
        }

        if (maxIndex > -1)
        {
            //determine which side point are we going to cut off
            Direction side = fmc.GetReachedSide(riverPath[0].x, riverPath[0].z, 0,
                0, terrain.terrainSize, 0, terrain.terrainSize);
            SetNotReachedSide(side);

            bool addNewBorderpoint = false;
            Vertex newBorderPoint = new Vertex(0, 0);
            //count point on border
            if (maxIndex + 1 < riverPath.Count && !ftm.IsOnBorder(riverPath[maxIndex + 1]))
            {
                addNewBorderpoint = true;
                newBorderPoint = fmc.GetPointOnBorderBetween(riverPath[maxIndex], riverPath[maxIndex + 1]);
            }

            //remove points beyond boundaries
            for (int i = maxIndex; i >= 0; i--)
            {
                //Debug.Log("cutting: " + riverPath[i]);
                riverPath.RemoveAt(i);
            }

            //add border point
            if (addNewBorderpoint)
            {
                //determine on which side is the new point
                Direction newReachedSide = fmc.GetReachedSide(newBorderPoint.x, newBorderPoint.z, 0,
                0, terrain.terrainSize-1, 0, terrain.terrainSize-1);
                SetReachedSide(newReachedSide);

                riverPath.Reverse();
                riverPath.Add(newBorderPoint);
                riverPath.Reverse();
                //Debug.Log("ading new point: " + newBorderPoint + " on side " + newReachedSide);
            }

        }
        
    }
    */

    public void SetReachedSide(Direction side)
    {
        switch (side)
        {
            case Direction.top:
                reachTop = true;
                break;
            case Direction.right:
                reachRight = true;
                break;
            case Direction.bot:
                reachBot = true;
                break;
            case Direction.left:
                reachLeft = true;
                break;
        }
    }

    public void SetNotReachedSide(Direction side)
    {
        switch (side)
        {
            case Direction.top:
                reachTop = false;
                break;
            case Direction.right:
                reachRight = false;
                break;
            case Direction.bot:
                reachBot = false;
                break;
            case Direction.left:
                reachLeft = false;
                break;
        }
    }

    //connect 2 rivers into 1
    public void ConnectWith(RiverInfo river2)
    {
        if (riverPath[0].Equals(river2.riverPath[0]))
        {
            riverPath.RemoveAt(0);
            riverPath.Reverse();
        }
        else if (riverPath[riverPath.Count-1].Equals(river2.riverPath[0]))
        {
            river2.riverPath.RemoveAt(0);
            river2.riverPath.Reverse();
        }
        else if (riverPath[0].Equals(river2.riverPath[riverPath.Count - 1]))
        {
            riverPath.RemoveAt(0);
            riverPath.Reverse();
        }
        else if (riverPath[riverPath.Count - 1].Equals(river2.riverPath[riverPath.Count - 1]))
        {
            river2.riverPath.RemoveAt(riverPath.Count - 1);
            river2.riverPath.Reverse();
        }
        else
        {
            Debug.Log("RIVERS DONT HAVE COMMON END POINT");
        }
        riverPath.AddRange(river2.riverPath);

        reachTop = reachTop || river2.reachTop;
        reachRight = reachRight || river2.reachRight;
        reachBot = reachBot || river2.reachBot;
        reachLeft = reachLeft || river2.reachLeft;
    }

    private void UpdateRiverValues(int diffX, int diffZ)
    {
        foreach(Vertex v in riverPath)
        {
            string s = "updating " + v;
            v.x += diffX;
            v.z += diffZ;
            s += " to " + v;
            //Debug.Log(s);
        }
    }

    public void UpdateDirection(Direction direction)
    {
        UpdateDirection(direction, riverPath);
    }

    public void UpdateDirection(Direction direction, List<Vertex> riverPath)
    {
        if (riverPath.Count == 0)
        {
            Debug.Log("no river");
            return;
        }
        frp.UpdateDirectionOfPath(direction, riverPath);

        switch (direction)
        {
            case Direction.top:
                topVertex = riverPath[riverPath.Count-1];
                botVertex = riverPath[0];
                break;
            case Direction.bot:
                topVertex = riverPath[0];
                botVertex = riverPath[riverPath.Count - 1];
                break;
        }
    }

    public override string ToString()
    {
        string info = "";
        info += "reachTop: " + reachTop + "\n";
        info += "reachRight: " + reachRight + "\n";
        info += "reachBot: " + reachBot + "\n";
        info += "reachLeft: " + reachLeft + "\n";
        foreach(Vertex v in riverPath)
        {
            info += riverPath.IndexOf(v)+": " + v + "\n";
        }
        info += "topVertex:" + topVertex+"\n";
        info += "rightVertex:" + rightVertex + "\n";
        info += "botVertex:" + botVertex+"\n";
        info += "leftVertex:" + leftVertex+"\n";

        return info;
    }

    public void DrawRiver()
    {
        foreach(Vertex v in riverPath)
        {
            fd.ColorPixel(v.x, v.z, 3, fd.pinkColor);
        }
    }
    

}