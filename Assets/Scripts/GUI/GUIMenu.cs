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
        if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, buttonHeight), "Procedural Terrain"))
        {
            SetMenuFlag(MenuEnum.fractalNature, !gm.fractalNatureFlag);
            Debug.Log("fractalNatureFlag: " + gm.fractalNatureFlag);
        }

        if (gm.fractalNatureFlag)
        {
            yPos += buttonHeight;

            //General settings button
            if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, buttonHeight), "General Settings"))
            {
                SetMenuFlag(MenuEnum.generalSettings, !gm.generalSettingsFlag);
                //Debug.Log("generalSettingsFlag: " + gm.generalSettingsFlag);
            }

            yPos += buttonHeight;

            //Filters menu button
            if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset - menuWidth / 2, buttonHeight), "Filters"))
            {
                SetMenuFlag(MenuEnum.filters, !gm.filterMenuFlag);
                //Debug.Log("filterMenuFlag: " + gm.filterMenuFlag);
            }

            //Erosion menu button
            if (GUI.Button(new Rect(Screen.width - menuWidth + menuWidth / 2, yPos, 
                menuWidth - rightOffset - menuWidth / 2, buttonHeight), "Erosion"))
            {
                SetMenuFlag(MenuEnum.erosion, !gm.erosionMenuFlag);
            }
            yPos += buttonHeight;

            //River menu button
            if (GUI.Button(new Rect(Screen.width - menuWidth, yPos,
                menuWidth - rightOffset - menuWidth / 2, buttonHeight), "River"))
            {
                SetMenuFlag(MenuEnum.river, !gm.riverMenuFlag);
            }

            //Debug menu button
            if (GUI.Button(new Rect(Screen.width - menuWidth + menuWidth / 2, yPos,
                menuWidth - rightOffset - menuWidth / 2, buttonHeight), "Debug"))
            {
                SetMenuFlag(MenuEnum.debug, !gm.debugMenuFlag);
            }

            yPos += buttonHeight;
        }

    }

    void SetMenuFlag(MenuEnum menu, bool value)
    {
        gm.generalSettingsFlag = false;
        gm.filterMenuFlag = false;
        gm.erosionMenuFlag = false;
        gm.riverMenuFlag = false;
        gm.debugMenuFlag = false;

        switch (menu)
        {
            case MenuEnum.fractalNature:
                if (value)
                    gm.fractalNatureFlag = true;
                else
                    gm.fractalNatureFlag = false;
                break;
            case MenuEnum.generalSettings:
                if (value)
                    gm.generalSettingsFlag = true;
                else
                    gm.generalSettingsFlag = false;
                break;
            case MenuEnum.filters:
                if (value)
                    gm.filterMenuFlag = true;
                else
                    gm.filterMenuFlag = false;
                break;
            case MenuEnum.erosion:
                if (value)
                    gm.erosionMenuFlag = true;
                else
                    gm.erosionMenuFlag = false;
                break;
            case MenuEnum.river:
                if (value)
                    gm.riverMenuFlag = true;
                else
                    gm.riverMenuFlag = false;
                break;
            case MenuEnum.debug:
                if (value)
                    gm.debugMenuFlag = true;
                else
                    gm.debugMenuFlag = false;
                break;
        }
        //Debug.Log(gm);
    }
}
