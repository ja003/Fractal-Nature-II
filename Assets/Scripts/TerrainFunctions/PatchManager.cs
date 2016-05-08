using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatchManager {

    public int patchSize;
    public GlobalCoordinates rMin;
    public GlobalCoordinates rMax;
    public GlobalCoordinates noise;
    public GlobalCoordinates patchLevel;

    public GridManager gm;

    public List<PatchLevel> patchOrder;

    public PatchManager(int patchSize)
    {
        this.patchSize = patchSize;
        rMin = new GlobalCoordinates(100);
        rMax = new GlobalCoordinates(100);
        noise = new GlobalCoordinates(100);
        patchLevel = new GlobalCoordinates(100);//-1 = random,0=low,1=medium,2=high
        
        gm = new GridManager(new Vector3(0, 0, 0), patchSize, patchSize);
        SetPatchOrder(PatchOrder.LMH);
    }

    /// <summary>
    /// returns average value of given parameter from neighbouring patches
    /// 666 if none is defined    
    public float GetNeighbourAverage(int _x, int _z, PatchInfo parameter)
    {
        int count = 0;
        float sum = 0;
        for(int x = _x - patchSize; x <= _x + patchSize; x += patchSize)
        {
            for (int z = _z - patchSize; z <= _z + patchSize; z += patchSize)
            {
                float v = 666;
                switch (parameter)
                {
                    case PatchInfo.rMin:
                        v = rMin.GetValue(x, z);
                        break;
                    case PatchInfo.rMax:
                        v = rMax.GetValue(x, z);
                        break;
                    case PatchInfo.noise:
                        v = noise.GetValue(x, z);
                        break;
                }
                if(v != 666)
                {
                    sum += v;
                    count++;
                }
            }
        }
        if (count == 0)
            return 666;
        else
            return sum / count;
    }

    /// <summary>
    /// returns minimal value of given parameter from neighbouring patches
    /// 666 if none is defined    
    public float GetNeighbourMin(int _x, int _z, PatchInfo parameter)
    {
        int count = 0;
        float min = 666;
        for (int x = _x - patchSize; x <= _x + patchSize; x += patchSize)
        {
            for (int z = _z - patchSize; z <= _z + patchSize; z += patchSize)
            {
                float v = 666;
                switch (parameter)
                {
                    case PatchInfo.rMin:
                        v = rMin.GetValue(x, z);
                        break;
                    case PatchInfo.rMax:
                        v = rMax.GetValue(x, z);
                        break;
                    case PatchInfo.noise:
                        v = noise.GetValue(x, z);
                        break;
                }
                if (v < min)
                {
                    min = v;
                    count++;
                }
            }
        }
        if (count == 0)
            return 666;
        else
            return min;
    }

    /// <summary>
    /// returns maximal value of given parameter from neighbouring patches
    /// 666 if none is defined    
    public float GetNeighbourMax(int _x, int _z, PatchInfo parameter, int stepSize)
    {
        int count = 0;
        float max = -666;
        //Debug.Log(_x + "," + _z);
        for (int x = _x - stepSize; x <= _x + stepSize; x += stepSize)
        {
            for (int z = _z - stepSize; z <= _z + stepSize; z += stepSize)
            {
                float v = -666;
                switch (parameter)
                {
                    case PatchInfo.rMin:
                        v = rMin.GetValue(x, z);
                        break;
                    case PatchInfo.rMax:
                        v = rMax.GetValue(x, z);
                        break;
                    case PatchInfo.noise:
                        v = noise.GetValue(x, z);
                        break;
                }
                if (v > max)
                {
                    max = v;
                    count++;
                }
                //Debug.Log(x + "," + z + ": " + v);
            }
        }
        if (count == 0)
            return 666;
        else
            return max;
    }

    /// <summary>
    /// sets random rMin, rMax and noise values
    /// </summary>
    public void SetValues(Vertex center, int patchSize, float rMinV, float rMaxV, float noise)
    {
        SetValues(center, patchSize, rMinV, rMaxV, noise, PatchLevel.random);
    }

    public void SetValues(Vertex center, int patchSize, float rMinV, float rMaxV, float noiseV, PatchLevel level)
    {
        switch (level)
        {
            case PatchLevel.random:
                patchLevel.SetValue(center.x, center.z, -1, true);
                break;
            case PatchLevel.low:
                //Debug.Log(center + ": LOW");
                patchLevel.SetValue(center.x, center.z, 0, true);
                break;
            case PatchLevel.medium:
                //Debug.Log(center + ": MEDIUM");
                patchLevel.SetValue(center.x, center.z, 1, true);
                break;
            case PatchLevel.high:
                //Debug.Log(center + ": HIGH");
                patchLevel.SetValue(center.x, center.z, 2, true);
                break;
        }

        rMin.SetValue(center.x, center.z, rMinV);
        rMax.SetValue(center.x, center.z, rMaxV);
        noise.SetValue(center.x, center.z, noiseV);
        /*
        for (int x = center.x - patchSize / 2; x <= center.x + patchSize / 2; x++)
        {
            for (int z = center.z - patchSize / 2; z <= center.z + patchSize / 2; z++)
            {
                rMin.SetValue(x, z, rMinV);
                rMax.SetValue(x, z, rMaxV);
                this.noise.SetValue(x, z, noise);
                
            }
        }*/
    }

    /// <summary>
    /// returns value of given parameter of patch containing point [x,z]
    /// </summary>
    public float GetValue(int x, int z, PatchInfo parameter)
    {
        Vertex center = gm.GetPointOnGrid(new Vertex(x, z));
        switch (parameter)
        {
            case PatchInfo.rMin:
                return rMin.GetValue(center.x, center.z);
            case PatchInfo.rMax:
                return rMax.GetValue(center.x, center.z);
            case PatchInfo.noise:
                return noise.GetValue(center.x, center.z);
        }

        return 666;
    }

    /// <summary>
    /// sets order in which will be patches generated
    /// </summary>
    public void SetPatchOrder(PatchOrder order)
    {
        patchOrder = new List<PatchLevel>();
        switch (order)
        {
            case PatchOrder.HLM:
                patchOrder.Add(PatchLevel.high);
                patchOrder.Add(PatchLevel.low);
                patchOrder.Add(PatchLevel.medium);
                break;
            case PatchOrder.HML:
                patchOrder.Add(PatchLevel.high);
                patchOrder.Add(PatchLevel.medium);
                patchOrder.Add(PatchLevel.low);
                break;
            case PatchOrder.LHM:
                patchOrder.Add(PatchLevel.low);
                patchOrder.Add(PatchLevel.high);
                patchOrder.Add(PatchLevel.medium);
                break;
            case PatchOrder.LMH:
                patchOrder.Add(PatchLevel.low);
                patchOrder.Add(PatchLevel.medium);
                patchOrder.Add(PatchLevel.high);
                break;
        }

        patchOrder.Add(PatchLevel.random);

    }

}

public enum PatchInfo
{
    rMin,
    rMax,
    noise
}

/// <summary>
/// order in which patches level are generated
/// affects final height distribution of terrain
/// random levels are always generated last
/// L = low, M = medium, H = high
/// </summary>
public enum PatchOrder
{
    LMH,
    HML,
    LHM,
    HLM
}