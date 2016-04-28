using UnityEngine;
using System.Collections;

public class GUIterrainPatch {

    public PatchManager pm;

    float buttonWidth = 40;
    float buttonHeight = 40;
    float centerX = Screen.width / 2;
    float centerZ = Screen.height / 2;

    int patchSize = 64;
    float low_rMin = -1f;
    float low_rMax = -0.3f;
    float low_noise = 2;

    float medium_rMin = -0.5f;
    float medium_rMax = 0.5f;
    float medium_noise = 2;

    float high_rMin = 0.4f;
    float high_rMax = 1.2f;
    float high_noise = 3;

    public int count = 3;

    public GUIterrainPatch()
    {
        pm = new PatchManager(patchSize);
    }

    public void OnGui()
    {
        centerX = Screen.width / 2;
        centerZ = Screen.height / 2;

        //center
        PatchLevel level = GetPatchLevel(0, 0);
        if (GUI.Button(new Rect(centerX, centerZ, buttonWidth, buttonHeight),LevelString(level)))
        {
            SetPatchValue(0, 0, GetNextLevel(level));
            Debug.Log("0,0");
        }
        //quadrant 1
        for (int x = 1; x < count; x++)
        {
            for (int z = 0; z < count; z++)
            {
                level = GetPatchLevel(x,z);
                if (GUI.Button(new Rect(centerX + x* buttonWidth, centerZ - z* buttonHeight, buttonWidth, buttonHeight), LevelString(level)))
                {
                    SetPatchValue(x,z, GetNextLevel(level));
                    Debug.Log(x + "," + z);
                }

            }
        }
        //quadrant 2
        for (int x = 0; x > -count; x--)
        {
            for (int z = 1; z < count; z++)
            {
                level = GetPatchLevel(x, z);
                if (GUI.Button(new Rect(centerX + x * buttonWidth, centerZ - z * buttonHeight, buttonWidth, buttonHeight), LevelString(level)))
                {
                    SetPatchValue(x, z, GetNextLevel(level));
                    Debug.Log(x + "," + z);
                }

            }
        }
        //quadrant 3
        for (int x = -1; x > -count; x--)
        {
            for (int z = 0; z >- count; z--)
            {
                level = GetPatchLevel(x, z);
                if (GUI.Button(new Rect(centerX + x * buttonWidth, centerZ - z * buttonHeight, buttonWidth, buttonHeight), LevelString(level)))
                {
                    SetPatchValue(x, z, GetNextLevel(level));
                    Debug.Log(x + "," + z);
                }

            }
        }
        //quadrant 4
        for (int x = 0; x < count; x++)
        {
            for (int z = -1; z > -count; z--)
            {
                level = GetPatchLevel(x, z);
                if (GUI.Button(new Rect(centerX + x * buttonWidth, centerZ - z * buttonHeight, buttonWidth, buttonHeight), LevelString(level)))
                {
                    SetPatchValue(x, z, GetNextLevel(level));
                    Debug.Log(x + "," + z);
                }

            }
        }

    }

    /// <summary>
    /// returns set level on patch [x,z]
    /// x,z = grid coordinates
    /// </summary>
    PatchLevel GetPatchLevel(int x, int z)
    {
        int level = (int)pm.patchLevel.GetValue(x * patchSize, z * patchSize);
        switch (level)
        {
            case -1:
                return PatchLevel.random;
            case 0:
                return PatchLevel.low;
            case 1:
                return PatchLevel.medium;
            case 2:
                return PatchLevel.high;
        }
        return PatchLevel.random;
    }

    /// <summary>
    /// sets valus to patch defined by center [x,z] and patchSize
    /// </summary>
    void SetPatchValue(int x, int z, PatchLevel level)
    {
        switch (level)
        {
            case PatchLevel.random:
                pm.SetValues(new Vertex(x * patchSize, z * patchSize),
                    patchSize, 666, 666, 666);
                break;

            case PatchLevel.low:
                pm.SetValues(new Vertex(x * patchSize, z * patchSize),
                    patchSize, low_rMin, low_rMax, low_noise, PatchLevel.low);
                break;
            case PatchLevel.medium:
                pm.SetValues(new Vertex(x * patchSize, z * patchSize),
                    patchSize, medium_rMin, medium_rMax, medium_noise, PatchLevel.medium);
                break;
            case PatchLevel.high:
                pm.SetValues(new Vertex(x * patchSize, z * patchSize),
                    patchSize, high_rMin, high_rMax, high_noise, PatchLevel.high);
                break;
        }
    }

    public PatchLevel GetNextLevel(PatchLevel level)
    {
        switch (level)
        {
            case PatchLevel.random:
                return PatchLevel.low;
            case PatchLevel.low:
                return PatchLevel.medium;
            case PatchLevel.medium:
                return PatchLevel.high;
            case PatchLevel.high:
                return PatchLevel.random;
        }
        return PatchLevel.random;
    }

    public string LevelString(PatchLevel level)
    {
        switch (level)
        {
            case PatchLevel.random:
                return "?";
            case PatchLevel.low:
                return "low";
            case PatchLevel.medium:
                return "medium";
            case PatchLevel.high:
                return "high";
        }
        return "X";
    }
}

public enum PatchLevel
{
    low,
    medium,
    high,
    random    
}