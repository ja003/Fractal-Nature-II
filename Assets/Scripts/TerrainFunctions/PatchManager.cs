using UnityEngine;
using System.Collections;

public class PatchManager {

    public int patchSize;
    public GlobalCoordinates rMin;
    public GlobalCoordinates rMax;
    public GlobalCoordinates noise;
    public GlobalCoordinates patchLevel;

    public PatchManager(int patchSize)
    {
        this.patchSize = patchSize;
        rMin = new GlobalCoordinates(100);
        rMax = new GlobalCoordinates(100);
        noise = new GlobalCoordinates(100);
        patchLevel = new GlobalCoordinates(100);//-1 = random,0=low,1=medium,2=high
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
    /// returns minimal value or given parameter from neighbouring patches
    /// 666 if none is defined    
    public float GetNeighbourMin(int _x, int _z, PatchInfo parameter)
    {
        int count = 0;
        float min = 0;
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

    public void SetValues(Vertex center, int patchSize, float rMinV, float rMaxV, float noise)
    {
        SetValues(center, patchSize, rMinV, rMaxV, noise, PatchLevel.random);
    }

    public void SetValues(Vertex center, int patchSize, float rMinV, float rMaxV, float noise, PatchLevel level)
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

        for (int x = center.x - patchSize / 2; x < center.x + patchSize / 2; x++)
        {
            for (int z = center.z - patchSize / 2; z < center.z + patchSize / 2; z++)
            {
                rMin.SetValue(x, z, rMinV);
                rMax.SetValue(x, z, rMaxV);
                this.noise.SetValue(x, z, noise);
                
            }
        }
    }

}

public enum PatchInfo
{
    rMin,
    rMax,
    noise
}