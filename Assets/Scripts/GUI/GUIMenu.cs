using UnityEngine;
using System.Collections;

public class GUIMenu
{
    int menuWidth;
    int rightOffset;
    int topOffset;
    int buttonHeight;
    public int yPos;

    GUIManager gm;

    public GUIMenu(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.menuButtonHeight;
    }

    public void OnGUI(int yPosition)
    {
        // Title
        //GUI.Box(new Rect(Screen.width - menuWidth, topOffset, menuWidth - rightOffset, buttonHeight), "Fractal Nature");
        yPos = yPosition;
        if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, buttonHeight), "Fractal Nature"))
        {
            gm.fractalNatureFlag = !gm.fractalNatureFlag;
        }

        if (gm.fractalNatureFlag)
        {
            yPos += buttonHeight;

            //General settings button
            if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, buttonHeight), "General Settings"))
            {
                gm.generalSettingsFlag = !gm.generalSettingsFlag;
            }

            yPos += buttonHeight;

            //Filters menu button
            if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset - menuWidth / 2, buttonHeight), "Filters"))
            {

            }

            //Erosion menu button
            if (GUI.Button(new Rect(Screen.width - menuWidth + menuWidth / 2, yPos, menuWidth - rightOffset - menuWidth / 2, buttonHeight), "Erosion"))
            {

            }
            yPos += buttonHeight;

            if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, buttonHeight), "River functions"))
            {
            }
            yPos += buttonHeight;
        }

    }
}
