using UnityEngine;
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
    //public int terrainSize;
    public int terrainWidth;
    public int terrainHeight;

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

        terrainWidth = rg.lt.terrainWidth;
        terrainHeight = rg.lt.terrainHeight;
    }

    /// <summary>
    /// find river path from given starting point on visible area
    /// </summary>
    public RiverInfo GetRiverPathFrom(Vertex start, List<Direction> reachedSides)
    {
        //int x_min = (int)rg.GetBotLeft().x;
        //int z_min = (int)rg.GetBotLeft().z;
        //int x_max = (int)rg.GetTopRight().x;
        //int z_max = (int)rg.GetTopRight().z;

        //return GetRiverPathFrom(start, reachedSides, rg.lt.GetVisibleArea());
        Debug.Log(rg.lt.globalTerrainC.definedArea);
        return GetRiverPathFrom(start, reachedSides, rg.lt.globalTerrainC.definedArea);
    }

    public RiverInfo GetRiverPathFrom(Vertex start, List<Direction> reachedSides,
        Area restrictedArea)
    {
        return GetRiverPathFrom(start, reachedSides, restrictedArea, new RiverInfo(rg));
    }

    /// <summary>
    /// find river path from given starting point on restricted area
    /// </summary>
    public RiverInfo GetRiverPathFrom(Vertex start, List<Direction> reachedSides,
        Area restrictedArea, RiverInfo ignoreRiver)
    {
        fd.ColorPixel(start.x, start.z, 5, redColor);
        float step = Math.Abs(start.height); //height can be negative
        
        int x_min = restrictedArea.botLeft.x;
        int z_min = restrictedArea.botLeft.z;
        int x_max = restrictedArea.topRight.x;
        int z_max = restrictedArea.topRight.z;

        //Debug.Log("start: " + start);
        //Debug.Log("on");
        //Debug.Log(x_min);
        //Debug.Log(z_min);
        //Debug.Log(x_max);
        //Debug.Log(z_max);
        //if(reachedSides.Count != 0)
        //    Debug.Log("reached: " + reachedSides[0]);

        if (step <= 0.1f) //step can't be too small
            step = 0.1f;
        else if (step > 0.1f) //step can't be too big
            step = 0.1f;

        Vertex highestPoint = ftm.GetHighestpoint(x_min, z_min, x_max, z_max);
        float maxThreshold = highestPoint.height; //highest value on map - river can't flow that heigh
        fd.ColorPixel(highestPoint.x, highestPoint.z, 5, blueColor);
        //Debug.Log(highestPoint);
        
        List<FloodNode> reachableNodes = new List<FloodNode>();
        reachableNodes.Add(new FloodNode(start, 0));
        /*foreach(Vertex v in ignoreNodes)
        {
            FloodNode fn = new FloodNode(v, 666);
            fn.processed = true;
            fn.vertex.side = Direction.none;
            reachableNodes.Add(fn);
        }*/

        float threshold;
        //if (start.height > 0)
            threshold = start.height + step;
        //else
           // threshold = step;

        //TODO: cancel default step!!!
        int gridStep = 30; //step between river path points
        int borderOffset = gridStep + 5;
        
        //bool reachedSide = false;
        Direction reachedSide = Direction.none;
        int finalIndex = 0;//index of final node (reached one of the sides)

        while (reachedSide == Direction.none)
        {
            for (int i = 0; i < reachableNodes.Count; i++)
            {
                FloodNode currentNode = reachableNodes[i];
                if (!currentNode.processed)
                {
                    //get local coordinates to check which side has been reached
                    //int x = (int)rg.localCoordinates.GetLocalCoordinates(currentNode.vertex).x;
                    //int z = (int)rg.localCoordinates.GetLocalCoordinates(currentNode.vertex).z;
                    int x = (int)currentNode.vertex.x;
                    int z = (int)currentNode.vertex.z;
                    /*if (i < 100)
                    {
                        Debug.Log(i + ":" + currentNode);
                        Debug.Log(x);
                        Debug.Log(z);

                    }*/
                    if (rg.IsRiverDefined(x,z) && rg.GetRiverOn(x, z) != ignoreRiver)
                    {
                        reachedSide = Direction.river;
                        currentNode.vertex.side = Direction.river;
                        finalIndex = i;
                        break;
                    }

                    reachedSide = fmc.GetReachedSide(x, z, borderOffset, x_min, z_min, x_max, z_max);
                    if (reachedSide != Direction.none)
                    {
                        //if(i < 100)
                        //    Debug.Log(currentNode);

                        //check if node is not on side that has already been reached
                        bool reachedAvailableSide = 
                            fmc.ReachedAvailableSide(x, z, reachedSides, borderOffset, x_min, z_min, x_max, z_max);

                        if (reachedAvailableSide)
                        {
                            finalIndex = i;
                            currentNode.vertex.side = reachedSide;
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
                        if (i > terrainWidth* terrainHeight)
                        {
                            Debug.Log("FAIL");
                            finalIndex = i;
                            reachedSide = Direction.top;
                            break;
                        }

                        List<Vertex> neighbours =
                            ftm.GetGlobal8Neighbours(currentNode.vertex, gridStep, 0, threshold, x_min, x_max, z_min, z_max);
                        if (neighbours.Count == 8)
                        {
                            currentNode.processed = true;
                        }
                        foreach (Vertex v in neighbours)
                        {
                            if (!rg.IsRiverDefined(v.x, v.z)) //remove if river can connect to river (right now doesn't look well)
                            {
                                if (v.height < threshold && !ignoreRiver.riverPath.Contains(v) && !reachableNodes.Contains(new FloodNode(v, i)))
                                {
                                    reachableNodes.Add(new FloodNode(v, i));
                                }
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

        if (reachedSide == Direction.river)
        {
            Vertex lastVertex = reachableNodes[pathIndex].vertex;
            reachableNodes[pathIndex].vertex.side = Direction.none;
            finalPath.Add(rg.GetRiverOn(lastVertex.x, lastVertex.z).GetClosestVertexTo(lastVertex));
            finalPath[0].side = Direction.river;
        }
        else
        {
            finalPath.Add(fmc.GetVertexOnBorder(reachableNodes[finalIndex].vertex,
                borderOffset, reachedSide,
                x_min, x_max, z_min, z_max)); //add new node which lies exactly on border            

        }
        RiverInfo river = new RiverInfo(rg);

        while (pathIndex != 0)//recursively add all vertices of found path
        {
            finalPath.Add(reachableNodes[pathIndex].vertex);
            pathIndex = reachableNodes[pathIndex].parentIndex;
            river.UpdateLowestPoint(reachableNodes[pathIndex].vertex);
        }
        finalPath.Add(start);

        //if added border node is too close to next node, delete the next one
        if (finalPath.Count > 1 &&
            Vector3.Distance(finalPath[0], finalPath[1]) < gridStep / 2)
        {
            //Debug.Log("removing " + finalPath[1]);
            //Debug.Log("too close to " + finalPath[0]);
            finalPath.RemoveAt(1);
        }


        finalPath.Reverse();
        
        
        
        river.riverPath = finalPath;

        river.gridStep = gridStep;

        reachedSides.Add(reachedSide);
        /*foreach(Direction side in reachedSides)
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
        }*/
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
