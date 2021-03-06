﻿using UnityEngine;
using System.Collections;

public class GUIErosion {

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    int sideOffset;
    float smallButtonWidth;
    float buttonWidth;


    GUIManager gm;
    GUIMessage message;

    public GUIErosion(GUIManager gm)
    {
        this.gm = gm;
        message = gm.message;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
        sideOffset = 10;
        buttonWidth = menuWidth / 2 - sideOffset - sideOffset / 2;
        smallButtonWidth = buttonWidth / 2;

        windStrength = 50;
        windEffect = 0.5f;
    }

    int erosionHCounter = 0;
    int erosionTCounter = 0;

    bool startRain = false;
    string startRainString = "START RAIN";
    float maxWaterIncrease = 0.2f;
    int currentStep = 1;

    bool floodRiver = false;
    string floodRiverString = "FLOOD RIVER";

    bool startErosionH= false;
    string startErosionHString = "START EROSION";

    string windDirectionString = "O";
    int windDir_i = 1;
    int windDir_j = 1;
    int windX = 0;
    int windZ = 0;

    float windStrength = 50;
    float windEffect = 0.5f;


    float erosionStrength = 0.1f;
    float viscosity = 0.15f;
    float deposition = 0.15f;
    float evaporation = 0.05f;

    bool hydraulicErosionMenu = true;
    bool thermalErosionMenu = false;

    bool startErosionT = false;
    string startErosionTString = "START EROSION";
    float iterations = 500;
    float minDif = 0.005f;
    float thermalStrength = 0.2f;

    int refreshFrame = 15;
    int refreshBuildFrame = 30;

    int refreshTerrainSize = 250;
    public bool refreshTerrain = true;

