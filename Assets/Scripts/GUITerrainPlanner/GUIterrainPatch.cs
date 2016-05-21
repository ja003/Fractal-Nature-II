using UnityEngine;
using System.Collections;

public class GUIterrainPatch {

    public PatchManager pm;
    

    float buttonWidth = 40;
    float buttonHeight = 40;
    float centerX = Screen.width / 2;
    float centerZ = Screen.height / 2;

    
    public int patchSize = 128;


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
    public int extraPatchCount = 1;

    public GUIterrainPatch(int patchSize)
    {
        pm = new PatchManager(patchSize);
        this.patchSize = patchSize;

        UpdatePatchAttributes();
        SetDefaultPatch(DefaultTerrain.random);
    }

   

    public void OnGui()
    {
        Color origColor = GUI.color;
        centerX = Screen.width / 2 - buttonWidth/2;
        centerZ = Screen.height / 2 - buttonHeight/2;
        

        GUI.Box(new Rect(centerX- (1+extraPatchCount)* buttonWidth,
            centerZ- (1+extraPatchCount)* buttonHeight, 
            (3+2*extraPatchCount) * buttonWidth, 
            (3+2*extraPatchCount) * buttonHeight),"");

        //center
        PatchLevel level = GetPatchLevel(0, 0);
        GUI.color = GetLevelColor(level);
        if (GUI.Button(new Rect(centerX, centerZ, buttonWidth, buttonHeight),GetLevelString(level)))
        {
            SetPatchValue(0, 0, GetNextLevel(level), true);
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
                    SetPatchValue(x,z, GetNextLevel(level), true);
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
                    SetPatchValue(x, z, GetNextLevel(level), true);
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
                    SetPatchValue(x, z, GetNextLevel(level), true);
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
                    SetPatchValue(x, z, GetNextLevel(level), true);
                }

            }
        }


        GUI.color = origColor;

    }

    /// <summary>
    /// limit extra patch count for better visual look
    /// </summary>
    public void UpdateExtraPatchCount(int count)
    {
        if (count > patchCount - 2)
        {
            count = patchCount - 2;
        }
        extraPatchCount = count;
    }

    /// <summary>
    /// updates rMin, rMax and noise values based on current patchSize
    /// </summary>
    public void UpdatePatchAttributes()
    {
        float scale =  patchSize/64 + 0.3f;

        low_rMin = scale * -1f;
        low_rMax = scale * -0.3f;
        low_noise = 2;

        medium_rMin = scale * -0.3f;
        medium_rMax = scale * 0.5f;
        medium_noise = 2;

        high_rMin = scale * 0.7f;
        high_rMax = scale * 1.5f;
        high_noise = 2.5f;
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
            case DefaultTerrain.random:
                SetRandomPatchLevel(5);
                pm.SetPatchOrder(PatchOrder.HLM);
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
    /// sets random PatchLevel to all patches in distance < radius
    /// generates 3 in advance
    /// </summary>
    public void SetRandomPatchLevel(int radius)
    {
        for(int i = 0;i < radius ; i++)
        {
            SetPatchLevelInRadius(i, PatchLevel.random);
        }
    }

    /// <summary>
    /// sets PatchLevel in |radius| distance from center
    /// </summary>
    public void SetPatchLevelInRadius(int radius, PatchLevel level)
    {
        for (int x = -radius; x <= radius; x++)
            SetPatchValue(x, radius, level);
        for (int x = -radius; x <= radius; x++)
            SetPatchValue(x, -radius, level);
        for (int z = -radius+1; z < radius; z++)
            SetPatchValue(radius, z, level);
        for (int z = -radius + 1; z < radius; z++)
            SetPatchValue(-radius, z, level);
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

    public void SetPatchValue(int x, int z, PatchLevel level)
    {
        SetPatchValue(x, z, level, false);
    }

    /// <summary>
    /// sets valus to patch defined by center [x,z] and patchSize
    /// randomValue = random value can be asigned to patch
    /// </summary>
    public void SetPatchValue(int x, int z, PatchLevel level, bool randomValue)
    {
        switch (level)
        {
            case PatchLevel.random:
                if (randomValue)
                {
                    pm.SetValues(new Vertex(x * patchSize, z * patchSize),
                        patchSize, 666, 666, 666);
                }
                else
                {
                    SetPatchRandomLevel(x, z);
                }
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



    /// <summary>
    /// determines and sets level value from neighbouring patches
    /// to [x,z] patch
    /// </summary>
    void SetPatchRandomLevel(int x, int z)
    {
        int top = (int)pm.patchLevel.GetValue(x, z + patchSize, 0);
        int right = (int)pm.patchLevel.GetValue(x + patchSize, z , 0);
        int bot = (int)pm.patchLevel.GetValue(x, z - patchSize, 0);
        int left = (int)pm.patchLevel.GetValue(x - patchSize, z, 0);

        int avg = (top + right + bot + left) / 4;
        if (x == 0 && z == 0)
            avg += (int)Random.Range(0f, 2.5f);
        
        float rnd = Random.Range(0f, 1f);

        if (rnd < 0.1f)
            avg -= 2;
        else if (rnd < 0.25f)
            avg -= 1;
        else if (rnd < 0.7)
            avg = avg;
        else if (rnd < 0.9)
            avg += 1;
        else
            avg += 2;

        avg = Mathf.Clamp(avg, 0, 2);

        PatchLevel level = GetLevelFromNum(avg);

        if (level == PatchLevel.random)
        {
            Debug.Log("random shouldn't be here - fail");
            return;
        }
        SetPatchValue(x, z, level);
    }

    /// <summary>
    /// returns PatchLevel representation of number
    /// </summary>
    public PatchLevel GetLevelFromNum(int number)
    {
        switch (number)
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
            //case PatchLevel.high:
            //    return PatchLevel.low; //for better user control
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
        UpdatePatchAttributes();
        pm = new PatchManager(patchSize);
        SetRandomPatchLevel(patchCount);
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
    valleys,
    random
}