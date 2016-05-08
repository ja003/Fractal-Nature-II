using UnityEngine;
using System.Collections;

public class GUIterrainHint {

    GUIterrainPlannerMenu tpm;
    GUIterrainPatch patch;

    public int menuWidth;
    public int rightOffset;
    public int buttonHeight;
    public int smallButtonHeight;
    public int sideOffset;
    public float buttonWidth;

    public GUIterrainHint(GUIterrainPlannerMenu tpm)
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

    public void OnGui(int yPos)
    {
        Color origColor = GUI.color;
        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, 5.5f * buttonHeight), "Hint");
        yPos += smallButtonHeight;

        PatchLevel level = PatchLevel.low;
        GUI.color = patch.GetLevelColor(level);
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth / 2, buttonHeight), patch.GetLevelString(level)))
        {
        }
        GUI.Label(new Rect(Screen.width - menuWidth +2* sideOffset + buttonWidth / 2, yPos + 10, buttonWidth, buttonHeight), " = low terrain");

        yPos += buttonHeight + 5;
        level = PatchLevel.medium;
        GUI.color = patch.GetLevelColor(level);
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth / 2, buttonHeight), patch.GetLevelString(level)))
        {
        }
        GUI.Label(new Rect(Screen.width - menuWidth + 2 * sideOffset + buttonWidth / 2, yPos + 10, 2*buttonWidth, buttonHeight), " = medium terrain");

        yPos += buttonHeight + 5;
        level = PatchLevel.high;
        GUI.color = patch.GetLevelColor(level);
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth / 2, buttonHeight), patch.GetLevelString(level)))
        {
        }
        GUI.Label(new Rect(Screen.width - menuWidth + 2 * sideOffset + buttonWidth / 2, yPos + 10, buttonWidth, buttonHeight), " = high terrain");

        yPos += buttonHeight + 5;
        level = PatchLevel.random;
        GUI.color = patch.GetLevelColor(level);
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth / 2, buttonHeight), patch.GetLevelString(level)))
        {
        }
        GUI.Label(new Rect(Screen.width - menuWidth + 2 * sideOffset + buttonWidth / 2, yPos-5, 2*buttonWidth - 4*sideOffset, 2*buttonHeight), " random terrain, values are calculated automatically");



        GUI.color = origColor;

    }
}
