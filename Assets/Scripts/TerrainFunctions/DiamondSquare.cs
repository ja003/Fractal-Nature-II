using System;
using System.Collections.Generic;
using System.Linq;

public class DiamondSquare
{
    int[] indices;

    Random random;
    //int noise = 2;
    //int gridSize = 257;
    int seed = 0;
    
    LocalTerrain lt;

    int patchSize = 0;

    TerrainGenerator tg;
    public MountainPeaksManager mountainPeaksManager;

    public DiamondSquare(TerrainGenerator terrainGenerator, int patchSize)
    {
        tg = terrainGenerator;
        this.patchSize = patchSize;
    }

    public void AssignFunctions(LocalTerrain localTerrain)
    {
        lt = localTerrain;
        //UnityEngine.Debug.Log(lt);
        //UnityEngine.Debug.Log(tg.gm);
        mountainPeaksManager = new MountainPeaksManager(tg.gm);
    }

    public void Initialize(int patchSize, float noise, float rMin, float rMax)
    {
        random = new Random();
        seed = random.Next(100);
        
        GenerateTerrain(patchSize, noise, rMin, rMax);
    }
    

    private float RandRange(Random r, float rMin, float rMax)
    {
        //return rMin + (float)r.NextDouble() * (rMax - rMin);
        //return r.Next(rMin, rMax);
        return UnityEngine.Random.Range(rMin, rMax);
    }
    
    /// <summary>
    /// Returns true if a is a power of 2, else false 
    /// </summary>    
    private bool pow2(int a)
    {
        return (a & (a - 1)) == 0;
    }

    
    int counter = 0;

    /// <summary>
    /// maps local coordinates to global and sets height
    /// </summary>
    public void SetLocalHeight(int x, int z, float value, bool overwrite)
    {
        /*if(counter < 10 && Double.IsNaN(value))
        {
            counter++;
            UnityEngine.Debug.Log(x + "," + z);
            UnityEngine.Debug.Log(rMin);
            UnityEngine.Debug.Log(rMax);
            UnityEngine.Debug.Log(noise);
        }*/
        if (Double.IsNaN(value))
            value = 0;

        lt.SetLocalHeight(x, z, value, overwrite);
    }

    /*
    public void SetLocalHeight(int x, int z)
    {
        float factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(0, 0, closestPeaks)) / maxDistance;
        float value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
        bool overwrite = false;
        //value = 1;
        if (debugPeaks)
            SetLocalHeight(0, 0, factor, overwrite); //DEBUG PEAKS
        SetLocalHeight(0, 0, value * factor, overwrite);
    }*/

    float maxDistance = 128;


    /// <summary>
    /// calculates smallest distance from given local point to one of give peaks
    /// distance can't be equal to maxDistance
    /// </summary>
    public float GetSmallestDistanceToPeak(int x, int z, List<Vertex> peaks)
    {
        float distance = 666;
        foreach (Vertex peak in peaks)
        {
            
            int globalX = (int)lt.GetGlobalCoordinate(x, z).x;
            int globalZ = (int)lt.GetGlobalCoordinate(x, z).z;
            float d = tg.fmc.GetDistance(globalX, globalZ, peak.x, peak.z);
            if (d < distance)
            {
                distance = d;
            }
        }
        if (peaks.Count == 0 || distance == 666)
        {
            UnityEngine.Debug.Log("no peak found");
            distance = maxDistance-1;
        }
        if (distance >= maxDistance && counter < 10)
        {
            distance = maxDistance - 1;
        }
        if (Double.IsNaN(distance))
            distance = 0;
        return distance;
    }
    /*
    private int LocalX(int x)
    {
        return x + (lt.terrainWidth / 2 - patchSize / 2);
    }

    private int LocalZ(int z)
    {
        return z + (lt.terrainHeight / 2 - patchSize / 2);
    }
    */
    /// <summary>
    /// set corners based on neighbouring values
    /// if neighbourhood is not defined, values are random
    /// </summary>
    public void SetupCorners(Random rand, float rMin, float rMax, int s, bool overwrite, List<Vertex> closestPeaks)
    {
        float neighbourhood = 666;
        float factor = 666;
        float value = 666;

        //UnityEngine.Debug.Log("setting: " + lt.GetGlobalCoordinate(0, 0));
        //UnityEngine.Debug.Log("setting: " + lt.GetGlobalCoordinate(s, 0));
        //UnityEngine.Debug.Log("setting: " + lt.GetGlobalCoordinate(0, s));
        //UnityEngine.Debug.Log("setting: " + lt.GetGlobalCoordinate(s, s));

        float rMinTop = 666;
        float rMinRight = 666;
        float rMinBot = 666;
        float rMinLeft = 666;

        float rMinNeigbourMax = 666;
        bool rMinMaxMethod = true;

        neighbourhood = lt.GetNeighbourHeight(0, 0);
        //UnityEngine.Debug.Log(lt.GetLocalHeight(0, 0));
        if (neighbourhood != 666)
        {
            SetLocalHeight(0, 0, neighbourhood, true);
        }
        else
        {
            factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(0, 0, closestPeaks)) / maxDistance;
            value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
            value = rMin;
            value = RandRange(rand, rMin, rMin + noise);
            
            
            rMinNeigbourMax = tg.pm.GetNeighbourMax(lt.GetGlobalCoordinate(0, 0).x, lt.GetGlobalCoordinate(0, 0).z, PatchInfo.rMin, 1);
            if (rMinMaxMethod)
                value = rMinNeigbourMax != 666 ? rMinNeigbourMax : rMin;
            //UnityEngine.Debug.Log(rMinNeigbourMax);

            //value = 1;
            if (debugPeaks)
                SetLocalHeight(0, 0, factor, overwrite); //DEBUG PEAKS
            factor = 1;
            SetLocalHeight(0, 0, value * factor, overwrite);
        }

