using UnityEngine;
using System.Collections;

public class GUIterrainPatch {

    public PatchManager pm;
    

    float buttonWidth = 40;
    float buttonHeight = 40;
    float centerX = Screen.width / 2;
    float centerZ = Screen.height / 2;

    
    public int patchSize = 64;


    float low_rMin;
    float low_rMax;
    float low_noise;

    float medium_rMin;
    float medium_rMax;
    float medium_noise;

    float high_rMin;
    float high_rMax;
    float high_noise;

    public int patchCount;

    public GUIterrainPatch(int patchSize)
    {
        pm = new PatchManager(patchSize);
        this.patchSize = patchSize;

        SetPatchAttributes();
        SetDefaultPatch(DefaultTerrain.valleys);
    }

    public void OnGui()
    {
        Color origColor = GUI.color;

        centerX = Screen.width / 2 - buttonWidth/2;
        centerZ = Screen.height / 2 - buttonHeight/2;

        //center
        PatchLevel level = GetPatchLevel(0, 0);
        GUI.color = GetLevelColor(level);
        if (GUI.Button(new Rect(centerX, centerZ, buttonWidth, buttonHeight),GetLevelString(level)))
        {
            SetPatchValue(0, 0, GetNextLevel(level));
            //Debug.Log("0,0");
        }
        //quadrant 1
        for (int x = 1; x < patchCount; x++)
        {
            for (int z = 0; z < patchCount; z++)
            {
                level = GetPatchLevel(x,z);
                GUI.color = GetLevelColor(level);

                if (GUI.Button(new Rect(centerX + x* buttonWidth, centerZ - z* buttonHeight, buttonWidth, buttonHeight), GetLevelString(level)))
                {
                    SetPatchValue(x,z, GetNextLevel(level));
                    //Debug.Log(x + "," + z);
                }

            }
        }
        //quadrant 2
        for (int x = 0; x > -patchCount; x--)
        {
            for (int z = 1; z < patchCount; z++)
            {
                level = GetPatchLevel(x, z);
                GUI.color = GetLevelColor(level);
                if (GUI.Button(new Rect(centerX + x * buttonWidth, centerZ - z * buttonHeight, buttonWidth, buttonHeight), GetLevelString(level)))
                {
                    SetPatchValue(x, z, GetNextLevel(level));
                    //Debug.Log(x + "," + z);
                }

            }
        }
        //quadrant 3
        for (int x = -1; x > -patchCount; x--)
        {
            for (int z = 0; z >- patchCount; z--)
            {
                level = GetPatchLevel(x, z);
                GUI.color = GetLevelColor(level);
                if (GUI.Button(new Rect(centerX + x * buttonWidth, centerZ - z * buttonHeight, buttonWidth, buttonHeight), GetLevelString(level)))
                {
                    SetPatchValue(x, z, GetNextLevel(level));
                    //Debug.Log(x + "," + z);
                }

            }
        }
        //quadrant 4
        for (int x = 0; x < patchCount; x++)
        {
            for (int z = -1; z > -patchCount; z--)
            {
                level = GetPatchLevel(x, z);
                GUI.color = GetLevelColor(level);
                if (GUI.Button(new Rect(centerX + x * buttonWidth, centerZ - z * buttonHeight, buttonWidth, buttonHeight), GetLevelString(level)))
                {
                    SetPatchValue(x, z, GetNextLevel(level));
                    //Debug.Log(x + "," + z);
                }

            }
        }


        GUI.color = origColor;

    }

    public void SetPatchAttributes()
    {
        float scale = 1.1f;

        low_rMin = scale * -1f;
        low_rMax = scale * -0.3f;
        low_noise = 2;

        medium_rMin = scale * -0.3f;
        medium_rMax = scale * 0.5f;
        medium_noise = 2;

        high_rMin = scale * 0.7f;
        high_rMax = scale * 1.5f;
        high_noise = 3;
    }

    bool valleyDir = true;
    public void SetDefaultPatch(DefaultTerrain type)
    {
        int w = 4; 
        int count = 5;//more than enough
        switch (type)
        {
            case DefaultTerrain.hillGrid:
                w = 4;
                for (int x = -count*w; x <= count * w; x += w)
                {
                    for (int z = -count * w; z <= count * w; z += w)
                    {
                        SetHill(x, z);
                    }
                }
                pm.SetPatchOrder(PatchOrder.HLM);

                break;
            case DefaultTerrain.valleys:
                w = 4;
                for (int i = -count * w; i <= count * w; i += w)
                {
                    SetValley(i, 0, w, 10, valleyDir);
                }
                valleyDir = !valleyDir;
                pm.SetPatchOrder(PatchOrder.LHM);
                break;
        }
    }

    /// <summary>
    /// sets valey lines starting in [x,z]
    /// </summary>
    /// <param name="valleyDir">true = vertical, false = horizontal</param>
    public void SetValley(int _x, int _z, int width, int length, bool valleyDir)
    {
        if (valleyDir)
        {
            for (int x = _x - width; x <= _x + width; x++)
            {
                for (int z = -length; z < length; z++)
                {
                    if (x % width == 0)
                        SetPatchValue(x, z, PatchLevel.low);
                    else if (x % width == 1 || x % width == -1)
                        SetPatchValue(x, z, PatchLevel.medium);
                    else
                        SetPatchValue(x, z, PatchLevel.high);
                }
            }
        }
        else
        {
            for (int z = _x - width; z <= _x + width; z++)
            {
                for (int x = -length; x < length; x++)
                {
                    if (z % width == 0)
                        SetPatchValue(x, z, PatchLevel.low);
                    else if (z % width == 1 || z % width == -1)
                        SetPatchValue(x, z, PatchLevel.medium);
                    else
                        SetPatchValue(x, z, PatchLevel.high);
                }
            }
        }
    }

    /// <summary>
    /// sets hill on [x,z] patch
    /// L|L|L|L|L
    /// L|M|M|M|L
    /// L|M|H|M|L
    /// L|M|M|M|L
    /// L|L|L|L|L
    /// </summary>
    public void SetHill(int _x, int _z)
    {
        for (int x = _x - 2; x <= _x+2; x++)
        {
            for (int z = _z - 2; z <= _z+2; z++)
            {
                SetPatchValue(x, z, PatchLevel.low);
            }
        }

        for (int x = _x - 1; x <= _x+ 1; x++)
        {
            for (int z = _z - 1; z <= _z+1; z++)
            {
                SetPatchValue(x, z, PatchLevel.medium);
            }
        }

        SetPatchValue(_x, _z, PatchLevel.high);
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

    public string GetLevelString(PatchLevel level)
    {
        switch (level)
        {
            case PatchLevel.random:
                return "?";
            case PatchLevel.low:
                return "L";
            case PatchLevel.medium:
                return "M";
            case PatchLevel.high:
                return "H";
        }
        return "X";
    }

    public Color GetLevelColor(PatchLevel level)
    {
        switch (level)
        {
            case PatchLevel.random:
                return Color.grey;
            case PatchLevel.low:
                return Color.blue;
            case PatchLevel.medium:
                return Color.green;
            case PatchLevel.high:
                return Color.red;
        }
        return Color.grey;
    }

    /// <summary>
    /// updates patchSize
    /// resets PatchManager
    /// </summary>
    public void UpdatePatchSize(int patchSize)
    {
        this.patchSize = patchSize;
        pm = new PatchManager(patchSize);
    }
}

public enum PatchLevel
{
    low,
    medium,
    high,
    random    
}

public enum DefaultTerrain
{
    hillGrid,
    valleys
}