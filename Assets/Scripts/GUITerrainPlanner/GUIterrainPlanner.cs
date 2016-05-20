using UnityEngine;
using System.Collections;

public class GUIterrainPlanner : MonoBehaviour
{

    GUIterrainPlannerMenu tpm;
    GUIterrainPatch patch;

    public int menuWidth;
    public int rightOffset;
    public int buttonHeight;
    public int smallButtonHeight;
    public int sideOffset;
    public float buttonWidth;

    float patchCount = 6;
    float extraPatches = 1;

    public GUIterrainPlanner(GUIterrainPlannerMenu tpm)
    {
        this.tpm = tpm;
        patch = tpm.patch;

        menuWidth = tpm.menuWidth;
        rightOffset = tpm.rightOffset;
        buttonHeight = tpm.buttonHeight;
        sideOffset = tpm.sideOffset;
        buttonWidth = tpm.buttonWidth;
        smallButtonHeight = buttonHeight / 2 + 5; 
    }
    public int yPosMax = 0;

    public void OnGui(int yPos)
    {

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, 8.5f * buttonHeight), "Terrain planner");

        yPos += smallButtonHeight;

        //Patch count
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, smallButtonHeight), "count: ");


        //Debug.Log(spikeThresholdFlag);
        patchCount = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + 4 * sideOffset + 10, yPos + 5,
                menuWidth - (6*sideOffset),
                smallButtonHeight), patchCount, 1, 10);
        tpm.patch.patchCount = (int)patchCount; 
        yPos += smallButtonHeight;

        //extra patch count
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth + 5, smallButtonHeight), "pregenerate: " + (int)extraPatches);
        extraPatches = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + 9 * sideOffset + 10, yPos + 5,
                menuWidth - (11 * sideOffset),
                smallButtonHeight), extraPatches, 0, 10);
        tpm.patch.UpdateExtraPatchCount((int)extraPatches);
        yPos += smallButtonHeight;

        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, menuWidth - rightOffset, buttonHeight),
            "Patch Size = " + patch.patchSize + "x" + patch.patchSize);


        yPos += smallButtonHeight;

        // 64x64 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "64x64"))
        {
            patch.UpdatePatchSize(64);
        }

        // 128x128 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + buttonWidth + 2 * sideOffset, yPos, buttonWidth, buttonHeight), "128x128"))
        {
            patch.UpdatePatchSize(128);
        }
        yPos += buttonHeight;
        /*
        // 256x256 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "256x256"))
        {
            patch.UpdatePatchSize(256);
        }

        // 512x512 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + buttonWidth + 2 * sideOffset, yPos, buttonWidth, buttonHeight), "512x512"))
        {
            patch.UpdatePatchSize(512);
        }*/
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2*buttonWidth, buttonHeight), "estimated time: " + GetTimeEstimate(patch.patchSize,(int)Mathf.Pow(3+2*(int)extraPatches, 2)) + " sec");

        yPos += buttonHeight;

        //default terrain
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "default terrain");
        yPos += smallButtonHeight;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Grid of hills"))
        {
            patch.SetDefaultPatch(DefaultTerrain.hillGrid);
        }

        yPos += buttonHeight + 5;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Valleys"))
        {
            patch.SetDefaultPatch(DefaultTerrain.valleys);
        }
        
        yPos += buttonHeight + 5;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Random"))
        {
            patch.SetDefaultPatch(DefaultTerrain.random);
        }

        yPos += buttonHeight + 10;
        yPosMax = yPos;
    }

    public int GetTimeEstimate(int patchSize, int patchCount)
    {
        float t = 1;
        if (patchSize <= 64)
            t = 1.3f;
        else
            t = 2f;

        return (int)(patchCount * t);
    }

}