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
                Debug.Log("generalSettingsFlag: " + gm.generalSettingsFlag);
            }

            yPos += buttonHeight;

            //Filters menu button
            if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset - menuWidth / 2, buttonHeight), "Filters"))
            {
                SetMenuFlag(MenuEnum.filters, !gm.filterMenuFlag);
                Debug.Log("filterMenuFlag: " + gm.filterMenuFlag);
            }

            //Erosion menu button
            if (GUI.Button(new Rect(Screen.width - menuWidth + menuWidth / 2, yPos, menuWidth - rightOffset - menuWidth / 2, buttonHeight), "Erosion"))
            {
                SetMenuFlag(MenuEnum.erosion, !gm.erosionMenuFlag);
            }
            yPos += buttonHeight;

            if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightOffset, buttonHeight), "River functions"))
            {
                SetMenuFlag(MenuEnum.river, !gm.riverMenuFlag);
            }
            yPos += buttonHeight;
        }

    }

    void SetMenuFlag(MenuEnum menu, bool value)
    {
        switch (menu)
        {
            case MenuEnum.fractalNature:
                if (value)
                {
                    gm.fractalNatureFlag = true;
                }
                else
                {
                    gm.fractalNatureFlag = false;
                }
                break;
            case MenuEnum.generalSettings:
                if (value)
                {
                    gm.generalSettingsFlag = true;
                    gm.filterMenuFlag = false;
                    gm.erosionMenuFlag = false;
                    gm.riverMenuFlag = false;

                }
                else
                {
                    gm.generalSettingsFlag = false;
                }
                break;
            case MenuEnum.filters:
                if (value)
                {
                    gm.filterMenuFlag = true;
                    gm.generalSettingsFlag = false;
                    gm.erosionMenuFlag = false;
                    gm.riverMenuFlag = false;

                }
                else
                {
                    gm.filterMenuFlag = false;
                }
                break;
            case MenuEnum.erosion:
                if (value)
                {
                    gm.erosionMenuFlag = true;
                    gm.filterMenuFlag = false;
                    gm.generalSettingsFlag = false;
                    gm.riverMenuFlag = false;

                }
                else
                {
                    gm.erosionMenuFlag = false;
                }
                break;
            case MenuEnum.river:
                if (value)
                {
                    gm.riverMenuFlag = true;
                    gm.filterMenuFlag = false;
                    gm.generalSettingsFlag = false;
                    gm.erosionMenuFlag = false;

                }
                else
                {
                    gm.riverMenuFlag = false;
                }
                break;
        }
        //Debug.Log(gm);
    }
}
