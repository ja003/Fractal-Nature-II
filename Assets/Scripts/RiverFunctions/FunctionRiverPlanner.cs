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

    public Color redColor = new Color(1, 0, 0);
    public Color greenColor = new Color(0, 1, 0);
    public Color blueColor = new Color(0, 0, 1);
    public Color pinkColor = new Color(1, 0, 1);

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
        fd.ColorPixel(start.x, start.z, 5, redColor);
        float step = Math.Abs(start.height); //height can be negative
        
        int x_min = restrictedArea.botLeft.x;
        int z_min = restrictedArea.botLeft.z;
        int x_max = restrictedArea.topRight.x;
        int z_max = restrictedArea.topRight.z;

        //Debug.Log("start: " + start);

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
        //int gridStep = 30; //step between river path points
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
                            errorRiver.errorMessage = "algorithm timeout";
                            return errorRiver;
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
                            //if (!rg.IsRiverDefined(v.x, v.z)) //remove if river can connect to river (right now doesn't look well)
                            //{
                            if (v.height < threshold && !rg.IsRiverDefined(v.x, v.z) && 
                                !reachableNodes.Contains(new FloodNode(v, i)) || //normal node
                                !reachedSides.Contains(Direction.river) && rg.IsRiverDefined(v.x, v.z) && //river node
                                !ignoreRiver.riverPath.Contains(v))
                            {
                                //if(ignoreRiver.riverPath.Count>0)
                                //    Debug.Log(ignoreRiver.riverPath[0]);
                                //Debug.Log(v);
                                reachableNodes.Add(new FloodNode(v, i));
                            }
                            //}
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
            if(!forceRiver && threshold > rg.riverLevel + 2 * step)
            {
                Debug.Log("threshold too high: " + threshold);
                errorRiver.errorMessage = "threshold too high: " + threshold;
                return errorRiver;
            }

            if (threshold > maxThreshold)
            {
                Debug.Log("step=" + step);
                Debug.Log("max=" + maxThreshold);
                Debug.Log("FAILz");
                errorRiver.errorMessage = "reached max threshold: " + threshold;
                return errorRiver;
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
            Debug.Log("removing " + finalPath[1]);
            Debug.Log("too close to " + finalPath[0]);
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
            //while(i < path.Count-2 && fmc.GetDistance(path[i], path[i + 2]) < 2* gridStep)
            //Vertex center = new Vertex((path[i].x + path[i + 2].x) / 2, (path[i].z + path[i + 2].z) / 2);
            //center.height = rg.lt.globalTerrainC.GetValue(center.x, center.z);
            Vertex next = path[i + 2];
            while(reduction < maxReduction && i < path.Count - 3 && 
                ftm.IsOnContour(path[i], next, river.threshold))
            //if (fmc.GetDistance(path[i], path[i+2]) < 2* gridStep)//corner detected
            {
                Debug.Log(i + " remove: " + path[i+1] + ", left: " + (path.Count-1));
                //center = new Vertex((path[i].x + path[i + 3].x) / 2, (path[i].z + path[i + 3].z) / 2);
                //center.height = rg.lt.globalTerrainC.GetValue(center.x, center.z);
                path.RemoveAt(i+1);
                next = path[i + 2];
                reduction++;
            }

            //not relevant with 4 neighbourhood method
            /*if(fmc.GetDistance(path[i], path[i+1]) < gridStep/2 ||
                fmc.GetDistance(path[i], path[i - 1]) < gridStep/2)
            {
                Debug.Log("remove: " + path[i]);
                path.RemoveAt(i);
            }*/
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


    //not working
    /*
    public List<Vertex> GetRiverPathFromTo2(Vertex start, Vertex end, float step, float e, int counter)
    {
        //int counter = 0;
        float threshold = Mathf.Max(start.height + e, end.height + e);
        Debug.Log("threshold: " + threshold);

        Vertex current = start;
        Vector3 dir = ((Vector3)end - start).normalized;
        List<Vertex> path = new List<Vertex>();
        Debug.Log("path from " + start + " to " + end);
        Vertex tmpVert = current;
        counter++;
        if (counter > 10)
            return path;

        //while (fmc.GetDistance(current,end) > (step + e) && ftm.IsInDefinedTerrain(current) && counter < 50)
        //{
        counter++;
        while (current.height < threshold && fmc.GetDistance(current, end) > (step + e))
        {
            path.Add(current);
            Debug.Log("add: " + current);
            tmpVert = current;
            current = new Vertex((int)(current.x + dir.x * step), (int)(current.z + dir.z * step));
            current.height = rg.lt.globalTerrainC.GetValue(current);
            Debug.Log("new: " + current);

        }
        if (fmc.GetDistance(current, end) < (step + e) && current.height < threshold)
        {
            if ((path.Count > 0 && !current.CoordinatesEquals(path[path.Count - 1])))
                path.Add(current);
        }
        else
        {
            //barrier reached
            Vertex barrier = current;
            current = tmpVert;
            while (barrier.height > threshold && fmc.GetDistance(barrier, end) > (step + e))
            {
                barrier = new Vertex((int)(barrier.x + dir.x * step), (int)(barrier.z + dir.z * step));
                barrier.height = rg.lt.globalTerrainC.GetValue(barrier);
            }
            Vertex center = new Vertex((current.x + barrier.x) / 2, (current.z + barrier.z) / 2);
            if (fmc.GetDistance(barrier, end) <= (step + e))//case when barrier is very close to end
            {
                center = barrier;
                barrier = end;
            }
            center.height = rg.lt.globalTerrainC.GetValue(center);
            Vertex b1 = center;
            Vertex b2 = center;
            Vector3 b_dir = ((Vector3)barrier - current).normalized; //perpendicular vector to barrier
            float tmp = b_dir.x;
            b_dir.x = -b_dir.z;
            b_dir.z = tmp;

            while (b1.height > threshold && b1.height < 666)
            {
                b1 = new Vertex((int)(b1.x + b_dir.x * step), (int)(b1.z + b_dir.z * step));
                b1.height = rg.lt.globalTerrainC.GetValue(b1);
            }
            while (b2.height > threshold && b2.height < 666)
            {
                b2 = new Vertex((int)(b2.x - b_dir.x * step), (int)(b2.z - b_dir.z * step));
                b2.height = rg.lt.globalTerrainC.GetValue(b2);
            }
            Vertex bc;
            if (b1.height != 666 && b2.height != 666) //both of them are in defined terrain => choose closer one
            {
                if (fmc.GetDistance(current, b1) < fmc.GetDistance(current, b2))
                    bc = b1;
                else
                    bc = b2;
            }
            else//1 of them is not defined for sure
            {
                if (b1.height < b2.height)
                    bc = b1;
                else
                    bc = b2;
            }

            //try method dependent on distance from end
            if (fmc.GetDistance(end, b1) < fmc.GetDistance(end, b2))
                bc = b1;
            else
                bc = b2;


            Debug.Log("current: " + current);
            Debug.Log("barrier: " + barrier);
            Debug.Log("center: " + center);
            Debug.Log("b1: " + b1);
            Debug.Log("b2: " + b2);
            Debug.Log("bc: " + bc);

            if (bc.height == 666)//if chosen point is not defined, find the closest defined point
            {
                //...
                Debug.Log(bc + " is out");
                Vector3 bourder_dir = ((Vector3)current - bc).normalized;
                while (bc.height == 666)
                {
                    bc = new Vertex((int)(bc.x + bourder_dir.x * step / 3), (int)(bc.z + bourder_dir.z * step / 3));
                    bc.height = rg.lt.globalTerrainC.GetValue(bc);
                }
                Debug.Log("moved to " + bc);
                //bc = current;
            }

            List<Vertex> path1 = GetRiverPathFromTo2(current, bc, step, e, counter);
            if (path1.Count > 0 && path[path.Count - 1] == path1[0])
            {
                Debug.Log("remove: " + path1[0]);
                path1.RemoveAt(0);
            }
            path.AddRange(path1);
            List<Vertex> path2 = GetRiverPathFromTo2(bc, barrier, step, e, counter);
            if (path2.Count > 0 && path[path.Count - 1] == path2[0])
            {
                Debug.Log("remove: " + path2[0]);
                path2.RemoveAt(0);
            }
            path.AddRange(path2);
        }

        if (path.Count > 0 && fmc.GetDistance(path[path.Count - 1], end) > (step + e))
        {
            List<Vertex> pathEnd = GetRiverPathFromTo2(path[path.Count - 1], end, step, e, counter);
            path.AddRange(pathEnd);
        }


        //}
        if (path.Count == 0)
            path.Add(start);

        if (path.Count > 0 && !end.CoordinatesEquals(path[path.Count - 1]))
            path.Add(end);
        return path;
    }
    */
}
