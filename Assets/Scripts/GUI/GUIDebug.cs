using UnityEngine;
using System.Collections;

public class GUIDebug {

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    int sideOffset;
    bool colorMode;
    bool waterMap;



    GUIManager gm;

    public GUIDebug(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
        sideOffset = 10;
    }

    public void OnGui(int yPosition)
    {
        float buttonWidth = menuWidth / 2 - sideOffset - sideOffset / 2;

        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 9 * buttonHeight), "Debug");

        yPos += buttonHeight + 5;
        
        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3* rightMenuOffset, buttonHeight), "heightmap"))
        {
            ChangeDebugMode(DebugMode.heightmap);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;

        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), "rMin"))
        {
            ChangeDebugMode(DebugMode.rMin);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;

        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), "rMax"))
        {
            ChangeDebugMode(DebugMode.rMax);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;

        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), "roughness"))
        {
            ChangeDebugMode(DebugMode.roughness);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;

        bool colorModeFlag = colorMode;
        colorModeFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), colorMode, "  color mode");
        if (colorModeFlag != colorMode)
        {
            colorMode = colorModeFlag;
            gm.cm.terrainGenerator.colorMode = colorMode;
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;

        bool waterMapFlag = waterMap;
        waterMapFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), waterMap, "  water");
        if (waterMapFlag != waterMap)
        {
            waterMap = waterMapFlag;
            gm.cm.terrainGenerator.erosionHydraulicLayer = waterMap;
            gm.cm.terrainGenerator.build();
        }

        yPos += buttonHeight + 5;

    }

    public void ChangeDebugMode(DebugMode mode)
    {
        gm.cm.terrainGenerator.debugHeightmap = false;
        gm.cm.terrainGenerator.debugRmin = false;
        gm.cm.terrainGenerator.debugRmax = false;
        gm.cm.terrainGenerator.debugRoughness = false;
        if(mode == DebugMode.heightmap)
            gm.cm.terrainGenerator.debugHeightmap = true;
        else if (mode == DebugMode.rMin)
            gm.cm.terrainGenerator.debugRmin = true;
        else if (mode == DebugMode.rMax)
            gm.cm.terrainGenerator.debugRmax = true;
        else if (mode == DebugMode.roughness)
            gm.cm.terrainGenerator.debugRoughness = true;

    }
}


public enum DebugMode
{
    heightmap,
    rMin,
    rMax, 
    roughness
}