        neighbourhood = lt.GetNeighbourHeight(s, 0);
        if (neighbourhood != 666)
        {
            SetLocalHeight(s, 0, neighbourhood, true);
        }
        else
        {
            factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(s, 0, closestPeaks)) / maxDistance;
            value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
            value = rMin;
            value = RandRange(rand, rMin, rMin + noise);

            rMinNeigbourMax = tg.pm.GetNeighbourMax(lt.GetGlobalCoordinate(s, 0).x, lt.GetGlobalCoordinate(s, 0).z, PatchInfo.rMin, 1);
            if (rMinMaxMethod)
                value = rMinNeigbourMax != 666 ? rMinNeigbourMax : rMin;
            //UnityEngine.Debug.Log(rMinNeigbourMax);

            //value = 1;
            if (debugPeaks)
                SetLocalHeight(s, 0, factor, overwrite); //DEBUG PEAKS
            factor = 1;
            SetLocalHeight(s, 0, value * factor, overwrite);
        }

        neighbourhood = lt.GetNeighbourHeight(0, s);
        if (neighbourhood != 666)
        {
            SetLocalHeight(0, s, neighbourhood, true);
        }
        else
        {
            factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(0, s, closestPeaks)) / maxDistance;
            value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
            value = rMin;
            value = RandRange(rand, rMin, rMin + noise);

            rMinNeigbourMax = tg.pm.GetNeighbourMax(lt.GetGlobalCoordinate(0, s).x, lt.GetGlobalCoordinate(0, s).z, PatchInfo.rMin, 1);
            if (rMinMaxMethod)
                value = rMinNeigbourMax != 666 ? rMinNeigbourMax : rMin;
            //UnityEngine.Debug.Log(rMinNeigbourMax);

            //value = 1;
            if (debugPeaks)
                SetLocalHeight(0, s, factor, overwrite); //DEBUG PEAKS
            factor = 1;
            SetLocalHeight(0, s, value * factor, overwrite);
        }

        neighbourhood = lt.GetNeighbourHeight(s, s);
        if (neighbourhood != 666)
        {
            SetLocalHeight(s, s, neighbourhood, true);
        }
        else
        {
            factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(s, s, closestPeaks)) / maxDistance;
            value = RandRange(rand, rMin * lowFactor, rMax * highFactor);
            value = rMin;
            value = RandRange(rand, rMin, rMin + noise);

            rMinNeigbourMax = tg.pm.GetNeighbourMax(lt.GetGlobalCoordinate(s,s).x, lt.GetGlobalCoordinate(s,s).z, PatchInfo.rMin, 1);
            if (rMinMaxMethod)
                value = rMinNeigbourMax != 666 ? rMinNeigbourMax : rMin;
            //UnityEngine.Debug.Log(rMinNeigbourMax);

            //value = 1;
            if (debugPeaks)
                SetLocalHeight(s,s, factor, overwrite); //DEBUG PEAKS
            factor = 1;
            SetLocalHeight(s, s, value * factor, overwrite);
        }
    }


    float factorConstant = 3;
    float highFactor = 1;
    float lowFactor = 1;
    bool debugPeaks = false;
    float rMin;
    float rMax;
    float noise;
    List<Vertex> closestPeaks;
    Random rand;


    public void DiamondSquareGrid(int size, int seed = 0, 
        float rMin = 0, float rMax = 255, float noise = 0.0f)
    {
        // Fail if grid size is not of the form (2 ^ n) - 1 or if min/max values are invalid
        //int s = size - 1;
        int s = size;
        if (!pow2(s) || rMin >= rMax)
        {
            UnityEngine.Debug.Log("size has to be power of 2. size = " + size);
            UnityEngine.Debug.Log("rMin = " + rMin);
            UnityEngine.Debug.Log("rMax = " + rMax);
            return;
        }

        float modNoise = 0.0f;

        this.rMin = rMin;
        this.rMax = rMax;
        this.noise = noise;
        this.rand = (seed == 0 ? new Random() : new Random(seed));
        closestPeaks = mountainPeaksManager.GetClosestPeaks(tg.localTerrain.localTerrainC.center);
        
        float defaultHeight = -20; //only to detect deffects in process. If terrain has some bad height (too high/low), there is some error

        bool overwrite = false;
        // Seed the first four corners
        
        float neighbourhood = 666;


        float factor = 666;

        SetupCorners(rand, rMin, rMax, s, overwrite, closestPeaks);

        float s0, s1, s2, s3, d0, d1, d2, d3, cn;
        
        float height = 0;

        for (int i = s; i > 1; i /= 2)
        {
            // reduce the random range at each step
            float modNoiseOrig = (rMax - rMin) * noise * ((float)i / s);
            //float modNoiseOrig = noise * ((float)i / s);

            // diamonds
            for (int z = 0; z < s; z += i)
            {
                for (int x = 0; x < s; x += i)
                {
                    s0 = lt.GetLocalHeight(x, z, defaultHeight); //shouldn't need to define default 'undefined' value
                    s1 = lt.GetLocalHeight(x + i, z, defaultHeight);
                    s2 = lt.GetLocalHeight(x, z + i, defaultHeight);
                    s3 = lt.GetLocalHeight(x + i, z + i, defaultHeight);

                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x + (i / 2), z + (i / 2), closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x + (i / 2), z + (i / 2), modNoiseOrig);
                    height = GetHeight(x + (i / 2), z + (i / 2),(s0 + s1 + s2 + s3) / 4.0f, modNoise);
                    SetLocalHeight(x + (i / 2), z + (i / 2), height, overwrite);
                }
            }
            counter = 0;
             
            // squares
            for (int z = 0; z < s; z += i)
            {
                for (int x = 0; x < s; x += i)
                {
                    s0 = lt.GetLocalHeight(x, z, defaultHeight);
                    s1 = lt.GetLocalHeight(x + i, z, defaultHeight);
                    s2 = lt.GetLocalHeight(x, z + i, defaultHeight);
                    s3 = lt.GetLocalHeight(x + i, z + i, defaultHeight);
                    cn = lt.GetLocalHeight(x + (i / 2), z + (i / 2), defaultHeight);

                    d0 = z <= 0 ? (s0 + s1 + cn) / 3.0f :
                        (s0 + s1 + cn + lt.GetLocalHeight(x + (i / 2), z - (i / 2), defaultHeight)) / 4.0f;
                    d1 = x <= 0 ? (s0 + cn + s2) / 3.0f :
                        (s0 + cn + s2 + lt.GetLocalHeight(x - (i / 2), z + (i / 2), defaultHeight)) / 4.0f;
                    d2 = x >= s - i ? (s1 + cn + s3) / 3.0f :
                        (s1 + cn + s3 + lt.GetLocalHeight(x + i + (i / 2), z + (i / 2), defaultHeight)) / 4.0f; 
                    d3 = z >= s - i ? (cn + s2 + s3) / 3.0f :
                        (cn + s2 + s3 + lt.GetLocalHeight(x + (i / 2), z + i + (i / 2), defaultHeight)) / 4.0f;
                    

                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x+ (i / 2), z, closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x + (i / 2), z, modNoiseOrig); 
                    height = GetHeight(x + (i / 2), z, d0, modNoise);
                    SetLocalHeight(x + (i / 2), z, height, overwrite);


                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x, z + (i / 2), closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x, z + (i / 2), modNoiseOrig);
                    height = GetHeight(x, z + (i / 2), d1, modNoise);
                    SetLocalHeight(x, z + (i / 2), height, overwrite);


                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x + i, z + (i / 2), closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x + i, z + (i / 2), modNoiseOrig);
                    height = GetHeight(x + i, z + (i / 2), d2, modNoise);
                    SetLocalHeight(x + i, z + (i / 2), height, overwrite);


                    //factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x + (i / 2), z + i, closestPeaks)) / maxDistance;
                    modNoise = GetModNoise(x + (i / 2), z + i, modNoiseOrig);
                    height = GetHeight(x + (i / 2), z + i,d3, modNoise);
                    SetLocalHeight(x + (i / 2), z + i, height, overwrite);
                    
                }
            }
        }
        //UnityEngine.Debug.Log("Diamond square complete");
    }

    public float GetHeight(int x, int z, float initValue, float modNoise)
    {
        float factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x, z, closestPeaks)) / maxDistance;
        if (debugPeaks)
        {
            
            return factor;
        }
        

        lowFactor = 2*GetSmallestDistanceToPeak(x, z, closestPeaks) / maxDistance;
        highFactor = 2*(maxDistance - GetSmallestDistanceToPeak(x, z, closestPeaks)) / maxDistance;

        if (lowFactor > highFactor)
            highFactor = lowFactor;
        else
            lowFactor = highFactor;

        if (counter < 10 && (lowFactor > 1 || highFactor > 1))
        {
            //UnityEngine.Debug.Log(factor);
            //UnityEngine.Debug.Log(GetSmallestDistanceToPeak(x, z, closestPeaks));
            //UnityEngine.Debug.Log(lowFactor);
            //UnityEngine.Debug.Log(highFactor);

            counter++;
        }

        //modNoise *= (float)Math.Sqrt(factor);
        float minNoise = -modNoise * lowFactor;// + (float)Math.Sqrt(factor);
        float maxNoise = modNoise * highFactor;// + (float)Math.Sqrt(factor);
        /*if(counter < 10)
        {
            counter++;
            UnityEngine.Debug.Log(modNoise);
            UnityEngine.Debug.Log(rMin);
            UnityEngine.Debug.Log(rMax);
        }*/


        //minNoise = rMin/10 * modNoise;
        //maxNoise = rMax/10 * modNoise;

        float height = initValue + RandRange(rand, minNoise, maxNoise);
        
        return height;
    }

    public float GetModNoise(int x, int z, float modNoiseOrig)
    {
        float factor = factorConstant * (maxDistance - GetSmallestDistanceToPeak(x, z, closestPeaks)) / maxDistance;

        float modNoise = modNoiseOrig;// = modNoiseOrig * factor;
        modNoise *= (float)Math.Sqrt(Math.Abs(factor));
        //if ((x > 10 && x <= 30))
        //{
        //    modNoise += factor;
        //}
        if (x < 64)
        {
            //modNoise /= (2*factor*factor);
            //modNoise -= factor;
            //if (modNoise < 0)
                //modNoise = 0;
        }
        /*if(counter < 5)
        {
            counter++;
            UnityEngine.Debug.Log(factor);
            UnityEngine.Debug.Log(modNoise);
        }*/

        return modNoise;
    }

    //float[][] ds;

    public void GenerateTerrain(int patchSize, float noise, float rMin, float rMax)
    {
    //    UnityEngine.Debug.Log("!!!");
    //    UnityEngine.Debug.Log(tg.localTerrain.localTerrainC.center);
        maxDistance = 2*(float)Math.Sqrt(patchSize * patchSize + patchSize * patchSize);

        //DiamondSquareGrid(patchSize, seed, -1, 1, roughness / 5.0f);
        DiamondSquareGrid(patchSize, seed, rMin, rMax, noise / 5.0f);

    }
}

