using UnityEngine;
using System.Collections;

public class GUIDebug {

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    int sideOffset;
    bool colorMode = false;
    bool waterMap = true;
    bool sedimentMap = true;
    bool thermalMap = true;

    float brightness = 0;
    float brightnessFlag = 0;


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

    int terrainSize = 50;

    public void OnGui(int yPosition)
    {
        float buttonWidth = menuWidth / 2 - sideOffset;

        yPos = yPosition;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset,17 * buttonHeight), "Debug");

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

        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), "noise"))
        {
            ChangeDebugMode(DebugMode.noise);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;

        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight),"brightness");

        brightness = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth, yPos + 5,
                menuWidth - sideOffset - buttonWidth - 5,
                buttonHeight), brightness, -1, 1);
        if (brightnessFlag != brightness)
        {
            brightnessFlag = brightness;
            gm.cm.terrainGenerator.terrainBrightness = brightness;
            gm.cm.terrainGenerator.build();
        }        
        yPos += buttonHeight;

        bool colorModeFlag = colorMode;
        colorModeFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), colorMode, "  color mode");
        if (colorModeFlag != colorMode)
        {
            colorMode = colorModeFlag;
            gm.cm.terrainGenerator.colorMode = colorMode;
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight;

        bool waterMapFlag = waterMap;
        waterMapFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), waterMap, "  water");
        if (waterMapFlag != waterMap)
        {
            waterMap = waterMapFlag;
            gm.cm.terrainGenerator.waterMesh = waterMap;
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight;

        bool sedimentMapFlag = sedimentMap;
        sedimentMapFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), sedimentMap, "  hydraulic erosion");
        if (sedimentMapFlag != sedimentMap)
        {
            sedimentMap = sedimentMapFlag;
            gm.cm.terrainGenerator.erosionHydraulicLayer = sedimentMap;
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight;

        bool thermalMapFlag = thermalMap;
        thermalMapFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, menuWidth - 3 * rightMenuOffset, buttonHeight), thermalMap, "  thermal erosion");
        if (thermalMapFlag != thermalMap)
        {
            thermalMap = thermalMapFlag;
            gm.cm.terrainGenerator.erosionThermalLayer = thermalMap;
            gm.cm.terrainGenerator.build();
        }


        yPos += buttonHeight + 5;


        GUI.Label(new Rect(Screen.width - menuWidth + 6 * sideOffset, yPos, buttonWidth, buttonHeight), "default terrain");
        yPos += buttonHeight;
        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, buttonWidth, buttonHeight), "gradientX_LR"))
        {
            terrainSize = gm.cm.localTerrain.terrainWidth;
            gm.cm.terrainGenerator.GenerateDefaultTerrain(TerrainType.gradientX_lr, terrainSize);
            gm.cm.terrainGenerator.build();
        }
        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset + buttonWidth + 5, yPos, buttonWidth, buttonHeight), "gradientX_RL"))
        {
            terrainSize = gm.cm.localTerrain.terrainWidth;
            gm.cm.terrainGenerator.GenerateDefaultTerrain(TerrainType.gradientX_rl, terrainSize);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;
        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, buttonWidth, buttonHeight), "gradientZ_LR"))
        {
            terrainSize = gm.cm.localTerrain.terrainWidth;
            gm.cm.terrainGenerator.GenerateDefaultTerrain(TerrainType.gradientZ_lr, terrainSize);
            gm.cm.terrainGenerator.build();
        }
        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset + buttonWidth + 5, yPos, buttonWidth, buttonHeight), "gradientZ_RL"))
        {
            terrainSize = gm.cm.localTerrain.terrainWidth;
            gm.cm.terrainGenerator.GenerateDefaultTerrain(TerrainType.gradientZ_rl, terrainSize);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;
        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, buttonWidth, buttonHeight), "radial +"))
        {
            terrainSize = gm.cm.localTerrain.terrainWidth;
            gm.cm.terrainGenerator.GenerateDefaultTerrain(TerrainType.gradient_radialPlus, terrainSize);
            gm.cm.terrainGenerator.build();
        }
        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset + buttonWidth + 5, yPos, buttonWidth, buttonHeight), "radial -"))
        {
            terrainSize = gm.cm.localTerrain.terrainWidth;
            gm.cm.terrainGenerator.GenerateDefaultTerrain(TerrainType.gradient_radialMinus, terrainSize);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;

        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset, yPos, buttonWidth, buttonHeight), "river"))
        {
            terrainSize = gm.cm.localTerrain.terrainWidth;
            gm.cm.terrainGenerator.GenerateDefaultTerrain(TerrainType.river, terrainSize);
            gm.cm.terrainGenerator.build();
        }
        if (GUI.Button(new Rect(Screen.width - menuWidth + rightMenuOffset + buttonWidth + 5, yPos, buttonWidth, buttonHeight), "constant"))
        {
            terrainSize = gm.cm.localTerrain.terrainWidth;
            gm.cm.terrainGenerator.GenerateDefaultTerrain(TerrainType.constant, terrainSize);
            gm.cm.terrainGenerator.build();
        }
        yPos += buttonHeight + 5;



        yPos += buttonHeight + 5;

    }

    public void ChangeDebugMode(DebugMode mode)
    {
        gm.cm.terrainGenerator.debugHeightmap = false;
        gm.cm.terrainGenerator.debugRmin = false;
        gm.cm.terrainGenerator.debugRmax = false;
        gm.cm.terrainGenerator.debugNoise = false;
        if(mode == DebugMode.heightmap)
            gm.cm.terrainGenerator.debugHeightmap = true;
        else if (mode == DebugMode.rMin)
            gm.cm.terrainGenerator.debugRmin = true;
        else if (mode == DebugMode.rMax)
            gm.cm.terrainGenerator.debugRmax = true;
        else if (mode == DebugMode.noise)
            gm.cm.terrainGenerator.debugNoise = true;

    }
}


public enum DebugMode
{
    heightmap,
    rMin,
    rMax, 
    noise
}