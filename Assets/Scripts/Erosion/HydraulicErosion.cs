﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HydraulicErosion  {
    public GlobalCoordinates hydraulicErosionMap;
    public GlobalCoordinates sedimentMap;
    public GlobalCoordinates waterMap;
    //public GlobalCoordinates outflowMap;//map of water value which will flow out of vertex in next iteration
    //public GlobalCoordinates inflowMap;//map of water value which will flow into vertex in next iteration
    public GlobalCoordinates stepMap;

    public GlobalCoordinates outflowTop;
    public GlobalCoordinates outflowRight;
    public GlobalCoordinates outflowBot;
    public GlobalCoordinates outflowLeft;

    public GlobalCoordinates velocityX;
    public GlobalCoordinates velocityZ;


    public ErosionGenerator eg;
    public LayerManager lm;
    public SpikeFilter sf;

    public FunctionTerrainManager ftm;

    public HydraulicErosion(ErosionGenerator erosionGenerator)
    {
        eg = erosionGenerator;
        lm = eg.lt.lm;

        hydraulicErosionMap = new GlobalCoordinates(100);
        sedimentMap = new GlobalCoordinates(100);
        waterMap = new GlobalCoordinates(100);
        stepMap = new GlobalCoordinates(100);
        //inflowMap = new GlobalCoordinates(100);
        outflowTop = new GlobalCoordinates(100);
        outflowRight = new GlobalCoordinates(100);
        outflowBot = new GlobalCoordinates(100);
        outflowLeft = new GlobalCoordinates(100);

        velocityX = new GlobalCoordinates(100);
        velocityZ = new GlobalCoordinates(100);

        erosionEffect = new HashSet<Vertex>();
        outflowField = new HashSet<Vertex>();
        noWater = new HashSet<Vertex>();
    }

    public void AssignFunctions(FunctionTerrainManager functionTerrainManager, SpikeFilter spikeFilter)
    {
        ftm = functionTerrainManager;
        sf = spikeFilter;
    }

    /// <summary>
    /// hot-fix
    /// filter erosion deffect with spike filter
    /// </summary>
    public void FilterErosionIn(Area area, float epsilon)
    {
        sf.FilterMapInRegion(hydraulicErosionMap, area, epsilon);
    }

    public void FloodRiver(RiverInfo river)
    {
        float value = 0.1f;
        foreach(Vertex v in river.riverPath)
        {
            value = waterMap.GetValue(v.x, v.z, 0) + value;
            if (value < maxOutflow * 4) {
                waterMap.SetValue(v.x, v.z, value);
                erosionEffect.Add(v);
            }
        }
    }

    /// <summary>
    /// increses amount of water on given area max by given value
    /// </summary>
    public void DistributeWater(Area area,int stepNumber, float maxWaterIncrease)
    {
        int x = Random.Range(area.botLeft.x, area.topRight.x);
        int z = Random.Range(area.botLeft.z, area.topRight.z);
        //x = 0;z = 0;

        float value = GetWaterValue(x, z) + Random.Range(0, maxWaterIncrease);
        value = GetWaterValue(x, z) + maxWaterIncrease;
        if (value < maxOutflow * 4)
        {
            
            //x = 0;
            //z = 0;
            waterMap.SetValue(x, z, value);
            erosionEffect.Add(new Vertex(x, z));
        }
    }
    int counter = 0;


    float e = 0.01f;

    public void HydraulicErosionStep()
    {
        //area, waterViscosity, strength,deposition, evaporation, windX, windZ, windStrength, windAngle
        HydraulicErosionStep(0.25f, 0.2f, 0.2f, 0.2f,0,0,100,-1);
    }

    /// <summary>
    /// perform 1 step number 'stepNumber' of hydraulic erosion on given area
    /// </summary>
    public void HydraulicErosionStep(float waterViscosity, float strength,float deposition, float evaporation, int windX, int windZ, float windStrength, float windAngle)
    {
        // Kc = water viscosity
        // Ks = strength or (1.0 - terrainDensity)
        // Kd = deposition factor
        // Ke = evaporation speed
        // Kr = droplet weight
        // G  = gravity

        //DistributeWater(area, stepNumber, 0.1f);
        UpdateOutflow(9.81f, windX, windZ, windStrength, windAngle);
        UpdateSediment(waterViscosity, strength, deposition);
        EvaporateWater(evaporation);
        //UpdateStepNumber(area, -666);
    }

    float T = 0.1f;  //delta time
    float L = 1.0f;  //pipe cross-sectional area
    float A = 1.0f;  //cell area
    //int wind_x = -1; //-1,0,1
    //int wind_z = 0; //-1,0,1
    //int windStrength = 10;
    //float windCoverage = -1f; //<-1,1>

    public HashSet<Vertex> erosionEffect;
    public HashSet<Vertex> outflowField;
    public HashSet<Vertex> noWater;


    public void UpdateOutflow(float G,
        int windX, int windZ, float windStrength, float windCoverage)
    {
        // Kc = water viscosity
        // Ks = strength or (1.0 - terrainDensity)
        // Kd = deposition factor
        // Ke = evaporation speed
        // Kr = droplet weight
        // G  = gravity
        
        foreach (Vertex v in erosionEffect)
        {
            int x = v.x;
            int z = v.z;

            // Initialise flux variables
            float fluxL, fluxB, fluxR, fluxT;
            float windForce = 0;
            // Get current point height
            float currentH = GetTerrainWatterValue(x, z);
            float windPotentialX = GetWindStrength(windStrength * windX, currentH, x, z);
            float windPotentialZ = GetWindStrength(windStrength * windZ, currentH, x, z);
            Vertex current = new Vertex(x, z);

            //TOP
            float hN = GetTerrainWatterValue(x, z + 1);
            float deltaH = currentH - hN;
            windForce = 0;
            if (windZ > 0 && deltaH > windCoverage)
                windForce = windPotentialZ;

            fluxT = Mathf.Max(0.0f, GetOutflowValue(x, z, Direction.top) + T * A * ((G * deltaH + windForce) / L));

            //RIGHT
            hN = GetTerrainWatterValue(x + 1, z);
            deltaH = currentH - hN;
            windForce = 0;
            if (windX > 0 && deltaH > windCoverage)
                windForce = windPotentialX;
            fluxR = Mathf.Max(0.0f, GetOutflowValue(x, z, Direction.right) + T * A * ((G * deltaH + windForce) / L));
            /*if (x == 4 && counter < 10)
            {
                Debug.Log("currentH:"+currentH);
                Debug.Log("hN:" + hN);
                Debug.Log("deltaH:" + deltaH);
                Debug.Log("fluxR:" + fluxR);
                counter++;
            }*/

            //BOT
            hN = GetTerrainWatterValue(x, z - 1);
            deltaH = currentH - hN;
            windForce = 0;
            if (windZ < 0 && deltaH > windCoverage)
                windForce = windPotentialZ;

            fluxB = Mathf.Max(0.0f, GetOutflowValue(x, z, Direction.bot) + T * A * ((G * deltaH + windForce) / L));

            //LEFT
            hN = GetTerrainWatterValue(x - 1, z);
            deltaH = currentH - hN;
            windForce = 0;
            if (windX < 0 && deltaH > windCoverage)
                windForce = windPotentialX;

            fluxL = Mathf.Max(0.0f, GetOutflowValue(x, z, Direction.left) + T * A * ((G * deltaH + windForce) / L));


            //If the sum of the outflow flux exceeds the water amount of the cell,
            //the flux value will be scaled down by a factor K to avoid negative
            //updated water height


            

            float K;
            if ((fluxL + fluxR + fluxT + fluxB) * T > GetWaterValue(x, z))
                K = Mathf.Min(1.0f, GetWaterValue(x, z) / ((fluxL + fluxR + fluxT + fluxB) * T));
            else
                K = 1;

            //Debug.Log(fluxT);
            //Debug.Log(fluxR);
            //Debug.Log(fluxB);
            //Debug.Log(fluxL);

            fluxT = Mathf.Min(maxOutflow, fluxT*K);
            fluxR = Mathf.Min(maxOutflow, fluxR * K);
            fluxB = Mathf.Min(maxOutflow, fluxB * K);
            fluxL = Mathf.Min(maxOutflow, fluxL*K);


            //prevent too high values
            outflowTop.SetValue(x, z, fluxT * K);
            outflowRight.SetValue(x, z, fluxR * K);
            outflowBot.SetValue(x, z, fluxB * K);
            outflowLeft.SetValue(x, z, fluxL * K);

            if (fluxT > 0)
                outflowField.Add(new Vertex(x, z + 1));
            if (fluxR > 0)
                outflowField.Add(new Vertex(x+1, z ));
            if (fluxB > 0)
                outflowField.Add(new Vertex(x, z - 1));
            if (fluxL > 0)
                outflowField.Add(new Vertex(x-1, z));
        }

        //Debug.Log(erosionEffect.Count);
        foreach (Vertex v in outflowField)
        {
            erosionEffect.Add(v);
        }
        //outflowField.Union(erosionEffect); //??? why not working?
        outflowField.Clear();
        //Debug.Log(erosionEffect.Count);
    }

    float maxOutflow = 0.5f;

    public void UpdateSediment(float Kc, float Ks, float Kd)
    {
        float waterSum = 0;

        //Debug.Log(erosionEffect.Count);
        //Debug.Log("---");

        foreach (Vertex v in erosionEffect)
        {
            
            //Debug.Log(v);

            int x = v.x;
            int z = v.z;

            float inL, inR, inB, inT;
            inT = GetOutflowValue(x, z + 1, Direction.bot);
            inR = GetOutflowValue(x + 1, z, Direction.left);
            inB = GetOutflowValue(x, z - 1, Direction.top);
            inL = GetOutflowValue(x - 1, z, Direction.right);


            //Compute inflow and outflow for velocity update
            float fluxIN = inL + inR + inB + inT;
            float fluxOUT = GetOutflowValue(x, z, Direction.top);
            fluxOUT += GetOutflowValue(x, z, Direction.right);
            fluxOUT += GetOutflowValue(x, z, Direction.bot);
            fluxOUT += GetOutflowValue(x, z, Direction.left);

            //V is net volume change for the water over time
            float V = Mathf.Max(0.0f, T * (fluxIN - fluxOUT));
            waterMap.SetValue(x, z, GetWaterValue(x, z) + V / (L * L));

            float velocity_x = inL - GetOutflowValue(x, z, Direction.left)
                + GetOutflowValue(x, z, Direction.right) - inR;
            float velocity_z = inT - GetOutflowValue(x, z, Direction.top)
                + GetOutflowValue(x, z, Direction.bot) - inB;

            Vector2 velocity = new Vector2(velocity_x, velocity_z);
            //velocity /= 2;

            // Compute maximum sediment capacity
            float C = Kc * 10 * velocity.magnitude * GetSlope(x, z);

            float currentSediment = GetSedimentValue(x, z);
            float currentHydraulicErosion = GetHydraulicErosionValue(x, z);

            float KS = Mathf.Max(0, Ks * (C - currentSediment));
            float KD = Mathf.Max(0, Kd * (currentSediment - C));

            //KS /= Mathf.Max(1+currentHydraulicErosion, 1);
            //KD /= Mathf.Max(1+currentHydraulicErosion, 1);

            if (KS != 0 && (C > currentSediment) && GetWaterValue(x, z) > currentSediment)
            {
                float newVal = currentHydraulicErosion - KS / Mathf.Max(currentHydraulicErosion, 1);

                hydraulicErosionMap.SetValue(x, z, currentHydraulicErosion - KS);
                //hydraulicErosionMap.SetValue(x, z, newVal);
                sedimentMap.SetValue(x, z, currentSediment + KS);
            }
            else if (KD != 0)
            {
                //Debug.Log(KD);

                hydraulicErosionMap.SetValue(x, z, currentHydraulicErosion + KD);
                sedimentMap.SetValue(x, z, currentSediment - KD);
            }

            sedimentMap.SetValue(x, z,
                GetSedimentValue((int)(x - velocity.x), (int)(z - velocity.y)));

            waterSum += GetWaterValue(x, z);                 
        }
        
        
        //Debug.Log("waterSum: " + waterSum);
    }

    
    public void EvaporateWater(float evaporation)
    {
        if (evaporation > 1)
            evaporation = 1;

        foreach (Vertex v in erosionEffect)
        {
            int x = v.x;
            int z = v.z;
            float newVal = GetWaterValue(x, z) * (1 - evaporation*evaporation);
            if (newVal < 0.01f)
            {
                newVal = 0;
            }
            waterMap.SetValue(x, z, newVal);
            if (newVal < 0)
            {
                Debug.Log("!");
            }
            if (GetWaterValue(x, z) <= 0)
            {
                noWater.Add(v);
                //Debug.Log("no water " + v);
            }
        }
        foreach (Vertex v in noWater)
        {
            erosionEffect.Remove(v);
        }
        noWater.Clear();
    }
    
    

    private float GetSlope(int x, int z)
    {
        //float topN = eg.GetTerrainValue(x, z + 1);
        //float rightN = eg.GetTerrainValue(x + 1, z);
        //float botN = eg.GetTerrainValue(x, z - 1);
        //float leftN = eg.GetTerrainValue(x - 1, z);

        float topN = eg.GetTerrainValue(x, z + 1) + GetHydraulicErosionValue(x,z+1);
        float rightN = eg.GetTerrainValue(x + 1, z)+ GetHydraulicErosionValue(x + 1, z);
        float botN = eg.GetTerrainValue(x, z - 1)+ GetHydraulicErosionValue(x, z - 1);
        float leftN = eg.GetTerrainValue(x - 1, z)+ GetHydraulicErosionValue(x - 1, z);

        float currentH = eg.GetTerrainValue(x, z) + GetHydraulicErosionValue(x, z);
        //to prevent too high slopes
        if ((topN+rightN+botN+leftN)/4 > currentH + e)
        {
            if (counter < 20)
            {
                counter++;
                //Debug.Log(x + "," + z + ": " + currentH);
            }
            return 0;
        }

        //if (!area.Contains(new Vertex(x, z + 1)))
        //    topN = eg.GetTerrainValue(x, z);
        //if (!area.Contains(new Vertex(x + 1, z)))
        //    rightN = eg.GetTerrainValue(x, z);
        //if (!area.Contains(new Vertex(x, z - 1)))
        //    botN = eg.GetTerrainValue(x, z);
        //if (!area.Contains(new Vertex(x - 1, z)))
        //    leftN = eg.GetTerrainValue(x, z);

        //Find normal
        Vector3 va = new Vector3(1, rightN - leftN, 0);
        Vector3 vb = new Vector3(0, topN - botN, 1);
        //Vector3 n = Vector3.Cross(va.normalized, vb.normalized);
        Vector3 n = Vector3.Cross(va, vb);

        //float val = Mathf.Max(0.05f, 1.0f - Mathf.Abs(Vector3.Dot(n, new Vector3(0, 1, 0))));
        float val = Vector3.Dot(n.normalized, new Vector3(0, 1, 0));
        //Debug.Log(x + "," + z + " : " + val);
        //Return dot product of normal with the Y axis
        return 10*(1-Mathf.Abs(val));
    }

    private float GetWindStrength(float strength, float height, int x, int z)
    {
        float slope = GetSlope(x, z);
        //Debug.Log(x + "," + z + " : " + slope);

        //Clamp to 0.005 
        //if (slope < 0.005f) slope = 0.005f;

        //Check if altitude scaling is not on 
        //Set value to null if so, 1
        //if (!windAltitude) height = 1;

        //Return wind potential force
        //return strength * height * slope;
        return Mathf.Abs(strength * slope);
    }

    //---GETTERS---//

    /// <summary>
    /// returns sum of filtered terrain, sediment and water
    /// </summary>
    public float GetTerrainWatterValue(int x, int z) //not sure how to name this:/
    {
        //float value = eg.GetTerrainValue(x, z) + GetHydraulicErosionValue(x, z) + GetWaterValue(x, z);
        float value = lm.GetCurrentHeight(x, z) + GetWaterValue(x, z);
        /*if(eg.lt.rg.rivers.Count > 0 && eg.lt.rg.rivers[0].globalRiverC.IsDefined(x, z) && counter < 10)
        {
            Debug.Log(eg.GetTerrainValue(x, z));
            counter++;
        }*/

        if ( value > 600  && counter < 50)
        {
            Debug.Log(x + "," + z);
            Debug.Log(eg.GetTerrainValue(x, z));
            Debug.Log(GetSedimentValue(x, z));
            Debug.Log(GetWaterValue(x, z));
            counter++;
        }
        return value;
    }
    
    public float GetOutflowValue(int x, int z, Direction direction)
    {
        float outflow = 666;
        switch (direction)
        {
            case Direction.top:
                outflow = outflowTop.GetValue(x, z);
                break;
            case Direction.right:
                outflow = outflowRight.GetValue(x, z);
                break;
            case Direction.bot:
                outflow = outflowBot.GetValue(x, z);
                break;
            case Direction.left:
                outflow = outflowLeft.GetValue(x, z);
                break;
        }
        
        if (outflow != 666)
            return outflow;
        else
            return 0;
    }
    
    /// <summary>
    /// returns step value
    /// 0 if not defined
    /// </summary>
    public int GetStepValue(int x, int z)
    {
        return (int)stepMap.GetValue(x, z, 0);
    }
        
    /// <summary>
    /// returns sediemnt value
    /// 0 if not defined
    /// </summary>
    public float GetHydraulicErosionValue(int x, int z)
    {
        return hydraulicErosionMap.GetValue(x, z, 0);
    }

    /// <summary>
    /// returns sediemnt value
    /// 0 if not defined
    /// </summary>
    public float GetSedimentValue(int x, int z)
    {
        return sedimentMap.GetValue(x, z, 0);
    }
    
    /// <summary>
    /// returns water value
    /// 0 if not defined
    /// </summary>
    public float GetWaterValue(int x, int z)
    {
        return waterMap.GetValue(x, z, 0);
    }
    
    public void ResetValues()
    {
        outflowTop.ResetQuadrants();
        outflowRight.ResetQuadrants();
        outflowBot.ResetQuadrants();
        outflowLeft.ResetQuadrants();

        sedimentMap.ResetQuadrants();
        waterMap.ResetQuadrants();
        stepMap.ResetQuadrants();
        hydraulicErosionMap.ResetQuadrants();
    }

    //---DEBUG---//

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

    public string ErosionValuesString(Area area)
    {
        string output = "hydraulic erosion values: \n";
        string s = "";
        for (int x = area.botLeft.x; x < area.topRight.x; x++)
        {
            for (int z = area.botLeft.z; z < area.topRight.z; z++)
            {
                s = (int)GetHydraulicErosionValue(x, z) + "." +
                    (int)(100 * (GetHydraulicErosionValue(x, z) - (int)GetHydraulicErosionValue(x, z)));
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

