﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HydraulicErosion  {
    public GlobalCoordinates sedimentMap;
    public GlobalCoordinates waterMap;
    public GlobalCoordinates stepMap;

    public ErosionGenerator eg;

    public FunctionTerrainManager ftm;

    public HydraulicErosion(ErosionGenerator erosionManager)
    {
        eg = erosionManager;

        sedimentMap = new GlobalCoordinates(100);
        waterMap = new GlobalCoordinates(100);
        stepMap = new GlobalCoordinates(100);
    }

    public void AssignFunctions(FunctionTerrainManager functionTerrainManager)
    {
        ftm = functionTerrainManager;
    }

    /// <summary>
    /// increses amount of water on given area max by given value
    /// </summary>
    public void DistributeWater(Area area, float maxWaterIncrease)
    {
        float value;
        for(int x = area.botLeft.x; x < area.topRight.x; x++)
        {
            for (int z = area.botLeft.z; z < area.topRight.z; z++)
            {
                value = GetWaterValue(x, z) + Random.Range(0, maxWaterIncrease);
                waterMap.SetValue(x, z, value);
                if(counter < 20 && value >= 600)
                {
                    Debug.Log(x + "," + z);
                    Debug.Log(value);
                    Debug.Log(GetWaterValue(x,z));
                    counter++;
                }
            }
        }
    }
    int counter = 0;

    /// <summary>
    /// perform 1 step number 'stepNumber' of hydraulic erosion on given area
    /// </summary>
    public void HydraulicErosionStep(Area area, int stepNumber, float outflowAmount)
    {
        bool valueChanged = false;
        float waterSum = 0;
        float e = 0.01f;
        for (int x = area.botLeft.x; x < area.topRight.x; x++)
        {
            for (int z = area.botLeft.z; z < area.topRight.z; z++)
            {
                Vertex current = new Vertex(x, z);
                float currentVal = GetTerrainWatterValue(x, z);
                float currentWater = GetWaterValue(x, z);

                //perform step only if required step hasn't already been processed here
                if (GetStepValue(x,z) < stepNumber && currentWater != 0) //and some water is present
                {
                    List<Vertex> neighbours = ftm.Get8Neighbours(current, 1);
                    //var rnd = new System.Random();
                    //var neighbours = neighbours0.OrderBy(item => rnd.Next());

                    
                    List<float> outflowList = new List<float>();
                    float outflowSum = 0;

                    foreach (Vertex n in neighbours)
                    {                        
                        float neighbourVal = GetTerrainWatterValue(n.x, n.z);
                        if (currentVal > (neighbourVal + e) && area.Contains(n)) //current is higher that neighbour
                        {
                            float dif = currentVal - neighbourVal;
                            dif /= 2;
                            if(dif > currentWater)
                            {
                                dif = currentWater;
                            }
                            outflowList.Add(dif);
                            outflowSum += dif;
                            //if (MoveWaterFromTo(current, n, dif))
                            //  valueChanged = true;
                        }
                        else
                        {
                            outflowList.Add(0);
                        }
                    }
                    float ratio = outflowSum / currentWater;

                    foreach (Vertex n in neighbours)
                    {
                        if (area.Contains(n) && outflowSum != 0)
                        {
                            float outflow = outflowList[neighbours.IndexOf(n)];
                            if(ratio > 1)
                                outflow /= ratio;

                            if (outflow != 0)
                            {
                                if (MoveWaterFromTo(current, n, outflow))
                                    valueChanged = true;
                            }
                        }
                    }


                    stepMap.SetValue(x, z, GetStepValue(x, z) + 1);
                }
            }
        }
        /*
        for (int x = area.botLeft.x; x < area.topRight.x; x++)
        {
            for (int z = area.botLeft.z; z < area.topRight.z; z++)
            {
                waterSum += GetWaterValue(x, z);
            }
        }*/
        int _x = -50;
        int _z = 50;
        //Debug.Log(eg.lt.globalTerrainC.GetValue(_x, _z));
        //Debug.Log(eg.GetTerrainValue(_x, _z));
        //Debug.Log(eg.GetTerrainValue(_x, _z) + GetWaterValue(_x, _z));
        //Debug.Log(GetWaterValue(_x, _z));
        
        if (valueChanged)
        {
            //Debug.Log("CHANGE");
        }
        else
            Debug.Log("nothing");
    }

    /// <summary>
    /// move waterAmount of water from v1 to v2
    /// calculate how much sediment does water carry and move this sediment from v1 to v2
    /// returns false if no change occured
    /// </summary>
    public bool MoveWaterFromTo(Vertex v1, Vertex v2, float waterAmount)
    {
        //if (counter < 10)
        //{
        //   Debug.Log("move: " + waterAmount);
        //    Debug.Log("from " + v1);
        //    Debug.Log("to " + v2);
        //    Debug.Log("waterAmount " + waterAmount);
        //    Debug.Log("GetWaterValue1 " + v2);
        //    counter++;
        //}

        if (waterAmount > GetWaterValue(v1.x, v1.z))
            waterAmount = GetWaterValue(v1.x, v1.z);

        if (counter < 10)
        {
            //Debug.Log("acually: " + waterAmount);
            counter++;
        }

        float sum1 = GetWaterValue(v1.x, v1.z) + GetWaterValue(v2.x, v2.z);

        waterMap.SetValue(v1.x, v1.z, GetWaterValue(v1.x, v1.z) - waterAmount);
        waterMap.SetValue(v2.x, v2.z, GetWaterValue(v2.x, v2.z) + waterAmount);

        float sum2 = GetWaterValue(v1.x, v1.z) + GetWaterValue(v2.x, v2.z);

        //if(sum1 != sum2 && counter < 10)
        //{
        //    Debug.Log("!");
        //    Debug.Log(sum1);
        //    Debug.Log(sum2);
        //    Debug.Log(waterAmount);
        //    counter++;
        //}


        //if (counter < 10)
        //{
        //    Debug.Log("result: " + waterMap.GetValue(v1.x, v1.z));
        //}

        float sedimentAmount = GetSedimentAmount(waterAmount);
        //sedimentMap.SetValue(v1.x, v1.z, GetSedimentValue(v1.x, v1.z) - sedimentAmount);
        //sedimentMap.SetValue(v2.x, v2.z, GetSedimentValue(v2.x, v2.z) + sedimentAmount);
        if (waterAmount > 0)
            return true;
        else
            return false;
    }

    public float GetSedimentAmount(float waterAmount)
    {
        if (waterAmount == 0)
            return 0;
        return waterAmount / 8 + Random.Range(0, 0.05f); //TODO: control sediment amount
    }


    /// <summary>
    /// returns sum of filtered terrain, sediment and water
    /// </summary>
    public float GetTerrainWatterValue(int x, int z) //not sure how to name this:/
    {
        float value = eg.GetTerrainValue(x, z) + GetSedimentValue(x, z) + GetWaterValue(x, z);
        /*if(eg.lt.rg.rivers.Count > 0 && eg.lt.rg.rivers[0].globalRiverC.IsDefined(x, z) && counter < 10)
        {
            Debug.Log(eg.GetTerrainValue(x, z));
            counter++;
        }*/

        if( value > 600  && counter < 50)
        {
            Debug.Log(x + "," + z);
            Debug.Log(eg.GetTerrainValue(x, z));
            Debug.Log(GetSedimentValue(x, z));
            Debug.Log(GetWaterValue(x, z));
            counter++;
        }
        return value;
    }

   
    /// <summary>
    /// returns step value
    /// 0 if not defined
    /// </summary>
    public int GetStepValue(int x, int z)
    {
        int step = (int)stepMap.GetValue(x, z);
        if (step != 666)
            return step;
        else
            return 0;
    }

    /// <summary>
    /// returns sediemnt value
    /// 0 if not defined
    /// </summary>
    public float GetSedimentValue(int x, int z)
    {
        float sediment = sedimentMap.GetValue(x, z);
        if (sediment != 666)
            return sediment;
        else
            return 0;
    }

    /// <summary>
    /// returns water value
    /// 0 if not defined
    /// </summary>
    public float GetWaterValue(int x, int z)
    {
        float water = waterMap.GetValue(x, z);
        if(water > 600 && water != 666 && counter < 30)
        {
            Debug.Log(x + "," + z + " too high " + water);
            counter++;
        }

        if (water != 666)
            return water;
        else
            return 0;
    }


    public string TerrainWaterValuesString(Area area)
    {
        string output = "Terrain + Water values: \n";
        string s = "";
        float value = 0;
        for (int x = area.botLeft.x; x < area.topRight.x; x++)
        {
            for (int z = area.botLeft.z; z < area.topRight.z; z++)
            {
                s = (int)Mathf.Abs(GetTerrainWatterValue(x, z)) + "." +
                    (int)(100 * (Mathf.Abs(GetTerrainWatterValue(x, z)) - (int)Mathf.Abs(GetTerrainWatterValue(x, z))));
                if (s.Length < 4)
                    s += "0";
                if (s.Length > 4)
                    s = s.Substring(0, 4);
                s += "|";
                output += s;
            }
            output += "\n";
        }

        return output;
    }


    public string WaterValuesString(Area area)
    {
        string output = "water values: \n";
        string s = "";
        for (int x = area.botLeft.x; x < area.topRight.x; x++)
        {
            for (int z = area.botLeft.z; z < area.topRight.z; z++)
            {
                s = (int)GetWaterValue(x, z) + "." +
                    (int)(100 * (GetWaterValue(x, z) - (int)GetWaterValue(x, z)));
                if (s.Length < 4)
                    s += "0";
                if (s.Length > 4)
                    s = s.Substring(0, 4);           
                s += "|";
                output += s;
            }
            output += "\n";
        }

        return output;
    }


}
