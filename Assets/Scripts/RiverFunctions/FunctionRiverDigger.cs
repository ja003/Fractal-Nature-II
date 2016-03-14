using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FunctionRiverDigger {

    public RiverGenerator rg;
    public LocalTerrain lt;

    public LocalCoordinates lc;
    public GlobalCoordinates globalRiverC;

    //GlobalCoordinates depthField;
    //GlobalCoordinates distField;

    public FunctionMathCalculator fmc;
    public FunctionTerrainManager ftm;
    public FunctionDebugger fd;

    

    public FunctionRiverDigger()
    {
    }

    public void AssignFunctions(RiverGenerator rg)
    {
        this.rg = rg;

        lc = rg.localCoordinates;
        globalRiverC = rg.globalRiverC;

        lt = rg.lt;
        fmc = rg.fmc;
        ftm = rg.ftm;
        fd = rg.fd;
    }

    /// <summary>
    /// randomly dostort all path nodes bot first and last
    /// </summary>
    public void DistortPath(List<Vertex> path, int maxDistort)
    {
        System.Random rnd = new System.Random();
        //foreach (Vertex v in path)
        for(int i = 1;i<path.Count-1;i++)
        {
            Vertex v = path[i];
            int distortX = rnd.Next(-maxDistort, maxDistort);
            int distortZ = rnd.Next(-maxDistort, maxDistort);
            v.Rewrite(v.x + distortX, v.z + distortZ, v.height);
        }
    }

    /// <summary>
    /// function for determine "strength" of each river path nodes
    /// </summary>
    public List<float> AssignWidthToPoints(List<Vertex> path, float minWidth)
    {
        List<float> sumOfPointNeighb = new List<float>();
        float totalSum = 0;
        float maxSum = 0;
        foreach (Vertex v in path)
        {
            sumOfPointNeighb.Add(ftm.GetSumFromNeighbourhood(v.x, v.z, 2 * (int)minWidth));
            totalSum += sumOfPointNeighb[path.IndexOf(v)]; 
            if(sumOfPointNeighb[path.IndexOf(v)] > maxSum){
                maxSum = sumOfPointNeighb[path.IndexOf(v)];
            }           
        }
        float averageSum = totalSum / sumOfPointNeighb.Count;

        List<float> finalWidthValues = new List<float>();
        //Debug.Log("-------------");
        foreach (float f in sumOfPointNeighb)
        {
            float value = 0;
            value = minWidth + minWidth / 2 * (maxSum - f) / maxSum;
            if (Double.IsPositiveInfinity(value) && counter<10) //TODO: fix assigning maxSum...shouldnt start with 0
            {
                value = minWidth;
                Debug.Log(f + "/" + maxSum + " IsPositiveInfinity");
                counter++;
            }
            finalWidthValues.Add(value);
        }
        
        return finalWidthValues;
    }

    

    public float GetLocalWidth(Vertex point, Vertex v1, Vertex v2,float w1,float w2)
    {
        if (v1.CoordinatesEquals(v2))
        {
            Debug.Log("same points!");
            Debug.Log(v1);
            Debug.Log(v2);
            return 0;
        }
        //project point on line v1-v2
        /////WTF? why is it here?
        //Vertex projectedP = fmc.ProjectPointOnLine(point, fmc.GetGeneralLineEquation(v1, v2));
        //measure distance to v1 and v2
        float dist1 = fmc.GetDistanceBetweenPoints(point, v1);
        float dist2 = fmc.GetDistanceBetweenPoints(point, v2);
        //Debug.Log(dist1);
        //Debug.Log(dist2);
        //map values to give total sum of 1
        float factor = dist1 + dist2;
        float factor1 = (factor - dist1) / factor;
        float factor2 = (factor - dist2) / factor;
        //Debug.Log(factor);
        //Debug.Log(factor1);
        //Debug.Log(factor2);

        //return sum of given widths multiplied by calculated factors

        return w1 * factor1 + w2*factor2;
    }

    /// <summary>
    /// digs river with default values
    /// </summary>
    public void DigRiver(List<Vertex> path)
    {
        DigRiver(path, 15, 2, 1f);
    }

    /// <summary>
    /// digs river path
    /// </summary>
    /// <param name="width">width of river corridor</param>
    /// <param name="widthFactor">defines area around river. 1 = only river, 2 = river and close area</param>
    /// <param name="maxDepth">depth in center of river</param>
    public void DigRiver(List<Vertex> path, int width, float widthFactor, float maxDepth)
    {
        //DigCorners(path, width, widthFactor, maxDepth);

        if(path.Count < 2)
        {
            Debug.Log("path too short: " + path.Count);
            return;
        }

        DigRiverPath(path, width, widthFactor, maxDepth);

        DigCorners(path, width, widthFactor, maxDepth);
    }

    /// <summary>
    /// digs river path (not corners)
    /// </summary>
    public void DigRiverPath(List<Vertex> path, int width, float widthFactor, float maxDepth)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vertex previous = i > 0 ? path[i - 1] : path[i];
            Vertex v1 = path[i];
            Vertex v2 = path[i + 1];
            Vertex next = i < path.Count - 2 ? path[i + 2] : path[i + 1];

            DigRiverPart(v1, v2, previous, next, width, widthFactor, maxDepth);
        }
    }

    /// <summary>
    /// digs river part v1 -> v2
    /// previous and next are used to determine which node does processed point belong to
    /// </summary>
    /// <param name="previous">node before v1</param>
    /// <param name="next">node after v2</param>
    public void DigRiverPart(Vertex v1, Vertex v2,Vertex previous, Vertex next, int width, float widthFactor, float maxDepth)
    {
        Vertex botLeft = fmc.CalculateBotLeft(v1, v2, width, widthFactor);
        Vertex topRight = fmc.CalculateTopRight(v1, v2, width, widthFactor);

        for (int x = botLeft.x; x < topRight.x; x++)
        {
            for (int z = botLeft.z; z < topRight.z; z++)
            {
                Vertex point = new Vertex(x, z);
                float distance = fmc.GetDistanceFromLine(point, v1, v2);
                float distV1 = fmc.GetDistance(point, v1);
                float distV2 = fmc.GetDistance(point, v2);
                float distanceFromCorners = Math.Max(distV1, distV2);
                float cornersDistance = fmc.GetDistance(v1, v2);

                if (distance < widthFactor * width && distanceFromCorners <= cornersDistance)
                {
                    float depth = GetDepth(distance, width, maxDepth);
                    float distanceFromPrevNext = 0;
                    float PrevNextDistance = 0;

                    if (distV1 < distV2)
                    {
                        distanceFromPrevNext = fmc.GetDistance(point, previous);
                        PrevNextDistance = fmc.GetDistance(v1, previous);
                    }
                    else
                    {
                        distanceFromPrevNext = fmc.GetDistance(point, next);
                        PrevNextDistance = fmc.GetDistance(v2, next);
                    }

                    if (depth < globalRiverC.GetValue(x, z) && 
                        (cornersDistance - distanceFromCorners) >= (PrevNextDistance - distanceFromPrevNext))
                    {
                        globalRiverC.SetValue(x, z, depth);
                        //globalRiverC.SetValue(x, z, -5);
                    }
                }
            }
        }
    }

    /// <summary>
    /// dig corners
    /// </summary>
    public void DigCorners(List<Vertex> path, int width, float widthFactor, float depth)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vertex previous = i > 0 ? path[i - 1] : path[i+1];
            Vertex corner = path[i];
            Vertex next = i < path.Count-1 ? path[i + 1] : path[i - 1];
            
            DigCorner(corner, previous, next, width, widthFactor, depth);
        }


    }

    /// <summary>
    /// gigs only certain angle of corner
    /// the angle is determined by sharpness of the path (from prev->corner->next sequence)
    /// </summary>
    public void DigCorner(Vertex corner, Vertex previous, Vertex next, int width, float widthFactor, float maxDepth)
    {
        Vertex botLeft = new Vertex(corner.x - (int)(widthFactor * width), 
            corner.z - (int)(widthFactor * width));
        Vertex topRight = new Vertex(corner.x + (int)(widthFactor * width), 
            corner.z + (int)(widthFactor * width));
        float cornerPreviousDistance = fmc.GetDistance(corner, previous);
        float cornerNextDistance = fmc.GetDistance(corner, next);

        for (int x = botLeft.x; x < topRight.x; x++)
        {
            for (int z = botLeft.z; z < topRight.z; z++)
            {
                Vertex point = new Vertex(x, z);
                float distance = fmc.GetDistance(point, corner);

                if (distance < widthFactor * width)// && distance > distancePrevious && distance > distanceNext)
                {
                    float pointPreviousDistance = fmc.GetDistance(point, previous);
                    float pointNextDistance = fmc.GetDistance(point, next);

                    if (pointPreviousDistance  >= cornerPreviousDistance && 
                        pointNextDistance >= cornerNextDistance) {
                        float depth = GetDepth(distance, width, maxDepth);
                        //if (depth < globalRiverC.GetValue(x, z))
                        if(!globalRiverC.IsDefined(x,z) || depth < globalRiverC.GetValue(x, z))
                        {
                            globalRiverC.SetValue(x, z, depth);
                            //globalRiverC.SetValue(x, z, -5);
                        }
                    }
                }
            }
        }
    }


    /*
    public void DigCorner(Vertex corner, int width, float widthFactor, float maxDepth)
    {
        Vertex botLeft = new Vertex(corner.x - (int)(widthFactor*width), corner.z - (int)(widthFactor * width));
        Vertex topRight = new Vertex(corner.x + (int)(widthFactor * width), corner.z + (int)(widthFactor * width));

        for(int x = botLeft.x; x < topRight.x; x++)
        {
            for (int z = botLeft.z; z < topRight.z; z++)
            {
                float distance = fmc.GetDistance(new Vertex(x, z), corner);
                if (distance < widthFactor * width)
                {
                    float depth = GetDepth(distance, width, maxDepth);
                    if (depth < globalRiverC.GetValue(x, z))
                    //if(!globalRiverC.IsDefined(x,z))
                    {
                        globalRiverC.SetValue(x, z, depth);
                    }
                }
            }
        }
    }
    */

    /// <summary>
    /// calls depth function (currently MySinc)
    /// </summary>
    public float GetDepth(float distance, float width, float maxDepth)
    {
        return MySinc(distance, width, maxDepth);
    }


    //obsolete
    /*
    public void DigRiver(List<Vertex> path)
    {
        DigRiver(path, 10, 0.7f);
    }


    GlobalCoordinates depthField;
    GlobalCoordinates distField;
    GlobalCoordinates pathMark;

    public void DigRiver(List<Vertex> path, int width, float depthFactor)
    {


        //float[,] depthField = new float[lt.terrainWidth, lt.terrainHeight]; //depth to dig
        //float[,] distField = new float[lt.terrainWidth, lt.terrainHeight]; //distance from line
        //float[,] pathMark = new float[lt.terrainWidth, lt.terrainHeight]; //path number which will effect the vertex



        //for (int x = 0; x < lt.terrainWidth; x++)
        //{
        //    for (int z = 0; z < lt.terrainHeight; z++)
        //    {
        //        depthField[x, z] = 666;//mark
        //        distField[x, z] = 666;//mark
        //    }
        //}

        //should be min 2
        int areaFactor = 2;

        List<float> widthInPoints = AssignWidthToPoints(path, width);
        

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vertex v1 = path[i];
            Vertex v2 = path[i + 1];
            float width1 = widthInPoints[i];
            float width2 = widthInPoints[i+1];
            

            Vertex topLeftCorner = new Vertex(0, 0);
            Vertex botRightCorner = new Vertex(0, 0);
            //evaluate position of affected area
            if (v1.x < v2.x)
            {
                topLeftCorner.x = (int)(v1.x - width1 * areaFactor);
                botRightCorner.x = (int)(v2.x + width2 * areaFactor);
            }
            else
            {
                topLeftCorner.x = (int)(v2.x - width2 * areaFactor);
                botRightCorner.x = (int)(v1.x + width1 * areaFactor);
            }
            if (v1.z < v2.z)
            {
                topLeftCorner.z = (int)(v2.z + width2 * areaFactor);
                botRightCorner.z = (int)(v1.z - width1 * areaFactor);
            }
            else
            {
                topLeftCorner.z = (int)(v1.z + width1 * areaFactor);
                botRightCorner.z = (int)(v2.z - width2 * areaFactor);
            }

            if (topLeftCorner.Equals(botRightCorner))
            {
                Debug.Log("same vertices");
                break;
            }

            
            for (int x = topLeftCorner.x; x < botRightCorner.x; x++)
            {
                for (int z = topLeftCorner.z; z > botRightCorner.z; z--)
                {
                    if (ftm.CheckBounds(x, z) && fmc.BelongsToPath(x, z, v1, v2, 2*width * areaFactor))
                    {
                        
                        bool valid = true;
                        Vertex vert = new Vertex(x, z);
                        float distance = fmc.GetDistanceFromLine(vert, v1, v2);

                        float depth = 0;

                        if (distance == 0) //sinc is not defined at 0
                            distance += 0.01f;
                        
                        //determine width (interpolation between points)
                        float localWidth = GetLocalWidth(vert, v1, v2, width1, width2);
                        //float localWidth = 1;
                        if(localWidth < width)
                        {
                            //Debug.Log("!");
                        }
                        
                        if(distance > areaFactor * localWidth)
                        {
                            valid = false;
                        }

                        if (Double.IsNaN(MySinc(distance, localWidth, depthFactor)) && counter < 10)
                        {
                            Debug.Log("[" + x + "," + z + "]: NaN");
                            counter++;
                        }
                        depth = MySinc(distance, localWidth, depthFactor);


                        if (valid && !depthField.IsDefined(vert)) //hasnt been modified yet
                        {
                            //depthField[vert.x, vert.z] = depth;
                            //distField[vert.x, vert.z] = distance;
                            //pathMark[vert.x, vert.z] = 1;//path

                            depthField.SetValue(vert, depth, false);
                            distField.SetValue(vert, distance, false);
                            pathMark.SetValue(vert, 1, false);

                            //widthFactor += 0.0003f;
                        }
                        else if (valid && depthField.IsDefined(vert)) //has been modified but I can dig it better (valid shouldnt be neccessary)
                        {
                            if (distance < distField.GetValue(vert))
                            {
                                //depthField[vert.x, vert.z] = Math.Min(depthField[vert.x, vert.z], depth);
                                //depthField[vert.x, vert.z] = depth;
                                //distField[vert.x, vert.z] = distance;
                                depthField.SetValue(vert, depth, false);
                                distField.SetValue(vert, distance, false);

                                //widthFactor += 0.0003f;
                            }
                            //depthField[vert.x, vert.z] = (depthField[vert.x, vert.z] + depth)/2;
                        }
                        
                        
                    }
                }
            }
            counter = 0;
            

        }


        //fix corners
        
        for (int i = 0; i < path.Count; i++)
        {
            Vertex corner = path[i];
            //Debug.Log(corner);
            for (int x = corner.x - 2 * areaFactor * width; x < corner.x + 2 * areaFactor * width; x++)
            {
                for (int z = corner.z - 2 * areaFactor * width; z < corner.z + 2 * areaFactor * width; z++)
                {
                    if (ftm.CheckBounds(x, z))
                    {
                        float distance = fmc.GetDistance(corner, new Vertex(x, z));
                        //if (i == 10)
                        //Debug.Log(i+":"+distance);

                        if (distance < areaFactor * widthInPoints[i])
                        {
                            float depth = 0;

                            if (distance == 0) //sinc is not defined at 0
                                distance += 0.01f;

                            if (Double.IsNaN(MySinc(distance, widthInPoints[i], depthFactor)) && counter < 10)
                            {
                                Debug.Log("[" + x + "," + z + "]: NaN");
                                counter++;
                            }
                            depth = MySinc(distance, widthInPoints[i], depthFactor);


                            if (!depthField.IsDefined(x, z)) //hasnt been modified yet
                            {
                                depthField.SetValue(x, z, depth);
                                //rg.ColorPixel(x, z, 1, greenColor);
                                pathMark.SetValue(x, z, 2);//corner
                                distField.SetValue(x, z, distance);
                                //Debug.Log(depth);
                            }
                            else if (depthField.IsDefined(x, z)) //has been modified but maybe badly
                            {
                                if (distance < distField.GetValue(x, z))
                                {
                                    depthField.SetValue(x, z, depth);
                                    distField.SetValue(x, z, distance);
                                    pathMark.SetValue(x, z, 2);
                                }
                                //depthField[x, z] = Math.Min(depthField[x, z], depth);
                                //depthField[x, z] = (depthField[x, z] + depth)/2; //blbost
                                //ColorPixel(x, z, 1,redColor);
                                //Debug.Log(x + "," + z);

                            }

                        }
                        else
                        {
                            //ColorPixel(x, z, 0, greenColor);
                        }
                    }
                }

            }
            counter = 0;
        }
        

        //filter result
        
        /*
        float[,] filtField = new float[lt.terrainWidth, lt.terrainHeight];
        for (int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 0; z < lt.terrainHeight; z++)
            {
                if (depthField.IsDefined(x, z))
                {
                    if (Double.IsNaN(ftm.GetLocalMedian(x, z, 2, depthField)) && counter < 10)
                    {
                        Debug.Log("[" + x + "," + z + "]: NaN");
                        counter++;
                    }
                    filtField[x, z] += ftm.GetLocalMedian(x,z,2, depthField);
                }
            }
        }
        counter = 0;
        
        //apply digging

        for (int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 0; z < lt.terrainHeight; z++)
            {
                if (depthField.IsDefined(x, z))
                {
                    //vertices[x, z].y += filtField[x, z] * depthFactor;
                    //if(Double.IsNaN(filtField[x, z] * depthFactor) && counter < 10)
                    //{
                    //    Debug.Log("[" + x + "," + z + "]: NaN");
                    //    counter++;
                    //}

                    //lc.SetLocalValue(x, z, filtField[x, z] * depthFactor, false, globalRiverC);
                    globalRiverC.SetValue(x, z, depthField.GetValue(x, z));
                }
            }
        }
        /*
        for (int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 50; z < 70; z++)
            {
                if (depthField[x, z] != 666)
                {
                    //vertices[x, z].y += filtField[x, z] * depthFactor;

                    lc.SetLocalValue(x, z, filtField[x, z] * depthFactor, true, globalRiverC);
                }
            }
        }

        //rg.terrain.build();


        //ColorPixel(20, 20, 0, greenColor);
        //color digging

        //pathmark:
        //  1 = river
        //  2 = corner

        for (int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 0; z < lt.terrainHeight; z++)
            {
                if (pathMark.GetValue(x, z) == 1)
                {
                    //fd.ColorPixel(x, z, 0, fd.redColor);
                }
                else if (pathMark.GetValue(x, z) == 2)
                {
                    //fd.ColorPixel(x, z, 0, fd.greenColor);
                }
            }
        }
        


    }
    */
    /*
    public void DigRiver(List<Vertex> path, int width, float depthFactor)
    {


        float[,] depthField = new float[lt.terrainWidth, lt.terrainHeight]; //depth to dig
        float[,] distField = new float[lt.terrainWidth, lt.terrainHeight]; //distance from line
        float[,] pathMark = new float[lt.terrainWidth, lt.terrainHeight]; //path number which will effect the vertex


        for (int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 0; z < lt.terrainHeight; z++)
            {
                depthField[x, z] = 666;//mark
                distField[x, z] = 666;//mark
            }
        }

        //should be min 2
        int areaFactor = 2;

        List<float> widthInPoints = AssignWidthToPoints(path, width);


        for (int i = 0; i < path.Count - 1; i++)
        {
            Vertex v1 = path[i];
            Vertex v2 = path[i + 1];
            float width1 = widthInPoints[i];
            float width2 = widthInPoints[i + 1];


            Vertex topLeftCorner = new Vertex(0, 0);
            Vertex botRightCorner = new Vertex(0, 0);
            //evaluate position of affected area
            if (v1.x < v2.x)
            {
                topLeftCorner.x = (int)(v1.x - width1 * areaFactor);
                botRightCorner.x = (int)(v2.x + width2 * areaFactor);
            }
            else
            {
                topLeftCorner.x = (int)(v2.x - width2 * areaFactor);
                botRightCorner.x = (int)(v1.x + width1 * areaFactor);
            }
            if (v1.z < v2.z)
            {
                topLeftCorner.z = (int)(v2.z + width2 * areaFactor);
                botRightCorner.z = (int)(v1.z - width1 * areaFactor);
            }
            else
            {
                topLeftCorner.z = (int)(v1.z + width1 * areaFactor);
                botRightCorner.z = (int)(v2.z - width2 * areaFactor);
            }

            if (topLeftCorner.Equals(botRightCorner))
            {
                Debug.Log("same vertices");
                break;
            }


            for (int x = topLeftCorner.x; x < botRightCorner.x; x++)
            {
                for (int z = topLeftCorner.z; z > botRightCorner.z; z--)
                {
                    if (ftm.CheckBounds(x, z) && fmc.BelongsToPath(x, z, v1, v2, 2 * width * areaFactor))
                    {

                        bool valid = true;
                        Vertex vert = new Vertex(x, z);
                        float distance = fmc.GetDistanceFromLine(vert, v1, v2);

                        float depth = 0;

                        if (distance == 0) //sinc is not defined at 0
                            distance += 0.01f;

                        //determine width (interpolation between points)
                        float localWidth = GetLocalWidth(vert, v1, v2, width1, width2);
                        //float localWidth = 1;
                        if (localWidth < width)
                        {
                            //Debug.Log("!");
                        }

                        if (distance > areaFactor * localWidth)
                        {
                            valid = false;
                        }

                        if (Double.IsNaN(MySinc(distance, localWidth, depthFactor)) && counter < 10)
                        {
                            Debug.Log("[" + x + "," + z + "]: NaN");
                            counter++;
                        }
                        depth = MySinc(distance, localWidth, depthFactor);


                        if (valid && depthField[vert.x, vert.z] == 666) //hasnt been modified yet
                        {
                            depthField[vert.x, vert.z] = depth;
                            distField[vert.x, vert.z] = distance;
                            pathMark[vert.x, vert.z] = 1;//path
                            //widthFactor += 0.0003f;
                        }
                        else if (valid && depthField[vert.x, vert.z] != 666) //has been modified but I can dig it better (valid shouldnt be neccessary)
                        {
                            if (distance < distField[vert.x, vert.z])
                            {
                                //depthField[vert.x, vert.z] = Math.Min(depthField[vert.x, vert.z], depth);
                                depthField[vert.x, vert.z] = depth;
                                distField[vert.x, vert.z] = distance;
                                //widthFactor += 0.0003f;
                            }
                            //depthField[vert.x, vert.z] = (depthField[vert.x, vert.z] + depth)/2;
                        }


                    }
                }
            }
            counter = 0;


        }


        //fix corners

        for (int i = 0; i < path.Count; i++)
        {
            Vertex corner = path[i];
            //Debug.Log(corner);
            for (int x = corner.x - 2 * areaFactor * width; x < corner.x + 2 * areaFactor * width; x++)
            {
                for (int z = corner.z - 2 * areaFactor * width; z < corner.z + 2 * areaFactor * width; z++)
                {
                    if (ftm.CheckBounds(x, z))
                    {
                        float distance = fmc.GetDistance(corner, new Vertex(x, z));
                        //if (i == 10)
                        //Debug.Log(i+":"+distance);

                        if (distance < areaFactor * widthInPoints[i])
                        {
                            float depth = 0;

                            if (distance == 0) //sinc is not defined at 0
                                distance += 0.01f;

                            if (Double.IsNaN(MySinc(distance, widthInPoints[i], depthFactor)) && counter < 10)
                            {
                                Debug.Log("[" + x + "," + z + "]: NaN");
                                counter++;
                            }
                            depth = MySinc(distance, widthInPoints[i], depthFactor);


                            if (depthField[x, z] == 666) //hasnt been modified yet
                            {
                                depthField[x, z] = depth;
                                //rg.ColorPixel(x, z, 1, greenColor);
                                pathMark[x, z] = 2;//corner
                                distField[x, z] = distance;
                                //Debug.Log(depth);
                            }
                            else if (depthField[x, z] != 666) //has been modified but maybe badly
                            {
                                if (distance < distField[x, z])
                                {
                                    depthField[x, z] = depth;
                                    distField[x, z] = distance;
                                    pathMark[x, z] = 2;
                                }
                                //depthField[x, z] = Math.Min(depthField[x, z], depth);
                                //depthField[x, z] = (depthField[x, z] + depth)/2; //blbost
                                //ColorPixel(x, z, 1,redColor);
                                //Debug.Log(x + "," + z);

                            }

                        }
                        else
                        {
                            //ColorPixel(x, z, 0, greenColor);
                        }
                    }
                }

            }
            counter = 0;
        }


        //filter result


        float[,] filtField = new float[lt.terrainWidth, lt.terrainHeight];
        for (int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 0; z < lt.terrainHeight; z++)
            {
                if (depthField[x, z] != 666)
                {
                    if (Double.IsNaN(ftm.GetLocalMedian(x, z, 2, depthField)) && counter < 10)
                    {
                        Debug.Log("[" + x + "," + z + "]: NaN");
                        counter++;
                    }
                    filtField[x, z] += ftm.GetLocalMedian(x, z, 2, depthField);
                }
            }
        }
        counter = 0;

        //apply digging

        for (int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 0; z < lt.terrainHeight; z++)
            {
                if (depthField[x, z] != 666)
                {
                    //vertices[x, z].y += filtField[x, z] * depthFactor;
                    if (Double.IsNaN(filtField[x, z] * depthFactor) && counter < 10)
                    {
                        Debug.Log("[" + x + "," + z + "]: NaN");
                        counter++;
                    }

                    lc.SetLocalValue(x, z, filtField[x, z] * depthFactor, false, globalRiverC);
                }
            }
        }
        
        //for (int x = 0; x < lt.terrainWidth; x++)
        //{
        //    for (int z = 50; z < 70; z++)
        //    {
        //        if (depthField[x, z] != 666)
        //        {
        //            //vertices[x, z].y += filtField[x, z] * depthFactor;

        //            lc.SetLocalValue(x, z, filtField[x, z] * depthFactor, true, globalRiverC);
        //        }
        //    }
        //}

        //rg.terrain.build();


        //ColorPixel(20, 20, 0, greenColor);
        //color digging

        //pathmark:
        //  1 = river
        //  2 = corner

        for (int x = 0; x < lt.terrainWidth; x++)
        {
            for (int z = 0; z < lt.terrainHeight; z++)
            {
                if (pathMark[x, z] == 1)
                {
                    //fd.ColorPixel(x, z, 0, fd.redColor);
                }
                else if (pathMark[x, z] == 2)
                {
                    //fd.ColorPixel(x, z, 0, fd.greenColor);
                }
            }
        }



    }
*/
    int counter = 0;

    public float MySinc(float x, float width, float depth)
    {
        if(x == 0 || width == 0)
        {
            return -depth;
        }
        double r = -(depth/ Math.PI) * Math.Sin(x * Math.PI / width) / x * width;
        if (Double.IsNaN(r) && counter < 10)
        {
            Debug.Log("NaN");
            counter++;
        }
        return (float)r;
        /*
        return (float)(-
            (depth/Math.PI)
            *Math.Sin((x / (width* widthFactor / Math.PI)))
            /(x / Math.PI))
            *(float)(width* widthFactor / Math.PI);
            */
    }

}
