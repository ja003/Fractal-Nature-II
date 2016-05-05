using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIRiver
{

    int menuWidth;
    int rightMenuOffset;
    int topOffset;
    int buttonHeight;
    public float yPos;
    int sideOffset;

    public float scaleY;
    public float visibleArea;

    //public float width;
    //public float depth;
    //public float areaEffect;


    public GUIManager gm;

    public List<bool> riverFlags;

    RiverGenerator rg;

    RiverInfo selectedRiver;
    RiverInfo defaultRiver;

    public GUIRiver(GUIManager gm)
    {
        this.gm = gm;

        menuWidth = gm.menuWidth;
        rightMenuOffset = gm.rightOffset;
        topOffset = gm.topOffset;
        buttonHeight = gm.smallButtonHeight;
        sideOffset = 10;
        scaleY = gm.scaleY;
        visibleArea = gm.visibleArea;
        
        rg = gm.cm.riverGenerator;
        rg.riverGui = this;

        riverFlags = new List<bool>();

        //width = 15;
        //areaEffect = 1;
        //depth = 0.2f;

        defaultRiver = new RiverInfo(rg);
        defaultRiver.SetDefaultValues();
        selectedRiver = defaultRiver;

    }

    public void OnGui(int yPosition)
    {
        float buttonWidth = menuWidth - sideOffset - sideOffset / 2;
        float buttonWidth2 = menuWidth / 2 - sideOffset - sideOffset / 2;

        yPos = yPosition;
        if (GUI.Button(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, buttonHeight), "Generate new river"))
        {
            rg.GenerateNewRiver(selectedRiver.width, selectedRiver.areaEffect, selectedRiver.depth, selectedRiver.gridStep);
            gm.cm.terrainGenerator.build();
        }

        yPos += buttonHeight + 5;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 5.5f * buttonHeight), "parameters");

        yPos += buttonHeight + 2;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth2, buttonHeight), 
            "width: " + (int)selectedRiver.width);
        selectedRiver.width = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth2, yPos + 5,
                menuWidth - sideOffset - buttonWidth2 - 5,
                buttonHeight), selectedRiver.width, 8f, 20f);

        yPos += buttonHeight + 2;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth2, buttonHeight), 
            "area: " + (int)selectedRiver.areaEffect + "." + (int)((selectedRiver.areaEffect - (int)selectedRiver.areaEffect) * 100));
        selectedRiver.areaEffect = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth2, yPos + 5,
                menuWidth - sideOffset - buttonWidth2 - 5,
                buttonHeight), selectedRiver.areaEffect, 0.5f, 2);

        yPos += buttonHeight + 2;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth2, buttonHeight),
            "depth: " + (int)selectedRiver.depth + "." + 
            (int)((selectedRiver.depth - (int)selectedRiver.depth) * 100));
        selectedRiver.depth = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth2, yPos + 5,
                menuWidth - sideOffset - buttonWidth2 - 5,
                buttonHeight), selectedRiver.depth, 0, 0.2f);

        yPos += buttonHeight + 2;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth2, buttonHeight),
            "step: " + selectedRiver.gridStep);
        selectedRiver.gridStep = (int)GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth2, yPos + 5,
                menuWidth - sideOffset - buttonWidth2 - 5,
                buttonHeight), selectedRiver.gridStep, 10, 50);


        yPos += buttonHeight + 10;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 11 * buttonHeight), "rivers");
        yPos += buttonHeight;

        for(int i = 0; i < riverFlags.Count;i++)
        {

            bool riverFlag = riverFlags[i];
            riverFlag = GUI.Toggle(new Rect(Screen.width - menuWidth + sideOffset, yPos, sideOffset, buttonHeight), riverFlags[i], "");
            if (riverFlag != riverFlags[i])
            {
                riverFlags[i] = riverFlag;
                //Debug.Log(i + ":" + riverFlag);
                gm.cm.terrainGenerator.build();
            }

            //GUI.Label(new Rect(Screen.width - menuWidth +  3 * sideOffset, yPos, buttonWidth/3, buttonHeight), "river " + i);
            if (GUI.Button(new Rect(Screen.width - menuWidth + 3 * sideOffset, yPos, buttonWidth / 2, buttonHeight), "river" + i))
            {
                selectedRiver = rg.rivers[i];
            }

            if (GUI.Button(new Rect(Screen.width - menuWidth + 7 * sideOffset + buttonWidth / 3, yPos, 3 * sideOffset, buttonHeight), "O"))
            {
                selectedRiver = rg.rivers[i];
                selectedRiver.globalRiverC.ResetQuadrants();
                rg.frd.DigRiver(selectedRiver);
                gm.cm.terrainGenerator.build();
                selectedRiver.DrawRiver();
            }

            if (GUI.Button(new Rect(Screen.width - menuWidth + 10 * sideOffset + buttonWidth/3, yPos, 3*sideOffset, buttonHeight), "X"))
            {
                if(i != 0)
                {
                    selectedRiver = rg.rivers[i];
                }
                else //reset to default values
                {
                    defaultRiver.SetDefaultValues();
                    selectedRiver = defaultRiver;
                }
                rg.DeleteRiverAt(i);
                gm.cm.terrainGenerator.build();
            }

            

            yPos += buttonHeight + 5;
        }

    }

}
