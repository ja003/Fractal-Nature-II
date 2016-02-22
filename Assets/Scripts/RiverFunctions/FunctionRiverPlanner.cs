﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FunctionRiverPlanner  {

    public RiverGenerator rg;
    public FunctionRiverDigger frd;
    public FunctionMathCalculator fmc;
    public FunctionTerrainManager ftm;
    public FunctionDebugger fd;
    public RiverInfo currentRiver;


    public Vector3[,] vertices;
    public int terrainSize;

    public Color redColor = new Color(1, 0, 0);
    public Color greenColor = new Color(0, 1, 0);
    public Color blueColor = new Color(0, 0, 1);
    public Color pinkColor = new Color(1, 0, 1);

    public FunctionRiverPlanner()
    {
    }

    public void AssignFunctions(RiverGenerator rg)
    {
        this.rg = rg;
        fd = rg.fd;
        ftm = rg.ftm;
        fmc = rg.fmc;
        frd = rg.frd;
    }


    /// <summary>
    /// find river path from given starting point on restricted area
    /// </summary>
    /// <param name="start">starting point</param>
    /// <param name="reachedSides">sideas already reached - result path can't end on any of these these sides</param>
    /// <param name="x_min">area restriction</param>
    /// <param name="x_max">area restriction</param>
    /// <param name="z_min">area restriction</param>
    /// <param name="z_max">area restriction</param>
    /// <returns></returns>
    public RiverInfo GetRiverPathFrom(Vertex start, List<Direction> reachedSides,
        int x_min, int x_max, int z_min, int z_max)
    {
        //int x_min = (int)botLeft.x;
        //int z_min = (int)botLeft.z;
        //int x_max = (int)topRight.x;
        //int z_max = (int)topRight.z;

        fd.ColorPixel(start.x, start.z, 5, redColor);
        float step = Math.Abs(start.height); //height can be negative
        //Debug.Log("start: " + start);
        //Debug.Log("ON");
        //Debug.Log(x_min);
        //Debug.Log(x_max);
        //Debug.Log(z_min);
        //Debug.Log(z_max);

        if (step <= 0.1f) //step can't be too small
            step = 0.1f;
        else if (step > 0.1f) //step can't be too big
            step = 0.1f;

        Vertex highestPoint = ftm.GetHighestpoint(x_min, x_max, z_min, z_max);
        float maxThreshold = highestPoint.height; //highest value on map - river can't flow that heigh
        fd.ColorPixel(highestPoint.x, highestPoint.z, 5, blueColor);
        //Debug.Log(highestPoint);
        
        List<FloodNode> reachableNodes = new List<FloodNode>();
        reachableNodes.Add(new FloodNode(start, 0));
        float threshold;
        //if (start.height > 0)
            threshold = start.height + step;
        //else
           // threshold = step;

        int gridStep = 20; //step between river path points
        int borderOffset = gridStep + 5;
        
        //bool reachedSide = false;
        Direction reachedSide = Direction.none;
        int finalIndex = 0;//index of final node (reached one of the sides)

        while (reachedSide == Direction.none)
        {
            for (int i = 0; i < reachableNodes.Count; i++)
            {
                //Debug.Log(i + " - threshold: " + threshold);

                FloodNode currentNode = reachableNodes[i];
                if (!currentNode.processed)
                {
                    int x = currentNode.vertex.x;
                    int z = currentNode.vertex.z;
                    //if (i < 100)
                    //    Debug.Log(i + ":" + currentNode);

                    reachedSide = fmc.GetReachedSide(x, z, borderOffset, x_min, x_max, z_min, z_max);
                    if (reachedSide != Direction.none)
                    {
                        //if(i < 100)
                        //    Debug.Log(currentNode);

                        //check if node is not on side that has already been reached
                        bool reachedAvailableSide = 
                            fmc.ReachedAvailableSide(x, z, reachedSides, borderOffset, x_min, x_max, z_min, z_max);

                        if (reachedAvailableSide)
                        {
                            finalIndex = i;
                            break;
                        }
                        else
                        {
                            reachedSide = Direction.none;
                        }
                        if (reachedSide != Direction.none)
                        {
                            Debug.Log("????");
                            break;
                        }
                    }
                    else
                    {
                        //Debug.Log(currentNode);
                    }

                    if (reachedSide != Direction.none)
                    {
                        break;
                    }

                    //dont process already processed nodes again
                    if (!currentNode.processed)
                    {
                        if (i > terrainSize * terrainSize)
                        {
                            Debug.Log("FAIL");
                            finalIndex = i;
                            reachedSide = Direction.top;
                            break;
                        }

                        List<Vertex> neighbours =
                            ftm.Get8Neighbours(currentNode.vertex, gridStep, 0, threshold, x_min, x_max, z_min, z_max);
                        if (neighbours.Count == 8)
                        {
                            currentNode.processed = true;
                        }
                        foreach (Vertex v in neighbours)
                        {
                            if (v.height < threshold && !reachableNodes.Contains(new FloodNode(v, i)))
                            {
                                reachableNodes.Add(new FloodNode(v, i));
                            }
                        }
                    }
                    else
                    {
                        //Debug.Log("skip " + i);
                    }
                }
                if (reachedSide != Direction.none)
                {
                    break;
                }
            }
            if (reachedSide != Direction.none)
            {
                break;
            }
            threshold += step; 

            if (threshold > maxThreshold)
            {
                Debug.Log("step=" + step);
                Debug.Log("max=" + maxThreshold);
                Debug.Log("FAILz");
                break;
            }
        }
        
        int pathIndex = finalIndex;
        List<Vertex> finalPath = new List<Vertex>();
        finalPath.Add(fmc.GetVertexOnBorder(reachableNodes[finalIndex].vertex, 
            borderOffset, reachedSide,
            x_min, x_max, z_min, z_max)); //add new node which lies exactly on border

        while (pathIndex != 0)//recursively add all vertices of found path
        {
            finalPath.Add(reachableNodes[pathIndex].vertex);
            pathIndex = reachableNodes[pathIndex].parentIndex;
        }
        finalPath.Add(start);
        finalPath.Reverse();
        
        
        RiverInfo river = new RiverInfo(rg);
        river.riverPath = finalPath;

        reachedSides.Add(reachedSide);
        foreach(Direction side in reachedSides)
        {
            switch (reachedSide)
            {
                case Direction.top:
                    river.reachTop = true;
                    break;
                case Direction.right:
                    river.reachRight = true;
                    break;
                case Direction.bot:
                    river.reachBot = true;
                    break;
                case Direction.left:
                    river.reachLeft = true;
                    break;
            }
        }
        return river;
    }

    public void UpdateDirectionOfPath(Direction direction, List<Vertex> path)
    {
        

        if (path.Count == 0)
            return;
        else
        {
            foreach(Vertex v in path)
            {
                //Debug.Log(v);
            }
        }

        switch (direction)
        {
            case Direction.top:
                if (path[0].z > path[path.Count - 1].z)
                {
                    path.Reverse();
                }
                break;
            case Direction.bot:
                if (path[0].z < path[path.Count - 1].z)
                {
                    path.Reverse();
                }
                break;
        }
    }

    
}