    public void OnGui(int yPosition)
    {
        //Debug.Log(windEffect);
        //Debug.Log(windStrength);

        yPos = yPosition;
        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 1 * buttonHeight), "");
        refreshTerrain = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos - 1.5f, 2*buttonWidth, buttonHeight), refreshTerrain, " refresh terrain");

        yPos += buttonHeight + 6;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Hydraulic erosion"))
        {
            hydraulicErosionMenu = !hydraulicErosionMenu;
        }

        //HYDRAULIC EROSION
        if (hydraulicErosionMenu)
        {
            yPos -= 2;
            GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 16f * buttonHeight), "");

            yPos += buttonHeight + 7;
            ///RAIN
            bool startRainFlag = startRain;
            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), startRainString))
            {
                startRain = !startRain;
                if (startRain)
                    startRainString = "STOP RAIN";
                else
                    startRainString = "START RAIN";
            }
            if (startRain && Time.frameCount % refreshFrame == 0)
            {
                //Debug.Log("rain");
                gm.cm.terrainGenerator.waterMesh = true;

                gm.cm.erosionGenerator.he.DistributeWater(gm.cm.localTerrain.GetVisibleArea(), currentStep, maxWaterIncrease);
                currentStep++;

                if(refreshTerrain)
                    gm.cm.terrainGenerator.build();
            }

            yPos += buttonHeight + 5;

            ///RIVER
            bool floodRiverFlag = floodRiver;
            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), floodRiverString))
            {
                floodRiver = !floodRiver;
                if (floodRiver)
                    floodRiverString = "STOP";
                else
                    floodRiverString = "FLOOD RIVER";
            }
            if (floodRiver && Time.frameCount % refreshFrame == 0)
            {
                //Debug.Log("FLOODING");
                gm.cm.terrainGenerator.waterMesh = true;

                foreach(RiverInfo river in gm.cm.riverGenerator.rivers)
                {
                    gm.cm.erosionGenerator.he.FloodRiver(river);
                }
                if(refreshTerrain)
                    gm.cm.terrainGenerator.build();
            }

            yPos += buttonHeight + 5;

            ///EROSION 
            bool startErosionFlag = startErosionH;
            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), startErosionHString))
            {
                startErosionH = !startErosionH;
                if (startErosionH)
                    startErosionHString = "STOP EROSION";
                else
                    startErosionHString = "START EROSION";

                if (!startErosionH)
                {
                    gm.cm.terrainGenerator.build();
                }
            }
            if((startRain || floodRiver || startErosionH) && Time.frameCount % refreshBuildFrame == 0)
            {
                gm.cm.erosionGenerator.he.FilterErosionIn(gm.cm.localTerrain.GetVisibleArea(), 0.05f);
                if(refreshTerrain && gm.cm.localTerrain.GetVisibleArea().GetSize() < refreshTerrainSize)
                    gm.cm.terrainGenerator.build();
            }


            if (startErosionH && Time.frameCount % refreshFrame == 1)
            {
                gm.cm.erosionGenerator.he.HydraulicErosionStep(viscosity, erosionStrength, deposition, evaporation, windX, windZ, windStrength, -windEffect);

                //gm.cm.terrainGenerator.build();

                message.ShowMessage("hydraulic erosion step: " + erosionHCounter, refreshFrame);
                erosionHCounter++;
            }

            yPos += buttonHeight + 3;

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "strength");
            erosionStrength = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), erosionStrength, 0, 0.2f);
            yPos += buttonHeight;
            
            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "evaporation");
            evaporation = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), evaporation, 0, 0.5f);
            yPos += buttonHeight + 3;

            ///////////////WIND///////////////////
            GUI.Box(new Rect(Screen.width - menuWidth + sideOffset / 2, yPos, menuWidth - rightMenuOffset - sideOffset, 7.5f * buttonHeight), "WIND");
            yPos += buttonHeight;


            GUI.Label(new Rect(Screen.width - menuWidth + 8 * sideOffset, yPos, buttonWidth, buttonHeight), "direction");
            yPos += buttonHeight + 2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == windDir_i && j == windDir_j)
                        windDirectionString = "O";
                    else
                        windDirectionString = "-";
                    if (GUI.Button(new Rect(Screen.width - menuWidth + 4 * sideOffset + i * smallButtonWidth + 1,
                        yPos + j * buttonHeight + 1, smallButtonWidth, buttonHeight), windDirectionString))
                    {
                        windDir_i = i;
                        windDir_j = j;

                        windX = i - 1;
                        windZ = -(j - 1);
                    }
                }
            }

            yPos += 3 * buttonHeight + 5;

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "strength");
            windStrength = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), windStrength, 0, 100);
            yPos += buttonHeight;
            
            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "effect: ");

            //renamed to "effect" because "windAngle" is not very intuitive
            windEffect = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), windEffect, -1, 1);
            yPos += buttonHeight + 5;

            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "RESET EROSION"))
            {
                message.ShowMessage("erosion values reset", 100);
                erosionHCounter = 0;
                gm.cm.erosionGenerator.he.ResetValues();
                gm.cm.terrainGenerator.build();
            }
        }

        yPos += buttonHeight + 8;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Thermal erosion"))
        {
            thermalErosionMenu = !thermalErosionMenu;
        }

        ///////////////THERMAL EROSION
        if (thermalErosionMenu)
        {
            yPos -= 2;
            GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 6.8f * buttonHeight), "");

            yPos += buttonHeight + 7;
            

            bool startErosionTFlag = startErosionT;
            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), startErosionTString))
            {
                //Debug.Log("iterations: " + iterations);
                startErosionT = !startErosionT;
                if (startErosionT)
                    startErosionTString = "STOP EROSION";
                else
                {
                    startErosionTString = "START EROSION";
                    gm.cm.terrainGenerator.build();
                }
            }
            if (startErosionT && Time.frameCount % 10 == 0)
            {
                gm.cm.erosionGenerator.te.ThermalErosionStep(gm.cm.localTerrain.GetVisibleArea(), (int)iterations, minDif, thermalStrength);

                if (refreshTerrain && gm.cm.localTerrain.GetVisibleArea().GetSize() < refreshTerrainSize)
                    gm.cm.terrainGenerator.build();

                message.ShowMessage("thermal erosion step: " + erosionTCounter, refreshFrame);
                erosionTCounter++;
            }

            yPos += buttonHeight + 3;

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "iterations");
            iterations = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), iterations, 20, 1500);
            yPos += buttonHeight;

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "min diff");
            minDif = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), minDif, 0.00001f, 0.03f);
            yPos += buttonHeight;

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "strength");
            thermalStrength = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), thermalStrength, 0, 0.5f);

            yPos += buttonHeight+2;

            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "RESET EROSION"))
            {
                gm.cm.erosionGenerator.te.ResetErosion();
                gm.cm.terrainGenerator.build();
                erosionTCounter = 0;

            }
            yPos += buttonHeight+5;

        }
    }
}
