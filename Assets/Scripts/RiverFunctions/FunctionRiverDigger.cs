using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FunctionRiverDigger {

    public RiverGenerator rg;
    public LocalTerrain lt;

    public LocalCoordinates lc;
    //public GlobalCoordinates globalRiverC;

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
        //globalRiverC = rg.globalRiverC;

        lt = rg.lt;
        fmc = rg.fmc;
        ftm = rg.ftm;
        fd = rg.fd;
    }

    /// <summary>
    /// randomly distort all but first and last path nodes 
    /// </summary>
    public void DistortPath2(List<Vertex> path, int maxDistort)
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
    /// distorts path based on close terrain values
    /// </summary>
    /// <param name="path"></param>
    /// <param name="maxDistort"></param>
    public void DistortPath(List<Vertex> path, float maxDistort, int gridStep)
    {
        for (int i = 1; i < path.Count - 1; i++)
        {
            //do not disturn if nodes are too close to each other
            if(!(Vector3.Distance(path[i], path[i-1]) < (gridStep/2) ||
                Vector3.Distance(path[i], path[i + 1]) < (gridStep / 2)))
            {
                Vertex v = path[i];
                Vector3 orientation = ((Vector3)path[i + 1] - path[i - 1]).normalized;
                Vector3 normal = new Vector3(-orientation.z, 0, orientation.x);
                Vertex v1 = v - normal * maxDistort;
                float d = 0;
                float neighbSum = 666;
                for(float c = 0; c < 2 * maxDistort; c += maxDistort/10)
                {
                    float sum = ftm.GetNeighbourhoodSum(v1 + normal * c, 10, 2);
                    if(sum < neighbSum)
                    {
                        neighbSum = sum;
                        d = c;
                    }
                }
                v.Rewrite((int)(v.x + d * normal.x), (int)(v.z + d * normal.z), v.height);
            }
        }
    }


    /// <summary>
    /// function for determine "strength" of each river path nodes
    /// </summary>
    /*public List<float> AssignWidthToPoints(List<Vertex> path, float minWidth)
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
    */


    private float GetLocalWidth(Vertex point, Vertex v1, Vertex v2,float w1,float w2)
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
    public void DigRiver(RiverInfo river)
    {
        //DigRiver(river, 15, 1, 0.6f);
        DigRiver(river, (int)river.width, river.areaEffect, river.depth);
    }

    RiverInfo currentRiver; //river which is being digged

    /// <summary>
    /// digs river path
    /// </summary>
    /// <param name="width">width of river corridor</param>
    /// <param name="widthFactor">also "areaEffect". defines area around river. 1 = only river, 2 = river and close area</param>
    /// <param name="maxDepth">depth in center of river</param>
    public void DigRiver(RiverInfo river, int width, float widthFactor, float maxDepth)
    {
        //optimize areaEffect
        if (widthFactor > 1 && widthFactor < 1.5f)
            widthFactor = 1;
        else if (widthFactor >= 1.5)
            widthFactor = 2;

        widthFactor = 2;//for current depth function (aTan) it is neccesarry to have bigger area effect

        //DigCorners(path, width, widthFactor, maxDepth);
        currentRiver = river;
        List<Vertex> path = river.riverPath;

        if(path.Count < 2)
        {
            Debug.Log("path too short: " + path.Count);
            return;
        }

        //Debug.Log("digging: " + currentRiver);
        //Debug.Log("lowest: " + currentRiver.lowestPoint);

        DigRiverPath(path, width, widthFactor, maxDepth);

        DigCorners(path, width, widthFactor, maxDepth);
    }

    /// <summary>
    /// digs river path (not corners)
    /// </summary>
    private void DigRiverPath(List<Vertex> path, int width, float widthFactor, float maxDepth)
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
    private void DigRiverPart(Vertex v1, Vertex v2,Vertex previous, Vertex next, int width, float widthFactor, float maxDepth)
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

                if (lt.globalTerrainC.IsDefined(x, z)&& distance < widthFactor * width && distanceFromCorners <= cornersDistance)
                {
                    float depth = GetDepth(lt.globalTerrainC.GetValue(x, z), distance, width, maxDepth);
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

                    if (depth < currentRiver.globalRiverC.GetValue(x, z) && 
                        (cornersDistance - distanceFromCorners) >= (PrevNextDistance - distanceFromPrevNext))
                    {
                        currentRiver.globalRiverC.SetValue(x, z, depth);
                        //globalRiverC.SetValue(x, z, -5);
                    }
                }
            }
        }
    }

    /// <summary>
    /// dig corners
    /// </summary>
    private void DigCorners(List<Vertex> path, int width, float widthFactor, float depth)
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
    private void DigCorner(Vertex corner, Vertex previous, Vertex next, int width, float widthFactor, float maxDepth)
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

                if (lt.globalTerrainC.IsDefined(x, z) && distance < widthFactor * width)// && distance > distancePrevious && distance > distanceNext)
                {
                    float pointPreviousDistance = fmc.GetDistance(point, previous);
                    float pointNextDistance = fmc.GetDistance(point, next);

                    if (pointPreviousDistance  >= cornerPreviousDistance && 
                        pointNextDistance >= cornerNextDistance) {
                        float depth = GetDepth(lt.globalTerrainC.GetValue(x, z), distance, width, maxDepth);
                        //if (depth < globalRiverC.GetValue(x, z))
                        if(!currentRiver.globalRiverC.IsDefined(x,z) || depth < currentRiver.globalRiverC.GetValue(x, z))
                        {
                            currentRiver.globalRiverC.SetValue(x, z, depth);
                            //globalRiverC.SetValue(x, z, -5);
                        }
                    }
                }
            }
        }
    }



    /// <summary>
    /// calls depth function (currently MySinc)
    /// </summary>
    private float GetDepth(float origHeight, float distance, float width, float maxDepth)
    {
        float dif = origHeight - currentRiver.lowestPoint.height;
        if (dif < 0)
            dif = 0;

        if (currentRiver.lowestPoint.height == 666 && counter < 10)
        {
            //Debug.Log("?");
            counter++;
        }
        if (origHeight == 666 && counter < 10)
        {
            Debug.Log("!");
            counter++;
        }

        //return MySinc(distance, width, (dif + 1)*maxDepth);
        return MyArctan(distance, width, (dif + 1) * maxDepth);
    }
    
    int counter = 0;

    private float MySinc(float x, float width, float depth)
    {
        if(x == 0 || width == 0)
        {
            return -depth;
        }
        double r = -(depth/ Math.PI) * (Math.Sin(x * Math.PI / width) / x) * width;

        if(counter < 10)
        {
            Debug.Log(x + "," + width + "," + depth + " = " + r);
            counter++;
        }

        /*if (Double.IsNaN(r) && counter < 10)
        {
            Debug.Log("NaN");
            counter++;
        }*/
        return (float)r;
        /*
        return (float)(-
            (depth/Math.PI)
            *Math.Sin((x / (width* widthFactor / Math.PI)))
            /(x / Math.PI))
            *(float)(width* widthFactor / Math.PI);
            */
    }

    private float MyArctan(float x, float width, float depth)
    {
        return depth * Mathf.Atan(Mathf.Abs(x) - width / 2) - depth * (Mathf.PI / 2);
    }

}
