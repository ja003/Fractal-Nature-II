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

    
    public int terrainWidth;
    public int terrainHeight;
    
    RiverInfo errorRiver; //only as error message carrier

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

        errorRiver = new RiverInfo(rg);
        errorRiver.errorMessage = "-";

        terrainWidth = rg.lt.terrainWidth;
        terrainHeight = rg.lt.terrainHeight;
    }

    /// <summary>
    /// find river path from given starting point on visible area
    /// </summary>
    public RiverInfo GetRiverFrom(Vertex start, List<Direction> reachedSides, int gridStep, bool forceRiver)
    {
        //return GetRiverPathFrom(start, reachedSides, rg.lt.GetVisibleArea());
        //Debug.Log(rg.lt.globalTerrainC.definedArea);
        return GetRiverFrom(start, reachedSides, rg.lt.globalTerrainC.definedArea, gridStep, forceRiver);
    }

    public RiverInfo GetRiverFrom(Vertex start, List<Direction> reachedSides,
        Area restrictedArea, int gridStep, bool forceRiver)
    {
        return GetRiverFrom(start, reachedSides, restrictedArea, new RiverInfo(rg), gridStep, forceRiver);
    }

    /// <summary>
    /// find river path from given starting point on restricted area
    /// forceRiver = generates river even on not very suitable terrain 
    ///     used when generating connecting river
    /// </summary>
    public RiverInfo GetRiverFrom(Vertex start, List<Direction> reachedSides,
        Area restrictedArea, RiverInfo ignoreRiver, int gridStep, bool forceRiver)
    {
        float step = Math.Abs(start.height); //height can be negative
        
        int x_min = restrictedArea.botLeft.x;
        int z_min = restrictedArea.botLeft.z;
        int x_max = restrictedArea.topRight.x;
        int z_max = restrictedArea.topRight.z;
        
        if (step <= 0.1f) //step can't be too small
            step = 0.1f;
        else if (step > 0.1f) //step can't be too big
            step = 0.1f;

        Vertex highestPoint = ftm.GetHighestpoint(x_min, z_min, x_max, z_max);
        float maxThreshold = highestPoint.height; //highest value on map - river can't flow that heigh
        
        List<FloodNode> reachableNodes = new List<FloodNode>();
        reachableNodes.Add(new FloodNode(start, 0));

        float threshold = start.height + step;
        
        int borderOffset = gridStep + 5;
        
        Direction reachedSide = Direction.none;
        int finalIndex = 0;//index of final node (reached one of the sides)

        while (reachedSide == Direction.none)
        {
            for (int i = 0; i < reachableNodes.Count; i++)
            {
                FloodNode currentNode = reachableNodes[i];
                if (!currentNode.processed)
                {
                    int x = (int)currentNode.vertex.x;
                    int z = (int)currentNode.vertex.z;
                    if (rg.IsRiverDefined(x,z) && rg.GetRiverOn(x, z) != ignoreRiver && 
                        !ignoreRiver.riverPath.Contains(currentNode.vertex) && !reachedSides.Contains(Direction.river))
                    {
                        reachedSide = Direction.river;
                        currentNode.vertex.side = Direction.river;
                        finalIndex = i;
                        break;
                    }

                    reachedSide = fmc.GetReachedSide(x, z, borderOffset, x_min, z_min, x_max, z_max);
                    if (reachedSide != Direction.none)
                    {
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
                            errorRiver.errorMessage = "FAIL - algorithm timeout";
                            return errorRiver;
                        }

                        List<Vertex> neighbours =
                            ftm.GetGlobal8Neighbours(currentNode.vertex, gridStep, 0, threshold, x_min, x_max, z_min, z_max);
                        if (neighbours.Count == 8)
                        {
                            currentNode.processed = true;
                        }
                        foreach (Vertex v in neighbours)
                        {
                            if (v.height < threshold && !rg.IsRiverDefined(v.x, v.z) && 
                                !reachableNodes.Contains(new FloodNode(v, i)) || //normal node
                                !reachedSides.Contains(Direction.river) && rg.IsRiverDefined(v.x, v.z) && //river node
                                !ignoreRiver.riverPath.Contains(v))
                            {
                                reachableNodes.Add(new FloodNode(v, i));
                            }
                        }
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
            if(!forceRiver && threshold > rg.riverLevel + 2 * step)
            {
                Debug.Log("threshold too high: " + threshold);
                errorRiver.errorMessage = "RIVER FAIL\n reached max threshold: " + threshold;
                return errorRiver;
            }

            if (threshold > maxThreshold)
            {
                Debug.Log("step=" + step);
                Debug.Log("max=" + maxThreshold);
                errorRiver.errorMessage = "RIVER FAIL\n reached max threshold:  " + threshold;
                return errorRiver;
            }
        }
        
        int pathIndex = finalIndex;
        List<Vertex> finalPath = new List<Vertex>();

        if (reachedSide == Direction.river)
        {
            Vertex lastVertex = reachableNodes[pathIndex].vertex;
            reachableNodes[pathIndex].vertex.side = Direction.none;
            finalPath.Add(rg.GetRiverOn(lastVertex.x, lastVertex.z).GetClosestVertexTo(lastVertex));
            Debug.Log("added river node: " + finalPath[0]);
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
            Vector3.Distance(finalPath[0], finalPath[1]) < gridStep / 2 &&
            finalPath[1] != start)
        {
            finalPath.RemoveAt(1);
        }


        finalPath.Reverse();
        
        
        
        river.riverPath = finalPath;

        river.gridStep = gridStep;

        reachedSides.Add(reachedSide);
        return river;
    }

    public RiverInfo GetRiverFromTo(Vertex start, Vertex end)
    {
        Debug.Log("getting river from " + start + " to " + end);
        int gridStep = 20;
        float e = 0.2f;

        List<Vertex> path = GetRiverPathFromTo(start, end, gridStep, e, 0);
        RiverInfo river = new RiverInfo(rg);
        river.riverPath = path;
        river.gridStep = gridStep;
        river.threshold = Mathf.Max(start.height + e, end.height + e); 

        //SimplifyRiver(river, 3);
        //SimplifyRiver(river, 1);
        return river;
    }

    public void OptimizeRiverCorners(RiverInfo river)
    {
        List<Vertex> path = river.riverPath;
        int gridStep = river.gridStep;
        for (int i = path.Count - 1; i > 1; i--)
        {
            if (fmc.GetDistance(path[i], path[i -2]) <= (gridStep+1))
            {
                Debug.Log("remove: " + path[i-1]);
                path.RemoveAt(i-1);
            }
        }
    }

    /// <summary>
    /// removes all unnecessary nodes from river
    /// </summary>
    /// <param name="river"></param>
    public void SimplifyRiver(RiverInfo river, int maxReduction)
    {
        List<Vertex> path = river.riverPath;
        int gridStep = river.gridStep;
        int reduction;
        for (int i = 0; i < path.Count - 2; i++)
        {
            reduction = 0;
            Vertex next = path[i + 2];
            while(reduction < maxReduction && i < path.Count - 3 && 
                ftm.IsOnContour(path[i], next, river.threshold))
            //if (fmc.GetDistance(path[i], path[i+2]) < 2* gridStep)//corner detected
            {
                Debug.Log(i + " remove: " + path[i+1] + ", left: " + (path.Count-1));
                path.RemoveAt(i+1);
                next = path[i + 2];
                reduction++;
            }
            
        }
    }
    
    
    public List<Vertex> GetRiverPathFromTo(Vertex start, Vertex end, int step, float e, int counter)
    {
        float threshold = Mathf.Max(start.height + e, end.height + e);
        List<Vertex> path = new List<Vertex>();
        List<Vertex> borderPath = new List<Vertex>();
        List<Vertex> ignoreV = new List<Vertex>();//list of vertices where path can't be for sure

        path.Add(start);
        Vertex current = start;
        
        while(fmc.GetDistance(current, end) > (1.5*step - e) && counter < 666)
        {
            counter++;
            //Debug.Log(counter);
            List<Vertex> neighbours = ftm.Get4Neighbours(current, step);
            bool newCurrent = false;
            //Debug.Log(current);
            
            float dist = 666;
            foreach (Vertex n in neighbours)
            {
                if(n.height < threshold && fmc.GetDistance(n,end) < dist &&
                    !path.Contains(n) && !ignoreV.Contains(n))
                {
                    current = n;
                    newCurrent = true;
                    dist = fmc.GetDistance(n, end);
                }
                if(n.height == 666)//vertex is out of bounds => it can be path through border
                {
                    //if (borderPath.Count == 0 ||
                    //    (borderPath.Count > 0 &&
                    //    fmc.GetDistance(borderPath[borderPath.Count - 1], end) < fmc.GetDistance(n, end)))
                    if(borderPath.Count > path.Count+1)
                    {
                        Debug.Log("new border");
                        borderPath.AddRange(path);
                        borderPath.Add(n);
                    }
                }
            }
            if (newCurrent)
            {
                path.Add(current);
            }
            else
            {
                while (!newCurrent && current != start)
                {
                    ignoreV.Add(current);
                    path.RemoveAt(path.Count - 1);
                    current = path[path.Count - 1];
                    neighbours = ftm.Get4Neighbours(current, step);
                    foreach (Vertex n in neighbours)
                    {
                            if (n.height < threshold &&
                            !path.Contains(n) && !ignoreV.Contains(n))
                        {
                            newCurrent = true;
                            current = n;
                            path.Add(current);
                        }
                        if (newCurrent)
                            break;
                    }
                }
            }
            if(current == start)
            {
                Debug.Log("border");
                return borderPath;//if borderpath is empty => no path found                
            }
        }
        Debug.Log(counter);
        return path;
    }

    
}
