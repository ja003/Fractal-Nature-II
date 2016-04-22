using UnityEngine;
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

    public GUIErosion(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
        sideOffset = 10;
        buttonWidth = menuWidth / 2 - sideOffset - sideOffset / 2;
        smallButtonWidth = buttonWidth / 2;
    }

    bool startRain = false;
    string startRainString = "START RAIN";
    float maxWaterIncrease = 0.2f;
    int currentStep = 1;

    bool startErosionH= false;
    string startErosionHString = "START EROSION";

    string windDirectionString = "O";
    int windDir_i = 1;
    int windDir_j = 1;
    int windX = 0;
    int windZ = 0;

    float windStrength = 20;
    float windAngle = 0;


    float erosionStrength = 0.1f;
    float viscosity = 0.1f;
    float deposition = 0.1f;
    float evaporation = 0.2f;

    bool hydraulicErosionMenu = false;
    bool thermalErosionMenu = true;

    bool startErosionT = false;
    string startErosionTString = "START EROSION";
    float iterations = 500;
    float minDif = 0.005f;
    float thermalStrength = 0.2f;

    public void OnGui(int yPosition)
    {


        yPos = yPosition;
                
        yPos += 2;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Hydraulic erosion"))
        {
            hydraulicErosionMenu = !hydraulicErosionMenu;
        }

        //HYDRAULIC EROSION
        if (hydraulicErosionMenu)
        {
            GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 16.5f * buttonHeight), "");

            yPos += buttonHeight + 5;

            bool startRainFlag = startRain;
            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), startRainString))
            {
                startRain = !startRain;
                if (startRain)
                    startRainString = "STOP RAIN";
                else
                    startRainString = "START RAIN";
            }
            if (startRain && Time.frameCount % 30 == 0)
            {
                //Debug.Log("rain");
                gm.cm.terrainGenerator.waterMesh = true;

                gm.cm.erosionGenerator.he.DistributeWater(gm.cm.localTerrain.GetVisibleArea(), currentStep, maxWaterIncrease);
                currentStep++;
                gm.cm.terrainGenerator.build();
            }

            yPos += buttonHeight + 5;

            bool startErosionFlag = startErosionH;
            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), startErosionHString))
            {
                startErosionH = !startErosionH;
                if (startErosionH)
                    startErosionHString = "STOP EROSION";
                else
                    startErosionHString = "START EROSION";
            }
            if (startErosionH && Time.frameCount % 30 == 15)
            {
                gm.cm.erosionGenerator.he.HydraulicErosionStep(gm.cm.localTerrain.GetVisibleArea(), viscosity, erosionStrength, deposition, evaporation, windX, windZ, windStrength, windAngle);

                gm.cm.terrainGenerator.build();
            }

            yPos += buttonHeight + 3;

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "strength");
            erosionStrength = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), erosionStrength, 0, 0.2f);
            yPos += buttonHeight;

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "deposition");
            deposition = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), deposition, 0, 0.2f);
            yPos += buttonHeight;

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "viscosity");
            viscosity = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), viscosity, 0, 0.2f);
            yPos += buttonHeight;

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "evaporation");
            evaporation = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), evaporation, 0, 0.6f);
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

            GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "angle: " + (int)(90 * windAngle) + "." +
                Mathf.Abs((int)((90 * windAngle - (int)(90 * windAngle)) * 100)));
            windAngle = GUI.HorizontalSlider(new Rect(
                    Screen.width - menuWidth + buttonWidth, yPos + 5,
                    menuWidth - sideOffset - buttonWidth - 5,
                    buttonHeight), windAngle, -1, 1);
            yPos += buttonHeight + 5;

            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "RESET EROSION"))
            {
                gm.cm.erosionGenerator.he.ResetValues();
                gm.cm.terrainGenerator.build();
            }
        }

        yPos += buttonHeight + 5;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "Thermal erosion"))
        {
            thermalErosionMenu = !thermalErosionMenu;
        }

        ///////////////THERMAL EROSION
        if (thermalErosionMenu)
        {
            GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 6.5f * buttonHeight), "");

            yPos += buttonHeight + 5;
            

            bool startErosionTFlag = startErosionT;
            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), startErosionTString))
            {
                startErosionT = !startErosionT;
                if (startErosionT)
                    startErosionTString = "STOP EROSION";
                else
                    startErosionTString = "START EROSION";
            }
            if (startErosionT && Time.frameCount % 10 == 0)
            {
                gm.cm.erosionGenerator.te.ThermalErosionStep(gm.cm.localTerrain.GetVisibleArea(), (int)iterations, minDif, thermalStrength);

                gm.cm.terrainGenerator.build();
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
            yPos += buttonHeight;

            if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, 2 * buttonWidth, buttonHeight), "RESET EROSION"))
            {
                gm.cm.erosionGenerator.te.thermalErosionMap.ResetQuadrants();
                gm.cm.terrainGenerator.build();
            }
            yPos += buttonHeight+5;

        }
    }
}
