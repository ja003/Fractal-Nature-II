﻿using UnityEngine;
using System.Collections;

public class GUIterrainPatch {

    public PatchManager pm;

    float buttonWidth = 40;
    float buttonHeight = 40;
    float centerX = Screen.width / 2;
    float centerZ = Screen.height / 2;

    public int patchSize = 64;
    float low_rMin = -1f;
    float low_rMax = -0.3f;
    float low_noise = 2;

    float medium_rMin = -0.3f;
    float medium_rMax = 0.5f;
    float medium_noise = 2;

    float high_rMin = 0.7f;
    float high_rMax = 1.5f;
    float high_noise = 3;

    public int count = 3;

    public GUIterrainPatch(int patchSize)
    {
        pm = new PatchManager(patchSize);
        this.patchSize = patchSize;
        SetDefaultPatch();
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
        for (int x = 1; x < count; x++)
        {
            for (int z = 0; z < count; z++)
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
        for (int x = 0; x > -count; x--)
        {
            for (int z = 1; z < count; z++)
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
        for (int x = -1; x > -count; x--)
        {
            for (int z = 0; z >- count; z--)
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
        for (int x = 0; x < count; x++)
        {
            for (int z = -1; z > -count; z--)
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

    public void SetDefaultPatch()
    {
        for(int x = -2; x <= 2; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                SetPatchValue(x,z, PatchLevel.low);
            }
        }
        
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                SetPatchValue(x, z, PatchLevel.medium);
            }
        }

        SetPatchValue(0, 0, PatchLevel.high);
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