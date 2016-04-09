using UnityEngine;
using System.Collections;

public class PatchManager {

    public int patchSize;
    public GlobalCoordinates rMin;
    public GlobalCoordinates rMax;
    public GlobalCoordinates roughness;

    public PatchManager(int patchSize)
    {
        this.patchSize = patchSize;
        rMin = new GlobalCoordinates(100);
        rMax = new GlobalCoordinates(100);
        roughness = new GlobalCoordinates(100);
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
                    case PatchInfo.rougness:
                        v = roughness.GetValue(x, z);
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
                    case PatchInfo.rougness:
                        v = roughness.GetValue(x, z);
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

    public void SetValues(Vertex center, int patchSize, float rMinV, float rMaxV, float roughnessV)
    {
        for (int x = center.x - patchSize / 2; x < center.x + patchSize / 2; x++)
        {
            for (int z = center.z - patchSize / 2; z < center.z + patchSize / 2; z++)
            {
                rMin.SetValue(x, z, rMinV);
                rMax.SetValue(x, z, rMaxV);
                roughness.SetValue(x, z, roughnessV);
            }
        }
    }

}

public enum PatchInfo
{
    rMin,
    rMax,
    rougness
}