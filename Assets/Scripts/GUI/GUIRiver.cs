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

    public float width;
    public float depth;
    public float areaEffect;


    GUIManager gm;

    public List<bool> riverFlags;

    RiverGenerator rg;

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

        width = 15;
        areaEffect = 1;
        depth = 0.2f;
    }

    public void OnGui(int yPosition)
    {
        float buttonWidth = menuWidth - sideOffset - sideOffset / 2;
        float buttonWidth2 = menuWidth / 2 - sideOffset - sideOffset / 2;

        yPos = yPosition;
        if (GUI.Button(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth, buttonHeight), "Generate new river"))
        {
            rg.GenerateNewRiver(width, areaEffect, depth);
            gm.cm.terrainGenerator.build();
        }

        yPos += buttonHeight + 5;

        GUI.Box(new Rect(Screen.width - menuWidth, yPos, menuWidth - rightMenuOffset, 4.5f * buttonHeight), "parameters");

        yPos += buttonHeight + 2;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth2, buttonHeight), "width: " + (int)width);
        width = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth2, yPos + 5,
                menuWidth - sideOffset - buttonWidth2 - 5,
                buttonHeight), width, 8f, 20f);

        yPos += buttonHeight + 2;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth2, buttonHeight), 
            "area: " + (int)areaEffect + "." + (int)((areaEffect - (int)areaEffect) * 100));
        areaEffect = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth2, yPos + 5,
                menuWidth - sideOffset - buttonWidth2 - 5,
                buttonHeight), areaEffect, 0.5f, 2);

        yPos += buttonHeight + 2;
        GUI.Label(new Rect(Screen.width - menuWidth + sideOffset, yPos, buttonWidth2, buttonHeight),
            "depth: " + (int)depth + "." + (int)((depth - (int)depth) * 100));
        depth = GUI.HorizontalSlider(new Rect(
                Screen.width - menuWidth + buttonWidth2, yPos + 5,
                menuWidth - sideOffset - buttonWidth2 - 5,
                buttonHeight), depth, 0, 1.5f);


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
                gm.cm.terrainGenerator.build();
            }

            GUI.Label(new Rect(Screen.width - menuWidth +  3 * sideOffset, yPos, buttonWidth/3, buttonHeight), "river " + i);
            
            if (GUI.Button(new Rect(Screen.width - menuWidth + 3 * sideOffset + buttonWidth/3, yPos, 3*sideOffset, buttonHeight), "X"))
            {
                rg.DeleteRiverAt(i);
                gm.cm.terrainGenerator.build();
            }

            

            yPos += buttonHeight + 5;
        }

    }

}
