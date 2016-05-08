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

    float patchCount = 3;

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

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, 8 * buttonHeight), "Terrain planner");

        yPos += smallButtonHeight;

        //Patch count
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "count: ");


        //Debug.Log(spikeThresholdFlag);
        patchCount = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + 4 * sideOffset + 10, yPos + 5,
                menuWidth - (6*sideOffset),
                buttonHeight), patchCount, 1, 10);
        tpm.patch.count = (int)patchCount;


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

        // 256x256 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "256x256"))
        {
            patch.UpdatePatchSize(256);
        }

        // 512x512 patch size
        if (GUI.Button(new Rect(Screen.width - menuWidth + buttonWidth + 2 * sideOffset, yPos, buttonWidth, buttonHeight), "512x512"))
        {
            patch.UpdatePatchSize(512);
        }

        yPos += buttonHeight + 5;

        //default terrain
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "default terrain");
        yPos += smallButtonHeight;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Random Hills"))
        {
            patch.SetDefaultPatch();
        }

        yPos += buttonHeight + 5;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Random Hills"))
        {
            patch.SetDefaultPatch();
        }

        yPos += buttonHeight + 5;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Random Hills"))
        {
            patch.SetDefaultPatch();
        }

        yPos += buttonHeight + 10;
        yPosMax = yPos;
    }

}